using MR;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SpellCaster : MonoBehaviour
{
    struct TimePoint3F
    {
        public Vector3 Position;
        public float Time;

        public TimePoint3F(Vector3 position, float time)
        {
            Position = position;
            Time = time;
        }
    }

    [Serializable]
    public struct Event
    {
        public UnityEvent OnBeginCastSpell;
        public UnityEvent OnEndCastSpell;
    }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    [SerializeField] private InputActionReference m_ButtonRegisterSpell;
#endif
    [SerializeField] private InputActionReference m_ButtonCastSpell;
    [SerializeField, Tooltip("In second")] private float m_PositionInterval = 0.5f;

    [SerializeField] private TrailRenderer m_MagicTrail;
    [SerializeField] private TextMeshProUGUI m_SpellScore1;
    [SerializeField] private TextMeshProUGUI m_SpellScore2;
    [SerializeField] private TextMeshProUGUI m_SpellScore3;

    [SerializeField] private GameObject m_SpellInvokationParticle;

    public Event Events;

    SpellGestureRecognizer m_Recognizer;
    SpellDescriptor[] m_SpellDataArray;

    Matrix4x4 m_StartCastSpellMatrix;
    Gesture m_CastSpellPositionsTime;

    float m_CurrentCatingSpellTime;
    float m_StartCatingSpellTime;

    bool m_IsCastingSpell;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    bool m_IsRecordingSpell;
#endif

    // Start is called before the first frame update
    void Start()
    {
        m_Recognizer = new SpellGestureRecognizer();
        m_SpellDataArray = Resources.LoadAll<SpellDescriptor>("");
        m_CastSpellPositionsTime = new Gesture(0.2f, 0);

        foreach (SpellDescriptor spellMovementDescriptor in m_SpellDataArray)
        {
            m_Recognizer.LoadGesture(spellMovementDescriptor);
        }

        m_SpellScore1.text = "";
        m_SpellScore2.text = "";
        m_SpellScore3.text = "";
    }

    // Update is called once per frame
    void Update()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        bool isRecordSpellButtonTrigger = m_ButtonRegisterSpell.action.ReadValue<bool>();

        if (!m_IsRecordingSpell && isRecordSpellButtonTrigger)
        {
            StartCastingSpell();
            m_IsRecordingSpell = true;
        }

        while (m_IsRecordingSpell && Time.time >= m_CurrentCatingSpellTime + m_PositionInterval)
        {
            m_CurrentCatingSpellTime += m_PositionInterval;
            RecordPosition();
        }

        if (m_IsRecordingSpell && !isRecordSpellButtonTrigger)
        {
            EndCastingSpell();
            m_IsRecordingSpell = false;
        }
#endif

        bool isCastSpellButtonStrigger = m_ButtonCastSpell.action.ReadValue<bool>();
        if (!m_IsCastingSpell && isCastSpellButtonStrigger)
        {
            StartCastingSpell();
            m_IsCastingSpell = true;
        }

        while (m_IsCastingSpell && Time.time >= m_CurrentCatingSpellTime + m_PositionInterval)
        {
            RecordPosition();
            m_CurrentCatingSpellTime += m_PositionInterval;
        }

        if (m_IsCastingSpell && !isCastSpellButtonStrigger)
        {
            EndCastingSpell();
            m_IsCastingSpell = false;
        }
    }

    private void StartCastingSpell()
    {
        m_MagicTrail.emitting = true;
        m_StartCastSpellMatrix = transform.localToWorldMatrix.inverse;
        m_StartCatingSpellTime = Time.time;
        m_CurrentCatingSpellTime = Time.time;
        m_CastSpellPositionsTime.TimedPoints.Add(new Gesture.TimedPoint(new Vector3(0, 0, 0), 0)); // Record the fist position
        Events.OnBeginCastSpell?.Invoke();
    }

    private void RecordPosition()
    {
        float timeSc = (Time.time - m_StartCatingSpellTime);
        m_CastSpellPositionsTime.TimedPoints.Add(new Gesture.TimedPoint(m_StartCastSpellMatrix.MultiplyPoint3x4(transform.position), timeSc));

        if (m_IsCastingSpell)
        {
            GestureResultList reconizeList = m_Recognizer.Recognize(m_CastSpellPositionsTime.TimedPoints, m_CastSpellPositionsTime.ErrorThreashold);

            m_SpellScore1.text = GetSpellScoreTxt(reconizeList[0]);
            m_SpellScore2.text = GetSpellScoreTxt(reconizeList[1]);
            m_SpellScore3.text = GetSpellScoreTxt(reconizeList[2]);

            float bestScore = reconizeList.Score;
            if (bestScore > 0.25)
            {
                m_SpellInvokationParticle.SetActive(true);
                m_SpellInvokationParticle.transform.localScale = (bestScore - 0.25f) / 4f * Vector3.one;
            }
        }
    }

    string GetSpellScoreTxt(GestureResultList.GestureResult result)
    {
        return result == null ? "" : $"{result.Name} score:{result.Score.ToString("0.00")}";
    }

    private void EndCastingSpell()
    {
        if (IsSpellValid)
        {
#if UNITY_EDITOR
            if (m_IsRecordingSpell)
            {
                string uniqueId = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
                string path = $"{Application.dataPath}/Gesture {uniqueId}.json";
                m_Recognizer.SaveGesture(path, m_CastSpellPositionsTime);
                Debug.Log($"New spell gesture register at {path}");
            }
#endif
            if (m_IsCastingSpell)
            {
                GestureResultList reconizeList = m_Recognizer.Recognize(m_CastSpellPositionsTime.TimedPoints, m_CastSpellPositionsTime.ErrorThreashold);
                Debug.Log($"Spell recognize origin = {reconizeList}");

                m_SpellScore1.text = GetSpellScoreTxt(reconizeList[0]);
                m_SpellScore2.text = GetSpellScoreTxt(reconizeList[1]);
                m_SpellScore3.text = GetSpellScoreTxt(reconizeList[2]);

                if (reconizeList.Score > 0.5)
                {
                    foreach (SpellDescriptor spellDescriptor in m_SpellDataArray)
                    {
                        if (reconizeList.Name.Equals(spellDescriptor.SpellName)) 
                        {
                            GameObject spell = PoolManagerSingleton.Instance.Create(spellDescriptor.SpellPrefab, transform.position, transform.rotation);
                            spell.transform.localScale *= Mathf.Clamp01(((float)reconizeList.Score));
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"[{nameof(SpellCaster)}] Unknow pattern cast");
                }
            }
        }

        m_SpellInvokationParticle.SetActive(false);
        Events.OnEndCastSpell?.Invoke();
        m_MagicTrail.emitting = false;
        m_CastSpellPositionsTime.TimedPoints.Clear();
    }

    bool IsSpellValid => m_CastSpellPositionsTime.TimedPoints.Count > 3;
}

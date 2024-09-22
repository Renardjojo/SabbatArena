using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SpellPreviewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_SpellNameTxt;

    [SerializeField] private InputActionReference m_PreviousSpellButton;
    [SerializeField] private InputActionReference m_NextSpellButton;
    [SerializeField] private Mesh m_PreviewMesh;
    [SerializeField] private Material m_PreviewPendingMaterial;
    [SerializeField] private Material m_PreviewDoneMaterial;
    [SerializeField] private Transform m_Pivot;
    [SerializeField] private SpellCaster m_SpellCaster;

    private SpellDescriptor[] m_SpellDescriptorArray;
    private SpellDescriptor m_CurrentSpellDescriptor;
    int m_CurrentSpellIndex = 0;
    bool m_IsCastingSpell = false;
    float m_StartCastSpellTimer;

    Matrix4x4 m_StartSpellMatrix;

    // Start is called before the first frame update
    void Start()
    {
        m_SpellDescriptorArray = Resources.LoadAll<SpellDescriptor>("");
        m_CurrentSpellIndex = 0;
        UpdateSelectedSpell();

        m_SpellCaster.Events.OnBeginCastSpell.AddListener(OnBeginCastSpell);
        m_SpellCaster.Events.OnEndCastSpell.AddListener(OnEndCastSpell);
    }

    void OnBeginCastSpell()
    {
        m_StartSpellMatrix = m_Pivot.localToWorldMatrix;
        m_IsCastingSpell = true;
        m_StartCastSpellTimer = Time.time;
    }

    void OnEndCastSpell()
    {
        m_IsCastingSpell = false;
    }

    private void SelectPreviousSpell()
    {
        m_CurrentSpellIndex--;
        if (m_CurrentSpellIndex < 0)
            m_CurrentSpellIndex = m_SpellDescriptorArray.Count() - 1;

        UpdateSelectedSpell();
    }

    private void SelectNextSpell()
    {
        m_CurrentSpellIndex++;
        if (m_CurrentSpellIndex >= m_SpellDescriptorArray.Count())
            m_CurrentSpellIndex = 0;

        UpdateSelectedSpell();
    }

    void UpdateSelectedSpell()
    {
        m_CurrentSpellDescriptor = m_SpellDescriptorArray[m_CurrentSpellIndex];

        if (m_CurrentSpellDescriptor == null)
            return;

        m_SpellNameTxt.text = m_CurrentSpellDescriptor.SpellName;
    }

    private void Update()
    {
        if (m_PreviousSpellButton.action.ReadValue<bool>())
            SelectPreviousSpell();

        if (m_NextSpellButton.action.ReadValue<bool>())
            SelectNextSpell();

        if (m_CurrentSpellDescriptor)
        {
            int objCount = m_CurrentSpellDescriptor.Gesture.TimedPoints.Count;
            List<Matrix4x4> matDones = new List<Matrix4x4>(objCount);
            List<Matrix4x4> matPending = new List<Matrix4x4>(objCount);

            float errorThreashold = m_CurrentSpellDescriptor.Gesture.ErrorThreashold;
            Vector3 scale = new Vector3(errorThreashold, errorThreashold, errorThreashold);
            Matrix4x4 scaleMat = Matrix4x4.Scale(scale);
            Matrix4x4 referential = m_IsCastingSpell ? m_StartSpellMatrix : m_Pivot.localToWorldMatrix;

            for (int i = 0; i < objCount; i++)
            {
                if (m_IsCastingSpell && m_CurrentSpellDescriptor.Gesture.TimedPoints[i].Time < Time.time - m_StartCastSpellTimer)
                {
                    matDones.Add(Matrix4x4.Translate(referential.MultiplyPoint3x4(m_CurrentSpellDescriptor.Gesture.TimedPoints[i].Point)) * scaleMat);
                }
                else
                {
                    matPending.Add(Matrix4x4.Translate(referential.MultiplyPoint3x4(m_CurrentSpellDescriptor.Gesture.TimedPoints[i].Point)) * scaleMat);

                }
            }

            Graphics.DrawMeshInstanced(m_PreviewMesh, 0, m_PreviewPendingMaterial, matPending);
            Graphics.DrawMeshInstanced(m_PreviewMesh, 0, m_PreviewDoneMaterial, matDones);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using Color = UnityEngine.Color;
using System.Xml;
using UnityEngine.Experimental.GlobalIllumination;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Spell", menuName = "MagicRoom/Spell")]
public class SpellDescriptor : ScriptableObject
{
    public string SpellName;
    public List<string> SpellVocalInvocation;
    public GameObject SpellPrefab;
    public Gesture Gesture;

#if UNITY_EDITOR
    public TextAsset FromJSon;

    private void OnValidate()
    {
        if (FromJSon)
        {
            Gesture gesture = JsonUtility.FromJson<Gesture>(FromJSon.text);
            Gesture = gesture;
            FromJSon = null;
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpellDescriptor))]
public class SpellDescriptorDrawer : Editor
{
    private PreviewRenderUtility m_PreviewRenderUtility;
    private Texture m_OutputTexture;

    private const float PointSize = 0.05f;
    private const float PreviewResolution = 200f;

    private void DrawPreviw(Rect rect, SpellDescriptor spellDatas)
    {
        if (m_PreviewRenderUtility != null)
            m_PreviewRenderUtility.Cleanup();
        m_PreviewRenderUtility = new PreviewRenderUtility(true);
        System.GC.SuppressFinalize(m_PreviewRenderUtility);

        Camera camera = m_PreviewRenderUtility.camera;
        camera.fieldOfView = 30f;
        camera.nearClipPlane = 0.3f;
        camera.farClipPlane = 1000;
        camera.transform.position = new Vector3(0f, 0f, -3f);
        camera.transform.LookAt(Vector3.zero);

        List<GameObject> objects = new();

        foreach (Gesture.TimedPoint point in  spellDatas.Gesture.TimedPoints)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = point.Point;
            obj.transform.localScale = new Vector3(PointSize, PointSize, PointSize);
            objects.Add(obj);
        }

        m_OutputTexture = CreatePreviewTexture(objects);

        foreach (GameObject obj in objects)
            DestroyImmediate(obj);

        if (m_PreviewRenderUtility != null || m_OutputTexture != null)
            GUI.DrawTexture(rect, m_OutputTexture);
    }

    private RenderTexture CreatePreviewTexture(List<GameObject> objects)
    {
        m_PreviewRenderUtility.BeginPreview(new Rect(0, 0, PreviewResolution, PreviewResolution), GUIStyle.none);

        m_PreviewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
        m_PreviewRenderUtility.lights[0].intensity = 2;

        foreach(GameObject obj in objects)
            m_PreviewRenderUtility.AddSingleGO(obj);

        m_PreviewRenderUtility.camera.Render();
        return (RenderTexture)m_PreviewRenderUtility.EndPreview();
    }

    public override void OnInspectorGUI()
    {
        SpellDescriptor spellDatas = (SpellDescriptor)target;
        // Draw default inspector for other properties

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spell gesture preview", EditorStyles.boldLabel);
        Rect previewRect = EditorGUILayout.GetControlRect(GUILayout.Height(PreviewResolution), GUILayout.Width(PreviewResolution));

        Undo.RecordObject(spellDatas, "Display and change point");

        DrawPreviw(previewRect, spellDatas);
    }
}
#endif

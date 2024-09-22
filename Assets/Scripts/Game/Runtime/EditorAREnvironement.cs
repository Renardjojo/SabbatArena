using UnityEngine;

public class EditorAREnvironement : MonoBehaviour
{
    [SerializeField] private GameObject m_Prefab;

    void Awake()
    {
        Instantiate(m_Prefab);
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        Destroy(this);
    }

    private void OnValidate()
    {
        gameObject.tag = "EditorOnly";
    }
}

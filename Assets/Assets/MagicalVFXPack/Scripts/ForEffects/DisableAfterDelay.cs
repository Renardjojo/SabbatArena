using UnityEngine;

namespace MR
{
    public class DisableAfterDelay : MonoBehaviour
    {
        [SerializeField] private float m_delay;
        private float m_time;

        private void OnEnable()
        {
            m_time = Time.time;
        }

        void Update()
        {
            if (Time.time > m_time + m_delay)
            {
                PoolManagerSingleton.Instance.Discard(gameObject);
            }
        }
    }
}

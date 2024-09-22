using UnityEngine;

namespace MR
{
    public class ObjectMove : MonoBehaviour
    {
        public float time;
        float m_time;
        float m_time2;
        public float MoveSpeed = 10;
        public bool AbleHit;
        public float HitDelay;
        public GameObject m_hitObject;
        GameObject m_makedObject;
        public float DestroyTime2;
        public bool useInPoolManager = true;

        private void OnEnable()
        {
            m_time = Time.time;
            m_time2 = Time.time;
        }

        void LateUpdate()
        {
            if (Time.time > m_time + time)
            {
                if (useInPoolManager)
                    PoolManagerSingleton.Instance.Discard(gameObject);
                else
                    Destroy(gameObject);
            }

            Vector3 previousPos = transform.position;
            transform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed);
            Vector3 newPos = transform.position;
            Vector3 previousToNewPos = newPos - previousPos;
            float previousToNewDist = Vector3.Distance(previousPos, newPos);

            if (AbleHit)
            {
                RaycastHit hit;
                if (Physics.Raycast(previousPos, previousToNewPos / previousToNewDist, out hit, previousToNewDist))
                {
                    if (Time.time > m_time2 + HitDelay)
                    {
                        m_time2 = Time.time;
                        HitObj(hit);
                        PoolManagerSingleton.Instance.Discard(gameObject);
                    }
                }
            }
        }

        void HitObj(RaycastHit hit)
        {
            if (useInPoolManager)
            {
                m_makedObject = PoolManagerSingleton.Instance.Create(m_hitObject, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else
            {
                m_makedObject = Instantiate(m_hitObject, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
            }
        }
    }
}
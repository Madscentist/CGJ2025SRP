using UnityEngine;

namespace AI
{
    public class JSONPetsName : MonoBehaviour {

        // Start is called before the first frame update

        private float nextPollTime = 0f;

        // Update is called once per frame

        void Update()
        {
            if (Time.time >= nextPollTime)
            {
                nextPollTime = Time.time + 2f;
                //Debug.Log("�ֶ���ѯ - ʱ��: " + Time.time);
                // ��ѯ�߼�
            }
        }
    }
}

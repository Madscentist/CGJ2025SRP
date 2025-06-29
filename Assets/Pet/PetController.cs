using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class PetController : MonoBehaviour
    {
        public Animator animator;

        [Header("球面移动设置")] public Transform sphereCenter; // 球心
        public float moveSpeed = 2f;

        public int heavy = 1;

        [Header("随机走停设置")] public float walkTime = 3f; // 行走时长
        public float idleTime = 2f; // 停留时长

        private bool isWalk;
        private Vector3 moveDirection;
        private Vector3 originScale;
        private PetsController _petsController;

        private void Start()
        {
            originScale = transform.localScale;
            sphereCenter = FindObjectOfType<Gravety2Sphere>().centerOfMass;
            _petsController = FindObjectOfType<PetsController>();
            StartCoroutine(WalkIdleLoop());
        }

        private void Update()
        {
            animator.SetBool("walk", isWalk || _petsController.IsRotating());

            if (isWalk)
            {
                Move();
            }
        }

        IEnumerator WalkIdleLoop()
        {
            while (true)
            {
                PickRandomDirection();
                isWalk = true;

                yield return new WaitForSeconds(walkTime);

                isWalk = false;

                yield return new WaitForSeconds(idleTime);

                // Feed(2);
            }
        }

        void PickRandomDirection()
        {
            // 定义四个局部方向
            Vector3[] directions = new Vector3[]
            {
                transform.forward,
                -transform.forward,
                transform.right,
                -transform.right
            };

            // 随机选一个
            int index = Random.Range(0, directions.Length);
            moveDirection = directions[index];
        }

        void Move()
        {
            // 投影到当前切线平面
            Vector3 gravityUp = (transform.position - sphereCenter.position).normalized;
            Vector3 tangentDir = Vector3.ProjectOnPlane(moveDirection, gravityUp).normalized;

            transform.position += tangentDir * moveSpeed * Time.deltaTime;

            // 让角色面朝移动方向
            if (tangentDir != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(tangentDir, gravityUp);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 5f);
            }

            // ✅ 根据局部方向设置 localScale.x 翻转
            Vector3 localDir = transform.InverseTransformDirection(tangentDir);

            if (localDir.x < 0)
            {
                // 左走，X 轴反向
                transform.localScale = new Vector3(-Mathf.Abs(originScale.x), originScale.y, originScale.z);
            }
            else if (localDir.x > 0)
            {
                // 右走，X 轴正向
                transform.localScale = new Vector3(Mathf.Abs(originScale.x), originScale.y, originScale.z);
            }
        }

        public void Feed(int value)
        {
            heavy += value;
            // 保留翻转状态，只放大绝对值
            float flipSign = Mathf.Sign(transform.localScale.x);
            transform.localScale = new Vector3(flipSign * Mathf.Pow(heavy, 1f / 3f) * Mathf.Abs(originScale.x),
                Mathf.Pow(heavy, 1f / 3f) * originScale.y,
                Mathf.Pow(heavy, 1f / 3f) * originScale.z);
        }
    }
}
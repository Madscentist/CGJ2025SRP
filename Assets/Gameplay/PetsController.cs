using UnityEngine;

namespace Gameplay
{
    public class PetsController : MonoBehaviour
    {

        [Header("Settings")]
        [Tooltip("Tag to identify selectable objects")]
        public string selectableTag = "Pet"; // 可选中对象的Tag

        [Tooltip("The instance B that will be rotated")]
        public GameObject instanceB; // 需要被旋转的实例B

        [Tooltip("Rotation speed when right-clicking")]
        public float clickRotationSpeed = 2f; // 右键点击旋转速度

        [Tooltip("Rotation speed when dragging")]
        public float dragRotationSpeed = 2f; // 拖拽旋转速度

        [Header("States")]
        [SerializeField]
        private GameObject selectedObject; // 当前选中的对象
        private bool isDragging = false; // 是否正在拖拽
        private float rightClickHoldTime = 0f; // 右键按住时间
        private Vector2 dragStartAngle; // 拖拽起始角度

        public float rotationSpeed; // 旋转平滑速度
        public float focusSpeed;
        private Quaternion targetRotation; // 目标旋转
        private bool isRotating = false; // 是否正在旋转
        private bool isFocusing = false;

        void Start()
        {
            instanceB = GameObject.FindGameObjectsWithTag("Land")[0];
        }

        void Update()
        {

            
            HandleLeftClickSelection();
            HandleRightClickRotation();
            HandleRightClickDragRotation();
            SmoothRotate();
        }

        public bool IsRotating()
        {
            return isRotating;
        }
        
        // 取消选择
        void DeselectObject()
        {
            if (selectedObject != null)
            {
                selectedObject = null;
                isRotating = false;
            }
        }

        // 左键点击选择/取消选择
        void HandleLeftClickSelection()
        {
            if (Input.GetMouseButtonDown(0)) // 左键按下
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // 检查是否点击了可选中对象
                    if (hit.collider.CompareTag(selectableTag))
                    {
                        // 选中新对象
                        SelectObject(hit.collider.gameObject);
                        GetComponent<PetManager>().SelectPet(hit.collider.gameObject);
                    }
                    else
                    {
                        // 点击了其他对象，取消选择
                        DeselectObject();
                    }
                }
                else
                {
                    // 点击了空白处，取消选择
                    DeselectObject();
                    GetComponent<PetManager>().QuitQr();
                }
            }
        }

        // 选中对象
        void SelectObject(GameObject obj)
        {
            DeselectObject(); // 先取消当前选择

            selectedObject = obj;
            Vector3 sourceDirection = (selectedObject.transform.position - instanceB.transform.position).normalized;
            Vector3 targetDirection = new Vector3(0, 1, -4).normalized;
            targetRotation = Quaternion.FromToRotation(sourceDirection, targetDirection) * instanceB.transform.rotation;
            isFocusing = true;
        }

        void SmoothRotate()
        {

            if (isRotating)
            {
                
                Vector3 oldPosition = selectedObject.transform.position;
                // 平滑旋转到目标方向
                instanceB.transform.rotation = Quaternion.Slerp(
                    instanceB.transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
                selectedObject.transform.position = oldPosition;

                // 当旋转接近完成时停止
                if (Quaternion.Angle(instanceB.transform.rotation, targetRotation) < 0.1f)
                {
                    instanceB.transform.rotation = targetRotation;
                    selectedObject.transform.position = oldPosition;
                    isRotating = false;
                }
            }
            else if (isFocusing)
            {
                // 平滑旋转到目标方向
                instanceB.transform.rotation = Quaternion.Slerp(
                    instanceB.transform.rotation,
                    targetRotation,
                    Time.deltaTime * focusSpeed
                );

                // 当旋转接近完成时停止
                if (Quaternion.Angle(instanceB.transform.rotation, targetRotation) < 0.1f)
                {
                    instanceB.transform.rotation = targetRotation;
                    isFocusing = false;
                }
            }
        }

        // 右键点击旋转（短按）
        void HandleRightClickRotation()
        {
            if (selectedObject != null && Input.GetMouseButtonUp(1) && !isDragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == instanceB)
                {
                    // 计算从球心到点击点的向量
                    Vector3 hitPointDirection = (hit.point - instanceB.transform.position).normalized;

                    // 计算需要旋转到的目标方向（摄像机前方）
                    Vector3 targetDirection = -Camera.main.transform.forward;
                    // 计算从当前点到目标点的旋转
                    targetRotation = Quaternion.FromToRotation(hitPointDirection, targetDirection) * instanceB.transform.rotation;
                    isRotating = true;
                }
            }

        }

        // 左键拖拽旋转（长按）
        void HandleRightClickDragRotation()
        {
            if (Input.GetMouseButton(0))
            {
                rightClickHoldTime += Time.deltaTime;

                // 按住超过0.3秒视为开始拖拽
                if (rightClickHoldTime >= 0.1f)
                {
                    if (!isDragging)
                    {
                        // 开始拖拽
                        isDragging = true;
                        dragStartAngle = Input.mousePosition;
                    }
                    else
                    {
                        // 持续拖拽
                        Vector2 currentAngle = Input.mousePosition;
                        Vector2 angleDelta = currentAngle - dragStartAngle;

                        //2. 旋转instanceB
                        instanceB.transform.Rotate(Vector3.up, -angleDelta.x * dragRotationSpeed, Space.World);
                        instanceB.transform.Rotate(Vector3.right, angleDelta.y * dragRotationSpeed, Space.World);

                        dragStartAngle = currentAngle;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                rightClickHoldTime = 0f;
            }
        }
    }
}

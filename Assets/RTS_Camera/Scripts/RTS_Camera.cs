using UnityEngine;
using System.Collections;

namespace RTS_Cam
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RTS Camera")]
    public class RTS_Camera : MonoBehaviour
    {

        #region Foldouts

#if UNITY_EDITOR

        public int lastTab = 0;

        public bool movementSettingsFoldout;
        public bool zoomingSettingsFoldout;
        public bool rotationSettingsFoldout;
        public bool heightSettingsFoldout;
        public bool mapLimitSettingsFoldout;
        public bool targetingSettingsFoldout;
        public bool inputSettingsFoldout;

#endif

        #endregion

        private Transform m_Transform; //camera tranform
        private Camera m_camera;
        public bool useFixedUpdate = false; //use FixedUpdate() or Update()

        #region Movement

        public float keyboardMovementSpeed = 5f; //speed with keyboard movement
        public float screenEdgeMovementSpeed = 3f; //spee with screen edge movement
        public float followingSpeed = 5f; //speed when following a target
        public float rotationSped = 3f;
        public float panningSpeed = 10f;
        public float mouseRotationSpeed = 10f;
        public float fixedAngle = -90f;
        public float fixedTranslation = 15f;

        #endregion

        #region Height

        public bool autoHeight = true;
        public LayerMask groundMask = -1; //layermask of ground or other objects that affect height

        public float maxHeight = 10f; //maximal height
        public float minHeight = 15f; //minimnal height
        public float heightDampening = 5f; 
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 25f;

        private float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

        #endregion

        #region MapLimits

        public bool limitMap = true;
        public Vector2 limitX = new Vector2(-50f,50f); //x limit of map
        public Vector2 limitY = new Vector2(-50f,50f); //z limit of map

        #endregion

        #region Targeting

        public Transform targetFollow; //target to follow
        public Vector3 targetOffset;

        /// <summary>
        /// are we following target
        /// </summary>
        public bool FollowingTarget
        {
            get
            {
                return targetFollow != null;
            }
        }

        #endregion

        #region Input

        public bool useScreenEdgeInput = true;
        public float screenEdgeBorder = 25f;

        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        public bool usePanning = true;
        public KeyCode panningKey = KeyCode.Mouse2;

        public bool useKeyboardZooming = true;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;

        public bool useScrollwheelZooming = true;
        public string zoomingAxis = "Mouse ScrollWheel";

        public bool useKeyboardRotation = true;
        public KeyCode rotateRightKey = KeyCode.X;
        public KeyCode rotateLeftKey = KeyCode.Z;
        public bool fixedRotation;

        public bool useMouseRotation = true;
        public KeyCode mouseRotationKey = KeyCode.Mouse1;

        private Vector2 KeyboardInput
        {
            get { return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero; }
        }

        private Vector2 MouseInput
        {
            get { return Input.mousePosition; }
        }

        private float ScrollWheel
        {
            get { return Input.GetAxis(zoomingAxis); }
        }

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);
                if (zoomIn && zoomOut)
                    return 0;
                else if (!zoomIn && zoomOut)
                    return 1;
                else if (zoomIn && !zoomOut)
                    return -1;
                else 
                    return 0;
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKeyDown(rotateRightKey);
                bool rotateLeft = Input.GetKeyDown(rotateLeftKey);
                if(rotateLeft && rotateRight)
                    return 0;
                else if(rotateLeft && !rotateRight)
                    return -1;
                else if(!rotateLeft && rotateRight)
                    return 1;
                else 
                    return 0;
            }
        }

        #endregion

        #region Unity_Methods

        private void Start()
        {
            m_Transform = transform;
            m_camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }

        #endregion

        #region RTSCamera_Methods

        /// <summary>
        /// update camera movement and rotation
        /// </summary>
        private void CameraUpdate()
        {
            if (FollowingTarget)
                FollowTarget();
            else
                Move();

            HeightCalculation();
            Rotation();
            LimitPosition();
        }

        /// <summary>
        /// move camera with keyboard or with screen edge
        /// </summary>
        private void Move()
        {
            if (useKeyboardInput)
            {
                Vector3 desiredMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);

                desiredMove *= keyboardMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, m_Transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (useScreenEdgeInput)
            {
                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, m_Transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }       
        
            if(usePanning && Input.GetKey(panningKey) && MouseAxis != Vector2.zero)
            {
                Vector3 desiredMove = new Vector3(-MouseAxis.x, 0, -MouseAxis.y);

                desiredMove *= panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, m_Transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }
        }

        /// <summary>
        /// calcualte height
        /// </summary>
        private void HeightCalculation()
        {
            /*if (m_camera.orthographic) {
                if(useScrollwheelZooming)
                    zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
                if (useKeyboardZooming)
                    zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;
            } else {*/
                float distanceToGround = DistanceToGround();
                if(useScrollwheelZooming)
                    zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
                if (useKeyboardZooming)
                    zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;

                zoomPos = Mathf.Clamp01(zoomPos);

                float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomPos);
                float difference = 0; 

                if(distanceToGround != targetHeight)
                    difference = targetHeight - distanceToGround;

                m_Transform.position = Vector3.Lerp(m_Transform.position, 
                    new Vector3(m_Transform.position.x, targetHeight + difference, m_Transform.position.z), Time.deltaTime * heightDampening);
            //}
        }

        enum CameraPosition { SE, NE, NW, SW }
        CameraPosition m_camPos = CameraPosition.SE;

        /// <summary>
        /// rotate camera
        /// </summary>
        private void Rotation()
        {
            if(useKeyboardRotation) {
                if (fixedRotation) {
                    float translationValue = fixedTranslation * RotationDirection;
                    switch (m_camPos) {
                        case CameraPosition.NW :
                            translationValue *= -1;
                            goto case CameraPosition.SE;
                        case CameraPosition.SE :
                            if (RotationDirection > 0) {
                                m_Transform.Translate(Vector3.forward*translationValue);
                                limitY += new Vector2(translationValue,translationValue);
                            } else if (RotationDirection < 0) {
                                m_Transform.Translate(Vector3.right*translationValue);
                                limitX += new Vector2(translationValue,translationValue);
                            }
                            break;

                        case CameraPosition.SW :
                            translationValue *= -1;
                            goto case CameraPosition.NE;
                        case CameraPosition.NE :
                            if (RotationDirection > 0) {
                                m_Transform.Translate(Vector3.right*-translationValue);
                                limitX += new Vector2(-translationValue,-translationValue);
                            } else if (RotationDirection < 0) {
                                m_Transform.Translate(Vector3.forward*translationValue);
                                limitY += new Vector2(translationValue,translationValue);
                            }
                            break;
                        default :
                        break;
                    }

                    m_Transform.Rotate(Vector3.up, fixedAngle*RotationDirection, Space.World);
                    m_camPos += RotationDirection;
                    if (m_camPos.GetHashCode() < 0 ) m_camPos = CameraPosition.SW;
                    if (m_camPos.GetHashCode() > 3 ) m_camPos = CameraPosition.SE;

                } else {
                    transform.Rotate(Vector3.up, RotationDirection * Time.deltaTime * rotationSped, Space.World);
                }
            }

            if (useMouseRotation && Input.GetKey(mouseRotationKey))
                m_Transform.Rotate(Vector3.up, -MouseAxis.x * Time.deltaTime * mouseRotationSpeed, Space.World);
        }

        /// <summary>
        /// follow targetif target != null
        /// </summary>
        private void FollowTarget()
        {
            Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z) + targetOffset;
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
        }

        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            if (!limitMap)
                return;
                
            m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, limitX.x, limitX.y),
                m_Transform.position.y,
                Mathf.Clamp(m_Transform.position.z, limitY.x, limitY.y));

            //m_Transform.eulerAngles = new Vector3(m_Transform.eulerAngles.x, ClampAngle(m_Transform.eulerAngles.y, -90, 90), m_Transform.eulerAngles.z);
        }

        private float ClampAngle(float angle, float from, float to)
        {
            if (angle < 0f) angle = 360 + angle;
            if (angle > 180f) return Mathf.Max(angle, 360+from);
            return Mathf.Min(angle, to);
        }

        /// <summary>
        /// set the target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            targetFollow = target;
        }

        /// <summary>
        /// reset the target (target is set to null)
        /// </summary>
        public void ResetTarget()
        {
            targetFollow = null;
        }

        /// <summary>
        /// calculate distance to ground
        /// </summary>
        /// <returns></returns>
        private float DistanceToGround()
        {
            Ray ray = new Ray(m_Transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, groundMask.value))
                return (hit.point - m_Transform.position).magnitude;

            return 0f;
        }

        #endregion
    }
}
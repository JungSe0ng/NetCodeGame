using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SK
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;

        public Camera cameraObject;
        public PlayerManager player;
        [SerializeField] Transform cameraPivotTransform;

        // CHANGE THESE TO TWEAK CAMERA PERFORMANCE
        [Header("Camera Stteings")]
        private float cameraSmoothSpeed = 1f;   // THE BIGGER THIS NUMBER, THE LONGER FOR THE CAMERA TO REACH ITS POSITION DURING MOVEMENT
        [SerializeField]
        private float leftAndRightRotatitonSpeed = 220f;
        [SerializeField]
        private float upAndDownRotatitonSpeed = 220f;
        [SerializeField]
        private float minimunPivot = -30f;
        [SerializeField]
        private float maximumPivot = 60f;
        [SerializeField]
        private float cameraCollisionRadius = 0.2f;
        [SerializeField]
        private LayerMask collideWithLayers;


        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition; // USED FOR CAMERA COLLISIONS (MOVES THE CAMERA OBJECT TO THIS POSITION UPON COLLIDING)
        [SerializeField]
        private float leftAndRightLookAngle;
        [SerializeField]
        private float upAndDownLookAngle;
        private float cameraZPosition;    // VALUES USED FOR CAMERA COLLISIONS
        private float targetCameraZPosition;     // VALUES USED FOR CAMERA COLLISIONS

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            cameraZPosition = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraActions()
        {
            if (player != null)
            {
                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        private void HandleRotations()
        {
            // IF LOCKED ON, FORCE ROTATION TOWARDS TARGET
            // ELSE ROTATE REGULARLY

            // ROTATE LEFT AND RIGHT BASED ON HORIZONTAL MOVEMENT ON THE RIGHT JOYSTICK
            leftAndRightLookAngle += (PlayerInputManager.instacne.cameraHorizontalInput * leftAndRightRotatitonSpeed) * Time.deltaTime;
            // ROTATE UP AND DOWN BASED ON VERTICAL MOVEMENT ON THE RIGHT JOYSTICK
            upAndDownLookAngle -= (PlayerInputManager.instacne.cameraVerticalInput * upAndDownRotatitonSpeed) * Time.deltaTime;
            // CLAMP THE UP AND DOWN LOOK ANGLE BETWEEN A MIN AND MAX VALUE
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimunPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotatiton;

            // ROTATE THIS GAMEOBJECT LEFT AND RIGHT
            cameraRotation.y = leftAndRightLookAngle;
            targetRotatiton = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotatiton;

            // ROTATE THIS PIVOT GAMEOBJECT UP AND DOWN
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotatiton = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotatiton;

        }

        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;
            RaycastHit hit;
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction , out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
            {
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            if(Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SK
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;

        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection; 

        [SerializeField] private float walkingSpeed = 2f;
        [SerializeField] private float runningSpeed = 5f;
        [SerializeField] private float rotationSpeed = 15f;
        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
            // AERIAL MOVEMENT
        }

        private void GetVerticalAndHorizontalInputs()
        {
            verticalMovement = PlayerInputManager.instacne.verticalInput;
            horizontalMovement = PlayerInputManager.instacne.horizontalInput;

            // CLAMP THE MOVEMENTS
        }

        private void HandleGroundedMovement()
        {

            GetVerticalAndHorizontalInputs();

            // OUR MOVE DIRECTION IS BASED ON OUR CAMERAS FACING PERSPECTIVE & OUR MOVEMENT INPUTS
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0f;

            if (PlayerInputManager.instacne.moveAmount > 0.5f)
            {
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.instacne.moveAmount <= 0.5f)
            {
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0f;

            if(targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Camera_Controllers
{
    public class CameraController : MonoBehaviour
    {
        public PlayerInput input;

        public float moveSpeed;
        public float lookSpeed;
        public float zoomSpeed;
        public float minVerticalAngle = -80f; // Prevent looking too far down
        public float maxVerticalAngle = 80f; // Prevent looking too far up

        public bool isFreeCameraActive;

        [SerializeField] private List<GameObject> units = new List<GameObject>();
        private List<Camera> unitCameras = new List<Camera>();
        private int currentCameraIndex = 0;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private float zoomDirection = 0f;

        private Transform cameraTransform;
        private float pitch;

        public void OnMove(CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();

            if (moveInput.magnitude > 0.1f && !isFreeCameraActive)
            {
                ActivateFreeCamera();
            }
        }

        public void OnLook(CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        public void OnZoomIn(CallbackContext context)
        {
            if (context.started) zoomDirection = 1f;
            if (context.canceled) zoomDirection = 0f;
        }

        public void OnZoomOut(CallbackContext context)
        {
            if (context.started) zoomDirection = -1f;
            if (context.canceled) zoomDirection = 0f;
        }

        public void OnNextCamera(CallbackContext context)
        {
            if (context.performed)
            {
                CycleUnitCamera(1);
            }
        }

        public void OnPreviousCamera(CallbackContext context)
        {
            if (context.performed)
            {
                CycleUnitCamera(-1);
            }
        }

        public void CycleUnitCamera(int direction)
        {
            unitCameras.RemoveAll(cam => cam == null);
        
            if (unitCameras.Count == 0) return;

            currentCameraIndex = (currentCameraIndex + direction + unitCameras.Count) % unitCameras.Count;

            // Disable all cameras except the selected one
            for (int i = 0; i < unitCameras.Count; i++)
            {
                unitCameras[i].gameObject.SetActive(i == currentCameraIndex);
            }

            isFreeCameraActive = false;
            gameObject.GetComponent<Camera>().enabled = false; // Disable free camera
        }

        private void ActivateFreeCamera()
        {
            unitCameras.RemoveAll(cam => cam == null);
            
            foreach (Camera cam in unitCameras)
            {
                cam.gameObject.SetActive(false);
            }

            gameObject.GetComponent<Camera>().enabled = true;
            isFreeCameraActive = true;
        }

        private void AssignCameras()
        {
            if (units.Count > 0)
            {
                foreach (GameObject unit in units)
                {
                    Camera newCamera = unit.GetComponentInChildren<Camera>();
                    unitCameras.Add(newCamera);
                }
            }
        }

        public void SetUp()
        {
            foreach (GameObject unit in GameManager.Instance.units)
            {
                units.Add(unit);
            }

            AssignCameras();
            ActivateFreeCamera();
        }

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (!isFreeCameraActive)
            {
                cameraTransform.position = unitCameras[currentCameraIndex].transform.position;
                cameraTransform.rotation = unitCameras[currentCameraIndex].transform.rotation;
                return;
            }
            // Movement
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            cameraTransform.position += cameraTransform.TransformDirection(moveDirection) * (moveSpeed * Time.deltaTime);
        
            // Rotation
            cameraTransform.Rotate(Vector3.up, lookInput.x * lookSpeed, Space.World);
            pitch -= lookInput.y * lookSpeed;
            pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
            cameraTransform.localRotation = Quaternion.Euler(pitch, cameraTransform.eulerAngles.y, 0f);
        
            // Zoom
            cameraTransform.position += cameraTransform.forward * (zoomDirection * zoomSpeed * Time.deltaTime);
        }
    }
}

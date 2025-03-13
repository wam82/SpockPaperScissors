using System.Collections.Generic;
using AI_Foundation;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Camera_Controllers
{
    public class UnitPlacement : MonoBehaviour
    {
        public PlayerInput input;
    
        [Header("Camera Settings")] 
        // public Camera placementCamera;
        public Vector2 movementBounds = new Vector2(15, 15);
        private Vector2 originalPosition;
        public float moveSpeed;
        public float rotateSpeed;
        public float zoomSpeed;
        public float minCameraY;
        public float maxCameraY;

        [Header("Unit Placement Settings")] 
        public GameObject faction;
        public Transform ghost;
        public float unitY;
        public Vector3 offset = new Vector3(5f, 0, 0);
        private List<GameObject> units = new List<GameObject>();
        private List<Camera> unitCameras = new List<Camera>();
        private int unitIndex = 0;

        private Vector2 moveInput;
        private float zoomInput;
        private float rotateInput;

        public void OnMove(CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void OnRotate(CallbackContext context)
        {
            rotateInput = context.ReadValue<Vector2>().x;
        }
    
        public void OnZoomIn(CallbackContext context)
        {
            if (context.started) zoomInput = 1f;
            if (context.canceled) zoomInput = 0f;
        }

        public void OnZoomOut(CallbackContext context)
        {
            if (context.started) zoomInput = -1f;
            if (context.canceled) zoomInput = 0f;
        }

        public void OnPlace(CallbackContext context)
        {
            if (context.performed)
            {
                if (unitIndex < units.Count)
                {
                    Place();
                }
                else
                {
                    ConfirmPlacement();
                }
            }
        }

        public void OnUndo(CallbackContext context)
        {
            if (context.performed)
            {
                if (unitIndex >= units.Count)
                {
                    ghost.gameObject.SetActive(true);
                }
                Undo();
            }
        }

        public void SetUp()
        {
            faction = GameManager.Instance.playerFaction;
        
            originalPosition = new Vector2(faction.transform.position.x, faction.transform.position.z);
        
            foreach (Transform child in faction.transform)
            {
                units.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }

        private void DisplayUnit(GameObject unit)
        {
            if (unit == null)
            {
                Debug.LogWarning("Unit is null");
                return;
            }

            Camera unitCamera = unit.GetComponentInChildren<Camera>();
            if (unitCamera != null)
            {
                unitCameras.Add(unitCamera);
                unitCamera.enabled = false;
            }
            unit.SetActive(true);
            unit.GetComponent<IndividualAI>().enabled = false;
        }

        private void HandleCameraMovement()
        {
            // Calculate camera's local right and forward directions (flattened on the XZ plane)
            Vector3 cameraRight = transform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            Vector3 cameraForward = transform.up;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            // Compute movement based on camera's orientation
            Vector3 deltaMovement = (cameraRight * moveInput.x + cameraForward * moveInput.y) * (moveSpeed * Time.deltaTime);
            Vector3 newPosition = transform.position + deltaMovement;
    
            // Clamp position within the movement bounds (assuming originalPosition.x and originalPosition.y represent world X and Z)
            newPosition.x = Mathf.Clamp(newPosition.x, originalPosition.x - movementBounds.x, originalPosition.x + movementBounds.x);
            newPosition.z = Mathf.Clamp(newPosition.z, originalPosition.y - movementBounds.y, originalPosition.y + movementBounds.y);
    
            transform.position = newPosition;
    
            // Handle zooming
            transform.position += transform.forward * (zoomInput * zoomSpeed * Time.deltaTime);
            Vector3 clampedPosition = transform.position;
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minCameraY, maxCameraY);
            transform.position = clampedPosition;
    
            transform.Rotate(0f, 0f, -rotateInput * rotateSpeed * Time.deltaTime);
        }


        private void UpdateGhost()
        {
            Vector3 ghostPosition = transform.position;
            ghostPosition.y = unitY;
        
            // Grid snapping would go here
            // Example: ghostPos.x = Mathf.Round(ghostPos.x / gridSize) * gridSize;
        
            ghost.position = ghostPosition;
            ghost.Rotate(0f, rotateInput * rotateSpeed * Time.deltaTime, 0f);
        }

        private void Place()
        {
            if (unitIndex >= units.Count)
            {
                Debug.LogWarning("All units have been placed");
                return;
            }
        
            GameObject unit = units[unitIndex];

            if (unit == null)
            {
                Debug.LogError("Selected unit is null");
                return;
            }
        
            Vector3 position = ghost.position;
            position.y = 0;
            unit.transform.position = position;
            unit.transform.rotation = ghost.rotation;

            unitIndex++;

            transform.position += offset;

            if (unitIndex >= units.Count)
            {
                ghost.gameObject.SetActive(false);
            }
        
            DisplayUnit(unit);
        }

        private void Undo()
        {
            if (unitIndex <= 0)
            {
                Debug.LogWarning("No unites to undo");
                return;
            }

            unitIndex--;
            GameObject unit = units[unitIndex];

            if (unit == null)
            {
                Debug.LogError("Unit is null");
                return;
            }
        
            transform.position = new Vector3(unit.transform.position.x, transform.position.y, unit.transform.position.z);
        
            HideUnit(unit);
        }

        private void HideUnit(GameObject unit)
        {
            if (unit == null)
            {
                return;
            }
            unit.SetActive(false);
        }

        private void ConfirmPlacement()
        {
            foreach (Camera cam in unitCameras)
            {
                cam.enabled = true;
            }
            GameManager.Instance.CompleteSetup();
        }

        public List<GameObject> GetUnits()
        {
            return units;
        }

        private void Update()
        {
            HandleCameraMovement();
            UpdateGhost();
        }
    }
}

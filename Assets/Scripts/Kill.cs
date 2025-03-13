using AI_Foundation;
using Camera_Controllers;
using UnityEngine;

public class Kill : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (IsTarget(other.transform))
        {
            Camera cam = other.gameObject.GetComponentInChildren<Camera>();
            if (cam != null && cam.enabled)
            {
                GameManager.instance.mainCamera.GetComponent<CameraController>().CycleUnitCamera(1);
            }
            PursuitRegistry.Instance.DestroyPursuit(other.transform);
            GameManager.instance.RemovePlayerUnit(other.gameObject);
            Destroy(other.gameObject);
        }
    }
    
    private bool IsTarget(Transform other)
    {
        foreach (string targetTag in transform.GetComponent<IndividualAI>().targetTags)
        {
            if (other.CompareTag(targetTag))
            {
                return true;
            }
        }
        return false;
    }
}

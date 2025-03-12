using AI_Foundation;
using UnityEngine;

public class Kill : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (IsTarget(other.transform))
        {
            PursuitRegistry.Instance.DestroyPursuit(other.transform);
            GameManager.Instance.RemovePlayerUnit(other.gameObject);
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

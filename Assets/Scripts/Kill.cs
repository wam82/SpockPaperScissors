using UnityEngine;

public class Kill : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (IsTarget(other.transform))
        {
            PursuitRegistry.Instance.DestroyPursuit(other.transform);
            Destroy(other.gameObject);
        }
    }
    
    private bool IsTarget(Transform other)
    {
        foreach (string targetTag in transform.GetComponent<IndividualAI>().validTargetTags)
        {
            if (other.CompareTag(targetTag))
            {
                return true;
            }
        }
        return false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (isTarget(other.transform))
        {
            PursuitRegistry.Instance.DestroyPursuit(other.transform);
            Destroy(other.gameObject);
        }
    }
    
    private bool isTarget(Transform other)
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

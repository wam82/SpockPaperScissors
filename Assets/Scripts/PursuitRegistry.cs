using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitRegistry : MonoBehaviour
{
    private static PursuitRegistry instance;

    public static PursuitRegistry Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("PursuitRegistry");
                instance = go.AddComponent<PursuitRegistry>();
            }
            
            return instance;
        }
    }
    
    private Dictionary<Transform, Transform> pursuits = new();
    private Dictionary<Transform, Transform> latestTargets = new();

    public void RegisterPursuit(Transform flee, Transform pursuit)
    {
        if (!pursuits.ContainsKey(flee))
        {
            pursuits.Add(flee, pursuit);
        }
        // Debug.Log("Pursuit registered for: " + flee.name + " & " + pursuit.name);
    }

    public void RemovePursuit(Transform flee)
    {
        if (pursuits.ContainsKey(flee))
        {
            Transform pursuer = pursuits[flee];
            pursuits.Remove(flee);

            if (!latestTargets.ContainsKey(pursuer))
            {
                latestTargets.Add(pursuer, flee);
            }
        }
    }

    public void ClearPursuer(Transform pursuer)
    {
        if (latestTargets.ContainsKey(pursuer))
        {
            latestTargets.Remove(pursuer);
        }
    }

    public bool WasPursuing(Transform unit)
    {
        return latestTargets.ContainsKey(unit);
    }

    public void DestroyPursuit(Transform flee)
    {
        
        List<Transform> toRemove = new List<Transform>();

        foreach (KeyValuePair<Transform, Transform> kvp in pursuits)
        {
            if (ReferenceEquals(kvp.Value, flee))
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (Transform key in toRemove)
        {
            Transform pursuer = pursuits[key];
            pursuits.Remove(key);
            latestTargets.Remove(key);
        }
    }
    
    
}

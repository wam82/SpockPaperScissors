using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject parent;
    public float radius = 100f;
    public int segmentCount = 20;
    public float wallThickness = 1f;

    void Start() {
        CreateWall();
    }

    void CreateWall() {
        float angleStep = 360f / segmentCount;

        for (int i = 0; i < segmentCount; i++) {
            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 position = new Vector3(Mathf.Cos(radians) * radius, 0, Mathf.Sin(radians) * radius);
            GameObject wallSegment = Instantiate(wallPrefab, position, Quaternion.identity);
            wallSegment.transform.LookAt(new Vector3(0, wallSegment.transform.position.y, 0));
            wallSegment.transform.position += wallSegment.transform.forward * (wallThickness / 2);

            wallSegment.transform.parent = parent.transform;
        }
    }
}

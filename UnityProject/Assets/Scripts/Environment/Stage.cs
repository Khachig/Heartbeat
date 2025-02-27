using UnityEngine;
using System.Collections.Generic;

// Script for Stage information and movement
public class Stage : MonoBehaviour
{
    public int numLanes = 4;
    public float tunnelRadius = 3f;
    public float speed = 50f;
    public float rotationSpeed = 10f;
    public GameObject tube;

	private Vector3 targetPoint;
	private Quaternion targetRotation;
    private List<Vector3> points;
	private int pointsIdx;

	void Start () {
        points = new List<Vector3>{};
        if (tube)
        {
            for(int i = 0; i < tube.transform.childCount; i++)
            {
                Transform point = tube.transform.GetChild(i);
                points.Add(point.position);
            }
        }
		pointsIdx = 0;
        SetTargetPoint();
	}
	
	// Update is called once per frame
	void Update () {
        if (points.Count < 2)
            return;
		transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed *  Time.deltaTime);
		if (Vector3.Distance(transform.position, targetPoint) < 15f) 
		{
            SetTargetRotation();
		}
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f) 
		{
			pointsIdx = (pointsIdx + 1) % points.Count;
            SetTargetPoint();
		}
	}

    void SetTargetPoint() {
        targetPoint = points[pointsIdx];
    }

    void SetTargetRotation() {
        targetRotation = Quaternion.LookRotation(points[(pointsIdx + 3) % points.Count] - transform.position);
    }
}

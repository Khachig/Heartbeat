using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

// Script for Stage information and movement
public class Stage : MonoBehaviour
{
    public int numLanes = 4;
    public float tunnelRadius = 3f;
    public float speed = 50f;
    public float rotationSpeed = 10f;

	private Vector3 targetPoint;
	private Quaternion targetRotation;
    private List<Vector3> points;
	private int pointsIdx;

    // To be used by tunnel generation to set points for camera path
    public void SetPoints(List<Vector3> newPoints) {
        this.points = newPoints;
        SetTargetPoint();
    }

	void Start () {
        points = new List<Vector3>{Vector3.zero};
		pointsIdx = 0;
        SetTargetPoint();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed *  Time.deltaTime);
		if (Vector3.Distance(transform.position, targetPoint) < 15f) 
		{
            SetTargetRotation();
		}
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f) 
		{
			pointsIdx++;
            SetTargetPoint();
		}
	}

    void SetTargetPoint() {
        Assert.IsTrue(pointsIdx < points.Count); 
        targetPoint = points[pointsIdx];
    }

    void SetTargetRotation() {
        Assert.IsTrue(pointsIdx + 1 < points.Count); 
        targetRotation = Quaternion.LookRotation(points[pointsIdx + 1] - transform.position);
    }
}

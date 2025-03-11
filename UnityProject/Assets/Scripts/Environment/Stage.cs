using UnityEngine;
using System.Collections.Generic;

// Script for Stage information and movement
public class Stage : MonoBehaviour
{
    public int numLanes = 4;
    public float tunnelRadius = 3f;
    public float speed = 50f;
    public float rotationSpeed = 10f;
    public float judgementLineOffset = 10f;
    public GameObject tube;
    public GameObject offLimitLanePrefab;

    private GameObject[] offLimitLanes;
	private Vector3 targetPoint;
	private Quaternion targetRotation;
    private List<Vector3> points;
	private int pointsIdx;

	private static Stage instance;

    private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start () {
        offLimitLanes = new GameObject[numLanes];
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

    private Vector3 GetXYPosForLane(int laneIdx)
    {
        float angleStep = 360f / numLanes;
        // -90 bc we want lane 0 to be at bottom of screen
        float angle = (angleStep * laneIdx - 90) * Mathf.Deg2Rad;
        Vector3 newposition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        newposition = newposition.normalized * tunnelRadius;
        return newposition;
    }
    private int GetModLane(int lane)
    {
        return ((lane % numLanes) + numLanes) % numLanes;
    }

    private void SetTargetPoint() {
        targetPoint = points[pointsIdx];
    }

    private void SetTargetRotation() {
        targetRotation = Quaternion.LookRotation(points[(pointsIdx + 3) % points.Count] - transform.position);
    }

    private void SpawnOffLimitLane(int lane)
    {
        if (offLimitLanes[lane] != null)
            return;

        Vector3 pos = GetXYPosForLane(lane) * 2f + Vector3.forward * 20f;
        float angleStep = 360f / numLanes;
        // -90 bc we want lane 0 to be at bottom of screen
        float angle = (angleStep * lane - 90);
        Quaternion rot = Quaternion.Euler(angle, 90, 0);
        GameObject newOffLimitLane = Instantiate(offLimitLanePrefab, pos, Quaternion.identity);
        newOffLimitLane.transform.parent = transform;
        newOffLimitLane.transform.localPosition = pos;
        newOffLimitLane.transform.localRotation = rot;
        offLimitLanes[lane] = newOffLimitLane;
    }

    private void DeSpawnOffLimitLane(int lane)
    {
        if (offLimitLanes[lane] == null)
            return;

        Destroy(offLimitLanes[lane]);
        offLimitLanes[lane] = null;
    }


    
    public static class Lanes
	{
		public static int GetNumLanes() => instance.numLanes;
		public static float GetJudgementLineOffset() => instance.judgementLineOffset;
		public static int GetModLane(int lane) => instance.GetModLane(lane);
		public static Vector3 GetXYPosForLane(int laneIdx) => instance.GetXYPosForLane(laneIdx);
        public static void SpawnOffLimitLane(int lane) => instance.SpawnOffLimitLane(lane);
        public static void DeSpawnOffLimitLane(int lane) => instance.DeSpawnOffLimitLane(lane);
        public static bool IsOffLimitLaneActive(int lane) => instance.offLimitLanes[lane] != null;
	}
}

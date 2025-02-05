using UnityEngine;

public class TubeGen : MonoBehaviour
{
    [Header("Line Settings")]
    public Vector3 startPoint = new Vector3(0, 0, 0);
    public Vector3 endPoint = new Vector3(0, 0, 2);  
    public float lineWidth = 0.1f;                   
    public Color lineColor = Color.red;              

    private LineRenderer lineRenderer;

    void Start()
    {
        GenerateLineSegment();
    }

    void GenerateLineSegment()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Set the positions of the line segment
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint); // Start point
        lineRenderer.SetPosition(1, endPoint);   // End point

        // Set the width of the line
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // Set the material (you can create a default material or assign one)
        if (lineRenderer.material == null)
        {
            lineRenderer.material = new Material(Shader.Find("Standard"));
        }

        // Set the color of the line
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }
}

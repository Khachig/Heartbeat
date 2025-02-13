using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

/// <summary>
/// An example class for how to use EasyRhythm for FMOD.
/// First, we have to add the interface IEasyListener so we can use the OnBeat and Marker methods.
/// </summary>
public class ShapeController : MonoBehaviour, IEasyListener
{
    // SHAPE CONTROL

    // We will animate a cube using callbacks from the FMOD event (myAudioEvent)
    public GameObject cube; 
    public float animTime = 0.1f;
    private float timer = 0;
    public float scaleSpeed = 0.05f;

    public Color[] cubeColors;
    private int currentColor = 0;

    public GameObject sphere;
    public GameObject cylinder;
    public Mesh capsule;

    public void Update()
    {
        timer += Time.deltaTime; // A timer for the cube animation
    }

    public void OnBeat(EasyEvent audioEvent)
    {
        // Resets the cube animation on beats 1 and 3
        if (audioEvent.CurrentBeat == 1 || audioEvent.CurrentBeat == 3)
        {
            ToggleCubeColor();
            StartCoroutine(AnimateCube());
            
        }   
        
        if (audioEvent.CurrentBeat == 1)
        {
            StartCoroutine(AnimateSphere(audioEvent));
            StartCoroutine(AnimateCylinder(audioEvent));
        }
    }

    public void ToggleCubeColor()
    {
        currentColor = 1 - currentColor;
        cube.GetComponent<Renderer>().material.color = cubeColors[currentColor];
    }

    public void CylinderToCapsule()
    {
        cylinder.GetComponent<MeshFilter>().mesh = capsule;
    }

    // A quick way to animate the cube
    // However, we recommened using the DOTween library available on the Unity Asset Store for animating objects!
    private IEnumerator AnimateCube()
    {
        timer = 0;
        yield return null;

        timer = Time.deltaTime;
        cube.transform.localScale = Vector3.one;

        timer += Time.deltaTime; // A timer for the cube animation

        var cubeTrans = cube.transform;
        Vector3 newScale = cube.transform.localScale;
        float cubeSize = cube.transform.localScale.x;

        while (timer > 0)
        {
            cubeSize += scaleSpeed / 100;
            newScale = new Vector3(cubeSize, cubeSize, cubeSize);
            cubeTrans.localScale = newScale;
            yield return null;
        }
    }

    private IEnumerator AnimateSphere(EasyEvent audioEvent)
    {
        var journeyTime = audioEvent.BeatLength() * 2;

        var minSize = Vector3.one;
        var maxSize = Vector3.one * 1.5f;

        sphere.transform.localScale = minSize;

        float startTime = Time.time;

        float timeSinceStarted = Time.time - startTime;
        float percentageComplete = timeSinceStarted / journeyTime;

        while (percentageComplete <= 1)
        {
            timeSinceStarted = Time.time - startTime;
            percentageComplete = timeSinceStarted / journeyTime;

            var newSize = Vector3.SlerpUnclamped(minSize, maxSize, percentageComplete);

            sphere.transform.localScale = newSize;
            yield return null;
        }

        startTime = Time.time;
        percentageComplete = 0;

        while (percentageComplete <= 1)
        {
            timeSinceStarted = Time.time - startTime;
            percentageComplete = timeSinceStarted / journeyTime;

            var newSize = Vector3.SlerpUnclamped(maxSize, minSize, percentageComplete);

            sphere.transform.localScale = newSize;
            yield return null;
        }
    }

    private IEnumerator AnimateCylinder(EasyEvent audioEvent)
    {
        var journeyTime = audioEvent.BeatLength();

        var elapsedTime = 0f;

        var currentRotation = cylinder.transform.rotation;

        float startTime = Time.time;

        float timeSinceStarted = Time.time - startTime;
        float percentageComplete = timeSinceStarted / journeyTime;

        while (percentageComplete <= 1)
        {
            timeSinceStarted = Time.time - startTime;
            percentageComplete = timeSinceStarted / journeyTime;

            var zRotation = Mathf.Lerp(currentRotation.eulerAngles.z, currentRotation.eulerAngles.z - 90, percentageComplete);

            cylinder.transform.rotation = Quaternion.Euler(0, 0, zRotation);

            elapsedTime += Time.deltaTime / journeyTime;
            yield return null;
        }
    }
}

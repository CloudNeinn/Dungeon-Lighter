using Math.Spline;
using System.Collections.Generic;
using UnityEngine;

public class SplineFollower : MonoBehaviour
{
    [Header("Input")]
    public SplineBehaviour splineBehaviour;

    [Header("Settings")]
    public float speed = 5f;
    public bool playOnStart = true;
    public bool orientToPath = true;

    [Header("Debug")]
    public bool debug = false;

    private float distanceTravelled = 0f;
    private float totalLength = 0f;
    private bool isPlaying = false;

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    private void Update()
    {
        if (!isPlaying) return;
        if (splineBehaviour == null || splineBehaviour.GeneratedSplinePoints == null || splineBehaviour.GeneratedSplinePoints.Count < 2) return;

        distanceTravelled += speed * Time.deltaTime;

        if (splineBehaviour.closedLoop)
        {
            distanceTravelled %= GetTotalLength();
        }
        else
        {
            distanceTravelled = Mathf.Min(distanceTravelled, GetTotalLength());
        }

        FollowPath();
    }

    public void Play()
    {
        totalLength = 0f; // reset so it's recalculated fresh
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
    }

    public void ResetToStart()
    {
        distanceTravelled = 0f;
        totalLength = 0f;
    }

    private float GetTotalLength()
    {
        if (totalLength > 0f) return totalLength;

        var pts = splineBehaviour.GeneratedSplinePoints;
        totalLength = 0f;
        for (int i = 1; i < pts.Count; i++)
            totalLength += Vector3.Distance(pts[i - 1].position, pts[i].position);

        if (debug) Debug.LogFormat("[SplineFollower] Total spline length: {0}", totalLength);
        return totalLength;
    }

    private void FollowPath()
    {
        var pts = splineBehaviour.GeneratedSplinePoints;
        float accumulated = 0f;

        for (int i = 1; i < pts.Count; i++)
        {
            float segLen = Vector3.Distance(pts[i - 1].position, pts[i].position);

            if (accumulated + segLen >= distanceTravelled)
            {
                float t = segLen > 0f ? (distanceTravelled - accumulated) / segLen : 0f;

                transform.position = Vector3.Lerp(pts[i - 1].position, pts[i].position, t);

                if (orientToPath)
                {
                    Vector3 tangent = Vector3.Lerp(pts[i - 1].tangent, pts[i].tangent, t);
                    if (tangent != Vector3.zero)
                        transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
                }

                return;
            }

            accumulated += segLen;
        }

        // End of non-looping path — snap to last point
        CatmullRomSplinePoint last = pts[pts.Count - 1];
        transform.position = last.position;

        if (orientToPath && last.tangent != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(last.tangent, Vector3.up);

        if (!splineBehaviour.closedLoop)
        {
            isPlaying = false;
            if (debug) Debug.Log("[SplineFollower] Reached end of spline.");
        }
    }
}
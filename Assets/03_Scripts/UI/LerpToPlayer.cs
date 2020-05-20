using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attempts to follow the players gaze but not 1-1 we want a delay
/// </summary>
public class LerpToPlayer : MonoBehaviour
{
    public Transform FollowTarget;
    public Vector3 DistanceFromPlayer;
    public float LerpStep = .05f;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, (FollowTarget.position + FollowTarget.forward) + DistanceFromPlayer, LerpStep);
        transform.LookAt(2 * transform.position - FollowTarget.position);
    }
}

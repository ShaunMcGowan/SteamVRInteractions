using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Contains data about objects that can be grabbed
/// </summary>

[RequireComponent(typeof(Rigidbody),typeof(Collider))]
public class GrabableObject : MonoBehaviour
{
    public bool SnapOnGrab = false;
    public bool SnapBackToStart = false;

    private Vector3 startingPosition;

    private void Start()
    {
        startingPosition = transform.position;
    }
    public void SnapBack()
    {
        transform.position = startingPosition;
    }
}

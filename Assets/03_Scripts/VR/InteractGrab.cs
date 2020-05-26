using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerHand
{
    Left,
    Right
}
public class InteractGrab : MonoBehaviour
{
    public bool IsGrabbing = false;
    public ControllerHand Hand = ControllerHand.Left;
    private GameObject GrabbedObject;

    private Transform oldParent; // The parent of the object grabbed before it was grabbed
    private bool oldRigidBodyIsKinematic = false;
    private Rigidbody grabbedRB;

    // Used to calculate the average vel
    private Vector3 averageVeloctiy = Vector3.zero;
    private Vector3 lastFrameVelocity = Vector3.zero;



    private void Update()
    {
        if (IsGrabbing) // If an object is grabbed
        {
            CalculateVelocity();
        }
        CheckIfReleasing();
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleGrab(other.transform);
    }

    private void OnTriggerStay(Collider other)
    {
        HandleGrab(other.transform);
    }



    private void HandleGrab(Transform grabableObject)
    {
        if (!IsGrabbing && grabableObject.GetComponent<GrabableObject>() && Hand == ControllerHand.Left && VRInputManager.Instance.GetLeftGrip()) // Left Check
        {
            Grab(grabableObject.gameObject);
        }
        else if (!IsGrabbing && grabableObject.GetComponent<GrabableObject>() && Hand == ControllerHand.Right && VRInputManager.Instance.GetRightGrip()) // Right Check
        {
            Grab(grabableObject.gameObject);
        }
    }



    // Grabbing
    private void Grab(GameObject grabableObject)
    {
        IsGrabbing = true;
        GrabbedObject = grabableObject;
        grabbedRB = GrabbedObject.GetComponent<Rigidbody>();
        oldRigidBodyIsKinematic = grabbedRB.isKinematic;
        grabbedRB.isKinematic = true;
        oldParent = GrabbedObject.transform.parent;
        GrabbedObject.transform.parent = transform;
        if (GrabbedObject.GetComponent<GrabableObject>().SnapOnGrab)
        {
            GrabbedObject.transform.localPosition = Vector3.zero; // Centering Could add snap pos in the future
        }
    }

    private void CheckIfReleasing()
    {
        if (IsGrabbing && Hand == ControllerHand.Left && !VRInputManager.Instance.GetLeftGrip()) // Left Check
        {
            Release();
        }
        else if (IsGrabbing && Hand == ControllerHand.Right && !VRInputManager.Instance.GetRightGrip()) // Right Check
        {
            Release();
        }
    }

    // Releasing
    public void Release()
    {
        if (IsGrabbing)
        {
            GrabableObject grabScript = GrabbedObject.GetComponent<GrabableObject>();

            grabbedRB.isKinematic = oldRigidBodyIsKinematic;
           
            grabbedRB.AddForce(averageVeloctiy * 5, ForceMode.VelocityChange);
           
            GrabbedObject.transform.parent = oldParent;

            // Once reparent we snap back 
            if (grabScript.SnapBackToStart)
            {
                grabScript.SnapBack();
            }


            IsGrabbing = false;
            GrabbedObject = null;

            grabbedRB = null;
            averageVeloctiy = Vector3.zero;
            lastFrameVelocity = Vector3.zero;
        }
    }

   
    private void CalculateVelocity()
    {
        averageVeloctiy = ((transform.position - lastFrameVelocity)) / Time.deltaTime;
        lastFrameVelocity = transform.position;
    }

}

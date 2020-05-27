using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles grabbing object
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class InteractGrabObject : MonoBehaviour
{


    [Header("Grab Settings")]
    public int ControllerLayer = 12;
    public bool SnapToController = true;
    public bool SnapTonController2Hand = false;
    public bool HideControllerOnGrab = false;
    public bool HoldToGrab = true;
 
    public ButtonTypes GrabButton = ButtonTypes.Grip;

    // Hiden bool values
    [HideInInspector]
    public bool IsGrabbedLeft = false;
    [HideInInspector]
    public bool IsGrabbedRight = false;
    [HideInInspector]
    public bool IsGrabbed = false; // Used to make sure we cant grab it out of our hand
    [HideInInspector]
    public bool WasGrabbed = false; // True if held last frame





    private Transform oldParent;

    private Vector3 velocity = Vector3.zero;
    private Vector3 previousVelocity = Vector3.zero;

    private Vector3 torque = Vector3.zero;
    private Vector3 previousTorque = Vector3.zero;

    private Rigidbody rb;
    private bool isOldParentSet = false;

    private bool rightButtonPressed = false;
    private bool leftButtonPressed = false;

    private Controller rightController;
    private Controller leftController;

    private void Start()
    {
        leftController = VRPositionManager.Instance.LeftHand.GetComponent<Controller>();
        rightController = VRPositionManager.Instance.RightHand.GetComponent<Controller>();
        rb = GetComponent<Rigidbody>(); 
        if(rb == null)
        {
            Debug.LogError("No rigidbody attatched please attatch one in the inspector");
        }
    }

    private void UpdateGrabButton()
    {
        if (HoldToGrab)
        {
            if (GrabButton == ButtonTypes.Grip)
            {
                leftButtonPressed = VRInputManager.Instance.GetLeftGrip();
                rightButtonPressed = VRInputManager.Instance.GetRightGrip();
            }
            else if (GrabButton == ButtonTypes.Trigger)
            {
                leftButtonPressed = VRInputManager.Instance.GetLeftTrigger();
                rightButtonPressed = VRInputManager.Instance.GetRightTrigger();
            }
        }
        else
        {
            if (GrabButton == ButtonTypes.Grip)
            {
                leftButtonPressed = VRInputManager.Instance.GetLeftGripDown();
                rightButtonPressed = VRInputManager.Instance.GetRightGripDown();
            }
            else if (GrabButton == ButtonTypes.Trigger)
            {
                leftButtonPressed = VRInputManager.Instance.GetLeftTriggerDown();
                rightButtonPressed = VRInputManager.Instance.GetRightTriggerDown();
            }
        }

    }

    private void Update()
    {
        CheckWasGrabbed();

        UpdateGrabButton();
        CalculatedAverageVelocity();
        UpdateControllers();
    }


    /// <summary>
    /// Handles updating the controllers on whats going on
    /// </summary>
    private void UpdateControllers()
    {
        leftController.IsGrabbing = IsGrabbedLeft;
        rightController.IsGrabbing = IsGrabbedRight;
    }
    /// <summary>
    /// Calculates the velocity of the last 10 frames
    /// </summary>
    private void CalculatedAverageVelocity()
    {
        // vel
        velocity = ((transform.position - previousVelocity)) / Time.deltaTime;
        previousVelocity = transform.position;
        
        // torque
        torque = ((transform.localEulerAngles - previousVelocity)) / Time.deltaTime;
        previousTorque = transform.localEulerAngles;

    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    private void CheckReleaseGrab()
    {
        if (HoldToGrab)
        {
            if (!leftButtonPressed && IsGrabbedLeft && IsGrabbed)
            {
                ReleaseGrabLeft();
            }

            if (!rightButtonPressed && IsGrabbedRight && IsGrabbed)
            {
                ReleaseGrabRight();
            }
        }
        else
        {
            if (leftButtonPressed && IsGrabbedLeft && IsGrabbed)
            {
                ReleaseGrabLeft();
            }

            if (rightButtonPressed && IsGrabbedRight && IsGrabbed)
            {
                ReleaseGrabRight();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == ControllerLayer)
        {
            if(!IsGrabbed)
            {
                if (HoldToGrab)
                {
                    if (leftButtonPressed && other.tag.Equals("LeftController") && !leftController.IsBeingUsed) // Making sure only the left can grab stuff if its the collision
                    {
                        if (!IsGrabbedLeft)
                        {
                            leftController.IsBeingUsed = true;
                            IsGrabbedLeft = true;
                            AttatchToController(other, VRPositionManager.Instance.LeftHand);
                        }
                    }

                    if (rightButtonPressed && other.tag.Equals("RightController") && !rightController.IsBeingUsed) // Making sure only the right can grab stuff if its the collision
                    {
                        if (!IsGrabbedRight)
                        {
                            rightController.IsBeingUsed = true;
                            IsGrabbedRight = true;
                         
                            AttatchToController(other, VRPositionManager.Instance.RightHand);
                        }
                    }
                }
                else
                {
                    if (leftButtonPressed && other.tag.Equals("LeftController") && !leftController.IsBeingUsed) // Making sure only the left can grab stuff if its the collision
                    {
                        if (!IsGrabbedLeft)
                        {
                            StartCoroutine(WaitToGrabLeft(other)); // waits till end of frame to set value to true
                        }
                    }

                    if (rightButtonPressed && other.tag.Equals("RightController") && !rightController.IsBeingUsed) // Making sure only the right can grab stuff if its the collision
                    {
                        if (!IsGrabbedRight)
                        {
                            StartCoroutine(WaitToGrabRight(other));
                        }
                    }
                }
               
            }
            CheckReleaseGrab(); // Might need to be changed if we don't have collision grabbing (Although when would that ever happen)
        }


    }


    /// <summary>
    /// Used to check if object was being grabbed last frame
    /// </summary>
    private void CheckWasGrabbed()
    {
        if(WasGrabbed && !IsGrabbed )
        {
            WasGrabbed = false;
        }

    }

    private IEnumerator WaitToGrabLeft(Collider other)
    {
        yield return new WaitForEndOfFrame();
        leftController.IsBeingUsed = true;
        IsGrabbedLeft = true;
        AttatchToController(other, VRPositionManager.Instance.LeftHand);
    }

    private IEnumerator WaitToGrabRight(Collider other)
    {
        yield return new WaitForEndOfFrame();
        rightController.IsBeingUsed = true;
        IsGrabbedRight = true;
        AttatchToController(other, VRPositionManager.Instance.RightHand);
    }
    /// <summary>
    /// Sets all the values of the object when we attatch to the controller
    /// </summary>
    public void AttatchToController(Collider other, GameObject controller)
    {

        rb.useGravity = false;
        rb.isKinematic = true;
        IsGrabbed = true;
        WasGrabbed = true;

        if (!isOldParentSet)
        {
            oldParent = transform.parent;
            isOldParentSet = true;
        }
        transform.parent = other.transform;

        if (SnapToController) // This should be changed for the 1 handed weapons deffinitly a bad way of doing this
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }
        if (SnapTonController2Hand)
        {
            transform.forward = controller.transform.forward;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = new Vector3(270.0f, 180.0f, 0.0f);
        }

        // Controller is only hidden if controller hidden bool is true
        HideController(controller);
    }

  

    private void Release()
    {
        rb.useGravity = true;
        IsGrabbed = false;

        if (oldParent != null) // The old parent can not ever be a holster
        {
           transform.parent = null;
        }
        else
        {
           transform.parent = oldParent;
        }
        
        rb.isKinematic = false;

        if (transform.GetComponent<Rigidbody>())
        {
            transform.GetComponent<Rigidbody>().velocity = velocity; // Adding velocity 
            transform.GetComponent<Rigidbody>().AddTorque(torque); // Adding velocity 
        }
        isOldParentSet = false; // When we are done we don't need to know what the old parent is
    }
    private void ReleaseGrabRight()
    {
        rightController.IsBeingUsed = false;
        rightController.IsGrabbing = false;
        IsGrabbedRight = false;
        Release();
        ShowController(VRPositionManager.Instance.RightHand);

    }
    private void ReleaseGrabLeft()
    {
        // Hand Specific stuff
        leftController.IsBeingUsed = false;
        leftController.IsGrabbing = false;
        IsGrabbedLeft = false;
        Release();
        ShowController(VRPositionManager.Instance.LeftHand);
    }


    public void HideController(GameObject controller)
    {
        if (HideControllerOnGrab)
        {
            if (controller.GetComponent<Controller>() != null)
            {
                controller.GetComponent<Controller>().HideController();
            }
            else
            {
                Debug.LogError("No Controller script attatched to the controller object :" + controller.name);
            }
        }
    }

    public void ShowController(GameObject controller)
    {
        if (HideControllerOnGrab)
        {
            if (controller.GetComponent<Controller>() != null)
            {
                controller.GetComponent<Controller>().ShowController();
            }
            else
            {
                Debug.LogError("No Controller script attatched to the controller object :" + controller.name);
            }
        }
    }


  
    /// <summary>
    /// Used to force dropping
    /// </summary>
    public void ReleaseGrab()
    {
        if (IsGrabbedLeft)
        {
            ReleaseGrabLeft();
        }
        else if (IsGrabbedRight)
        {
            ReleaseGrabRight();
        }
     
    }
}

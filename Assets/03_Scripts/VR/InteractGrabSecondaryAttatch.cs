using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement of rotation 
/// </summary>

[RequireComponent(typeof(InteractGrabObject))]
public class InteractGrabSecondaryAttatch : MonoBehaviour
{
    [HideInInspector]
    public bool IsGrabbedLeft = false;
    [HideInInspector]
    public bool IsGrabbedRight = false;
    [HideInInspector]
    public bool IsGrabbed = false;

    private InteractGrabObject mainGrab;
    private LayerMask controllerLayerMask;
    private Vector3 orginalGrabRotation = Vector3.zero;

    private Controller leftController;
    private Controller rightController;
   

    private void Start()
    {
        leftController = VRPositionManager.Instance.LeftHand.GetComponent<Controller>();
        rightController = VRPositionManager.Instance.RightHand.GetComponent<Controller>();
        mainGrab = GetComponent<InteractGrabObject>();

        controllerLayerMask = mainGrab.ControllerLayer;
    }

    private void Update()
    {
        if (IsGrabbed && IsGrabbedRight)
        {
            // Maybe add an offset to move down
            // transform.LookAt((2 * transform.position - rightController.transform.position)); // Look backwards
            transform.LookAt((rightController.transform.position));
            transform.position = leftController.transform.position;
        }
        if (IsGrabbed && IsGrabbedLeft)
        {
          //  transform.LookAt((2 * transform.position  -leftController.transform.position)); // Look backwards
            transform.LookAt((leftController.transform.position));
            transform.position = rightController.transform.position;
        }



        if (!mainGrab.IsGrabbed && IsGrabbed)
        {
            ReleaseGrabRight();
            ReleaseGrabLeft();
        }

        UpdateControllers();
    }

    private void CheckRelease()
    {
        if (mainGrab.HoldToGrab)
        {
            if (!VRInputManager.Instance.GetLeftGrip() && IsGrabbedLeft && IsGrabbed)
            {
                ReleaseGrabLeft();
            }

            if (!VRInputManager.Instance.GetRightGrip() && IsGrabbedRight && IsGrabbed)
            {
                ReleaseGrabRight();
            }
        }
        else
        {
            if (VRInputManager.Instance.GetLeftGripDown() && IsGrabbedLeft && IsGrabbed)
            {
                ReleaseGrabLeft();
            }

            if (VRInputManager.Instance.GetRightGripDown() && IsGrabbedRight && IsGrabbed)
            {
                ReleaseGrabRight();
            }
        }
    }

    /// <summary>
    /// Handles updating the controllers on whats going on
    /// </summary>
    private void UpdateControllers()
    {
        leftController.IsGrabbing = IsGrabbedLeft;
        rightController.IsGrabbing = IsGrabbedRight;
    }
    private void DrawDebugRay()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == controllerLayerMask)
        {
            if (mainGrab.IsGrabbed) // this can only work if a primary grab has been set
            {
                if (mainGrab.HoldToGrab)
                {
                    if (VRInputManager.Instance.GetLeftGrip() && other.tag.Equals("LeftController") && !leftController.IsBeingUsed) // Making sure only the left can grab stuff if its the collision
                    {
                        if (!IsGrabbedLeft)
                        {
                            leftController.IsBeingUsed = true;
                            IsGrabbedLeft = true;
                            OnGrab(VRPositionManager.Instance.LeftHand);
                        }
                    }

                    if (VRInputManager.Instance.GetRightGrip() && other.tag.Equals("RightController") && !rightController.IsBeingUsed) // Making sure only the right can grab stuff if its the collision
                    {
                        if (!IsGrabbedRight)
                        {
                            rightController.IsBeingUsed = true;
                            IsGrabbedRight = true;
                            OnGrab(VRPositionManager.Instance.RightHand);
                        }
                    }
                }
                else
                {
                    if (VRInputManager.Instance.GetLeftGripDown() && other.tag.Equals("LeftController") && !leftController.IsBeingUsed) // Making sure only the left can grab stuff if its the collision
                    {
                        if (!IsGrabbedLeft)
                        {
                            StartCoroutine(WaitToGrabLeft());
                        }
                    }

                    if (VRInputManager.Instance.GetRightGripDown() && other.tag.Equals("RightController") && !rightController.IsBeingUsed) // Making sure only the right can grab stuff if its the collision
                    {
                        if (!IsGrabbedRight)
                        {
                            StartCoroutine(WaitToGrabRight());
                        }
                    }
                }
            }
            CheckRelease();
        }
    }


    private IEnumerator WaitToGrabLeft()
    {
        yield return new WaitForEndOfFrame();
        print("Doing left grab");
        leftController.IsBeingUsed = true;
        IsGrabbedLeft = true;
        OnGrab(VRPositionManager.Instance.LeftHand);
    }



    private IEnumerator WaitToGrabRight()
    {
        yield return new WaitForEndOfFrame();
        print("Doing right grab");
        rightController.IsBeingUsed = true;
        IsGrabbedRight = true;
        OnGrab(VRPositionManager.Instance.RightHand);
    }


    /// <summary>
    /// Called when we grab the object
    /// </summary>
    private void OnGrab(GameObject controller)
    {
        IsGrabbed = true;     
        orginalGrabRotation = transform.localEulerAngles;

        mainGrab.HideController(controller);
    }

    private void ReleaseGrabRight()
    {
        transform.localEulerAngles = orginalGrabRotation;
        IsGrabbedRight = false;
        IsGrabbed = false;
        rightController.IsBeingUsed = false;
        rightController.IsGrabbing = false;
        mainGrab.ShowController(VRPositionManager.Instance.RightHand);
    }

    private void ReleaseGrabLeft()
    {
        transform.localEulerAngles = orginalGrabRotation;
        IsGrabbedLeft = false;
        IsGrabbed = false;
        leftController.IsBeingUsed = false;
        leftController.IsGrabbing = false;
        mainGrab.ShowController(VRPositionManager.Instance.LeftHand);
    }
}

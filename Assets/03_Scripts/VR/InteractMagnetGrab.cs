using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class InteractMagnetGrab : MonoBehaviour
{


    [Header("Ignored Raycast Layers")]
    public int LayersToIgnore = 12;

    public GameObject GrabIcon;

    public Transform RaycastTransform;
 

    private void Start()
    {
        if (RaycastTransform == null)
        {
            Debug.LogError("No Raycast transform set please add one to  " + transform.name);
        }
        LayersToIgnore = ~LayersToIgnore;
    }
    private void FixedUpdate()
    { 
        RayCastOut();
    }

    private void RayCastOut()
    {
        RaycastHit hit;

        if (Physics.SphereCast(new Ray(RaycastTransform.position, RaycastTransform.forward), .35f,out hit, Mathf.Infinity, LayersToIgnore))
        {
            if (hit.transform.GetComponent<InteractGrabObject>() && !GetComponent<Controller>().IsBeingUsed)
            {
                InteractGrabObject grab = hit.transform.GetComponent<InteractGrabObject>();

                // VRInputManager.Instance.SendHapticFeedBack(GetComponent<SteamVR_Behaviour_Pose>().inputSource);
                if (!grab.IsGrabbed)
                {
                    EnableGrabIcon(hit.transform.position);
                }


                if (!grab.IsGrabbed)
                {
                    if (VRInputManager.Instance.GetLeftGrip() && this.tag.Equals("LeftController")) // Making sure only the left can grab stuff if its the collision
                    {
                        if (!grab.IsGrabbedLeft)
                        {
                            GetComponent<Controller>().IsBeingUsed = true;
                            grab.IsGrabbedLeft = true;

                            hit.transform.position = transform.position;
                            grab.AttatchToController(GetComponentInChildren<Collider>(), VRPositionManager.Instance.LeftHand);
                            DisableGrabIcon();
                            print("oof you got me left controller " + hit.transform);
                        }

                    }
                    if (VRInputManager.Instance.GetLeftGrip() && this.tag.Equals("RightController")) // Making sure only the left can grab stuff if its the collision
                    {
                        if (!grab.IsGrabbedRight)
                        {
                            GetComponent<Controller>().IsBeingUsed = true;
                            grab.IsGrabbedRight = true;

                            hit.transform.position = transform.position;
                            print("oof you got me right controller " + hit.transform);
                            grab.AttatchToController(GetComponentInChildren<Collider>(), VRPositionManager.Instance.RightHand);
                            DisableGrabIcon();
                        }

                    }
                }
            }
            else
            {
                DisableGrabIcon();
            }
        }
    }

    private void EnableGrabIcon(Vector3 iconPos)
    {
        GrabIcon.SetActive(true);
        GrabIcon.transform.position = iconPos;
    }

    private void DisableGrabIcon()
    {
        GrabIcon.SetActive(false);
    }
}

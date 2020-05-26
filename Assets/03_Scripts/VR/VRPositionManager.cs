using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Singleton class that manages positions of controllers and cameras
/// </summary>
public class VRPositionManager : MonoBehaviour
{
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject PlaySpace;

    public Transform CameraParent; // Steam vr really hates me force rotating the camera so I have to get creative




    /// <summary>
    /// Janky
    /// </summary>
    public void SetRotation(Transform destination)
    {
        GameObject temp = new GameObject();
        temp.transform.position = CameraParent.transform.position;
        temp.transform.localEulerAngles = CameraParent.localEulerAngles;
        Transform oldParent = CameraParent.parent;
        CameraParent.parent = temp.transform;

        temp.transform.localEulerAngles = new Vector3(temp.transform.localEulerAngles.x, destination.localEulerAngles.y, temp.transform.localEulerAngles.z);
        print("What are the angles " + temp.transform.localEulerAngles);

        CameraParent.parent = oldParent;
        Destroy(temp);
    }

    /// <summary>
    /// Teleports player to position
    /// </summary>
    /// <param name="location">The vector3 location in world space</param>
    /// <param name="ignoreFloorPosition">Set true if you don't want the floor to align with the vr playspace</param>
    public void Teleport(Transform location, bool ignoreFloorPosition = false)
    {
        float yOffset = 0;


        Vector3 distanceFromCenterOffset = PlaySpace.transform.position - CameraParent.position; // The distance the camera rig is to the centre of the room we will use this to make sure the teleport it centered 

        if (!ignoreFloorPosition)
        {
            yOffset = PlaySpace.transform.position.y - location.position.y;
            PlaySpace.transform.position = new Vector3(location.position.x, location.position.y, location.position.z) + new Vector3(distanceFromCenterOffset.x, 0, distanceFromCenterOffset.z);
        }
        else
        {
            PlaySpace.transform.position = new Vector3(location.position.x, location.position.y, location.position.z) + distanceFromCenterOffset;
        }

        SetRotation(location);


    }



    //public Vector3[] GetTrackingPointsToWorldSpace() // Took out because this is made for OVR but I'll leave here because there is a steam vr type function
    //{
    //    Vector3[] worldPlaySpaceCordinates = new Vector3[(OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea)).Length];
    //    Vector3[] localCords = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
    //    for (int i = 0; i < localCords.Length; i++)
    //    {
    //        worldPlaySpaceCordinates[i] = transform.TransformPoint(localCords[i]);
    //    }
    //    return worldPlaySpaceCordinates;
    //}
    #region Singleton
    public static VRPositionManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("OVR Position manager already exist destroying the position manager attacthed to : " + transform.name);
            Destroy(this);
        }
    }

    #endregion
}

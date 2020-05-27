using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
/// <summary>
/// Handles all the input 
/// </summary>
/// 
public enum ButtonTypes
{
    Grip,
    AButton,
    BButton,
    Trigger
}
public class VRInputManager : MonoBehaviour
{





    #region triggerEvents

    public bool GetLeftTriggerDown()
    {
        return SteamVR_Actions._default.LeftTrigger.GetStateDown(SteamVR_Input_Sources.Any);
    }

    public bool GetRightTriggerDown()
    {
        return SteamVR_Actions._default.RightTrigger.GetStateDown(SteamVR_Input_Sources.Any);
    }
    public bool GetLeftTrigger()
    {
        return SteamVR_Actions._default.LeftTrigger.GetStateDown(SteamVR_Input_Sources.Any);
    }

    public bool GetRightTrigger()
    {
        return SteamVR_Actions._default.RightTrigger.GetStateDown(SteamVR_Input_Sources.Any);
    }

    #endregion



    #region gripEvents

    public bool GetLeftGrip()
    {
        return SteamVR_Actions._default.GripLeft.GetState(SteamVR_Input_Sources.Any);
    }

    public bool GetRightGrip()
    {
        return SteamVR_Actions._default.GripRight.GetState(SteamVR_Input_Sources.Any);
    }



    public bool GetLeftGripDown()
    {
        return SteamVR_Actions._default.GripLeft.GetStateDown(SteamVR_Input_Sources.Any);
    }

    public bool GetRightGripDown()
    {
        return SteamVR_Actions._default.GripRight.GetStateDown(SteamVR_Input_Sources.Any);
    }




    #endregion




    #region singleton
    public static VRInputManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            Debug.LogError("VRInputManager already exist destroying script attatched too : "+ transform.name);
        }
        
    }
    #endregion
}




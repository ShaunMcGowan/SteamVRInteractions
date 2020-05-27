using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles functions to do directly with the controllers
/// </summary>
public class Controller : MonoBehaviour
{
    public bool IsGrabbing = false;
    public bool IsBeingUsed = false;

    public void HideController()
    {
        foreach(MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
        {
            if (m.transform.tag.Contains("Controller"))
            {
                m.enabled = false;
            }
        }
    }

    public void ShowController()
    {
        if (!IsGrabbing)
        {
            foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
            {
                if (m.transform.tag.Contains("Controller"))
                {
                    m.enabled = true;
                }
            }
        }
    }
}

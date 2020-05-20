using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Raycast out from transform attatched to looking for UI
/// </summary>
public class VR_UIRaycaster : MonoBehaviour
{
    // Public Vars
    [Header("Raycast Settings")]
    public float RaycastDistance = Mathf.Infinity;

    public Transform UIBeam;


    // Private Vars
    private int raycastLayer = 11;
    private Transform hitElement;
    private bool isButtonSelected = false;


    private void Start()
    {
        raycastLayer = ~raycastLayer;
    }
    private void Update()
    {   
        CastRay();    
    }

    private void CastRay()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position,transform.forward,out hit,RaycastDistance, raycastLayer))
        {
            hitElement = hit.collider.transform;
            if (hitElement.GetComponent<Button>())
            {
                HandleButtonRaycast(hitElement.GetComponent<Button>());
            }
            else if (hitElement.GetComponent<Toggle>())
            {
                HandleToggleRaycast(hitElement.GetComponent<Toggle>());
            }
            TurnOnBeam(Vector3.Distance(transform.position,hit.point));
        }
        else
        {
            TurnOffBeam();
            UnselectButton();
        }
    }

    /// <summary>
    /// Handles raycasting specifically buttons
    /// </summary>
    private void HandleButtonRaycast(Button _button)
    {
        if (VRInputManager.Instance.GetLeftTriggerDown() && GetComponent<InteractGrab>().Hand == ControllerHand.Left || VRInputManager.Instance.GetRightTriggerDown() && GetComponent<InteractGrab>().Hand == ControllerHand.Right)
        {
            EventSystem.current.SetSelectedGameObject(null);
            isButtonSelected = false;
            _button.onClick.Invoke(); // Boom we clicked the button
        }
        else if (!isButtonSelected)
        {
            _button.Select();
            isButtonSelected = true;
        }
    }

    private void HandleToggleRaycast(Toggle _toggle)
    {
        if (VRInputManager.Instance.GetLeftTriggerDown() && GetComponent<InteractGrab>().Hand == ControllerHand.Left || VRInputManager.Instance.GetRightTriggerDown() && GetComponent<InteractGrab>().Hand == ControllerHand.Right)
        {
            EventSystem.current.SetSelectedGameObject(null);

            _toggle.isOn = !_toggle.isOn; 
        }
        else if (!isButtonSelected)
        {
            _toggle.Select();
        }
    }


    private void TurnOnBeam(float length)
    {
        if (UIBeam !=  null)
        {
            if (!UIBeam.gameObject.activeSelf)
            {
                UIBeam.gameObject.SetActive(true);
            }
         
            UIBeam.transform.localPosition = new Vector3(UIBeam.transform.localPosition.x, UIBeam.transform.localPosition.y, 0 + (length/2));
            UIBeam.transform.localScale = new Vector3(UIBeam.transform.localScale.x, UIBeam.transform.localScale.y, length);
        }
    }

    private void TurnOffBeam()
    {
        if (UIBeam != null && UIBeam.gameObject.activeSelf)
        {
            UIBeam.gameObject.SetActive(false);
        }
    }

    private void UnselectButton()
    {
        if (hitElement  != null && isButtonSelected )
        {
            EventSystem.current.SetSelectedGameObject(null);
            isButtonSelected = false;
        }
    }

}

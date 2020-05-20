using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TwoHandedGunSecondHandHold : MonoBehaviour
{
    public Interactable mainInteractable; // First hand

    private Interactable interactable; // Second hand / this hand
    private Quaternion secondRotationOffset;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
    }

    // This happens as soon as the main interactable detaches.
    public void ForceDetach()
    {
        if (interactable.attachedToHand)
        {
            interactable.attachedToHand.HoverUnlock(interactable);
            interactable.attachedToHand.DetachObject(gameObject);
        }
    }

    private Quaternion GetTargetRotation()
    {
        Vector3 mainHandUp = mainInteractable.attachedToHand.objectAttachmentPoint.up;
        Vector3 secondHandUp = interactable.attachedToHand.objectAttachmentPoint.up;

        return Quaternion.LookRotation(interactable.attachedToHand.transform.position - mainInteractable.attachedToHand.transform.position
            , mainHandUp);
    }

    private void OnHandHoverBegin(Hand hand)
    {
        hand.ShowGrabHint();
    }

    private void OnHandHoverEnd(Hand hand)
    {
        hand.HideGrabHint();
    }

    // This is called every frame that the object is interacted with
    private void HandAttachedUpdate(Hand hand)
    {
        if (mainInteractable.attachedToHand)
        {
            // Rotate the pivot
            if (mainInteractable.skeletonPoser)
            {
                Quaternion customHandPoseRotation = mainInteractable.skeletonPoser.GetBlendedPose(mainInteractable.attachedToHand.skeleton).rotation;
                mainInteractable.transform.rotation = GetTargetRotation() * secondRotationOffset * customHandPoseRotation;
            }
            else
            {
                mainInteractable.attachedToHand.objectAttachmentPoint.rotation = GetTargetRotation() * secondRotationOffset;
            }
        }
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes grabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(gameObject);

        // Grab
        if (interactable.attachedToHand == null && grabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, grabType, 0);
            hand.HoverLock(interactable);
            hand.HideGrabHint();
            secondRotationOffset = Quaternion.Inverse(GetTargetRotation()) * mainInteractable.attachedToHand.currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation;
        }
        // Release
        else if (isGrabEnding)
        {
            hand.DetachObject(gameObject);
            hand.HoverUnlock(interactable);
        }
    }
}

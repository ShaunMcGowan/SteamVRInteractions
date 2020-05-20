using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TwoHandedGun : MonoBehaviour
{
    public bool automatic = true;
    public float shootDelay = 0.15f;
    //public float shootSpeed = 1f;
    public float recoilAmount = 5f;

    public float rayCastRange = 800f;
    public float rayCastImpactForce = 1000f;
    public Transform rayCastSource;

    public AudioClip gunShotAudioClip;
    public AudioClip emptyShotAudioClip;

    public SteamVR_Action_Boolean fireAction;

    private Interactable interactable;
    private AudioSource audioSource;

    private new Rigidbody rigidbody = null;

    private Coroutine firingRoutine = null;
    private bool isFiringRoutineRunning = false;
    private WaitForSeconds waitTime = null;

    private void Awake()
    {
        waitTime = new WaitForSeconds(shootDelay);
    }
    // Start is called before the first frame update
    private void Start()
    {
        interactable = GetComponent<Interactable>();
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        SetupAudio();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        //check if grabbed
        if (interactable.attachedToHand != null)
        {
            // Get the hand source
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;

            // Check button is down
            if (!automatic && fireAction[source].stateDown)
            {
                PullTrigger();
                ReleaseTrigger();
            }
            else if (automatic && fireAction[source].stateDown)
            {
                PullTrigger();
            }
            else if (fireAction[source].stateUp)
            {
                ReleaseTrigger();
            }
        }
    }

    private void SetupAudio()
    {
        audioSource.clip = gunShotAudioClip;
    }

    private void PullTrigger()
    {
        if (!isFiringRoutineRunning)
        {
            firingRoutine = StartCoroutine(ShootSequence());
            isFiringRoutineRunning = true;
        }
    }

    private void ReleaseTrigger()
    {
        if (isFiringRoutineRunning && (firingRoutine != null))
        {
            StopCoroutine(firingRoutine);
            isFiringRoutineRunning = false;
        }
    }

    private IEnumerator ShootSequence()
    {
        while (gameObject.activeSelf)
        {
            RayCastShoot();
            audioSource.Play();
            ApplyRecoil();
            yield return waitTime;
        }
    }

    private void RayCastShoot()
    {
        RaycastHit hitInformation;

        bool rayCastDidHit = Physics.Raycast(rayCastSource.transform.position, rayCastSource.transform.forward, out hitInformation, rayCastRange);
        if (rayCastDidHit)
        {
            Transform objectHit = hitInformation.transform;

            // Apply Damage to Target
            // Target target = objectHit.GetComponent<Target>();
            // if (target)
            // {
            //     target.TakeDamage(damage);
            // }

            // Add Impact Force
            if (hitInformation.rigidbody)
            {
                hitInformation.rigidbody.AddForce(-hitInformation.normal * rayCastImpactForce);
            }

            // Bullet Impact Effect
            //GameObject impactObject = Instantiate(impactEffect, hitInformation.point, Quaternion.LookRotation(hitInformation.normal));
            //Destroy(impactObject, 0.5f);
        }
    }

    private void ApplyRecoil()
    {
        rigidbody.AddRelativeForce(Vector3.back * recoilAmount, ForceMode.Impulse);
    }
}

// References
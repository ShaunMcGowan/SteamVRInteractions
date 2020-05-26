using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Used to force the VR camera to spawn on top of gameobject
/// </summary>
public class Spawnpoint : MonoBehaviour
{
    public bool IgnoreFloorPosition = true;
    public bool DestroyOnSpawn = false;
    private IEnumerator Start()
    {
   
        yield return new WaitForEndOfFrame();

        VRPositionManager.Instance.Teleport(transform, IgnoreFloorPosition);
        if (DestroyOnSpawn)
        {
            Destroy(this.gameObject);
        }
    }


    


    

}

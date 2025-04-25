using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunColliderBridge : MonoBehaviour, IColliderBridge
{

    public PlayerObstacleInteractions playerObstacleInteractions;
    private void Start()
    {
        playerObstacleInteractions = GetComponentInParent<PlayerObstacleInteractions>();
    }
    public void OnCollisionEnter(Collision collision)
    {

    }
    public void OnCollisionStay(Collision collision)
    {

    }
    public void OnCollisionExit(Collision collision)
    {

    }
    public void OnTriggerEnter(Collider other)
    {

    }
    public void OnTriggerStay(Collider other)
    {

        playerObstacleInteractions.WallRunOnTriggerStay(other);
        
    }
    public void OnTriggerExit(Collider other)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderBridge : MonoBehaviour, IColliderBridge
{
    public PlayerAttackColliderManager bridge;
    private void Start()
    {
        bridge = PlayerManager.instance.playerAttackColliderManager;
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
        bridge.OnAttackTriggerEnter(other);
    }
    public void OnTriggerStay(Collider other)
    {
        bridge.OnAttackTriggerStay(other);
    }
    public void OnTriggerExit(Collider other)
    {
        bridge.OnAttackTriggerExit(other);
    }
}

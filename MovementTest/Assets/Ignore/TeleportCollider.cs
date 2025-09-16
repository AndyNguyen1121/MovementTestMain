using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCollider : MonoBehaviour
{
    public LayerMask collideLayer;
    public Vector3 teleportLocalPos;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & collideLayer) != 0)
        {
            other.transform.localPosition = teleportLocalPos;
        }
    }
}

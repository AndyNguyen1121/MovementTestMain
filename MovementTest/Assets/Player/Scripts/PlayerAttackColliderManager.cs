using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;


public class PlayerAttackColliderManager : MonoBehaviour
{
    public enum PlayerAttackColliders
    {
        LeftUpperLeg,
        LeftLowerLeg,
        LeftFoot,

        RightUpperLeg,
        RightLowerLeg,
        RightFoot,

        LeftUpperArm,
        LeftLowerArm,
        LeftHand,

        RightUpperArm,
        RightLowerArm,
        RightHand
    }

    public List<PlayerAttackColliders> activatedColliders = new();

    #region Clear List Button
    [ButtonField(nameof(ClearColliderList), "Clear Active Colliders")]
    [SerializeField] private Void buttonHolder;
    #endregion
    public void ClearColliderList() => activatedColliders.Clear();

    [SerializedDictionary("ColliderID", "Collider")]
    public SerializedDictionary<PlayerAttackColliders, Collider> attackColliders;

    private void Start()
    {
        HandleColliderActivation();
    }

    private void Update()
    {
 
    }
    public void OnAttackTriggerEnter(Collider other)
    {

    }

    public void OnAttackTriggerStay(Collider other)
    {

    }

    public void OnAttackTriggerExit(Collider other)
    {

    }

    public void OpenAttackCollider()
    {
        HandleColliderActivation();
    }

    public void CloseAttackCollider(int clearList)
    {
        if (clearList > 0)
        {
            foreach (var colliderPair in attackColliders)
            {
                colliderPair.Value.enabled = false;
            }
        }
        else
        {
            ClearColliderList();
            HandleColliderActivation();
        }
    }

    private void HandleColliderActivation()
    {
        foreach (var colliderPair in attackColliders)
        {
            if (activatedColliders.Contains(colliderPair.Key))
            {
                colliderPair.Value.enabled = true;
            }
            else
            {
                colliderPair.Value.enabled = false;
            }
        }
    }
}

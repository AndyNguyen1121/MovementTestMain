using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.Animations.Rigging;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    [Header("IK Settings")]
    public bool enableFootIK = true;
    public LayerMask validLayerInteractions;

    private Vector3 rightFootPosition, leftFootPosition, rightFootIKPos, leftFootIKPos;
    private float lastPelvisYPos, lastLeftFootYPos, lastRightFootYPos;
    private Quaternion rightFootRotation, leftFootRotation;
    private float previousRotationWeight;

    public float pelvisAdjustmentSpeed = 0.5f;
    public float footHeightAdjustmentSpeed = 0.5f;
    public float footRotationSpeed = 0.5f;
    public float maxRotationSlerp;
    public bool enableRotations = true;
    public Vector3 hintPos;

    public string leftFootAnimRotationName;
    public string rightFootAnimRotationName;
    

    [Header("Raycast Settings")]
    public float raycastLength;
    public float raycastDistanceAboveFoot;
    public Vector3 rayCastOffsetXZ;

    [Header("Slope Detection")]
    public float slopeAngle;
    public float slopeRaycastLength;
    public Transform hips;
    public bool onSlope = false;

    [Header("Toggle Settings")]
    public float disableIkSpeed;
    public float disableIkDuration;
    public bool IKOff = false;
    private Coroutine toggleAdjustmentCoroutine;

    [Header("Movement Detection")]
    public float idleTime;
    public float minimumTimeOnSlope;

    [Header("Debug")]
    public bool debugVisible = true;
    private Animator animator;
    public Vector3 localSpaceIkPos;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        DetectSlopes(ref slopeAngle);
        DetectIdleTime(ref idleTime);
        CheckIfCanFootIK();

        localSpaceIkPos = transform.InverseTransformPoint(rightFootIKPos);
    }

    private void LateUpdate()
    {
        if (!enableFootIK) return;

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

       

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (onSlope && idleTime < minimumTimeOnSlope)
        {
            lastPelvisYPos = animator.bodyPosition.y;
            /*lastLeftFootYPos = leftFootIKPos.y;
            lastRightFootYPos = rightFootIKPos.y;*/
            return;
        }
        if (!enableFootIK) return;

        FindValidIKPosition(rightFootPosition, HumanBodyBones.RightFoot, AvatarIKGoal.RightFoot, ref rightFootIKPos, ref rightFootRotation);
        FindValidIKPosition(leftFootPosition, HumanBodyBones.LeftFoot, AvatarIKGoal.LeftFoot, ref leftFootIKPos, ref leftFootRotation);

        IKOff = false;
        AdjustPelvisPosition();

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        RotateFoot(leftFootAnimRotationName, AvatarIKGoal.LeftFoot, leftFootRotation);
        RotateFoot(rightFootAnimRotationName, AvatarIKGoal.RightFoot, rightFootRotation);

        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIKPos, ref lastLeftFootYPos);
        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIKPos, ref lastRightFootYPos);
    }

    private void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, ref float lastFootPositionY)
    {
        Vector3 footIKPos = animator.GetIKPosition(foot);

        if (positionIkHolder != Vector3.zero)
        {
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);
            footIKPos = transform.InverseTransformPoint(footIKPos);

            float adjustedYPosition = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, footHeightAdjustmentSpeed);
            footIKPos.y += adjustedYPosition;

            lastFootPositionY = adjustedYPosition;
            footIKPos = transform.TransformPoint(footIKPos);
        }


        animator.SetIKPosition(foot, footIKPos);
    }

    private void RotateFoot(string animRotationParam, AvatarIKGoal foot, Quaternion rotation)
    {
        float lerpedWeight = 0f;

        if (animator.GetFloat(animRotationParam) > 0 && enableRotations)
        {
            animator.SetIKRotation(foot, rotation);
            lerpedWeight = Mathf.Lerp(previousRotationWeight, maxRotationSlerp, footRotationSpeed * Time.deltaTime);
            Debug.Log("Rotating");
        }
        else
        {
            lerpedWeight = Mathf.Lerp(previousRotationWeight, 0f, footRotationSpeed * Time.deltaTime);
        }

        previousRotationWeight = lerpedWeight;
        animator.SetIKRotationWeight(foot, lerpedWeight);
    }

    private void DetectSlopes(ref float slopeAngle)
    {
        RaycastHit hit;
        if (Physics.Raycast(hips.position, -transform.up, out hit, slopeRaycastLength, validLayerInteractions))
        {
            Vector3 planeVector = Vector3.ProjectOnPlane(-transform.up, hit.normal);
            slopeAngle = Vector3.SignedAngle(planeVector, transform.up, transform.up);
        }
        else
        {
            slopeAngle = 0f;
        }

        onSlope = slopeAngle > 100 && slopeAngle != 0 ? true : false;
    }

    private void FindValidIKPosition(Vector3 footPosition, HumanBodyBones foot, AvatarIKGoal foot1, ref Vector3 feetIkPositions, ref Quaternion feetIkRotation)
    {
        footPosition.y += raycastDistanceAboveFoot;
        RaycastHit hit;

        Quaternion initalRotation = animator.GetIKRotation(foot1);

        /*if (debugVisible)
        {
            Debug.DrawLine(footPosition + rayCastOffsetXZ, footPosition + (Vector3.down * raycastLength), Color.cyan);
        }*/

        if (Physics.Raycast(footPosition + rayCastOffsetXZ, -transform.up, out hit, raycastLength, validLayerInteractions))
        {

            feetIkPositions = hit.point;

            Quaternion slopeAlignment = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            feetIkRotation = slopeAlignment;

        }
        else
        {
            feetIkPositions = Vector3.zero;
        }
    }

    private void AdjustPelvisPosition()
    {
        if (leftFootIKPos == Vector3.zero || rightFootIKPos == Vector3.zero || lastPelvisYPos == 0)
        {
            lastPelvisYPos = animator.bodyPosition.y;
            return;
        }

        float leftFootOffset = leftFootIKPos.y - transform.position.y;
        float rightFootOffset = rightFootIKPos.y - transform.position.y;

        float desiredOffset = leftFootOffset < rightFootOffset ? leftFootOffset : rightFootOffset;
        
        Vector3 desiredPos = animator.bodyPosition + (Vector3.up * desiredOffset);
        desiredPos.y = Mathf.Lerp(lastPelvisYPos, desiredPos.y, pelvisAdjustmentSpeed * Time.deltaTime);
        animator.bodyPosition = desiredPos;
        lastPelvisYPos = desiredPos.y;
    }

    private void AdjustFeetTarget(ref Vector3 footPosition, HumanBodyBones foot)
    {
        footPosition = animator.GetBoneTransform(foot).position;
    }

    private void DetectIdleTime(ref float time)
    {
        if (PlayerInputManager.instance.movementDirection == Vector2.zero)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }

    }

    private void CheckIfCanFootIK()
    {
        enableFootIK = animator.GetBool("FootIK");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(rightFootPosition, 0.2f);
        Gizmos.DrawWireSphere(leftFootPosition, 0.2f);
        Gizmos.DrawRay(hips.position, transform.forward * slopeRaycastLength);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rightFootIKPos, 0.2f);
        Gizmos.DrawWireSphere(leftFootIKPos, 0.2f);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewFootIk : MonoBehaviour
{
    private Animator animator;
    private PlayerManager playerManager;

    [Header("Foot Raycast")]
    public float rayLength;
    public Vector3 rayOffset;
    public Vector3 footOffset;

    public float rightFootY;
    public float leftFootY;

    public float lastPelvisY;
    public float lastLeftFootY;
    public float lastRightFootY;

    public Vector3 rightFootIKPosition;
    public Vector3 leftFootIKPosition;

    [Header("Foot Rotations")]
    public Quaternion initialLeftFootRotation;
    public Quaternion initialRightFootRotation;
    public Quaternion rightFootTargetRotation, rightFootLastRotation;
    public Quaternion leftFootTargetRotation, leftFootLastRotation;
    public Vector3 leftFootUpDir;
    public Vector3 rightFootUpDir;

    [Header("Lerp Settings")]
    public float footLerpSpeed = 7f;
    public float pelvisLerpSpeed = 9f;

    public LayerMask stairLayer;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.instance;
        animator = GetComponent<Animator>();

        AnimationClip idleClip = null;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Idle")
            {
                idleClip = clip;
                break;
            }
        }

        if (idleClip != null)
        {
            idleClip.SampleAnimation(animator.gameObject, 0f);

            Transform leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

            initialLeftFootRotation = leftFoot.localRotation;
            initialRightFootRotation = rightFoot.localRotation;

            leftFootUpDir = leftFoot.up;
            rightFootUpDir = rightFoot.up;

            leftFootLastRotation = initialLeftFootRotation;
            rightFootLastRotation = initialRightFootRotation;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!animator.GetBool("FootIK"))
        {
            MovePelvis(ref lastPelvisY);

            leftFootIKPosition = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            rightFootIKPosition = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

            lastRightFootY = transform.InverseTransformPoint(rightFootIKPosition).y;
            lastLeftFootY = transform.InverseTransformPoint(leftFootIKPosition).y;
            return;
        }

        FindIkPosition(animator.GetBoneTransform(HumanBodyBones.LeftFoot), ref leftFootIKPosition, ref leftFootY, ref leftFootTargetRotation, initialLeftFootRotation, leftFootUpDir);
        FindIkPosition(animator.GetBoneTransform(HumanBodyBones.RightFoot), ref rightFootIKPosition, ref rightFootY, ref rightFootTargetRotation, initialRightFootRotation, rightFootUpDir);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        MoveFootIk(AvatarIKGoal.LeftFoot, leftFootIKPosition, ref lastLeftFootY);
        MoveFootIk(AvatarIKGoal.RightFoot, rightFootIKPosition, ref lastRightFootY);

        RotateFoot(AvatarIKGoal.LeftFoot, leftFootTargetRotation, initialLeftFootRotation, ref leftFootLastRotation);
        RotateFoot(AvatarIKGoal.RightFoot, rightFootTargetRotation, initialRightFootRotation, ref rightFootLastRotation);

        MovePelvis(ref lastPelvisY);
    }

    private void FindIkPosition(Transform footPosition, ref Vector3 ikWorldPos, ref float desiredYPos, ref Quaternion targetRotation, Quaternion initalRotation, Vector3 footUpDir)
    {
        RaycastHit hit;
        Vector3 adjustedFootOffsetDirection = footPosition.right * rayOffset.x + Vector3.up * rayOffset.y;
        Vector3 footForwardDir = footPosition.forward;
        footForwardDir.y = 0;

        adjustedFootOffsetDirection += footForwardDir * rayOffset.z;
        if (Physics.Raycast(footPosition.position + adjustedFootOffsetDirection, Vector3.down, out hit, rayLength, stairLayer))
        {
            desiredYPos = hit.point.y + footOffset.y;
            ikWorldPos = hit.point + footOffset;
        }
        else if (Physics.Raycast(footPosition.position + adjustedFootOffsetDirection, Vector3.down, out hit, rayLength, playerManager.whatIsGround))
        {
            desiredYPos = hit.point.y + footOffset.y;
            ikWorldPos = hit.point + footOffset;
        }

        if (hit.normal != Vector3.zero)
        {
            // Use footPosition (the IK foot transform) current forward
            Vector3 forward = footPosition.forward;

            // Project onto slope plane defined by hit.normal
            forward = Vector3.ProjectOnPlane(forward, hit.normal).normalized;

            // Create rotation looking forward with up aligned to slope normal
            targetRotation = Quaternion.LookRotation(forward, hit.normal);
        }

        Debug.DrawRay(footPosition.position + adjustedFootOffsetDirection, Vector3.down * rayLength);
    }
    private void MoveFootIk(AvatarIKGoal foot, Vector3 desiredPos, ref float lastYPos)
    {
        Vector3 footPos = animator.GetIKPosition(foot);

        if (desiredPos != Vector3.zero)
        {
            Vector3 localSpaceDesiredPos = transform.InverseTransformPoint(desiredPos);
            footPos = transform.InverseTransformPoint(footPos);

            float footYPos = Mathf.Lerp(lastYPos, localSpaceDesiredPos.y, footLerpSpeed);

            lastYPos = footYPos;
            footPos.y += footYPos;
            footPos = transform.TransformPoint(footPos);
        }

        animator.SetIKPosition(foot, footPos);
    }

    private void RotateFoot(AvatarIKGoal foot, Quaternion desiredRotation, Quaternion initialRotation, ref Quaternion lastRotation)
    {
        if (Quaternion.Angle(lastRotation, desiredRotation) > 0.01f)
        {
            Quaternion deltaRotation = desiredRotation * Quaternion.Inverse(initialRotation);

            float step = Time.deltaTime * 5f; 
            Quaternion partialRotation = Quaternion.Slerp(Quaternion.identity, deltaRotation, step);

            lastRotation = partialRotation * lastRotation;

            
            

        }

        animator.SetIKRotation(foot, desiredRotation);
    }

    private void MovePelvis(ref float lastPelvisY)
    {
        float rightFootOffset;
        float leftFootOffset;

        if (animator.GetBool("FootIK"))
        {
            rightFootOffset = rightFootIKPosition.y - transform.position.y;
            leftFootOffset = leftFootIKPosition.y - transform.position.y;
        }
        else
        {
            rightFootOffset = 0f;
            leftFootOffset = 0f;
        }

        float desiredOffset = rightFootOffset < leftFootOffset ? rightFootOffset : leftFootOffset;
        Vector3 desiredPos = transform.InverseTransformPoint(animator.GetBoneTransform(HumanBodyBones.Hips).position + (Vector3.up * desiredOffset));

        float yOffset = Mathf.Lerp(lastPelvisY, desiredPos.y, pelvisLerpSpeed * Time.deltaTime);
        lastPelvisY = yOffset;

        Vector3 finalPos = transform.InverseTransformPoint(animator.GetBoneTransform(HumanBodyBones.Hips).position);
        finalPos.y = yOffset;

        animator.bodyPosition = transform.TransformPoint(finalPos);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(leftFootIKPosition, 0.1f);
        Gizmos.DrawSphere(rightFootIKPosition, 0.1f);
    }
}

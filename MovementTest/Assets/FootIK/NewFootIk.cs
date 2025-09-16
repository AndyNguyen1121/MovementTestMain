using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewFootIk : MonoBehaviour
{
    private Animator animator;

    [Header("Foot Raycast")]
    public float rayLength;
    public Vector3 rayOffset;
    public Vector3 footOffset;

    private float rightFootY;
    private float leftFootY;

    private float lastPelvisY;
    private float lastLeftFootY;
    private float lastRightFootY;

    private Vector3 rightFootIKPosition;
    private Vector3 leftFootIKPosition;

    [Header("Foot Rotations")]
    private Quaternion rightFootTargetRotation, rightFootLastRotation;
    private Quaternion leftFootTargetRotation, leftFootLastRotation;

    [Header("Lerp Settings")]
    public float footLerpSpeed = 7f;
    public float pelvisLerpSpeed = 9f;
    public float rotationSpeed = 5f;

    [Header("Pelvis Height")]
    public float pelvisMinHeight;
    public float pelvisMaxHeight;

    [Header("Layers")]
    public LayerMask stairLayer;
    public LayerMask groundLayer;

    [Header("Debug")]
    public bool debugShow;
    public GameObject debugSphere;
    private GameObject sphere1;
    private GameObject sphere2;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleDebug();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (!animator.GetBool("FootIK"))
        {
            MovePelvis(ref lastPelvisY);
            leftFootIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
            rightFootIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);


            lastRightFootY = transform.InverseTransformPoint(rightFootIKPosition).y;
            lastLeftFootY = transform.InverseTransformPoint(leftFootIKPosition).y;

            return;
        }

        FindIkPosition(animator.GetBoneTransform(HumanBodyBones.LeftFoot), ref leftFootIKPosition, ref leftFootY, ref leftFootTargetRotation);
        FindIkPosition(animator.GetBoneTransform(HumanBodyBones.RightFoot), ref rightFootIKPosition, ref rightFootY, ref rightFootTargetRotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        MoveFootIk(AvatarIKGoal.LeftFoot, leftFootIKPosition, ref lastLeftFootY);
        MoveFootIk(AvatarIKGoal.RightFoot, rightFootIKPosition, ref lastRightFootY);

        RotateFoot(AvatarIKGoal.LeftFoot, leftFootTargetRotation, ref leftFootLastRotation);
        RotateFoot(AvatarIKGoal.RightFoot, rightFootTargetRotation, ref rightFootLastRotation);

        MovePelvis(ref lastPelvisY);
    }

    private void FindIkPosition(Transform footPosition, ref Vector3 ikWorldPos, ref float desiredYPos, ref Quaternion targetRotation)
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
        else if (Physics.Raycast(footPosition.position + adjustedFootOffsetDirection, Vector3.down, out hit, rayLength, groundLayer))
        {
            desiredYPos = hit.point.y + footOffset.y;
            ikWorldPos = hit.point + footOffset;
        }

        if (hit.normal != Vector3.zero)
        {
            Vector3 rotAxis = Vector3.Cross(Vector3.up, hit.normal);
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            Quaternion rot = Quaternion.AngleAxis(angle, rotAxis);
            targetRotation = rot;
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

            float footYPos = Mathf.Lerp(lastYPos, localSpaceDesiredPos.y, footLerpSpeed * Time.deltaTime);

            lastYPos = footYPos;
            footPos.y += footYPos;
            footPos = transform.TransformPoint(footPos);
        }


        animator.SetIKPosition(foot, footPos);

    }

    private void RotateFoot(AvatarIKGoal foot, Quaternion desiredRotation, ref Quaternion lastRotation)
    {
        if (Quaternion.Angle(lastRotation, desiredRotation) > 0.01f)
        {
            Quaternion partialRotation = Quaternion.Slerp(Quaternion.identity, desiredRotation, rotationSpeed * Time.deltaTime);
            lastRotation = partialRotation;
        }

        animator.SetIKRotation(foot, lastRotation * animator.GetIKRotation(foot));
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

        desiredPos.y = Mathf.Clamp(desiredPos.y, pelvisMinHeight, pelvisMaxHeight);
        float yOffset = Mathf.Lerp(lastPelvisY, desiredPos.y, pelvisLerpSpeed * Time.deltaTime);
        lastPelvisY = yOffset;

        Vector3 finalPos = transform.InverseTransformPoint(animator.GetBoneTransform(HumanBodyBones.Hips).position);
        finalPos.y = yOffset;

        animator.bodyPosition = transform.TransformPoint(finalPos);
    }

    private void HandleDebug()
    {
        if (debugShow)
        {
            if (sphere1 == null)
            {
                sphere1 = Instantiate(debugSphere);
                sphere2 = Instantiate(debugSphere);

                sphere1.transform.localScale = sphere2.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

                sphere1.GetComponent<Renderer>().material.color = Color.green;
                sphere2.GetComponent<Renderer>().material.color = Color.green;
            }

            sphere1.SetActive(true);
            sphere2.SetActive(true);

            if (rightFootIKPosition != Vector3.zero)
            {
                sphere1.transform.position = rightFootIKPosition;
            }

            if (leftFootIKPosition != Vector3.zero)
            {
                sphere2.transform.position = leftFootIKPosition;
            }
        }
        else if (sphere1 != null || sphere2 != null)
        {
            sphere1.SetActive(false);
            sphere2.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(leftFootIKPosition, 0.1f);
        Gizmos.DrawSphere(rightFootIKPosition, 0.1f);
    }
}

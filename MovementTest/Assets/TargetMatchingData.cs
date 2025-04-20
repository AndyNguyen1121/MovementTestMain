using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetMatchingData
{
    public Vector3 matchPosition = Vector3.zero;
    public Quaternion matchRotation = Quaternion.identity;
    public AvatarTarget target;
    public MatchTargetWeightMask weightMask;
    public float normalizedStartTime;
    public float normalizedEndTime;


}

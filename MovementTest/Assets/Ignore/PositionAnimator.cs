using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PositionAnimator : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float duration;
    public bool boomarang;

    private void Start()
    {
        if (!boomarang) 
            transform.DOLocalMove(endPosition, duration).From(startPosition).SetEase(Ease.Linear).SetLoops(-1);

        else
            transform.DOLocalMove(endPosition, duration).From(startPosition).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationLogo : MonoBehaviour
{
    private float _rotationAmount = 20f;
    private float _duration = 0.5f;

    private void Start()
    {
        transform.DORotate(new Vector3(0, 0, _rotationAmount), _duration).OnComplete(() =>
        {
            transform.DORotate(new Vector3(0, 0, -_rotationAmount), _duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        });
    }
}

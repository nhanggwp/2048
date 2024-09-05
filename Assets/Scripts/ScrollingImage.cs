using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingImage : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float _xSpeed = 0.1f;
    [SerializeField] private float _ySpeed = 0.1f;

    private Vector2 _uvOffset = Vector2.zero;

    private void Update()
    {
        _uvOffset.x += _xSpeed * Time.deltaTime;
        _uvOffset.y += _ySpeed * Time.deltaTime;

        _img.uvRect = new Rect(_uvOffset.x, _uvOffset.y, _img.uvRect.width, _img.uvRect.height);
    }
}

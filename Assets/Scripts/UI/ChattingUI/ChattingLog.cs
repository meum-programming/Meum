using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChattingLog : MonoBehaviour
{
    [SerializeField] private Text content;
    [SerializeField] private RawImage profileImage;
    private RectTransform _transform;

    public bool isMe { get; private set; } = true;

    public float height => _transform.sizeDelta.y;

    // private const int defaultHeight = 65;
    // private const int oneLineHeight = 22;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
    }

    public void SetData(string senderStr, string contentStr, bool isMe)
    {
        if(ReferenceEquals(_transform, null))
            _transform = GetComponent<RectTransform>();
        content.text = senderStr + ": " + contentStr;
        this.isMe = isMe;
        
        var sizeDelta = _transform.sizeDelta;
        sizeDelta.y = content.preferredHeight + 6.0f;
        Debug.Log(sizeDelta.y);
        _transform.sizeDelta = sizeDelta;
    }
}

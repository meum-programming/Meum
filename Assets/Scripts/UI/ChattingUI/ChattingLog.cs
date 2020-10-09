using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChattingLog : MonoBehaviour
{
    [SerializeField] private Text sender;
    [SerializeField] private Text content;
    [SerializeField] private Image notReadBadge;
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
        if(null == _transform)
            _transform = GetComponent<RectTransform>();
        sender.text = senderStr + ":";
        content.text = contentStr;
        this.isMe = isMe;
        
        var sizeDelta = _transform.sizeDelta;
        sizeDelta.y = content.preferredHeight + 40.0f;
        _transform.sizeDelta = sizeDelta;
    }

    public void EnableNotReadBadge()
    {
        if (notReadBadge == null) return;
        notReadBadge.gameObject.SetActive(true);
    }
    
    public void DisableNotReadBadge()
    {
        if (notReadBadge == null) return;
        notReadBadge.gameObject.SetActive(false);
    }
}

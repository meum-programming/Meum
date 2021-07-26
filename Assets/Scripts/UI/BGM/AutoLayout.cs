using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoLayout : MonoBehaviour
{
    RectTransform myRect;
    [SerializeField] RectTransform targetRect;

    [SerializeField] List<RectTransform> flexibleWidthList = new List<RectTransform>();

    private float targetAddValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        myRect = GetComponent<RectTransform>();

        foreach (var rect in flexibleWidthList)
        {
            targetAddValue -= rect.sizeDelta.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        LayOutCheck();
    }

    void LayOutCheck()
    {
        if (targetRect.sizeDelta.x == targetRect.rect.width + targetAddValue)
            return;

        Vector2 sizeDelta = myRect.sizeDelta;
        sizeDelta.x = targetRect.rect.width + targetAddValue;
        myRect.sizeDelta = sizeDelta;
    }
}

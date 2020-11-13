using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.Builder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Artwork2DContentTriggerEvents : MonoBehaviour
{
    [SerializeField] private float pressingTimeForMakeBanner = 0.0f;

    private bool _pointerIn = false;
    private float _pressingTime = 0.0f;
    private IEnumerator _timeRecording = null;

    private IEnumerator TimeRecording()
    {
        _pressingTime = 0.0f;
        while (true)
        {
            _pressingTime += Time.deltaTime;
            yield return null;
        }
    }

    public void PointerEnter()
    {
        _pointerIn = true;
    }

    public void PointerExit()
    {
        _pointerIn = false;
    }

    public void PointerDown()
    {
        if(_timeRecording != null)
            StopCoroutine(_timeRecording);
        StartCoroutine(_timeRecording = TimeRecording());
    }

    public void PointerUp()
    {
        if(_timeRecording != null)
            StopCoroutine(_timeRecording);

        if (!_pointerIn) return;
        
        if (_pressingTime < pressingTimeForMakeBanner)
        {
            var sidebar = GameObject.Find("ContentsSidebar").GetComponent<ContentsSidebar>();
            sidebar.Object2DState();
            
            var placer = GameObject.Find("Artworks").GetComponent<ArtworkPlacer>();
            placer.Select(GetComponent<UI.Content>());
        }
        else
        {
            var sidebar = GameObject.Find("ContentsSidebar").GetComponent<ContentsSidebar>();
            sidebar.BannerState(GetComponent<UI.Content>());
        }
    }
}

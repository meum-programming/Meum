using System;
using System.Collections;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Transform _cam;
    private IEnumerator _searchMainCamCoroutine;

    private void Start()
    {
        // StartCoroutine(SearchCamCoroutine());
    }

    // private IEnumerator SearchCamCoroutine()
    // {
    //     while (_cam == null)
    //     {
    //         var mainCam = Camera.main;
    //         if (mainCam != null)
    //         {
    //             _cam = mainCam.transform;
    //             break;
    //         }
    //
    //         yield return null;
    //     }
    // }

    private void LateUpdate()
    {
        if (_cam == null)
        {
            var mainCam = Camera.main;
            if (mainCam != null)
                _cam = mainCam.transform;
        }
        transform.LookAt(transform.position + _cam.forward);
    }
}

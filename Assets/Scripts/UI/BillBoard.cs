using System.Collections;
using UnityEngine;

namespace UI
{
    public class BillBoard : MonoBehaviour
    {
        private Transform _cam;
        private IEnumerator _searchMainCamCoroutine;

        private void LateUpdate()
        {
            if (ReferenceEquals(_cam, null))
            {
                var mainCam = Camera.main;
                if (!ReferenceEquals(mainCam, null))
                    _cam = mainCam.transform;
                else
                    return;
            }

            transform.LookAt(transform.position + _cam.forward);
        }
    }
}
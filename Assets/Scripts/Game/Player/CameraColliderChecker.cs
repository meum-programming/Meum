using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColliderChecker : MonoBehaviour
{
    public bool isColliderOn = false;

    public Transform cameraPivot;
    public BoxCollider boxCol;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BoxColChange();
    }

    void BoxColChange()
    {
        float checkDis = Vector3.Distance(cameraPivot.position, transform.position);

        Vector3 size = boxCol.size;
        size.z = checkDis;
        boxCol.size = size;

        Vector3 center = boxCol.center;
        center.z = checkDis * 0.5f -0.5f;
        boxCol.center = center;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (CanNotLayerCheck(collision))
            return;

        isColliderOn = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (CanNotLayerCheck(collision))
            return;

        isColliderOn = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (CanNotLayerCheck(collision))
            return;
        
        isColliderOn = false;
    }

    bool CanNotLayerCheck(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("LocalPlayer"))
        {
            return true;
        }

        return false;
    }

}

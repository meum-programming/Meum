using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColliderChecker : MonoBehaviour
{
    public bool isColliderOn = false;

    public Transform cameraPivot;
    public BoxCollider boxCol;

    public List<GameObject> objList = new List<GameObject>();

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
        Reset(checkDis);
    }

    void Reset(float checkDis)
    {
        Vector3 size = boxCol.size;
        size.z = checkDis;
        boxCol.size = size;

        Vector3 center = boxCol.center;
        center.z = checkDis * 0.5f - 0.5f;
        boxCol.center = center;
    }


    public void Test()
    {
        float checkDis = Vector3.Distance(cameraPivot.position, transform.position);


        int index = 0;

        for (int i = 0; i < 100; i++)
        {
            checkDis -= 0.1f;
            Reset(checkDis);

            index = i;

            if (isColliderOn == false)
            {
                break;
            }

        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (CanNotLayerCheck(collision))
            return;

        isColliderOn = true;

        if (!objList.Contains(collision.gameObject))
        {
            objList.Add(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (CanNotLayerCheck(collision))
            return;

        isColliderOn = true;
        if (!objList.Contains(collision.gameObject))
        {
            objList.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (CanNotLayerCheck(collision))
            return;

        isColliderOn = false;
        objList.Remove(collision.gameObject);
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

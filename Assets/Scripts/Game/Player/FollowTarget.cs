using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PosCheck();
        
    }

    void PosCheck() 
    {
        Vector3 checkPos = target.position;
        checkPos.y += 0.65f;

        if (transform.position != checkPos)
        {
            transform.position = checkPos;
        }
    }

}

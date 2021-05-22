using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporting : MonoBehaviour
{
    public Transform teleportTarget;

    void OnTriggerEnter(Collider other)
    {
        other.transform.position = teleportTarget.transform.position;
    }

}

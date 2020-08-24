using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private Transform camera;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Move();
            CameraRotation();
            CharacterRotation();
        }
    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");
        
        Vector3 moveHorizontal = camera.right * moveDirX;
        Vector3 moveVertical = camera.forward * moveDirZ;

        Vector3 delta = (moveHorizontal + moveVertical).normalized * walkSpeed;

        transform.position += delta;
    }


    private void CharacterRotation()
    {
        // 좌우 캐릭터 회전
        float yRotation = Input.GetAxisRaw("Mouse X");
        transform.Rotate(Vector3.up, yRotation*lookSensitivity);
    }

    private void CameraRotation()
    {
        // 상하 카메라 회전
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;
        camera.Rotate(Vector3.right, -cameraRotationX);
    }
}

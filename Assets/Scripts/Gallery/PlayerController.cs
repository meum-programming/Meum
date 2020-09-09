using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gallery
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed;
        [SerializeField] private float lookSensitivity;
        [SerializeField] private float cameraRotationLimit;
        [SerializeField] private Camera theCamera;
        private float _currentCameraRotationX = 0;
        private Vector3 _defaultEulerAngle = Vector3.zero;



        [Header("Camera View")] [SerializeField]
        private Transform originalCamPos;

        [SerializeField] private Transform thirdCamPos;
        [SerializeField] private float switchSpeed;
        private bool _firstPersonView = true;
        private IEnumerator _switching = null;

        private Animator _animator;
        private MultiPlay.DataSyncer _dataSyncer;
        private Rigidbody _myRigid;
        private float _distToGround;
        private Vector3 _colliderCenterPos;

        // Use this for initialization
        void Start()
        {
            _animator = GetComponent<Animator>();
            _dataSyncer = GameObject.FindWithTag("SocketIO").GetComponent<MultiPlay.DataSyncer>();
            _myRigid = GetComponent<Rigidbody>();
            Debug.Assert(_animator && _dataSyncer && _myRigid);

            var camTransform = theCamera.transform;
            camTransform.localPosition = originalCamPos.localPosition;
            camTransform.localRotation = originalCamPos.localRotation;
            _defaultEulerAngle = camTransform.localEulerAngles;

            var capsuleCollider = GetComponent<CapsuleCollider>();
            _distToGround = capsuleCollider.bounds.extents.y;
            _colliderCenterPos = capsuleCollider.center;
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position + _colliderCenterPos, -Vector3.up, _distToGround + 0.1f);
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            if (Input.GetMouseButton(1))
            {
                CameraRotation();
                CharacterRotation();
            }

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                _myRigid.AddForce(Vector3.up * 300.0f);
                _animator.SetTrigger(Animator.StringToHash("jump"));
                _dataSyncer.BroadCastAnimTrigger("jump");
            }

            if (Input.GetKeyDown(KeyCode.E) && _switching == null)
                StartCoroutine(_switching = SwitchView());
        }

        private void Move()
        {
            var selfTransform = transform;
            
            // float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");

            // Vector3 moveHorizontal = transform.right * moveDirX;
            Vector3 moveVertical = selfTransform.forward * moveDirZ;

            // Vector3 velocity = (moveHorizontal + moveVertical).normalized * walkSpeed;
            Vector3 velocity = moveVertical.normalized * walkSpeed;

            _myRigid.MovePosition(selfTransform.position + velocity * Time.deltaTime);
            if (velocity.sqrMagnitude > 1e-3)
                _animator.SetBool(Animator.StringToHash("walking"), true);
            else
                _animator.SetBool(Animator.StringToHash("walking"), false);
        }


        private void CharacterRotation()
        {
            // 좌우 캐릭터 회전
            if (_switching != null) return;
            float yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
            _myRigid.MoveRotation(_myRigid.rotation * Quaternion.Euler(characterRotationY));
        }

        private IEnumerator SwitchView()
        {
            var camTransform = theCamera.transform;
            float r = 0.0f;
            Vector3 src = camTransform.localPosition;
            Vector3 dst = _firstPersonView ? thirdCamPos.localPosition : originalCamPos.localPosition;
            Quaternion rotSrc = camTransform.localRotation;
            Quaternion rotDst = _firstPersonView ? thirdCamPos.localRotation : originalCamPos.localRotation;
            while (r < 1.0f)
            {
                camTransform.localRotation = Quaternion.Lerp(rotSrc, rotDst, r);
                camTransform.localPosition = Vector3.Lerp(src, dst, r);
                r = Mathf.Clamp(r + switchSpeed * Time.deltaTime, 0, 1);
                yield return null;
            }

            camTransform.localRotation = rotDst;
            camTransform.localPosition = dst;
            _defaultEulerAngle = camTransform.localEulerAngles;
            _currentCameraRotationX = 0;
            _firstPersonView = !_firstPersonView;
            _switching = null;
        }

        private void CameraRotation()
        {
            // 상하 카메라 회전
            if (_switching != null) return;
            float xRotation = Input.GetAxisRaw("Mouse Y");
            float cameraRotationX = xRotation * lookSensitivity;
            _currentCameraRotationX -= cameraRotationX;
            _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            var newRot = _defaultEulerAngle;
            newRot.x += _currentCameraRotationX;
            theCamera.transform.localEulerAngles = newRot;
        }
    }
}

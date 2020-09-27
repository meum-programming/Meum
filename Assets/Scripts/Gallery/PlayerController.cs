using System.Collections;
using System.Collections.Generic;
using Gallery.MultiPlay;
using Global.Socket;
using UnityEngine;

namespace Gallery
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed;
        [SerializeField] private float lookSensitivity;
        [SerializeField] private float cameraRotationLimit;
        [SerializeField] private Camera theCamera;
        [SerializeField] private Transform camPivot;
        private float _currentCameraRotationX = 0;
        private Vector3 _defaultEulerAngle = Vector3.zero;


        [Header("Camera View")] 
        [SerializeField] private Transform originalCamPos;
        [SerializeField] private Transform thirdCamPos;
        [SerializeField] private float switchSpeed;
        private bool _firstPersonView = true;
        private IEnumerator _switching = null;

        [Header("Ground Check")] 
        [SerializeField] private float detectDistance = 0.1f;
        [SerializeField] private float jumpEndDetectDistance = 0.7f;
        private float _distToGround;
        private Vector3 _colliderCenterPos;
        
        private PlayerAnimationController _animController;
        private Rigidbody _myRigid;

        // Use this for initialization
        void Start()
        {
            _animController = GetComponent<PlayerAnimationController>();
            _myRigid = GetComponent<Rigidbody>();

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
            return Physics.Raycast(transform.position + _colliderCenterPos, -Vector3.up, _distToGround + detectDistance);
        }

        private bool IsJumpEnded()
        {
            return Physics.Raycast(transform.position + _colliderCenterPos, -Vector3.up, 
                       _distToGround + jumpEndDetectDistance);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                CameraRotation();
                CharacterRotation();
            }
            CharacterRotationToFitCam();

            if (UI.ChattingUI.ChattingUI.Get().InputFieldActivated())
                return;
            
            Move();

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                _myRigid.AddForce(Vector3.up * 300.0f);
                _animController.SetTrigger(Animator.StringToHash("jump"));
            }
            _animController.SetIsJumpEnded(IsJumpEnded());

            if (Input.GetKeyDown(KeyCode.E) && _switching == null)
                StartCoroutine(_switching = SwitchView());
        }

        private void Move()
        {
            var selfTransform = transform;
            
            var moveDirX = Input.GetAxisRaw("Horizontal");
            var moveDirZ = Input.GetAxisRaw("Vertical");

            var moveHorizontal = transform.right * moveDirX;
            var moveVertical = selfTransform.forward * moveDirZ;

            var velocity = (moveHorizontal + moveVertical).normalized * walkSpeed;

            _myRigid.MovePosition(selfTransform.position + velocity * Time.deltaTime);
            
            // anim value setting
            if(moveDirX > 1e-3)
                _animController.SetHorizontalSpeed(1.0f);
            else if(moveDirX < -1e-3)
                _animController.SetHorizontalSpeed(-1.0f);
            else
                _animController.SetHorizontalSpeed(0.0f);
            
            if(moveDirZ > 1e-3)
                _animController.SetVerticalSpeed(1.0f);
            else if(moveDirZ < -1e-3)
                _animController.SetVerticalSpeed(-1.0f);
            else
                _animController.SetVerticalSpeed(0.0f);
        }


        private void CharacterRotation()
        {
            if (_switching != null || !_firstPersonView) return;
            
            var yRotation = Input.GetAxisRaw("Mouse X");
            transform.Rotate(Vector3.up, yRotation * lookSensitivity);
        }
        
        // http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
        private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
        {
            return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
        }

        private void CharacterRotationToFitCam()
        {
            if (_switching != null || _firstPersonView) return;
            
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) < 1e-3 && 
                Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 1e-3) return;
            var pivotRotWorld = camPivot.eulerAngles;
            var targetRot = Quaternion.LookRotation(camPivot.forward, Vector3.up);
            transform.rotation = Damp(transform.rotation, targetRot, 5.0f, Time.deltaTime);
            camPivot.eulerAngles = pivotRotWorld;
        }

        private IEnumerator SwitchView()
        {
            var camTransform = theCamera.transform;
            var r = 0.0f;
            var src = camTransform.localPosition;
            var dst = _firstPersonView ? thirdCamPos.localPosition : originalCamPos.localPosition;
            var camRotSrc = camTransform.localRotation;
            var camRotDst = _firstPersonView ? thirdCamPos.localRotation : originalCamPos.localRotation;
            var pivotRotSrc = camPivot.localRotation;
            var pivotRotDst = Quaternion.identity;
            while (r < 1.0f)
            {
                camTransform.localPosition = Vector3.Lerp(src, dst, r);
                camTransform.localRotation = Quaternion.Lerp(camRotSrc, camRotDst, r);
                camPivot.localRotation = Quaternion.Lerp(pivotRotSrc, pivotRotDst, r);
                r = Mathf.Clamp(r + switchSpeed * Time.deltaTime, 0, 1);
                yield return null;
            }

            camTransform.localPosition = dst;
            camTransform.localRotation = camRotDst;
            camPivot.localRotation = pivotRotDst;
            _defaultEulerAngle = camTransform.localEulerAngles;
            _currentCameraRotationX = 0;
            _firstPersonView = !_firstPersonView;
            _switching = null;
        }

        private void CameraRotation()
        {
            if (_switching != null) return;
            var xRotation = Input.GetAxisRaw("Mouse Y");
            var cameraRotationX = xRotation * lookSensitivity;
            _currentCameraRotationX -= cameraRotationX;
            _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            
            if (!_firstPersonView)
            {
                var yRotation = Input.GetAxisRaw("Mouse X");
                var cameraRotationY = yRotation * lookSensitivity;
                camPivot.Rotate(Vector3.up, cameraRotationY);
            }

            var newRotCam = _defaultEulerAngle;
            newRotCam.x += _currentCameraRotationX;
            theCamera.transform.localEulerAngles = newRotCam;
        }
    }
}

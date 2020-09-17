using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gallery.MultiPlay
{
    public class OtherPlayerController : MonoBehaviour
    {
        [SerializeField] private float lerpTime;
        [SerializeField] private Renderer playerRenderer;

        private Vector3 _posOrigin;
        private Vector3 _posDest;
        private Quaternion _rotOrigin = Quaternion.identity;
        private Quaternion _rotDest = Quaternion.identity;
        private float _lerpVal = 0.0f;
        private Transform _transform;
        private Animator _animator;

        private void Awake()
        {
            Debug.Assert(playerRenderer);
            
            _transform = transform;
            _animator = GetComponent<Animator>();
            Debug.Assert(_animator);
        }

        public void SetRendererEnabled(bool val)
        {
            playerRenderer.enabled = val;
        }

        public void SetOriginalTransform(Vector3 pos, Quaternion rot)
        {
            _posOrigin = pos;
            _rotOrigin = rot;
        }

        public void UpdateTransform(Vector3 posDest, Vector3 rotDest)
        {
            _posOrigin = _transform.position;
            _posDest = posDest;
            _rotOrigin = _transform.rotation;
            _rotDest = Quaternion.identity;
            _rotDest.eulerAngles = rotDest;
            _lerpVal = 0.0f;
        }

        public void AnimBoolChange(int id, bool value)
        {
            _animator.SetBool(id, value);
        }

        public void AnimTrigger(int id)
        {
            _animator.SetTrigger(id);
        }

        // Update is called once per frame
        void Update()
        {
            // Debug.Log(_posOrigin.ToString() + _posDest.ToString());
            _lerpVal = Mathf.Clamp(_lerpVal + Time.deltaTime / lerpTime, 0, 1);
            _transform.position = Vector3.Lerp(_posOrigin, _posDest, _lerpVal);
            _transform.rotation = Quaternion.Lerp(_rotOrigin, _rotDest, _lerpVal);
        }
    }
}
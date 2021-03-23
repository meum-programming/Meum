using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Socket
{
    /*
     * @brief RemotePlayer를 제어하는 컴포넌트
     * @details Socket.io로 받아온 위치 정보를 보간해서 위치를 정해주며 애니메이션을 업데이트, RemotePlayer을 정보를 저장
     */
    public class RemotePlayerController : MonoBehaviour
    {
        [SerializeField] private float lerpTime;
        [SerializeField] private Text nicknameField;
        
        [NonSerialized] public int UserPrimaryKey;
        public string Nickname
        {
            get { return _nickname; }
            set { 
                _nickname = value;
                nicknameField.text = _nickname;
            }
        }
        private string _nickname;

        private Vector3 _posOrigin;
        private Vector3 _posDest;
        private Quaternion _rotOrigin = Quaternion.identity;
        private Quaternion _rotDest = Quaternion.identity;
        private float _lerpVal = 0.0f;
        private Transform _transform;
        [SerializeField] Animator _animator;
        public PlayerChaChange playerChaChange;
        [SerializeField] Canvas playerCanvas;


        private Tween chaPosTween;
        private Tween chaRotTween;
        
        private void Awake()
        {
            _transform = transform;
            //_animator = GetComponent<Animator>();
            Debug.Assert(_animator);
        }

        public void SetRendererEnabled(bool val)
        {
            //gameObject.SetActive(val);
            playerChaChange.gameObject.SetActive(val);
            playerCanvas.gameObject.SetActive(val);
            //playerRenderer.enabled = val;
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
            _lerpVal = 0.0f;

            if (chaPosTween != null && chaPosTween.IsPlaying())
            {
                chaPosTween.Kill();
            }
            chaPosTween = transform.DOLocalMove(posDest, 0.3f);

            if (chaRotTween != null && chaRotTween.IsPlaying())
            {
                chaRotTween.Kill();
            }
            chaRotTween = _animator.transform.DOLocalRotate(rotDest, 0.3f);

        }

        public void AnimBoolChange(int id, bool value)
        {
            _animator.SetBool(id, value);
        }

        public void AnimFloatChange(int id, float value)
        {
            _animator.SetFloat(id, value);
        }

        public void AnimTrigger(int id)
        {
            _animator.SetTrigger(id);
        }

        public void AnimGesture(string name)
        {
            _animator.Play(name);
        }

        // Update is called once per frame
        void Update()
        {
            _lerpVal = Mathf.Clamp(_lerpVal + Time.deltaTime / lerpTime, 0, 1);
        }
    }
}
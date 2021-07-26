using Core.Socket;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.BuilderScene;
using UnityEngine;
//using UnityEngine.InputSystem;

namespace Game.Builder.Camera
{
    /*
     * @brief Build scene 에서 카메라의 이동(회전X)을 담당하는 컴포넌트
     */
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float runSpeed;
        [SerializeField] private Transform camera;
        [SerializeField] private BuilderSceneVerifyModals verifyModalManager;

        private Vector3 _moveVector;
        private bool _running;

        [SerializeField] private TMP_InputField urlInputField;

        private void Start()
        {
            ResetPosition();
        }

        public void ResetPosition()
        {
            //var spawnSite = GameObject.Find("SpawnSite").transform;
            //transform.position = spawnSite.position;
            transform.position = DataSynchronizer.Get().GetSpawnPos();
        }

        /*
         * @brief Update 함수에서 실제 이동을 함, OnMove는 moveVector 갱신만
         */
        private void Update()
        {
            InputCheck();

            if (verifyModalManager.showingModal) return;
            
            var direction = camera.right * _moveVector.x +
                            camera.forward * _moveVector.y;
            direction.Normalize();

            var delta = direction * ((_running ? runSpeed : speed) * Time.deltaTime);
            transform.Translate(delta, Space.World);
        }

        void InputCheck()
        {
            OnMove();
            OnRun();
        }

        public void OnMove()
        {
            if (urlInputField.isFocused)
                return;

            Vector2 moveValue = Vector2.zero;

            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
            {
                moveValue = Vector2.zero;
            }
            else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    moveValue.y += 1;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveValue.y -= 1;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    moveValue.x -= 1;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    moveValue.x += 1;
                }
            }

            //조이스틱 방향이 대각선이라면
            if (moveValue.x != 0 && moveValue.y != 0)
            {
                moveValue = new Vector2(moveValue.x * 0.74f, moveValue.y * 0.74f);
            }

            _moveVector = moveValue;
        }

        public void OnRun()
        {
            _running = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
    }
}

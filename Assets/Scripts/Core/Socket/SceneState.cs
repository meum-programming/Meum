using UnityEngine;
using UnityEngine.Assertions;

namespace Core.Socket
{
    /*
     * @brief 비정상적인 Scene간 이동을 막기 위해 Scene 이동 상태를 관리하는 클래스
     */
    public class SceneState
    {
        private enum State
        {
            NotInTheGalleryOrSquare,
            InTheSquare,
            InTheGallery,
            InTheGalleryOwn,
            InTheGalleryOwnEdit,
        }
        private State _currentState;

        public SceneState()
        {
            _currentState = State.NotInTheGalleryOrSquare;
        }

        public void Quit()
        {
            Assert.IsTrue(_currentState == State.InTheGallery ||
                          _currentState == State.InTheGalleryOwn ||
                          _currentState == State.InTheSquare
            );

            _currentState = State.NotInTheGalleryOrSquare;
        }

        public void EnterSquare()
        {
            Assert.IsTrue(_currentState == State.NotInTheGalleryOrSquare);
            
            _currentState = State.InTheSquare;
        }

        public void EnterGallery()
        {
            Assert.IsTrue(_currentState == State.NotInTheGalleryOrSquare);

            _currentState = State.InTheGallery;
        }

        public void EnterGalleryOwn()
        {
            Assert.IsTrue(_currentState == State.NotInTheGalleryOrSquare ||
                          _currentState == State.InTheGalleryOwnEdit);

            _currentState = State.InTheGalleryOwn;
        }

        public void EnterEdit()
        {
            Assert.IsTrue(_currentState == State.InTheGalleryOwn);
            
            _currentState = State.InTheGalleryOwnEdit;
        }

        public bool IsInGallery()
        {
            return _currentState == State.InTheGallery || _currentState == State.InTheGalleryOwn;
        }

        public bool IsInSquare()
        {
            return _currentState == State.InTheSquare;
        }

        public bool IsSubOfGallery()
        {
            return _currentState == State.InTheGallery || 
                   _currentState == State.InTheGalleryOwn || 
                   _currentState == State.InTheGalleryOwnEdit;
        }

        public bool IsSubOfGalleryOwn()
        {
            return _currentState == State.InTheGalleryOwn ||
                   _currentState == State.InTheGalleryOwnEdit;
        }

        public bool IsSubOfSquare()
        {
            return _currentState == State.InTheSquare;
        }

        public bool IsNotInGalleryOrSquare()
        {
            return _currentState == State.NotInTheGalleryOrSquare;
        }
    }
}

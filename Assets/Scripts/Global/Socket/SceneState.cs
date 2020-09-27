using UnityEngine;

namespace Global.Socket
{
    public class SceneState
    {
        private enum State
        {
            NotInTheGalleryOrSquare,
            InTheSquare,
            InTheSquareShop,
            InTheGallery,
            InTheGalleryShop,
            InTheGalleryOwn,
            InTheGalleryOwnShop,
            InTheGalleryOwnEdit,
        }

        private State _currentState;

        public SceneState()
        {
            _currentState = State.NotInTheGalleryOrSquare;
        }

        public void Quit()
        {
            if (_currentState != State.InTheGallery &&
                _currentState != State.InTheGalleryOwn &&
                _currentState != State.InTheSquare)
            {
                Debug.LogError("Global.Socket.SceneState - Quit: Invalid state transition");
                Application.Quit(-1);
            }

            _currentState = State.NotInTheGalleryOrSquare;
        }

        public void EnterSquare()
        {
            if (_currentState != State.NotInTheGalleryOrSquare &&
                _currentState != State.InTheSquareShop)
            {
                Debug.LogError("Global.Socket.SceneState - EnterSquare: Invalid state transition");
                Application.Quit(-1);
            }

            _currentState = State.InTheSquare;
        }

        public void EnterGallery()
        {
            if (_currentState != State.NotInTheGalleryOrSquare &&
                _currentState != State.InTheGalleryShop)
            {
                Debug.LogError("Global.Socket.SceneState - EnterGallery: Invalid state transition");
                Application.Quit(-1);
            }

            _currentState = State.InTheGallery;
        }

        public void EnterGalleryOwn()
        {
            if (_currentState != State.NotInTheGalleryOrSquare &&
                _currentState != State.InTheGalleryOwnShop &&
                _currentState != State.InTheGalleryOwnEdit)
            {
                Debug.Log(_currentState.ToString());
                Debug.LogError("Global.Socket.SceneState - EnterGalleryOwn: Invalid state transition");
                Application.Quit(-1);
            }

            _currentState = State.InTheGalleryOwn;
        }

        public void EnterShop()
        {
            if (_currentState != State.InTheGallery &&
                _currentState != State.InTheGalleryOwn &&
                _currentState != State.InTheSquare)
            {
                Debug.LogError("Global.Socket.SceneState - EnterShopping: Invalid state transition");
                Application.Quit(-1);
            }

            if (_currentState == State.InTheGallery)
                _currentState = State.InTheGalleryShop;
            else if (_currentState == State.InTheGalleryOwn)
                _currentState = State.InTheGalleryOwnShop;
            else
                _currentState = State.InTheSquareShop;
        }

        public void EnterEdit()
        {
            if (_currentState != State.InTheGalleryOwn)
            {
                Debug.LogError("Global.Socket.SceneState - EnterEdit: Invalid state transition");
                Application.Quit(-1);
            }

            _currentState = State.InTheGalleryOwnEdit;
        }

        public void ReturnToGallery()
        {
            if (_currentState != State.InTheGalleryShop &&
                _currentState != State.InTheGalleryOwnShop &&
                _currentState != State.InTheGalleryOwnEdit)
            {
                Debug.LogError("Global.Socket.SceneState - ReturnToGallery: Invalid state transition");
                Application.Quit(-1);
            }
            //
            // if (_currentState == State.InTheGalleryShop)
            //     _currentState = State.InTheGallery;
            // else
            //     _currentState = State.InTheGalleryOwn;
        }

        public void ReturnToSquare()
        {
            if (_currentState != State.InTheSquareShop)
            {
                Debug.LogError("Global.Socket.SceneState - ReturnToSquare: Invalid state transition");
                Application.Quit(-1);
            }

            _currentState = State.InTheSquare;
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
            return _currentState == State.InTheGallery || _currentState == State.InTheGalleryShop ||
                   _currentState == State.InTheGalleryOwn || _currentState == State.InTheGalleryOwnShop ||
                   _currentState == State.InTheGalleryOwnEdit;
        }

        public bool IsSubOfGalleryOwn()
        {
            return _currentState == State.InTheGalleryOwn || _currentState == State.InTheGalleryOwnShop ||
                   _currentState == State.InTheGalleryOwnEdit;
        }

        public bool IsSubOfSquare()
        {
            return _currentState == State.InTheSquare || _currentState == State.InTheSquareShop;
        }

        public bool IsNotInGalleryOrSquare()
        {
            return _currentState == State.NotInTheGalleryOrSquare;
        }
    }
}

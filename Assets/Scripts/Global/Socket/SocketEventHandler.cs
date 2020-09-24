using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySocketIO;
using UnitySocketIO.Events;

namespace Global.Socket
{
    public class SocketEventHandler
    {
        private SocketIOController _socket;
        private SceneState _state;
        private SceneLoader _loader;

        private int _id = -1;

        public SocketEventHandler(SocketIOController socket, 
                                  SceneState state, SceneLoader loader)
        {
            _socket = socket;
            _state = state;
            _loader = loader;
        }

        public void AssignHandler()
        {
            _socket.On("galleryEnteringSuccess", OnGalleryEnteringSuccess);
            _socket.On("squareEnteringSuccess", OnSquareEnteringSuccess);
            _socket.On("userQuit", OnUserQuit);
            _socket.On("userInfo", OnUserInfo);
            _socket.On("animTrigger", OnAnimTrigger);
            _socket.On("animBoolChange", OnAnimBoolChange);
            _socket.On("animFloatChange", OnAnimFloatChange);
            _socket.On("changeCharacter", OnChangeCharacter);
            _socket.On("updateArtworks", OnUpdateArtworks);
            _socket.On("chatting", OnChatting);
        }
    
        #region Event Handlers
        private void OnGalleryEnteringSuccess(SocketIOEvent e)
        {
            var data = JsonConvert.DeserializeObject<EnteringSuccessEventData>(e.data);
            _id = data.id;
            _loader.LoadGallery(data);
        }
        
        
        private void OnSquareEnteringSuccess(SocketIOEvent e)
        {
            var data = JsonConvert.DeserializeObject<SquareEnteringSuccessEventData>(e.data);
            _id = data.id;
            _loader.LoadSquare(data);
        }

        private void OnUserQuit(SocketIOEvent e)
        {
            var data = JsonConvert.DeserializeObject<UserQuitEventData>(e.data);
            if (_id == data.id)
            { 
                MeumDB.Get().ClearTextureBuffer();
                DataSyncer.Get().Reset();
                SceneManager.LoadScene("Lobby");
            }
            else
            {
                DataSyncer.Get().DeactivatePlayer(data.id);
            }
        }

        private void OnUserInfo(SocketIOEvent e)
        {
            if (_state.IsNotInGalleryOrSquare()) return;
            
            var data = JsonConvert.DeserializeObject<UserInfoEventData>(e.data);
            if (data.id != _id)
            {
                DataSyncer.Get().UpdateOtherPlayer(data);
            }
        }

        private void OnAnimTrigger(SocketIOEvent e)
        {
            if (_state.IsNotInGalleryOrSquare()) return;
            
            var data = JsonConvert.DeserializeObject<AnimTriggerEventData>(e.data);
            if (data.id == _id) return;
            DataSyncer.Get().AnimTrigger(data.id, data.name);
        }

        private void OnAnimBoolChange(SocketIOEvent e)
        {
            if (_state.IsNotInGalleryOrSquare()) return;
            
            var data = JsonConvert.DeserializeObject<AnimBoolChangeEventData>(e.data);
            if (data.id == _id) return;
            DataSyncer.Get().AnimBoolChange(data.id, data.name, data.value);
        }
        
        private void OnAnimFloatChange(SocketIOEvent e)
        {
            if (_state.IsNotInGalleryOrSquare()) return;
            
            var data = JsonConvert.DeserializeObject<AnimFloatChangeEventData>(e.data);
            if (data.id == _id) return;
            DataSyncer.Get().AnimFloatChange(data.id, data.name, data.value);
        }

        private void OnChangeCharacter(SocketIOEvent e)
        {
            if (_state.IsNotInGalleryOrSquare()) return;
            
            var data = JsonConvert.DeserializeObject<ChangeCharacterEventData>(e.data);
            if (data.id == _id)
            {
                DataSyncer.Get().ChangePlayerCharacter(data.charId);
            }
            else
            {
                DataSyncer.Get().ChangeOtherPlayerCharacter(data.id, data.charId);
            }
        }

        private void OnUpdateArtworks(SocketIOEvent e)
        {
            if(_state.IsInGallery())
                _loader.SerializeArtworks();
        }

        private void OnChatting(SocketIOEvent e)
        {
            if (_state.IsInGallery() || _state.IsInSquare())
            {
                var data = JsonConvert.DeserializeObject<ChattingData>(e.data);
                if (!UI.ChattingUI.ChattingUI.InstanceExist())
                {
                    Debug.LogError("Global.Socket.SocketEventHandler - OnChatting : ChattingUI must exist when player in the gallery or square");
                    Application.Quit(-1);
                }

                string nickname;
                if (_id == data.id)
                    nickname = MeumSocket.Get().PlayerInfo.nickname;
                else
                    nickname = DataSyncer.Get().ID2Nickname(data.id);
                UI.ChattingUI.ChattingUI.Get().AddMessage(nickname, data.message);
            }
        }
        #endregion
        
        #region Event Data structs
        public struct EnteringSuccessEventData
        {
            public int id;
            public int roomId;
            public int roomType;
            public int maxN;
        }
        
        public struct SquareEnteringSuccessEventData
        {
            public int id;
            public int maxN;
        }
        
        private struct UserQuitEventData
        {
            public int id;
        }
        
        public struct UserInfoEventData
        {
            public int id;
            public Vector3 position;
            public Vector3 rotation;
            public int userKey;
            public string nickname;
        }
        
        private struct AnimTriggerEventData
        {
            public int id;
            public string name;
        }

        private struct AnimBoolChangeEventData
        {
            public int id;
            public string name;
            public bool value;
        }
        
        private struct AnimFloatChangeEventData
        {
            public int id;
            public string name;
            public float value;
        }
        
        private struct ChangeCharacterEventData
        {
            public int id;
            public int charId;
        }
        
        // updateArtworks does not have event data
        
        private struct ChattingData
        {
            public int id;
            public string message;
        }
        #endregion
    }
}

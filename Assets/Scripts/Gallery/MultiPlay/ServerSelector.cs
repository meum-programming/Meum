using System;
using SocketIO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerSelector : MonoBehaviour
{
    private InputField type;
    private InputField id;
    private InputField maxN;
    private SocketIOComponent _socket = null;

    private static ServerSelector _globalInstance = null;

    private void Awake()
    {
        if (_globalInstance == null)
        {
            _socket = GetComponent<SocketIOComponent>();
            _socket.Connect();
            _socket.autoConnect = true;
            DontDestroyOnLoad(gameObject);
            _globalInstance = this;
            SceneManager.sceneLoaded += Load;
        } else
            Destroy(gameObject);
    }

    private void Load(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ServerSelect_forTest")
        {
            Debug.Log("ServerSelect scene");
            type = GameObject.Find("InputField_GalType").GetComponent<InputField>();
            id = GameObject.Find("InputField_RoomId").GetComponent<InputField>();
            maxN = GameObject.Find("InputField_MaxN").GetComponent<InputField>();

            GameObject.Find("Enter").GetComponent<Button>().onClick.AddListener(Enter);
            GameObject.Find("Create").GetComponent<Button>().onClick.AddListener(Create);
        }
    }

    public void Enter()
    {
        JSONObject data = new JSONObject();
        data.AddField("roomId", Int32.Parse(id.text));
        _socket.Emit("enteringRoom", data);
    }

    public void Create()
    {
        JSONObject data = new JSONObject();
        var roomType = Convert.ToInt32(type.text);
        data.AddField("roomType", roomType);
        data.AddField("roomId", Convert.ToInt32(id.text));
        data.AddField("maxN", Convert.ToInt32(maxN.text));
        _socket.Emit("createRoom", data);
    }

    public void Quit()
    {
        _socket.Emit("quitRoom");
        SceneManager.LoadScene("ServerSelect_forTest");
        GetComponent<DataSyncer>().ResetRoom();
    }
}

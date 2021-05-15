using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Test_WebWaiting_Room : MonoBehaviour
{
    [SerializeField] WebHandler webHandler;

    // Start is called before the first frame update
    void Start()
    {
        TokenSetBtnClick();
        RoomSetBtnClick();
        //AddresableTest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TokenSetBtnClick()
    {
        //webHandler.SetToken("61");
        webHandler.SetToken("60");
        //webHandler.SetToken("59");
        //webHandler.SetToken("149");
        //webHandler.SetToken("1");
        //webHandler.SetToken("157");
        //webHandler.SetToken("242");
        //webHandler.SetToken("238");
        //webHandler.SetToken("8");

    }

    public void RoomSetBtnClick()
    {
        //webHandler.EnterRoom("Guest");
        webHandler.EnterRoom("믐MASTER");
        //webHandler.EnterRoom("믐대표서비스");
        //webHandler.EnterRoom("blackgallery");
        //webHandler.EnterRoom("string");
        //webHandler.EnterRoom("the_other_layers");
        //webHandler.EnterRoom("sangminTest");
        //webHandler.EnterRoom("unseenland");
        //webHandler.EnterRoom("UGKIM");
    }

    void AddresableTest()
    {

        //Debug.LogWarning(Addressables.kAddressablesRuntimeDataPath);

#if !UNITY_EDITOR
    PlayerPrefs.DeleteKey( Addressables.kAddressablesRuntimeDataPath );
#endif

        StartCoroutine(loadOn());
        


        //obj.transform.localScale = Vector3.one * 100;
    }

    IEnumerator loadOn()
    {
        //AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("sofa");
        //yield return handle;
        //if (handle.Result != null)
          //  Complete(handle.Result);
          
        var data = Addressables.InstantiateAsync("sofa", Vector3.zero, Quaternion.identity, transform);
        yield return data.IsDone;


        Complete(data.Result);
    }

    void Complete(GameObject resultObj)
    {
        GameObject obj = Instantiate(resultObj, Vector3.zero, Quaternion.identity, transform);
        Debug.LogWarning(obj);
    }



}

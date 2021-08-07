using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoSingleton<AddressableManager>
{
    private Dictionary<string, GameObject> objDic = new Dictionary<string, GameObject>();

    private void Awake()
    {
        PlayerPrefs.DeleteKey(Addressables.kAddressablesRuntimeDataPath);
    }

    /// <summary>
    /// 오브젝트 리턴
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="parant"></param>
    /// <param name="completed"></param>
    public void GetObj(string objName, UnityAction<GameObject> completed = null)
    {
        string path = objName.Replace("artwork_1master.meum/", "");
        path = path.Replace("https://api.meum.me/datas/", "");

        //Dictionary에 값이 없다면
        if (objDic.ContainsKey(path) == false)
        {
            //다운로드 실행
            StartCoroutine(DownLoadObj(path, completed));
        }
        else
        {
            CompletedOn(path, completed);
        }
    }

    /// <summary>
    /// 다운로드 실행
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="parant"></param>
    /// <param name="completed"></param>
    /// <returns></returns>
    public IEnumerator DownLoadObj(string objName, UnityAction<GameObject> completed = null)
    {
        bool downLoadOn = false;

        string path = objName.Replace("artwork_1master.meum/", "");
        path = path.Replace("https://api.meum.me/datas/", "");

        //Dictionary에 값이 없다면
        if (objDic.ContainsKey(path) == false)
        {
            //다운로드 시작
            Addressables.InstantiateAsync(path, transform).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (handle.Result != null && objDic.ContainsKey(path) == false )
                {
                    //obj를 Dictionary에 추가한다.
                    handle.Result.gameObject.SetActive(false);
                    objDic.Add(path, handle.Result);
                }
                downLoadOn = true;
            };    
        }
        else
        {
            downLoadOn = true;
        }

        //다운로드 완료될때까지 대기
        yield return new WaitUntil(() => downLoadOn);

        CompletedOn(path, completed);
    }

    void CompletedOn(string objName, UnityAction<GameObject> completed)
    {
        //완료 이벤트 실행
        if (completed != null && objDic.ContainsKey(objName))
            completed(objDic[objName]);
    }

}

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
        //Dictionary에 값이 없다면
        if (objDic.ContainsKey(objName) == false)
        {
            //다운로드 실행
            StartCoroutine(DownLoadObj(objName, completed));
        }
        else
        {
            CompletedOn(objName, completed);
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

        //Dictionary에 값이 없다면
        if (objDic.ContainsKey(objName) == false)
        {
            //다운로드 시작
            Addressables.InstantiateAsync(objName, transform).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (objDic.ContainsKey(objName) == false)
                {
                    //obj를 Dictionary에 추가한다.
                    handle.Result.gameObject.SetActive(false);
                    objDic.Add(objName, handle.Result);
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

        CompletedOn(objName, completed);
    }

    void CompletedOn(string objName, UnityAction<GameObject> completed)
    {
        //완료 이벤트 실행
        if (completed != null && objDic.ContainsKey(objName))
            completed(objDic[objName]);
    }

}

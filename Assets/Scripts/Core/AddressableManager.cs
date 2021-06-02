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

    public void GetObj(string objName , UnityAction<GameObject> completed = null)
    {
        if (objDic.ContainsKey(objName))
        {
            if (completed != null)
            {
                completed(objDic[objName]);
            }
        }
        else
        {
            Addressables.InstantiateAsync(objName, transform).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (objDic.ContainsKey(objName) == false)
                {
                    objDic.Add(objName, handle.Result);
                }

                if (completed != null)
                {
                    completed(objDic[objName]);
                }

            };
        }
        
    }

}

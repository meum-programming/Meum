using Core.Socket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GestureManager
{
	private static GestureManager instance;
	public static GestureManager Instance
	{
		get
		{
			if (instance == null)
			{
				if (instance == null)
				{
					instance = new GestureManager();
				}
			}
			return instance;
		}
	}

	Dictionary<int, GestureModel> modelData = new Dictionary<int, GestureModel>();
	Dictionary<int, int> userSlotData = new Dictionary<int, int>();
	public UnityAction slotChangeOn = null;

	private GestureManager()
	{
		Init();
	}

	void Init()
	{
		ModelDataSet();
	}

	void ModelDataSet()
	{
		List<GestureModel> modelDataList = Resources.Load<MeumSaveData>("MeumSaveData").gestureModelList;
		modelData = new Dictionary<int, GestureModel>();

		for (int i = 0; i < modelDataList.Count; i++)
        {
			modelData.Add(modelDataList[i].id, modelDataList[i]);
		}

		GestureRequest gestureRequest = new GestureRequest()
		{
			requestStatus = 0,
			uid = MeumSocket.Get().GetPlayerPk(),
			successOn = ResultData =>
			{
				EmojiDataSet((GestureData)ResultData);
			}
		};
		gestureRequest.RequestOn();

    }

	/// <summary>
	/// 유저 저장 정보 세팅
	/// </summary>
	/// <param name="gestureData"></param>
	void EmojiDataSet(GestureData gestureData)
	{
		//저장된 값이 없다면
		if (gestureData.data.Length == 1 && gestureData.data[0] == string.Empty) 
		{
			userSlotData = new Dictionary<int, int>();
			userSlotData.Add(0, 0);
			userSlotData.Add(1, 1);
			userSlotData.Add(2, 2);
			userSlotData.Add(3, 3);
			UserDataRequest();
			return;
		}

		userSlotData = new Dictionary<int, int>();

		for (int i = 0; i < gestureData.data.Length; i++)
        {
			int value = int.Parse(gestureData.data[i]);

            if (value != -1)
			{
				userSlotData.Add(i, value);
			}
		}

		if (slotChangeOn != null)
		{
			slotChangeOn();
		}
	}

	public GestureModel GetModel(int index)
	{
        if (modelData.ContainsKey(index))
		{
			return modelData[index];
		}

		return null;
	}

	public GestureModel GetSlotData(int index)
	{
		if (index == -1)
		{
			return GetModel(index);
		}

		if (userSlotData.ContainsKey(index))
		{
			int modelId = userSlotData[index];
			return GetModel(modelId);
		}

		return null;
	}

	public void SetSlotData(int index , int modelId)
	{
        if (modelId == -1)
		{
			if (userSlotData.ContainsKey(index))
			{
				userSlotData.Remove(index);
			}
		}
        else
		{
			if (userSlotData.ContainsKey(index))
			{
				userSlotData[index] = modelId;
			}
			else
			{
				userSlotData.Add(index, modelId);
			}
		}

		UserDataRequest();

		if (slotChangeOn != null)
		{
			slotChangeOn();
		}
	}

	public void SlotAllClear()
	{
		userSlotData = new Dictionary<int, int>();

		UserDataRequest();

		if (slotChangeOn != null)
		{
			slotChangeOn();
		}

	}

	void UserDataRequest()
	{
		string data = string.Empty;

		List<int> dataList = new List<int>();

        for (int i = 0; i < 10; i++)
        {
			int index = -1;

			if (userSlotData.ContainsKey(i))
			{
				index = userSlotData[i];
			}

			dataList.Add(index);
		}

        for (int i = 0; i < dataList.Count; i++)
        {
            if (i > 0)
			{
				data += ",";
			}

			data += dataList[i];
		}

        GestureRequest gestureRequest = new GestureRequest()
        {
            requestStatus = 1,
            uid = MeumSocket.Get().GetPlayerPk(),
            userSaveData = data
        };
        gestureRequest.RequestOn();


    }
}

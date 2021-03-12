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

		userSlotData = new Dictionary<int, int>();
		userSlotData.Add(0, 0);
		userSlotData.Add(1, 1);
		userSlotData.Add(2, 2);
		userSlotData.Add(3, 3);

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

        

		if (slotChangeOn != null)
		{
			slotChangeOn();
		}
	}

	public void SlotAllClear()
	{
		userSlotData = new Dictionary<int, int>();

        if (slotChangeOn != null)
		{
			slotChangeOn();
		}

	}

}

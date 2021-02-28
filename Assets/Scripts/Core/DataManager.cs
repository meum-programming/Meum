using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
	private static DataManager instance;
	public static DataManager Instance
	{
		get
		{
			if (instance == null)
			{
				if (instance == null)
				{
					instance = new DataManager();
				}
			}
			return instance;
		}
	}

	private DataManager() 
	{
		Init();
	}

	public ChaCustomizingSaveData chaCustomizingSaveData = null;

	void Init()
	{
		chaCustomizingSaveData = new ChaCustomizingSaveData(0, 0, 0, 0);
	}
}

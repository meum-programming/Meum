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

	public float mouseSensitivityValue = 0.5f;
	public string mouseSensitivityKey = "mouseSensitivity";
	public bool roomSaveOn = false;

	private DataManager() 
	{
		Init();
	}

	public ChaCustomizingSaveData chaCustomizingSaveData = null;

	void Init()
	{
		chaCustomizingSaveData = new ChaCustomizingSaveData(0, 0, 0, 0);

		mouseSensitivityValue = PlayerPrefs.GetFloat(mouseSensitivityKey, mouseSensitivityValue);
	}

	public void SetMouseSensitivityValue(float mouseSensitivityValue)
	{
		this.mouseSensitivityValue = mouseSensitivityValue;

		PlayerPrefs.SetFloat(mouseSensitivityKey, mouseSensitivityValue);
		PlayerPrefs.Save();
	}

	public float GetMouseSensitivityValue()
	{
		if (mouseSensitivityValue == 0)
		{
			return 0.1f;
		}

		return mouseSensitivityValue;
	}

}

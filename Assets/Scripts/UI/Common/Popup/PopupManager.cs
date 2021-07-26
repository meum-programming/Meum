using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupManager : MonoBehaviour
{
	private static PopupManager instance;
	public static PopupManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<PopupManager>();

				if (instance == null)
				{
					instance = new GameObject("PopupManager").AddComponent<PopupManager>();
					//GameObject.DontDestroyOnLoad(instance.gameObject);
				}
			}
			return instance;
		}
	}

	void Awake()
	{
		Init();
	}

	void Init() 
	{
		GalleryController galleryController = FindObjectOfType<GalleryController>();
		transform.SetParent(galleryController.transform);
		transform.localPosition = Vector3.zero;
	}


	public void OkPopupCreate(string contensStr, UnityAction okBtnClickEvent, UnityAction cancelBtnClickEvent = null , bool maskClickDestoryOn = false, string okBtnStr = "예", string cancelBtnStr = "아니오")
	{
		OkPopup okPopup = Instantiate(Resources.Load<OkPopup>("Popup/OkPopup"), transform);
		okPopup.DataSet(contensStr, okBtnClickEvent , cancelBtnClickEvent , maskClickDestoryOn,  okBtnStr, cancelBtnStr);

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BGMListItem : MonoBehaviour
{
    [SerializeField] Text bgmNameText;
    [SerializeField] Text maxTimeText;

    private int index = 0;
    private BGMSaveData bGMSaveData = null;

    public UnityAction<int> clickOn = null;

    // Start is called before the first frame update
    public void BGMDataSet(int index , BGMSaveData bGMSaveData)
    {
        this.index = index;
        this.bGMSaveData = bGMSaveData;
        bgmNameText.text = bGMSaveData.name;
        maxTimeText.text = GetTimeText(bGMSaveData.maxTime);
    }

    string GetTimeText(int timeValue)
    {
        int min = timeValue / 60;
        int sec = timeValue % 60;

        return string.Format("{0}:{1:D2}", min, sec);
    }

    public void ClickOn()
    {
        if (clickOn != null)
        {
            clickOn(index);
        }
    }

}

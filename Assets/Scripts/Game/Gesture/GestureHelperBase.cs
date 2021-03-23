using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureHelperBase : MonoBehaviour
{
    public GestureSettingPopup gestureSettingPopup;
    [HideInInspector] public GestureModel gestureModel;
    [SerializeField] protected Image iconImage;
    [SerializeField] protected Image nullImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

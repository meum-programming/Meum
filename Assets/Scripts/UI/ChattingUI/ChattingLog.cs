using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChattingLog : MonoBehaviour
{
    #region SerializeFields
    
    [SerializeField] private TextMeshProUGUI nickname;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private Image badgeImage;
    [SerializeField] private Color[] colorByTypes;
    
    #endregion
    
    #region PrivateFields
    
    private RectTransform _transform;
    
    #endregion
    
    #region PublicFields
    
    public float height => _transform.sizeDelta.y;

    #endregion
    
    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
    }

    public void SetData(string senderStr, string contentStr, int type)
    {
        if(ReferenceEquals(_transform, null))
            _transform = GetComponent<RectTransform>();
        nickname.text = senderStr;
        content.text = contentStr;

        nickname.color = colorByTypes[type];
        content.color = colorByTypes[type];
        
        var sizeDelta = _transform.sizeDelta;
        sizeDelta.y = content.preferredHeight;
        _transform.sizeDelta = sizeDelta;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class BannerSetting : MonoBehaviour
{
    [SerializeField] private InputField urlInputField;
    [SerializeField] private UI.Content selectedContent;
    [SerializeField] private Button applyBtn;

    private void Awake()
    {
        applyBtn.onClick.AddListener(Apply);
    }

    public void SetSelectedContent(UI.Content content)
    {
        selectedContent.data = content.data;
    }

    private void Apply()
    {
        var placer = GameObject.Find("Artworks").GetComponent<Game.Artwork.ArtworkPlacer>();
        placer.CreateSelectedBanner(selectedContent, urlInputField.text);
    }
}

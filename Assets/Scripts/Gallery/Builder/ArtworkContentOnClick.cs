using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.Builder;
using UnityEngine;
using UnityEngine.UI;

public class ArtworkContentOnClick : MonoBehaviour
{
    private void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(SelectSelf);
    }

    private void SelectSelf()
    {
        var placer = GameObject.Find("Artworks").GetComponent<ArtworkPlacer>();
        placer.Select(GetComponent<UI.Content>());
    }
}

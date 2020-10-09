﻿using UnityEngine;
using UnityEngine.UI;

public class TutorialModal : MonoBehaviour
{
    [SerializeField] private Sprite[] tutorialPages;
    [SerializeField] private Image page;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button closeButton;  
    
    private int _currentPage = 0;

    private void Awake()
    {
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextPage);
        
        prevButton.onClick.RemoveAllListeners();
        prevButton.onClick.AddListener(PrevPage);
        
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Close);
        
        prevButton.gameObject.SetActive(false);
        page.sprite = tutorialPages[_currentPage];
    }

    private void NextPage()
    {
        if (_currentPage + 1 >= tutorialPages.Length) return;
        
        ++_currentPage;
        page.sprite = tutorialPages[_currentPage];
        
        if(_currentPage == (tutorialPages.Length-1))
            nextButton.gameObject.SetActive(false);
        else
            nextButton.gameObject.SetActive(true);
        prevButton.gameObject.SetActive(true);
    }

    private void PrevPage()
    {
        if (_currentPage - 1 < 0) return;
        
        --_currentPage;
        page.sprite = tutorialPages[_currentPage];
        
        if(_currentPage == 0)
            prevButton.gameObject.SetActive(false);
        else
            prevButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}

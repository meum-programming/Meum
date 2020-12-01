using System.Collections;
using Core;
using UnityEngine;
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

    private void Start()
    {
        StartCoroutine(CheckIfTutorialAndActivate());
    }

    private IEnumerator CheckIfTutorialAndActivate()
    {
        var cd = new CoroutineWithData(this, MeumDB.Get().GetIsTutorial());
        yield return cd.coroutine;
        if (cd.result == null)
            yield break;
        var isTutorial = (bool)cd.result;
        
        if (isTutorial)
            StartCoroutine(MeumDB.Get().PatchTutorialClear());
        else
            gameObject.SetActive(false);
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
    
    public void Show()
    {
        if (gameObject.activeSelf) return;

        _currentPage = 0;
        prevButton.gameObject.SetActive(false);
        page.sprite = tutorialPages[_currentPage];

        gameObject.SetActive(true);
    }
}

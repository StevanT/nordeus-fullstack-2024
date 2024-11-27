using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    [SerializeField] private FadeableScript screenCover;

    [SerializeField] private FadeableScript titleCover;
    [SerializeField] private FadeableScript endingTextFadeable, endingTextBackgroundFadeable;
    [SerializeField] private Text endingText, endingTextBackground;
    
    private bool mapInputEnabled = false;
    private bool uiInputEnabled = false;

    public bool UIInputEnabled() 
    { return uiInputEnabled; }

    public bool MapInputEnabled()
    { return uiInputEnabled ? mapInputEnabled : uiInputEnabled; }

    public void ShowTitle(string title)
    {
        endingText.text = endingTextBackground.text = title;
        endingTextFadeable.FadeIn();
        endingTextBackgroundFadeable.FadeIn();
        titleCover.FadeIn();
        mapInputEnabled = false;
    }

    public void ShowScreen()
    {
        screenCover.FadeOut();
        StartCoroutine(CShowScreen());
    }

    public void ChangeToScene(string sceneName)
    {
        uiInputEnabled = mapInputEnabled = false;
        screenCover.FadeIn();
        StartCoroutine(CChangeToScene(sceneName));
    }

    private IEnumerator CShowScreen()
    {
        yield return new WaitForSeconds(.55f);
        uiInputEnabled = mapInputEnabled = true;
    }
    
    private IEnumerator CChangeToScene(string sceneName)
    {
        yield return new WaitForSeconds(.55f);
        SceneManager.LoadScene(sceneName);
    }
}

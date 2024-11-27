using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TransitionController))]
public class MainMenuScript : MonoBehaviour
{
    private TransitionController transitionController;

    [Header("Graphics")]
    [SerializeField] private Sprite toggleOn;
    [SerializeField] private Sprite toggleOff;

    [Header("UI")] 
    [SerializeField] private Image offlineToggle;
    [SerializeField] private Image offlineToggleBack;
    [SerializeField] private Button classicButton;
    [SerializeField] private Button timedButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button offlineButton;
    
    [Header("Audio")]
    [SerializeField] private AudioControllerScript audioController;

    void Start()
    {
        transitionController = GetComponent<TransitionController>();
        transitionController.ShowScreen();

        offlineToggle.sprite = offlineToggleBack.sprite = GridScript.online ? toggleOff : toggleOn;
        
        classicButton.onClick.AddListener(ButtonClassicClick);
        timedButton.onClick.AddListener(ButtonTimedClick);
        statsButton.onClick.AddListener(ButtonStatsClick);
        offlineButton.onClick.AddListener(ButtonOfflineClick);
    }

    void ButtonClassicClick()
    {
        if (!transitionController.UIInputEnabled()) return;
        
        audioController.PlayButtonClick();
        transitionController.ChangeToScene("ClassicScreen");
    }

    void ButtonTimedClick()
    {
        if (!transitionController.UIInputEnabled()) return;
        
        audioController.PlayButtonClick();
        transitionController.ChangeToScene("TimedScreen");
    }
    
    void ButtonOfflineClick()
    {
        if (!transitionController.UIInputEnabled()) return;
        
        audioController.PlayButtonClick();
        GridScript.online = !GridScript.online;
        offlineToggle.sprite = offlineToggleBack.sprite = GridScript.online ? toggleOff : toggleOn;
    }

    void ButtonStatsClick()
    {
        if (!transitionController.UIInputEnabled()) return;
        
        audioController.PlayButtonClick();
        transitionController.ChangeToScene("StatsScreen");
    }
    
}

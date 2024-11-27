using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StatsControllerScript))]
[RequireComponent(typeof(TransitionController))]
public class StatsScript : MonoBehaviour
{
    private TransitionController transitionController;

    [Header("UI")] 
    [SerializeField] private Button backButton;

    [Header("Stats UI")] 
    [SerializeField] private Text onlineStats;
    [SerializeField] private Text onlineStatsBack;
    [SerializeField] private Text offlineStats;
    [SerializeField] private Text offlineStatsBack;
    
    [Header("Audio")]
    [SerializeField] private AudioControllerScript audioController;

    private void Start()
    {
        transitionController = GetComponent<TransitionController>();
        transitionController.ShowScreen();
        
        backButton.onClick.AddListener(ButtonBackClick);

        Statistics stats = StatsControllerScript.GetAllStats();

        string onlineStatsText = "";
        onlineStatsText += "Classic Games Won: " + stats.OnlineGameWon + "/" + stats.OnlineGamesPlayed + "\n";
        onlineStatsText += "Classic Game Accuracy: " + stats.OnlineAverageAccuracy.ToString("n2") + "% \n";
        onlineStatsText += "Timed High Score: " + stats.OnlineTimedHighScore + "\n";
        string offlineStatsText = "";
        offlineStatsText += "Classic Games Won: " + stats.OfflineGamesWon + "/" + stats.OfflineGamesPlayed + "\n";
        offlineStatsText += "Classic Game Accuracy: " + stats.OfflineAverageAccuracy.ToString("n2") + "% \n";
        offlineStatsText += "Timed High Score: " + stats.OfflineTimedHighScore + "\n";

        onlineStats.text = onlineStatsBack.text = onlineStatsText;
        offlineStats.text = offlineStatsBack.text = offlineStatsText;
    }
    
    private void ButtonBackClick()
    {
        if (!transitionController.UIInputEnabled()) return;
        
        audioController.PlayButtonClick();
        transitionController.ChangeToScene("MenuScreen");
    }
}

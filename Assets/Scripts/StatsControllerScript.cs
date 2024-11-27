using UnityEngine;

public class StatsControllerScript : MonoBehaviour
{
    public void AddPlayedGame(bool online)
    {
        string statName = online ? "onlineGamesPlayed" : "offlineGamesPlayed";
        if (!PlayerPrefs.HasKey(statName))
        {
            PlayerPrefs.SetInt(statName, 1);
            return;
        }
        PlayerPrefs.SetInt(statName, PlayerPrefs.GetInt(statName) + 1);
    }
    
    public void AddWonGame(bool online)
    {
        string statName = online ? "onlineGamesWon" : "offlineGamesWon";
        if (!PlayerPrefs.HasKey(statName))
        {
            PlayerPrefs.SetInt(statName, 1);
            return;
        }
        PlayerPrefs.SetInt(statName, PlayerPrefs.GetInt(statName) + 1);
    }
    
    public void AddGuess(bool online, float accuracy)
    {
        string guessesName = online ? "onlineGuesses" : "offlineGuesses";
        string accuracyName = online ? "onlineAverageAccuracy" : "offlineAverageAccuracy";
        if (!PlayerPrefs.HasKey(guessesName))
        {
            PlayerPrefs.SetInt(guessesName, 1);
            PlayerPrefs.SetFloat(accuracyName, accuracy);
            return;
        }
        PlayerPrefs.SetFloat(accuracyName,
            (PlayerPrefs.GetFloat(accuracyName) * PlayerPrefs.GetInt(guessesName)
            + accuracy) / (PlayerPrefs.GetInt(guessesName) + 1)
        );
        PlayerPrefs.SetInt(guessesName, PlayerPrefs.GetInt(guessesName) + 1);
    }

    public void AddHighScore(bool online, int score)
    {
        PlayerPrefs.SetInt(
            online ? "onlineTimedHighScore" : "offlineTimedHighScore",
            score
            );
    }

    public static int GetHighScore(bool online)
    {
        return PlayerPrefs.GetInt(
            online ? "onlineTimedHighScore" : "offlineTimedHighScore",
            0
            );
    }

    public static Statistics GetAllStats()
    {
        return new Statistics
        {
            OnlineGamesPlayed = PlayerPrefs.GetInt("onlineGamesPlayed", 0),
            OnlineGameWon = PlayerPrefs.GetInt("onlineGamesWon", 0),
            OnlineGuesses = PlayerPrefs.GetInt("onlineGuesses", 0),
            OnlineAverageAccuracy = PlayerPrefs.GetFloat("onlineAverageAccuracy", 0),
            OnlineTimedHighScore = PlayerPrefs.GetInt("onlineTimedHighScore", 0),
            OfflineGamesPlayed = PlayerPrefs.GetInt("offlineGamesPlayed", 0),
            OfflineGamesWon = PlayerPrefs.GetInt("offlineGamesWon", 0),
            OfflineGuesses = PlayerPrefs.GetInt("offlineGuesses", 0),
            OfflineAverageAccuracy = PlayerPrefs.GetFloat("offlineAverageAccuracy", 0),
            OfflineTimedHighScore = PlayerPrefs.GetInt("offlineTimedHighScore", 0)
        };
    }
}

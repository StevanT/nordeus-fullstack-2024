using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private Text successText;
    [SerializeField] private Text successTextBackground;

    public void SetSuccessText(float success)
    {
        successText.text = successTextBackground.text =
            success.ToString("n1") + "%";
    }
    
    public void SetScoreText(int score)
    {
        successText.text = successTextBackground.text = score.ToString();
    }
}

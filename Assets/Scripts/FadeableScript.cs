using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FadeableScript : MonoBehaviour
{
    [SerializeField] private Animator fadeAnimator;
    
    private static readonly int Transparent = Animator.StringToHash("transparent");

    public void FadeIn() { fadeAnimator.SetBool(Transparent, false); }
    public void FadeOut() { fadeAnimator.SetBool(Transparent, true); }
}

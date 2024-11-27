using UnityEngine;

public class AudioControllerScript : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    [Header("Sounds")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip waterClick;
    [SerializeField] private AudioClip correctClick;
    [SerializeField] private AudioClip wrongClick;
    [SerializeField] private AudioClip success;
    [SerializeField] private AudioClip failure;

    private void PlaySoundSetPitch(AudioClip clip)
    {
        source.pitch = 1;
        source.clip = clip;
        source.Play();
    }

    private void PlaySoundRandomPitch(AudioClip clip)
    {
        source.pitch = Random.Range(0.95f, 1.05f);
        source.clip = clip;
        source.Play();
    }

    public void PlayButtonClick() { PlaySoundRandomPitch(buttonClick); }
    public void PlayWaterClick() { PlaySoundRandomPitch(waterClick); }
    public void PlayCorrectClick() { PlaySoundRandomPitch(correctClick); }
    public void PlayWrongClick() { PlaySoundRandomPitch(wrongClick); }
    
    public void PlaySuccess() { PlaySoundSetPitch(success); }
    public void PlayFailure() { PlaySoundSetPitch(failure); }
}

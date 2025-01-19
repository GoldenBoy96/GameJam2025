using UnityEngine;

public class AudioLoadingControllerDemo : MonoBehaviour
{
    public void PlayAudioDemo()
    {
        AudioManager.Instance.PlayAudio(AudioConstants.DEMO_SOUND);
    }
}

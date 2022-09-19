using UnityEngine;

public class SePlayer : MonoBehaviour
{

    [Tooltip("AudioSource components.")]
    private AudioSource _audioSource;

    [Tooltip("AudioClips available to play.")]
    [SerializeField] private AudioClip[] _audioClips;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Fire and forget. Made for shorter AudioClips (footsteps, hits, jumps, etc).
    public void PlayOneShot(int audioClipIndex)
    {
        _audioSource.PlayOneShot(_audioClips[audioClipIndex], 1);
    }

    /*
    Used to ignore AudioListener.pause, a method which pauses all AudioSources. Useful
    to keep some audios playing when a pause screen is triggered, for example.
    */
    public void IgnoreListenerPause(bool value)
    {
        _audioSource.ignoreListenerPause = value;
    }

}
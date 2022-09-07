using UnityEngine;

public class AudioPlayer : MonoBehaviour
{

    [Tooltip("AudioSource components.")]
    [SerializeField] private AudioSource[] _audioSources;
    [SerializeField] private AudioClip[] _audioClips;

    // Fire and forget. Made for shorter AudioClips (footsteps, hits, jumps, etc).
    public void PlayOneShot(int audioClipIndex){
        _audioSources[0].PlayOneShot(_audioClips[audioClipIndex], 1);
    }

    /*
    Only one AudioClip can be played at a time per AudioSource this way, but the audio
    can be managed as it plays. Made for longer AudioClips (mainly music and ambience).
    */
    public void Play(int audioClipIndex, int audioSourceIndex = 0)
    {
        _audioSources[audioSourceIndex].clip = _audioClips[audioClipIndex];
        _audioSources[audioSourceIndex].Play();
    }

    public void Stop(int audioSourceIndex = 0)
    {
        _audioSources[audioSourceIndex].Stop();
    }

    /*
    Used to ignore AudioListener.pause, a method which pauses all AudioSources. Useful
    for keeping some audios playing when a pause screen is triggered, for instance.
    */
    public void SetIgnoreListenerPause(bool value, int audioSourceIndex = 0){
        _audioSources[audioSourceIndex].ignoreListenerPause = value;
    }

}
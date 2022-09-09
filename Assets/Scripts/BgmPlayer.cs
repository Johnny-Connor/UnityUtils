using System.Collections;
using UnityEngine;

public class BgmPlayer : MonoBehaviour
{

    [Tooltip("AudioClips available to play.")]
    [SerializeField] private AudioClip[] _audioClips;

    // AudioSource components.
    private AudioSource _mainAudioSource, _auxAudioSource; /* They must be altered together
    because both AudioSources are used during the .FadeToAudioClip method. */

    // Variables.
    private bool _isMainAudioSourceSupposedToBePlaying = true;
    private float _volumeBeforeFade; /* Must update this value whenever the volume is edited
    outside the .FadeToAudioClip method. */

    private void Awake() {

        void CopyAudioSourceFields(AudioSource copyFrom, AudioSource pasteTo){
            pasteTo.mute = copyFrom.mute;
            pasteTo.outputAudioMixerGroup = copyFrom.outputAudioMixerGroup;
            pasteTo.bypassEffects = copyFrom.bypassEffects;
            pasteTo.bypassListenerEffects = copyFrom.bypassListenerEffects;
            pasteTo.playOnAwake = copyFrom.playOnAwake;
            pasteTo.loop = copyFrom.loop;
            pasteTo.priority = copyFrom.priority;
            pasteTo.volume = copyFrom.volume;
            pasteTo.pitch = copyFrom.pitch;
            pasteTo.panStereo = copyFrom.panStereo;
            pasteTo.spatialBlend = copyFrom.spatialBlend;
            pasteTo.reverbZoneMix = copyFrom.reverbZoneMix;
            pasteTo.dopplerLevel = copyFrom.dopplerLevel;
            pasteTo.spread = copyFrom.spread;
            pasteTo.rolloffMode = copyFrom.rolloffMode;
            pasteTo.minDistance = copyFrom.minDistance;
            pasteTo.maxDistance = copyFrom.maxDistance;
        }

        _mainAudioSource = GetComponent<AudioSource>();
        _volumeBeforeFade = _mainAudioSource.volume;
        _auxAudioSource = gameObject.AddComponent<AudioSource>();
        CopyAudioSourceFields(_mainAudioSource, _auxAudioSource);

    }

    /*
    Only one AudioClip can be played at a time per AudioSource this way, but the audio
    can be managed as it plays. Made for longer AudioClips (mainly music and ambience).
    */
    public void Play(int audioClipIndex){
        if (!_mainAudioSource.isPlaying && !_auxAudioSource.isPlaying){
            if (_isMainAudioSourceSupposedToBePlaying){
                _mainAudioSource.clip = _audioClips[audioClipIndex];
                _mainAudioSource.Play();
            }
            else{
                _auxAudioSource.clip = _audioClips[audioClipIndex];
                _auxAudioSource.Play();
            }
        }
        else{
            Debug.LogError("BgmPlayer.Play was called when an AudioClip was being played in one or both BGM AudioSources of the GameObject. If you want to switch AudioClips, use BgmPlayer.FadeToAudioClip instead.");
        }
    }

    // Stops the AudioSources from playing their current AudioClips.
    public void Stop(){
        _mainAudioSource.Stop();
        _auxAudioSource.Stop();
    }

    /*
    Used to ignore AudioListener.pause, a method which pauses all AudioSources. Useful
    to keep some audios playing when a pause screen is triggered, for example.
    */
    public void IgnoreListenerPause(bool isIgnoringListenerPause){
        _mainAudioSource.ignoreListenerPause = isIgnoringListenerPause;
        _auxAudioSource.ignoreListenerPause = isIgnoringListenerPause;
    }

    // Fades to another AudioClip.
    public void FadeToAudioClip(int fadeToAudioClipIndex, float fadeTime = 1.25f){

        IEnumerator AudioClipTransition(){
            float fadeTimeElapsed = 0;
            if (_isMainAudioSourceSupposedToBePlaying){
                _auxAudioSource.clip = _audioClips[fadeToAudioClipIndex];
                _auxAudioSource.Play();
                while (fadeTimeElapsed < fadeTime){
                    _mainAudioSource.volume = Mathf.Lerp(_volumeBeforeFade, 0, fadeTimeElapsed/fadeTime);
                    _auxAudioSource.volume = Mathf.Lerp(0, _volumeBeforeFade, fadeTimeElapsed/fadeTime);
                    fadeTimeElapsed += Time.deltaTime;
                    yield return null;
                }
                // Rounding final volume values because Time.deltaTime is not 100% precise.
                _auxAudioSource.volume = _volumeBeforeFade;
                _mainAudioSource.volume = 0;
                _mainAudioSource.Stop();
            }
            else{
                _mainAudioSource.clip = _audioClips[fadeToAudioClipIndex];
                _mainAudioSource.Play();
                while (fadeTimeElapsed < fadeTime){
                    _auxAudioSource.volume = Mathf.Lerp(_volumeBeforeFade, 0, fadeTimeElapsed/fadeTime);
                    _mainAudioSource.volume = Mathf.Lerp(0, _volumeBeforeFade, fadeTimeElapsed/fadeTime);
                    fadeTimeElapsed += Time.deltaTime;
                    yield return null;
                }
                // Rounding final volume values because Time.deltaTime is not 100% precise.
                _mainAudioSource.volume = _volumeBeforeFade;
                _auxAudioSource.volume = 0;
                _auxAudioSource.Stop();
            }
            _isMainAudioSourceSupposedToBePlaying = !_isMainAudioSourceSupposedToBePlaying;
        }

        StopAllCoroutines();
        StartCoroutine(AudioClipTransition());

    }

}
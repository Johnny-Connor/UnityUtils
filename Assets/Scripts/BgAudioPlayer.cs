using System.Collections;
using UnityEngine;

public class BgAudioPlayer : MonoBehaviour
{

    // Settings.
    [Tooltip("When fading to another AudioClip, will the new song start from the beginning?")]
    [SerializeField] private bool _ResetAudioClipUponTransition;

    // AudioClips.
    [Tooltip("AudioClips available to play.")]
    [SerializeField] private AudioClip[] _audioClips;
    private int[] _audioClipsTimeSamples; /* Stores the playback position of each
    AudioClip. */

    // AudioSources.
    private AudioSource _originalAudioSource, _auxAudioSource; /* Their properties must be 
    altered together because both AudioSources are used during .FadeToAudioClip */
    private bool _isOriginalAudioSourceBeingUsed = true; /* If false, _auxAudioSource will
    be used. */

    // Other variables.
    private bool _isAudioClipTransitionCoroutineRunning;
    private float _volumeSetBeforeFade; /* Used in .FadeToAudioClip to define how high the 
    volume of the fading in AudioSource will be by the end of the effect. Must update this
    value whenever an AudioSource's volume is altered. */
    private int _selectedAudioClipIndex = -1; /* AudioClip selected to be played in the
    current instance of .FadeToAudioClip. */
    private int _previouslySelectedAudioClipIndex = -1; /* AudioClip selected to be played
    in the previous instance of .Play or .FadeToAudioClip. */

    private void Awake()
    {

        void CopyAudioSourceProperties(AudioSource copyFrom, AudioSource pasteTo)
        {
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

        _audioClipsTimeSamples = new int[_audioClips.Length];

        _originalAudioSource = GetComponent<AudioSource>();
        _volumeSetBeforeFade = _originalAudioSource.volume;

        _auxAudioSource = gameObject.AddComponent<AudioSource>();
        CopyAudioSourceProperties(_originalAudioSource, _auxAudioSource);

    }

    /* Only one AudioClip can be played at a time per AudioSource this way, but the audio
    can be managed as it plays. Made for longer AudioClips (mainly music and ambience). */
    public void Play(int audioClipIndex)
    {
        if (!_originalAudioSource.isPlaying && !_auxAudioSource.isPlaying)
        {
            if (_isOriginalAudioSourceBeingUsed)
            {
                _originalAudioSource.clip = _audioClips[audioClipIndex];
                _originalAudioSource.Play();
            }
            else
            {
                _auxAudioSource.clip = _audioClips[audioClipIndex];
                _auxAudioSource.Play();
            }
            _previouslySelectedAudioClipIndex = audioClipIndex;
        }
        else
        {
            Debug.LogWarning("Can't play this AudioClip because another AudioClip is already being played by this script. If you want to switch AudioClips, use BgmPlayer.FadeToAudioClip instead.");
        }
    }

    // Stops the AudioSources from playing their current AudioClips.
    public void Stop()
    {
        _originalAudioSource.Stop();
        _auxAudioSource.Stop();
    }

    /* Used to ignore AudioListener.pause, a method which pauses all AudioSources. Useful to
    keep some audios playing when a pause screen is triggered, for example. */
    public void IgnoreListenerPause(bool isIgnoringListenerPause)
    {
        _originalAudioSource.ignoreListenerPause = isIgnoringListenerPause;
        _auxAudioSource.ignoreListenerPause = isIgnoringListenerPause;
    }

    // Fades to another AudioClip.
    public void FadeToAudioClip(int fadeToAudioClipIndex, float fadeTime = 1.25f)
    {

        IEnumerator AudioClipTransition(AudioSource fadingOutAudioSource, AudioSource fadingInAudioSource)
        {

            void SaveTimeSamples()
            {
                // Registering the playback position of the fading in AudioClip.
                _audioClipsTimeSamples[fadeToAudioClipIndex] = fadingInAudioSource.timeSamples;

                if (fadingOutAudioSource.isPlaying)
                {
                    // Registering the playback position of the fading out AudioClip.
                    _audioClipsTimeSamples[_previouslySelectedAudioClipIndex] = fadingOutAudioSource.timeSamples;
                }
            }

            _isAudioClipTransitionCoroutineRunning = true;

            // Switching to the desired fade in AudioClip and playing it.
            fadingInAudioSource.clip = _audioClips[fadeToAudioClipIndex];
            if (!_ResetAudioClipUponTransition)
            {
                fadingInAudioSource.timeSamples = _audioClipsTimeSamples[fadeToAudioClipIndex];
            }
            fadingInAudioSource.Play();

            // Variables used in while loop.
            float fadeTimeElapsed = 0;
            float FadingInVolumeBeforeWhile = fadingInAudioSource.volume;
            float FadingOutVolumeBeforeWhile = fadingOutAudioSource.volume;

            while (fadeTimeElapsed < fadeTime)
            {
                /* Saving the playback position of the AudioClips (in case this function is
                called again during this while loop). */
                SaveTimeSamples();

                fadingOutAudioSource.volume = Mathf.Lerp(FadingOutVolumeBeforeWhile, 0, fadeTimeElapsed/fadeTime);
                fadingInAudioSource.volume = Mathf.Lerp(FadingInVolumeBeforeWhile, _volumeSetBeforeFade, fadeTimeElapsed/fadeTime);
                
                fadeTimeElapsed += Time.deltaTime;
                
                yield return null;
            }

            // Rounding final volume values because Time.deltaTime is not 100% precise.
            fadingInAudioSource.volume = _volumeSetBeforeFade;
            fadingOutAudioSource.volume = 0;

            /* Saving the playback position from the faded out AudioClip one last time
            before stopping the faded out AudioSource. */
            fadingOutAudioSource.Pause();
            if (fadingOutAudioSource.isPlaying)
            {
                _audioClipsTimeSamples[_previouslySelectedAudioClipIndex] = fadingOutAudioSource.timeSamples;
            }
            fadingOutAudioSource.Stop();

            // Storing the fadeToAudioClipIndex of this instance.
            _previouslySelectedAudioClipIndex = _selectedAudioClipIndex;

            _isAudioClipTransitionCoroutineRunning = false;
        }

        // If this function is called again before the coroutine ends.
        if(_isAudioClipTransitionCoroutineRunning)
        {
            /* _previouslySelectedAudioClipIndex stores the _selectedAudioClipIndex from the
            previous instance of this function. */
            _previouslySelectedAudioClipIndex = _selectedAudioClipIndex;
        }

        if (fadeToAudioClipIndex == _previouslySelectedAudioClipIndex)
        {
            Debug.LogWarning("Can't fade to an AudioClip which is already being played.");
        }
        else
        {
            _selectedAudioClipIndex = fadeToAudioClipIndex;

            StopAllCoroutines();
            _isAudioClipTransitionCoroutineRunning = false;

            if (_isOriginalAudioSourceBeingUsed)
            {
                StartCoroutine( AudioClipTransition( _originalAudioSource, _auxAudioSource ) );
            }
            else
            {
                StartCoroutine( AudioClipTransition( _auxAudioSource, _originalAudioSource ) );            
            }
            _isOriginalAudioSourceBeingUsed = !_isOriginalAudioSourceBeingUsed;
        }

    }

}
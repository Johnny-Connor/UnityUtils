using System.Collections;
using UnityEngine;

public class BgAudioPlayer : MonoBehaviour
{

    // Settings.
    [Tooltip("Will the next song start from the beginning?")]
    [SerializeField] private bool _ResetAudioClipUponTransition;

    // AudioClips.
    [Tooltip("AudioClips available to play.")]
    [SerializeField] private AudioClip[] _audioClips;
    private int[] _audioClipsTimeSamples; /* Stores the playback position of each
    AudioClip. */

    // AudioSources.
    private AudioSource _originalAudioSource, _auxAudioSource; /* Their properties must be 
    altered together because both AudioSources are used during fading effects */
    private bool _isOriginalAudioSourceBeingUsed = true; /* False when the other AudioSource
    is being used. */

    // Variables used in .Play.
    private float _maxVolumeSetBeforeFade; /* How high the volume of the fading in
    AudioSource will be by the end of the .Play function. Must update this value whenever
    the volume of an AudioSource is altered */
    private int _requestedAudioClipIndex = -1; /* AudioClip requested to be played in the
    current instance of the .Play function. */

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
        _maxVolumeSetBeforeFade = _originalAudioSource.volume;
        _originalAudioSource.volume = 0;

        _auxAudioSource = gameObject.AddComponent<AudioSource>();
        CopyAudioSourceProperties(_originalAudioSource, _auxAudioSource);

    }

    /*
    Only one AudioClip can be played at a time per AudioSource this way, but the audio can
    be managed as it plays. Made for longer AudioClips (mainly music and ambience).
    */
    public void Play(int fadeToAudioClipIndex, float fadeTime = 0)
    {

        int _previouslyRequestedAudioClipIndex; /* AudioClip requested to be played in the
        previous instance. */

        IEnumerator AudioClipTransition(AudioSource fadingInAudioSource, AudioSource fadingOutAudioSource)
        {

            void SaveTimeSamplesFromRequestedAudioClip()
            {
                // Registering the playback position of the fading in AudioClip.
                _audioClipsTimeSamples[_requestedAudioClipIndex] = fadingInAudioSource.timeSamples;
            }

            void SaveTimeSamplesFromPreviouslyRequestedAudioClip()
            {
                if (_previouslyRequestedAudioClipIndex >= 0)
                {
                    // Registering the playback position of the fading out AudioClip.
                    _audioClipsTimeSamples[_previouslyRequestedAudioClipIndex] = fadingOutAudioSource.timeSamples;
                }
            }

            /*
            Saving the playback position of the AudioClip requested in the previous instance
            for if it gets requested again at some point.
            */
            SaveTimeSamplesFromPreviouslyRequestedAudioClip();

            /*
            Switching to the desired fade in AudioClip and playing it at the correct
            playback position.
            */
            fadingInAudioSource.clip = _audioClips[fadeToAudioClipIndex];
            if (_ResetAudioClipUponTransition)
            {
                _audioClipsTimeSamples[fadeToAudioClipIndex] = 0;
            }
            fadingInAudioSource.timeSamples = _audioClipsTimeSamples[fadeToAudioClipIndex];
            fadingInAudioSource.Play();

            // Variables used in while loop below.
            float fadeTimeElapsed = 0;
            float FadingInVolumeBeforeWhile = fadingInAudioSource.volume;
            float FadingOutVolumeBeforeWhile = fadingOutAudioSource.volume;

            while (fadeTimeElapsed < fadeTime)
            {
                /*
                Saving the playback position of the fading in AudioClip (in case this
                function gets called again during this while loop).
                */
                SaveTimeSamplesFromRequestedAudioClip();

                fadingOutAudioSource.volume = Mathf.Lerp(FadingOutVolumeBeforeWhile, 0, fadeTimeElapsed/fadeTime);
                fadingInAudioSource.volume = Mathf.Lerp(FadingInVolumeBeforeWhile, _maxVolumeSetBeforeFade, fadeTimeElapsed/fadeTime);
                
                fadeTimeElapsed += Time.deltaTime;
                
                yield return null;
            }

            // Rounding final volume values because Time.deltaTime is not 100% precise.
            fadingInAudioSource.volume = _maxVolumeSetBeforeFade;
            fadingOutAudioSource.volume = 0;

            fadingOutAudioSource.Stop();
            
        }

        // Storing the AudioClip requested in the previous instance.
        _previouslyRequestedAudioClipIndex = _requestedAudioClipIndex;

        // Storing the AudioClip requested in this instance.
        _requestedAudioClipIndex = fadeToAudioClipIndex;

        if (_previouslyRequestedAudioClipIndex == _requestedAudioClipIndex && (_originalAudioSource.isPlaying || _auxAudioSource.isPlaying))
        {
            Debug.LogWarning("The requested AudioClip is already being played.");
        }
        else
        {
            StopAllCoroutines();

            if (_isOriginalAudioSourceBeingUsed)
            {
                StartCoroutine( AudioClipTransition( _auxAudioSource, _originalAudioSource ) );
            }
            else
            {
                StartCoroutine( AudioClipTransition( _originalAudioSource, _auxAudioSource ) );          
            }
            _isOriginalAudioSourceBeingUsed = !_isOriginalAudioSourceBeingUsed;
        }

    }

    // Stops the AudioSources from playing their current AudioClips.
    public void Stop()
    {
        /*
        .Pause is used to prevent the TimeSamples of the AudioSources from resetting (this 
        would make .SaveTimeSamplesFromPreviouslyRequestedAudioClip not work).
        */
        if (_isOriginalAudioSourceBeingUsed)
        {
            _originalAudioSource.Pause();
            _auxAudioSource.Stop();
        }
        else
        {
            _auxAudioSource.Pause();
            _originalAudioSource.Stop();
        }
    }

    /*
    Used to ignore AudioListener.pause, a method which pauses all AudioSources. Useful to
    keep some audios playing when a pause screen is triggered, for example.
    */
    public void IgnoreListenerPause(bool isIgnoringListenerPause)
    {
        _originalAudioSource.ignoreListenerPause = isIgnoringListenerPause;
        _auxAudioSource.ignoreListenerPause = isIgnoringListenerPause;
    }

}
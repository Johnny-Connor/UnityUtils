using System.Collections;
using UnityEngine;

public class BgAudioPlayer : MonoBehaviour
{

    #region Variables
    // Settings.
    [Tooltip("Will the next song start from the beginning?")]
    [SerializeField] private bool _resetAudioClipUponTransition;

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

    // Other variables.
    private float _maxVolumeSetBeforeFade; /* How high the volume of the fading in
    AudioSource will be by the end of the .Play function. Must update this value whenever
    the volume of an AudioSource is altered */
    private int _requestedAudioClipIndex = -1; /* AudioClip requested to be played in the
    current instance of the .Play function. Negative values represent silence. */
    #endregion Variables

    #region MonoBehaviour Functions/Methods
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
    #endregion MonoBehaviour Functions/Methods

    #region Custom Functions/Methods
    /*
    Only one AudioClip can be played at a time per AudioSource this way, but the audio can
    be managed as it plays. Made for longer AudioClips (mainly music and ambience).
    */
    public void Play(int audioClipIndex, float fadeTime = 0)
    {

        int _previouslyRequestedAudioClipIndex; /* AudioClip requested to be played in the
        previous instance of this function. */

        void AudioClipTransition (AudioSource NotInUseAudioSource, AudioSource InUseAudioSource)
        {

            void SwitchToAndPlayRequestedAudioClip()
            {
                if (audioClipIndex >= 0)
                {
                    /*
                    Switching to the requested AudioClip and playing it at the correct 
                    playback position.
                    */
                    NotInUseAudioSource.clip = _audioClips[audioClipIndex];
                    if (_resetAudioClipUponTransition)
                    {
                        _audioClipsTimeSamples[audioClipIndex] = 0;
                    }
                    NotInUseAudioSource.timeSamples = _audioClipsTimeSamples[audioClipIndex];
                    NotInUseAudioSource.Play();
                }
            }

            void SaveTimeSamplesFromPreviouslyRequestedAudioClip()
            {
                if (_previouslyRequestedAudioClipIndex >= 0)
                {
                    /*
                    Registering the playback position of the previously requested AudioClip.
                    */
                    _audioClipsTimeSamples[_previouslyRequestedAudioClipIndex] = InUseAudioSource.timeSamples;
                }
            }

            IEnumerator VolumeTransition()
            {

                void SaveTimeSamplesFromRequestedAudioClip()
                {
                    if (_requestedAudioClipIndex >= 0)
                    {
                        // Registering the playback position of the requested AudioClip.
                        _audioClipsTimeSamples[_requestedAudioClipIndex] = NotInUseAudioSource.timeSamples;
                    }
                }

                // Variables used in while loop below.
                float fadeTimeElapsed = 0;
                float FadingInVolumeBeforeWhile = NotInUseAudioSource.volume;
                float FadingOutVolumeBeforeWhile = InUseAudioSource.volume;

                while (fadeTimeElapsed < fadeTime)
                {
                    /*
                    Saving the playback position of the AudioClips (in case this function
                    gets called again during this while loop).
                    */
                    SaveTimeSamplesFromPreviouslyRequestedAudioClip();
                    SaveTimeSamplesFromRequestedAudioClip();

                    InUseAudioSource.volume = Mathf.Lerp(FadingOutVolumeBeforeWhile, 0, fadeTimeElapsed/fadeTime);
                    if (audioClipIndex >= 0)
                    {
                        NotInUseAudioSource.volume = Mathf.Lerp(FadingInVolumeBeforeWhile, _maxVolumeSetBeforeFade, fadeTimeElapsed/fadeTime);
                    }
                    else
                    {
                        NotInUseAudioSource.volume = Mathf.Lerp(FadingInVolumeBeforeWhile, 0, fadeTimeElapsed/fadeTime);                
                    }

                    fadeTimeElapsed += Time.deltaTime;
                    
                    yield return null;
                }

                if (audioClipIndex >= 0)
                {
                    /*
                    Rounding final volume values because Time.deltaTime is not 100% precise.
                    */
                    NotInUseAudioSource.volume = _maxVolumeSetBeforeFade;
                    InUseAudioSource.volume = 0;
                    InUseAudioSource.Stop();
                }
                else
                {
                    /*
                    Rounding final volume values because Time.deltaTime is not 100% precise.
                    */
                    NotInUseAudioSource.volume = 0;
                    InUseAudioSource.volume = 0;

                    /*
                    .Pause is used to prevent the TimeSamples of the AudioSources from 
                    resetting (this would make 
                    .SaveTimeSamplesFromPreviouslyRequestedAudioClip not work).
                    */
                    InUseAudioSource.Pause();
                    NotInUseAudioSource.Stop();
                }

            }

            /*
            Saving the playback position of the AudioClip requested in the previous instance
            for if it gets requested again at some point.
            */
            SaveTimeSamplesFromPreviouslyRequestedAudioClip();

            SwitchToAndPlayRequestedAudioClip();

            StopAllCoroutines();

            StartCoroutine( VolumeTransition() );

        }

        // Storing the AudioClip requested in the previous instance.
        _previouslyRequestedAudioClipIndex = _requestedAudioClipIndex;

        // Storing the AudioClip requested in this instance.
        _requestedAudioClipIndex = audioClipIndex;

        if ( ( _previouslyRequestedAudioClipIndex == _requestedAudioClipIndex ) || ( _previouslyRequestedAudioClipIndex <= -1 && _requestedAudioClipIndex <= -1 ) )
        {
            Debug.LogWarning("The requested AudioClip is already being played.");
        }
        else
        {
            if (_isOriginalAudioSourceBeingUsed)
            {
                AudioClipTransition(_auxAudioSource, _originalAudioSource);
            }
            else
            {
                AudioClipTransition(_originalAudioSource, _auxAudioSource);
            }
            _isOriginalAudioSourceBeingUsed = !_isOriginalAudioSourceBeingUsed;
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
    #endregion Custom Functions/Methods

    #region Properties
    public bool ResetAudioClipUponTransition
    {
        get { return _resetAudioClipUponTransition; }
        set { _resetAudioClipUponTransition = value; }
    }
    #endregion Properties

}
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioFader : MonoBehaviour
{
    //A nice friendly fader! Make sure to pop this on a gameobject with an AudioSource in order have it fade
    //I toyed with the idea of making this script inherit from AudioSource instead of Monobehaviour, but that seemed like it could be annoying! :)

    //The audio source we're fading
    AudioSource audioSource;

    [Header("Settings")]
    //Change these numbers to different limits if they aren't what you need! :)
    [Range(0.01f, 5f)]
    public float defaultFadeLength = 1;

    [Range(0.01f, 1f)]
    [Tooltip("The volume that fading in will fade up to - good if you don't want a source at max volume")]
    public float fadedInVolume = 1;

    private void Start()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    #region Fade in, unpause and fade in
    /// <summary>
    /// Nice and easy way to fade in - this does not play the sound, use <seealso cref="Unpause"/> for that.
    /// </summary>
    [ContextMenu("Fade In")]
    public void FadeIn()
    {
        FadeIn(defaultFadeLength);
    }

    /// <summary>
    /// Starts the audio before then fading back in
    /// </summary>
    [ContextMenu("Unpause then fade in")]
    public void Unpause()
    {
        FadeIn(defaultFadeLength, true);
    }

    /// <summary>
    /// Starts the audio before then fading back in, with a custom time that this will take in
    /// </summary>
    /// <param name="customFadeLength">The time you'd like it to take when fading out</param>
    public void Unpause(float customFadeLength)
    {
        FadeIn(customFadeLength, true);
    }

    /// <summary>
    /// Nice easy way to fade in, but with a custom time that it takes to fade in
    /// </summary>
    /// <param name="customFadeLength">The time you'd like it to take when fading out</param>
    /// <param name="changePlayStatus">If true, pause or play the audio on mute or unmute</param>
    public void FadeIn(float customFadeLength, bool changePlayStatus = false)
    {
        FadeTo(fadedInVolume, customFadeLength, changePlayStatus);
    }
    #endregion


    #region Fade out, fade out then pause
    /// <summary>
    /// Nice and easy way to fade out. This doesn't pause the audio
    /// </summary>
    [ContextMenu("Fade Out")]
    public void FadeOut()
    {
        FadeOut(defaultFadeLength);
    }

    /// <summary>
    /// Nice and easy way to fade out. This doesn't pause the audio and uses a custom fade length
    /// </summary>
    /// <param name="customFadeLength">The time youy'd like to take when fading out</param>
    public void FadeOut(float customFadeLength)
    {
        FadeOut(customFadeLength, false);
    }

    /// <summary>
    /// Fades out the audio, then pauses the source
    /// </summary>
    [ContextMenu("Fade then Pause")]
    public void Pause()
    {
        FadeOut(defaultFadeLength, true);
    }

    /// <summary>
    /// Nice and easy way to fade out, but with a custom time that it takes to fade out. This doesn't pause the audio.
    /// </summary>
    /// <param name="customFadeLength">The time you'd like to to take when fading out</param>
    /// <param name="changePlayStatus">If true, pause or play the audio on mute or unmute</param>
    public void FadeOut(float customFadeLength, bool changePlayStatus = false)
    {

        FadeTo(0, customFadeLength, changePlayStatus);
    }

    #endregion


    #region Main Fade Code
    /// <summary>
    /// Fades your audiosource to <paramref name="targetLevel"/> using the setting for defaultFadeLength set in the inspector
    /// </summary>
    /// <param name="targetLevel">The level you want to set your audio to</param>
    /// 
    public void FadeTo(float targetLevel, bool changePlayStatus = false)
    {
        StopAllCoroutines();
        StartCoroutine(FadeToCo(targetLevel, defaultFadeLength, changePlayStatus));
    }

    /// <summary>
    /// Fades your audiosource to <paramref name="targetLevel"/> using a custom time you pass in using <paramref name="customFadeLength"/>
    /// </summary>
    /// <param name="targetLevel">The level you want to set your audio to</param>
    /// <param name="customFadeLength">The time it takes to transition to targetLevel</param>
    public void FadeTo(float targetLevel, float customFadeLength, bool changePlayStatus = false)
    {
        StopAllCoroutines();

        if (customFadeLength <= 0)
        {
            Debug.LogWarning("customFadeLength can't have a negative number - that would be in the past! Using defaultFadeLength");
            customFadeLength = defaultFadeLength;
        }

        StartCoroutine(FadeToCo(targetLevel, customFadeLength, changePlayStatus));
    }


    /// <summary>
    /// The main fade coroutine - use the voids to call this, as they handle stopping this function so that you don't get 2 fades on the go at once!
    /// </summary>
    /// <param name="targetLevel">The the level to fade to</param>
    /// <param name="fadeLength">The time it takes for it to fade</param>
    /// <param name="changePlayStatus">If the audio is coming from muted, or will be muted, play or pause it accordingly if this is true. Leave false to leave the audio playing in the background</param>
    /// <returns></returns>
    IEnumerator FadeToCo(float targetLevel, float fadeLength, bool changePlayStatus = false)
    {

        if(audioSource.volume == 0 && changePlayStatus)
        {
            audioSource.Play();
        }

#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            Debug.Log("Can't use fading unless you're in play mode!");
            yield break;
        }
#endif

        float currentTime = 0;
        float start = audioSource.volume;
        
        while (currentTime < fadeLength)
        {
            //Add to the currentTime the amount of time between this frame and the last one, then we use that to make a percentage against the total time
            currentTime += Time.deltaTime;

            //Imagine a line, lerp is "Here are two numbers - start and targetLevel, what percent between those two numbers do we need
            //If you put in start, targetLevel, 0.1 - you'd get 10% of the way between the two, put 1 in and you'd get targetLevel immediately
            //By using currentTime that we're adding to against the total seconds, we get that percentage!
            audioSource.volume = Mathf.Lerp(start, targetLevel, currentTime / fadeLength);
            yield return null;
        }

        audioSource.volume = targetLevel;

        if(audioSource.volume == 0 && changePlayStatus)
        {
            audioSource.Pause();
        }

        yield break;
    }
    #endregion
}

// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads.
    /// </summary>
    [CommandInfo("Audio",
                 "Play Music",
                 "Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads.")]
    [AddComponentMenu("")]
    public class PlayMusic : Command
    {
        [Tooltip("Music sound clip to play")]
        [SerializeField] protected AudioClip musicClip;

        [Tooltip("Time to begin playing in seconds. If the audio file is compressed, the time index may be inaccurate.")]
        [SerializeField] protected float atTime;

        [Tooltip("The volume at which the music will play")]
        [Range(0f,1f)]
        [SerializeField] protected float volume = 1f;

        [Tooltip("The music will start playing again at end.")]
        [SerializeField] protected bool loop = true;
    
        [Tooltip("Length of time to fade out previous playing music.")]
        [SerializeField] protected float fadeDuration = 1f;

        [Tooltip("Music channel to use for this music clip.")]
        [Range(0,2)]
        [SerializeField] protected int audioChannel = 0;

        [Tooltip("Whether to synchronize this with channel 0 or not.")]
        [SerializeField] protected bool sync;

        #region Public members

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            float startTime = Mathf.Max(0, atTime);
            musicManager.PlayMusic(musicClip, loop, volume, fadeDuration, startTime, audioChannel, sync);
                
            Continue();
        }
                    
        public override string GetSummary()
        {
            if (musicClip == null)
            {
                return "Error: No music clip selected";
            }

            return musicClip.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
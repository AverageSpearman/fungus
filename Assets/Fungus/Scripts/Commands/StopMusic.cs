// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stops the currently playing game music.
    /// </summary>
    [CommandInfo("Audio", 
                 "Stop Music", 
                 "Stops the currently playing game music.")]
    [AddComponentMenu("")]
    public class StopMusic : Command
    {
        #region Public members

        [Tooltip("Music channel to stop.")]
        [Range(0,2)]
        [SerializeField] protected int audioChannel = 0;

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.StopMusic(audioChannel);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        public override string GetSummary()
        {
            return "Stop channel " + audioChannel;
        }

        #endregion
    }
}
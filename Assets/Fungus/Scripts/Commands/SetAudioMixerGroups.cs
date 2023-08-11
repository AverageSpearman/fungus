using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Fungus {
    /// <summary>
    /// Sets the mixer output groups for the music manager channels.
    /// </summary>
    [CommandInfo("Audio",
                    "Set Mixer Groups",
                    "Sets the mixer output groups for the music manager channels.")]
    [AddComponentMenu("")]
    public class SetAudioMixerGroups : Command
    {
        [Tooltip("Mixer group for Music")]
        [SerializeField] protected AudioMixerGroup musicMixerGroup;

        [Tooltip("Mixer group for Ambiance")]
        [SerializeField] protected AudioMixerGroup ambianceMixerGroup;

        [Tooltip("Mixer group for Effects")]
        [SerializeField]protected AudioMixerGroup effectMixerGroup;
        //CUSTOM: Sets mixer group outputs for music manager channels
        #region 

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            if(musicMixerGroup != null) musicManager.SetMusicMixerGroup(musicMixerGroup);
            if(ambianceMixerGroup != null) musicManager.SetAmbianceMixerGroup(ambianceMixerGroup);
            if(effectMixerGroup != null) musicManager.SetEffectMixerGroup(effectMixerGroup);
            Continue();
        }
        public override string GetSummary()
        {
            return "";
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }

}

// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Audio;

namespace Fungus
{
    /// <summary>
    /// Music manager which provides basic music and sound effect functionality.
    /// Music playback persists across scene loads.
    /// </summary>
    //[RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        protected AudioSource[] audioSourcesMusic;
        protected AudioSource audioSourceAmbiance;
        protected AudioSource audioSourceSoundEffect;
        

        void Reset()
        {
            int audioSourceCount = this.GetComponents<AudioSource>().Length;
            for (int i = 0; i < 5 - audioSourceCount; i++)
                gameObject.AddComponent<AudioSource>();

        }

        protected virtual void Awake()
        {
            Reset();
            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSourcesMusic = new AudioSource[3];
            audioSourcesMusic[0] = audioSources[0];
            audioSourcesMusic[1] = audioSources[1];
            audioSourcesMusic[2] = audioSources[2];
            audioSourceAmbiance = audioSources[3];
            audioSourceSoundEffect = audioSources[4];
        }

        protected virtual void Start()
        {
            foreach(AudioSource audioSourceMusic in audioSourcesMusic){
                audioSourceMusic.playOnAwake = false;
                audioSourceMusic.loop = true;
            }
            
        }

        #region Public members

        /// <summary>
        /// Plays game music using an audio clip.
        /// One music clip may be played at a time per channel.
        /// </summary>
        public void PlayMusic(AudioClip musicClip, bool loop, float volume, float fadeDuration, float atTime, int channel = 0, bool sync = false)
        {
            AudioSource audioSourceMusic = audioSourcesMusic[channel];
            if (audioSourceMusic == null || audioSourceMusic.clip == musicClip)
            {
                return;
            }

            if (Mathf.Approximately(fadeDuration, 0f))
            {
                audioSourceMusic.clip = musicClip;
                audioSourceMusic.loop = loop;
                audioSourceMusic.time = (sync)? audioSourcesMusic[0].time : atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                audioSourceMusic.Play();
            }
            else
            {
                float startVolume = audioSourceMusic.volume;

                LeanTween.value(gameObject, startVolume, 0f, fadeDuration/2)
                    .setOnUpdate((v) => {
                        // Fade out current music
                        audioSourceMusic.volume = v;
                    }).setOnComplete(() => {
                        // Play new music
                        audioSourceMusic.clip = musicClip;
                        audioSourceMusic.loop = loop;
                        audioSourceMusic.time = (sync)? audioSourcesMusic[0].time : atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                        audioSourceMusic.Play();
                        LeanTween.value(gameObject, 0f, volume, fadeDuration/2).setOnUpdate((v) => {
                        // Fade in new music
                        audioSourceMusic.volume = v;
                        });
                    });
            }
        }

        /// <summary>
        /// Plays a sound effect once, at the specified volume.
        /// </summary>
        /// <param name="soundClip">The sound effect clip to play.</param>
        /// <param name="volume">The volume level of the sound effect.</param>
        public virtual void PlaySound(AudioClip soundClip, float volume)
        {
            audioSourceSoundEffect.PlayOneShot(soundClip, volume);
        }

        /// <summary>
        /// Plays a sound effect with optional looping values, at the specified volume.
        /// </summary>
        /// <param name="soundClip">The sound effect clip to play.</param>
        /// <param name="loop">If the audioclip should loop or not.</param>
        /// <param name="volume">The volume level of the sound effect.</param>
        public virtual void PlayAmbianceSound(AudioClip soundClip, bool loop, float volume)
        {
            audioSourceAmbiance.loop = loop;
            audioSourceAmbiance.clip = soundClip;
            audioSourceAmbiance.volume = volume;
            audioSourceAmbiance.Play();
        }

        /// <summary>
        /// Shifts the game music pitch to required value over a period of time.
        /// </summary>
        /// <param name="pitch">The new music pitch value.</param>
        /// <param name="duration">The length of time in seconds needed to complete the pitch change.</param>
        /// <param name="onComplete">A delegate method to call when the pitch shift has completed.</param>
        public virtual void SetAudioPitch(float pitch, float duration, System.Action onComplete, int channel = 0)
        {
            AudioSource audioSourceMusic = audioSourcesMusic[channel];
            if (Mathf.Approximately(duration, 0f))
            {
                audioSourceMusic.pitch = pitch;
                audioSourceAmbiance.pitch = pitch;
                if (onComplete != null)
                {
                    onComplete();
                }
                return;
            }

            LeanTween.value(gameObject,
                audioSourceMusic.pitch,
                pitch,
                duration).setOnUpdate((p) =>
                {
                    audioSourceMusic.pitch = p;
                    audioSourceAmbiance.pitch = p;
                }).setOnComplete(() =>
                {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }

        /// <summary>
        /// Fades the game music volume to required level over a period of time.
        /// </summary>
        /// <param name="volume">The new music volume value [0..1]</param>
        /// <param name="duration">The length of time in seconds needed to complete the volume change.</param>
        /// <param name="onComplete">Delegate function to call when fade completes.</param>
        public virtual void SetAudioVolume(float volume, float duration, System.Action onComplete, int channel = 0)
        {
            AudioSource audioSourceMusic = audioSourcesMusic[channel];
            if (Mathf.Approximately(duration, 0f))
            {
                if (onComplete != null)
                {
                    onComplete();
                }

                audioSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("musicVol", volume);
                return;
            }

            LeanTween.value(gameObject,
                audioSourceMusic.volume,
                volume,
                duration).setOnUpdate((v) => {
                    audioSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("musicVol", v);
                }).setOnComplete(() => {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }
        public virtual void SetAmbianceVolume(float volume, float duration, System.Action onComplete, int channel = 0){
            if (Mathf.Approximately(duration, 0f))
            {
                if (onComplete != null)
                {
                    onComplete();
                }

                audioSourceAmbiance.outputAudioMixerGroup.audioMixer.SetFloat("ambientVol", volume);
                return;
            }

            LeanTween.value(gameObject,
                audioSourceAmbiance.volume,
                volume,
                duration).setOnUpdate((v) => {
                    audioSourceAmbiance.outputAudioMixerGroup.audioMixer.SetFloat("ambientVol", v);
                }).setOnComplete(() => {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }
        public virtual void SetEffectsVolume(float volume, float duration, System.Action onComplete, int channel = 0){
            if (Mathf.Approximately(duration, 0f))
            {
                if (onComplete != null)
                {
                    onComplete();
                }

                audioSourceSoundEffect.outputAudioMixerGroup.audioMixer.SetFloat("soundVol", volume);
                return;
            }

            LeanTween.value(gameObject,
                audioSourceSoundEffect.volume,
                volume,
                duration).setOnUpdate((v) => {
                    audioSourceSoundEffect.outputAudioMixerGroup.audioMixer.SetFloat("soundVol", v);
                }).setOnComplete(() => {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }
        /// <summary>
        /// Stops playing game music.
        /// </summary>
        public virtual void StopMusic(int channel = 0)
        {
            AudioSource audioSourceMusic = audioSourcesMusic[channel];

            audioSourceMusic.Stop();
            audioSourceMusic.clip = null;
        }

        /// <summary>
        /// Stops playing game ambiance.
        /// </summary>
        public virtual void StopAmbiance()
        {
            audioSourceAmbiance.Stop();
            audioSourceAmbiance.clip = null;
        }

        /// <summary>
        /// Sets music mixer group.
        /// </summary>
        public virtual void SetMusicMixerGroup(AudioMixerGroup mixerGroup){
            foreach(AudioSource audioSourceMusic in audioSourcesMusic) audioSourceMusic.outputAudioMixerGroup = mixerGroup;
        }
        /// <summary>
        /// Sets ambiance mixer group.
        /// </summary>
        public virtual void SetAmbianceMixerGroup(AudioMixerGroup mixerGroup){
            audioSourceAmbiance.outputAudioMixerGroup = mixerGroup;
        }
        /// <summary>
        /// Sets effects mixer group.
        /// </summary>
        public virtual void SetEffectMixerGroup(AudioMixerGroup mixerGroup){
            audioSourceSoundEffect.outputAudioMixerGroup = mixerGroup;
        }

        #endregion
    }
}
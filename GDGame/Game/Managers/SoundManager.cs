using System;
using System.Collections.Generic;
using System.Diagnostics;
using GDGame.Enums;
using GDGame.EventSystem;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Managers
{
    public class SoundManager
    {
        #region Private variables

        private AudioEmitter emitter;
        private AudioListener listener;

        private int currentMusicIndex;
        private List<SoundEffect> currentMusicQueue;

        private SoundEffectInstance musicInstance;

        private Dictionary<string, SoundEffect> musicTracks;
        private float musicVolume = 1f;
        private float sfxVolume = 1f;
        private Dictionary<SfxType, SoundEffect> soundEffects;
        private float volumeStep = 0.2f;

        #endregion

        #region Constructors

        public SoundManager()
        {
            emitter = new AudioEmitter();
            listener = new AudioListener();
            currentMusicQueue = new List<SoundEffect>();
            soundEffects = new Dictionary<SfxType, SoundEffect>();
            musicTracks = new Dictionary<string, SoundEffect>();
            EventManager.RegisterListener<SoundEventInfo>(HandleSoundEvent);
        }

        #endregion

        #region Methods

        public void AddMusic(string id, SoundEffect track)
        {
            if (!musicTracks.ContainsKey(id) && track != null)
                musicTracks.Add(id, track);
        }

        public void AddSoundEffect(SfxType sfxType, SoundEffect sfx)
        {
            if (!soundEffects.ContainsKey(sfxType) && sfx != null)
                soundEffects.Add(sfxType, sfx);
        }

        private void AddToVolume(SoundVolumeType volumeType, float value)
        {
            switch (volumeType)
            {
                case SoundVolumeType.Master:
                    SoundEffect.MasterVolume = Math.Clamp(SoundEffect.MasterVolume + value, 0, 1f);
                    break;
                case SoundVolumeType.Music:
                    musicVolume = Math.Clamp(musicVolume + value, 0, 1f);
                    break;
                case SoundVolumeType.Sfx:
                    sfxVolume = Math.Clamp(musicVolume + value, 0, 1f);
                    break;
            }

            musicInstance.Volume = musicVolume;
        }

        public void Dispose()
        {
            musicTracks.Clear();
            soundEffects.Clear();
        }

        private void PlayMusic(string id)
        {
            if (!musicTracks.ContainsKey(id))
            {
                Debug.WriteLine("No Music for the specified key found!");
                return;
            }

            SoundEffect track = musicTracks[id];
            if (track != null)
            {
                musicInstance.Stop();
                musicInstance = track.CreateInstance();
                musicInstance.Volume = musicVolume;
                musicInstance.Play();
            }
        }

        private void PlayMusic(SoundEffect musicTrack)
        {
            if (musicTrack != null)
            {
                musicInstance?.Stop();
                musicInstance = musicTrack.CreateInstance();
                musicInstance.Volume = musicVolume;
                musicInstance.Play();
            }
        }

        private void PlayNextMusic()
        {
            if (currentMusicQueue.Count == 0)
                return;

            if (++currentMusicIndex == currentMusicQueue.Count)
                currentMusicIndex = 0;

            SoundEffect nextSong = currentMusicQueue[currentMusicIndex];
            PlayMusic(nextSong);
        }

        private void PlaySoundEffect(SfxType sfxType, bool apply3D)
        {
            if (!soundEffects.ContainsKey(sfxType))
            {
                Debug.WriteLine("No Sound for the specified key found!");
                return;
            }

            SoundEffect sfx = soundEffects[sfxType];

            if (sfx != null)
            {
                SoundEffectInstance sei = sfx.CreateInstance();
                sei.Volume = sfxVolume;
                if(apply3D) sei.Apply3D(listener, emitter);
                sei.Play();
            }
        }

        private void SetMusicPlaybackState(SoundState state)
        {
            if (musicInstance == null)
                return;

            switch (state)
            {
                case SoundState.Playing:
                    musicInstance.Resume();
                    break;
                case SoundState.Paused:
                    musicInstance.Pause();
                    break;
                case SoundState.Stopped:
                    musicInstance.Stop();
                    break;
            }
        }

        public void StartMusicQueue(bool startOnRandomTrack = true)
        {
            currentMusicQueue.Clear();

            foreach (KeyValuePair<string, SoundEffect> keyValuePair in musicTracks)
                currentMusicQueue.Add(keyValuePair.Value);

            currentMusicIndex = startOnRandomTrack ? new Random().Next(0, currentMusicQueue.Count - 1) : 0;

            if (currentMusicQueue.Count > 0)
                PlayMusic(currentMusicQueue[currentMusicIndex]);
        }

        private void ToggleMusicPlaybackState()
        {
            if (musicInstance.State == SoundState.Playing)
                SetMusicPlaybackState(SoundState.Paused);
            else if (musicInstance.State == SoundState.Paused)
                SetMusicPlaybackState(SoundState.Playing);
        }

        #endregion

        #region Events

        private void HandleSoundEvent(SoundEventInfo info)
        {
            switch (info.soundEventType)
            {
                case SoundEventType.PlaySfx:
                    if (info.emitterTransform != null)
                    {
                        emitter.Position = info.emitterTransform.Translation;
                        PlaySoundEffect(info.sfxType, true);
                    }
                    else
                        PlaySoundEffect(info.sfxType, false);
                    break;
                case SoundEventType.PlayNextMusic:
                    PlayNextMusic();
                    break;
                case SoundEventType.IncreaseVolume:
                    AddToVolume(info.soundVolumeType, volumeStep);
                    break;
                case SoundEventType.DecreaseVolume:
                    AddToVolume(info.soundVolumeType, -volumeStep);
                    break;
                case SoundEventType.PauseMusic:
                    SetMusicPlaybackState(SoundState.Paused);
                    break;
                case SoundEventType.ResumeMusic:
                    SetMusicPlaybackState(SoundState.Playing);
                    break;
                case SoundEventType.ToggleMusicPlayback:
                    ToggleMusicPlaybackState();
                    break;
            }
        }

        #endregion
    }
}
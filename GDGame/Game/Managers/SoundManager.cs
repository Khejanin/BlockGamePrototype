using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GDGame.Enums;
using GDGame.EventSystem;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Managers
{
    /// <summary>
    /// Manager class that plays sounds by responding to events.
    /// </summary>
    public class SoundManager
    {
        #region Private variables

        private bool isPaused;

        private AudioEmitter emitter;
        private AudioListener listener;
        private Transform3D listenerTransform;

        private int currentMusicIndex;
        private List<SoundEffect> currentMusicQueue;
        private SoundEffectInstance currentInGameMusicInstance;
        private List<SoundEffectInstance> sfxInstances;

        private Dictionary<string, SoundEffect> inGameMusicTracks;
        private Dictionary<SfxType, SoundEffect> soundEffects;
        private float musicVolume = 1f;
        private float sfxVolume = 1f;
        private float volumeStep = 0.1f;

        #endregion

        #region Constructors

        public SoundManager()
        {
            emitter = new AudioEmitter();
            listener = new AudioListener();
            currentMusicQueue = new List<SoundEffect>();
            sfxInstances = new List<SoundEffectInstance>();

            soundEffects = new Dictionary<SfxType, SoundEffect>();
            inGameMusicTracks = new Dictionary<string, SoundEffect>();

            EventManager.RegisterListener<SoundEventInfo>(HandleSoundEvent);
            EventDispatcher.Subscribe(EventCategoryType.Menu, HandleMenuEvent);
        }

        #endregion

        #region Methods

        private void Pause()
        {
            isPaused = true;
        }

        private void Resume()
        {
            isPaused = false;
        }

        public void SetListenerPosition(Transform3D listenerTransform)
        {
            if(listenerTransform != null)
                this.listenerTransform = listenerTransform;
        }

        public void AddMusic(string id, SoundEffect track)
        {
            if (!inGameMusicTracks.ContainsKey(id) && track != null)
                inGameMusicTracks.Add(id, track);
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

            currentInGameMusicInstance.Volume = musicVolume;
            foreach (SoundEffectInstance instance in sfxInstances)
                instance.Volume = sfxVolume;
        }

        public void Dispose()
        {
            inGameMusicTracks.Clear();
            soundEffects.Clear();
        }

        private void PlayMusic(string id)
        {
            if (!inGameMusicTracks.ContainsKey(id))
            {
                Debug.WriteLine("No Music for the specified key found!");
                return;
            }

            SoundEffect track = inGameMusicTracks[id];
            if (track != null)
            {
                currentInGameMusicInstance.Stop();
                currentInGameMusicInstance = track.CreateInstance();
                currentInGameMusicInstance.Volume = musicVolume;
                currentInGameMusicInstance.Play();
            }
        }

        private void PlayMusic(SoundEffect musicTrack)
        {
            if (musicTrack == null) return;

            if (currentInGameMusicInstance != null)
                TimeManager.RemoveTimer(currentInGameMusicInstance.GetHashCode().ToString());

            currentInGameMusicInstance?.Stop();
            currentInGameMusicInstance = musicTrack.CreateInstance();
            currentInGameMusicInstance.Volume = musicVolume;
            currentInGameMusicInstance.Play();

            TimeManager.CallFunctionInSeconds(currentInGameMusicInstance.GetHashCode().ToString(), PlayNextMusic, (float)musicTrack.Duration.TotalSeconds);
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

        private void PlaySoundEffect(SfxType sfxType, Vector3? emitterPosition = null)
        {
            if (!soundEffects.ContainsKey(sfxType))
            {
                Debug.WriteLine("No Sound for the key" + sfxType.ToString("G") + " found!");
                return;
            }

            SoundEffect sfx = soundEffects[sfxType];

            if (sfx != null)
            {
                SoundEffectInstance sei = sfx.CreateInstance();
                sei.Volume = sfxVolume;
                if (emitterPosition != null)
                {
                    emitter.Position = (Vector3)emitterPosition;
                    listener.Position = listenerTransform.Translation;
                    sei.Apply3D(listener, emitter);
                }
                sei.Play();
                sfxInstances.Add(sei);
            }
        }

        private void SetMusicPlaybackState(SoundState state)
        {
            if (currentInGameMusicInstance == null)
                return;

            switch (state)
            {
                case SoundState.Playing:
                    currentInGameMusicInstance.Resume();
                    break;
                case SoundState.Paused:
                    currentInGameMusicInstance.Pause();
                    break;
                case SoundState.Stopped:
                    currentInGameMusicInstance.Stop();
                    break;
            }
        }

        private void SetSfxPlaybackState(SoundState state)
        {
            foreach (SoundEffectInstance instance in sfxInstances)
            {
                switch (state)
                {
                    case SoundState.Paused:
                        instance.Pause();
                        break;
                    case SoundState.Playing:
                        instance.Resume();
                        break;
                    case SoundState.Stopped:
                        instance.Stop();
                        break;
                }
            }
        }

        public void StartMusicQueue(bool startOnRandomTrack = true)
        {
            if (currentInGameMusicInstance != null)
                return;

            currentMusicQueue.Clear();

            foreach (KeyValuePair<string, SoundEffect> keyValuePair in inGameMusicTracks)
                currentMusicQueue.Add(keyValuePair.Value);

            currentMusicIndex = startOnRandomTrack ? new Random().Next(0, currentMusicQueue.Count - 1) : 0;

            if (currentMusicQueue.Count > 0)
                PlayMusic(currentMusicQueue[currentMusicIndex]);
        }

        private void ToggleMusicPlaybackState()
        {
            if (currentInGameMusicInstance.State == SoundState.Playing)
                SetMusicPlaybackState(SoundState.Paused);
            else if (currentInGameMusicInstance.State == SoundState.Paused)
                SetMusicPlaybackState(SoundState.Playing);
        }

        #endregion

        #region Events

        private void HandleMenuEvent(EventData data)
        {
            switch (data.EventActionType)
            {
                case EventActionType.OnPlay:
                    //StartMusicQueue();
                    break;
                case EventActionType.OnPause:
                    Pause();
                    break;
                case EventActionType.OnResume:
                    Resume();
                    break;
            }
        }

        private void HandleSoundEvent(SoundEventInfo info)
        {
            switch (info.soundEventType)
            {
                case SoundEventType.PlaySfx:
                    if (isPaused && info.category == SoundCategory.Gameplay) return;
                    PlaySoundEffect(info.sfxType, info.soundLocation);
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
                case SoundEventType.Mute:
                    AddToVolume(info.soundVolumeType, -1f);
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
                case SoundEventType.SetListener:
                    SetListenerPosition(info.listenerTransform);
                    break;
                case SoundEventType.PauseAll:
                    SetMusicPlaybackState(SoundState.Paused);
                    SetSfxPlaybackState(SoundState.Paused);
                    break;
                case SoundEventType.ResumeAll:
                    SetMusicPlaybackState(SoundState.Playing);
                    SetSfxPlaybackState(SoundState.Playing);
                    break;
            }
        }

        #endregion
    }
}
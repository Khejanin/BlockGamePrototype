using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    ///     Manager class that plays sounds by responding to events.
    /// </summary>
    public class SoundManager
    {
        #region Private variables

        private SoundEffectInstance currentInGameMusicInstance;

        private int currentMusicIndex;
        private List<SoundEffect> currentMusicQueue;

        private float currentMusicVolume = 1f, currentSfxVolume = 1f;

        private AudioEmitter emitter;

        private Dictionary<string, SoundEffect> inGameMusicTracks;

        private bool isPaused, sfxMuted, musicMuted, masterMuted;
        private AudioListener listener;
        private Transform3D listenerTransform;
        private float prevMusicVolume = 1f, prevSfxVolume = 1f, prevMasterVolume = 1f;
        private List<SoundEffectInstance> sfxInstances;
        private Dictionary<SfxType, SoundEffect> soundEffects;
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

        #region Public Method

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

        public void Dispose()
        {
            inGameMusicTracks.Clear();
            soundEffects.Clear();
        }

        public void SetListenerPosition(Transform3D listenerTransform)
        {
            if (listenerTransform != null)
                this.listenerTransform = listenerTransform;
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

        #endregion

        #region Private Method

        private void AddToVolume(SoundVolumeType volumeType, float value)
        {
            switch (volumeType)
            {
                case SoundVolumeType.Master:
                    SoundEffect.MasterVolume = Math.Clamp(SoundEffect.MasterVolume + value, 0, 1f);
                    break;
                case SoundVolumeType.Music:
                    currentMusicVolume = Math.Clamp(currentMusicVolume + value, 0, 1f);
                    break;
                case SoundVolumeType.Sfx:
                    currentSfxVolume = Math.Clamp(currentMusicVolume + value, 0, 1f);
                    break;
            }

            currentInGameMusicInstance.Volume = currentMusicVolume;
            foreach (SoundEffectInstance instance in sfxInstances)
                instance.Volume = currentSfxVolume;
        }

        private void Pause()
        {
            isPaused = true;
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
                currentInGameMusicInstance.Volume = currentMusicVolume;
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
            currentInGameMusicInstance.Volume = currentMusicVolume;
            currentInGameMusicInstance.Play();

            TimeManager.CallFunctionInSeconds(currentInGameMusicInstance.GetHashCode().ToString(), PlayNextMusic,
                (float) musicTrack.Duration.TotalSeconds);
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
                float volumeToApply = currentSfxVolume;
                if (emitterPosition != null)
                {
                    emitter.Position = (Vector3) emitterPosition;
                    listener.Position = listenerTransform.Translation;
                    sei.Apply3D(listener, emitter);
                }

                sei.Volume = volumeToApply;
                sei.Play();
                sfxInstances.Add(sei);
            }
        }

        private void Resume()
        {
            isPaused = false;
        }

        private void SetMusicPlaybackState(SoundState state)
        {
            if (currentInGameMusicInstance == null)
                return;

            switch (state)
            {
                case SoundState.Playing:
                    currentInGameMusicInstance.Resume();
                    TimeManager.ResumeTimer(currentInGameMusicInstance.GetHashCode().ToString());
                    break;
                case SoundState.Paused:
                    currentInGameMusicInstance.Pause();
                    TimeManager.PauseTimer(currentInGameMusicInstance.GetHashCode().ToString());
                    break;
                case SoundState.Stopped:
                    currentInGameMusicInstance.Stop();
                    TimeManager.RemoveTimer(currentInGameMusicInstance.GetHashCode().ToString());
                    break;
            }
        }

        private void SetSfxPlaybackState(SoundState state)
        {
            foreach (SoundEffectInstance instance in sfxInstances)
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

        private void ToggleMusicPlaybackState()
        {
            if (currentInGameMusicInstance.State == SoundState.Playing)
                SetMusicPlaybackState(SoundState.Paused);
            else if (currentInGameMusicInstance.State == SoundState.Paused)
                SetMusicPlaybackState(SoundState.Playing);
        }

        private void ToggleMuteVolume(SoundVolumeType volumeType)
        {
            switch (volumeType)
            {
                case SoundVolumeType.Master:
                    sfxMuted = musicMuted = masterMuted = !masterMuted;
                    if (!masterMuted)
                    {
                        prevMasterVolume = SoundEffect.MasterVolume;
                        SoundEffect.MasterVolume = 0;
                    }
                    else
                    {
                        SoundEffect.MasterVolume = prevMasterVolume;
                    }

                    break;
                case SoundVolumeType.Music:
                    musicMuted = !musicMuted;
                    if (!musicMuted)
                    {
                        prevSfxVolume = currentSfxVolume;
                        currentSfxVolume = 0;
                    }
                    else
                    {
                        currentMusicVolume = prevMusicVolume;
                    }

                    break;
                case SoundVolumeType.Sfx:
                    sfxMuted = !sfxMuted;
                    if (!sfxMuted)
                    {
                        prevSfxVolume = currentSfxVolume;
                        currentSfxVolume = 0;
                    }
                    else
                    {
                        currentSfxVolume = prevSfxVolume;
                    }

                    break;
            }

            currentInGameMusicInstance.Volume = currentMusicVolume;
            foreach (SoundEffectInstance instance in sfxInstances)
                instance.Volume = currentSfxVolume;
        }

        #endregion

        #region Events

        private void HandleMenuEvent(EventData data)
        {
            switch (data.EventActionType)
            {
                case EventActionType.OnPause:
                    Pause();
                    break;
                case EventActionType.OnResume:
                case EventActionType.OnPlay:
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
                case SoundEventType.ToggleMute:
                    ToggleMuteVolume(info.soundVolumeType);
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
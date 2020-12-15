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
    public class SoundManager : IDisposable
    {
        #region Private variables

        private SoundEffectInstance currentInGameMusicInstance;
        private int currentMusicIndex;
        private List<SoundEffect> currentMusicQueue;

        //Volume
        private float currentMusicVolume = 1f, currentSfxVolume = 1f;
        private AudioEmitter emitter;

        //Music
        private Dictionary<string, SoundEffect> inGameMusicTracks;
        private bool isPaused, sfxMuted, musicMuted, masterMuted;

        //3D Audio
        private AudioListener listener;
        private Transform3D listenerTransform;
        private float prevMusicVolume = 1f, prevSfxVolume = 1f, prevMasterVolume = 1f;

        //SFX
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

        /// <summary>
        ///     Adds a music track to the music queue with the given id
        /// </summary>
        /// <param name="id">The key to use in the dictionary</param>
        /// <param name="track">The music track to add</param>
        public void AddMusic(string id, SoundEffect track)
        {
            if (!inGameMusicTracks.ContainsKey(id) && track != null)
                inGameMusicTracks.Add(id, track);
        }

        /// <summary>
        ///     Add the sound effect to the SFX Dictionary
        /// </summary>
        /// <param name="sfxType">The key to use in the dictionary</param>
        /// <param name="sfx">The Sound Effect to add</param>
        public void AddSoundEffect(SfxType sfxType, SoundEffect sfx)
        {
            if (!soundEffects.ContainsKey(sfxType) && sfx != null)
                soundEffects.Add(sfxType, sfx);
        }

        /// <summary>
        ///     Clear all lists and dictionaries and unsubscribe from events
        /// </summary>
        public void Dispose()
        {
            inGameMusicTracks.Clear();
            soundEffects.Clear();
            currentMusicQueue.Clear();
            sfxInstances.Clear();
            EventManager.UnregisterListener<SoundEventInfo>(HandleSoundEvent);
            EventDispatcher.Unsubscribe(EventCategoryType.Menu, HandleMenuEvent);
        }

        /// <summary>
        ///     Starts the music queue (Add a track to queue by using the AddMusic method)
        /// </summary>
        /// <param name="startOnRandomTrack">Defines whether to start with the first track or a random track in the queue</param>
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

        /// <summary>
        ///     Adds the specified value to the volume of the specified type. Add negative value to decrease the volume.
        /// </summary>
        /// <param name="volumeType">The type of volume to add the value to (Master, Music or SFX)</param>
        /// <param name="value">The value to add to the volume</param>
        private void AddToVolume(SoundVolumeType volumeType, float value)
        {
            //Set the volume
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

            //Apply the volume to the currently playing sounds
            currentInGameMusicInstance.Volume = currentMusicVolume;
            foreach (SoundEffectInstance instance in sfxInstances)
                instance.Volume = currentSfxVolume;
        }

        private void Pause()
        {
            isPaused = true;
        }

        /// <summary>
        ///     Stops the currently playing music track and plays the track with the given key.
        /// </summary>
        /// <param name="id">The key of the music track to play</param>
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

        /// <summary>
        ///     Stops the currently playing music track and plays the specified one.
        /// </summary>
        /// <param name="musicTrack">The music track to play</param>
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

        /// <summary>
        ///     Plays the next song in the queue.
        /// </summary>
        private void PlayNextMusic()
        {
            if (currentMusicQueue.Count == 0)
                return;

            if (++currentMusicIndex == currentMusicQueue.Count)
                currentMusicIndex = 0;

            SoundEffect nextSong = currentMusicQueue[currentMusicIndex];
            PlayMusic(nextSong);
        }

        /// <summary>
        ///     Plays the sound effect for the specified SFX type. Can be played in 2D or 3D.
        /// </summary>
        /// <param name="sfxType">The type of Sound Effect to play</param>
        /// <param name="emitterPosition">The location to emit the sound from</param>
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

        /// <summary>
        ///     Sets the transform used by the listener, when playing 3D sounds
        /// </summary>
        /// <param name="listenerTransform">The transform to set the listener to</param>
        private void SetListenerTransform(Transform3D listenerTransform)
        {
            if (listenerTransform != null)
                this.listenerTransform = listenerTransform;
        }

        /// <summary>
        ///     Set the playback state of the currently playing music track (Pause, Resume or Stop)
        /// </summary>
        /// <param name="state">The state to set</param>
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

        /// <summary>
        ///     Set the playback state of all currently playing sfx instances (Pause, Resume or Stop)
        /// </summary>
        /// <param name="state">The state to set</param>
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
            //Pauses or Resumes the music based on the previous state
            if (currentInGameMusicInstance.State == SoundState.Playing)
                SetMusicPlaybackState(SoundState.Paused);
            else if (currentInGameMusicInstance.State == SoundState.Paused)
                SetMusicPlaybackState(SoundState.Playing);
        }

        private void ToggleMuteVolume(SoundVolumeType volumeType)
        {
            //Mutes or Unmutes volume based on the previous state. 

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
            //Handle Menu Events here. Pauses and Resumes the sound manager
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
            //Handle sound events here. Action performed based on SoundEventType
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
                    SetListenerTransform(info.listenerTransform);
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
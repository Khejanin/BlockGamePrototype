using System;
using System.Collections.Generic;
using System.Linq;
using GDGame.Actors;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Managers
{
    public class SoundManager : GameComponent
    {
        #region Private variables

        private int activeSongIndex;
        private Sounds currentSong;
        private List<Sounds> list;
        private float masterSound = 0.2f;
        private SoundEffectInstance mySoundInstance;

        #endregion

        #region Constructors

        public SoundManager(Game game) : base(game)
        {
            list = new List<Sounds>();
        }

        #endregion

        #region Properties, Indexers

        public Sounds ActiveSong => list[activeSongIndex];

        public int ActiveSongIndex
        {
            get => activeSongIndex;
            set
            {
                value %= list.Count;
                activeSongIndex = value;
            }
        }

        /// <summary>
        ///     Indexer for the camera manager
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// See
        /// <see href="www." />
        /// for more info
        public Sounds this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        #endregion

        #region Methods

        public void Add(Sounds newSong)
        {
            if (FindSound(newSong.ID) == null)
                list.Add(newSong);
        }

        public void ChangeMusicState()
        {
            switch (mySoundInstance.State)
            {
                case SoundState.Playing:
                    PauseSong();
                    break;
                case SoundState.Paused:
                    ResumeSong();
                    break;
            }
        }

        public Sounds FindSound(string id)
        {
            return list.FirstOrDefault(s => s.ID == id);
        }

        public void NextSong()
        {
            int next = activeSongIndex + 1;
            if (next >= list.Count)
                next = 0;

            while (list[next].ActorType != ActorType.MusicTrack) next = (next + 1) % list.Count;

            SwitchSong(next);
            mySoundInstance.Volume = masterSound;
            mySoundInstance.IsLooped = true;
            mySoundInstance.Play();
        }

        private void PauseSong()
        {
            if (mySoundInstance != null && mySoundInstance.State == SoundState.Playing)
                mySoundInstance.Pause();
        }

        private void PlaySfx(Sounds s)
        {
            if (s.ActorType == ActorType.SoundEffect)
            {
                SoundEffectInstance instance = s.GetSfx().CreateInstance();
                instance.IsLooped = false;
                instance.Play();
            }
        }

        public void PlaySoundEffect(string id)
        {
            Sounds s = FindSound(id);
            currentSong = s;
            if (s != null)
                switch (s.ActorType)
                {
                    case ActorType.MusicTrack:
                    {
                        if (currentSong != null)
                            if (s.ID != currentSong.ID)
                            {
                                mySoundInstance = s.GetSfx().CreateInstance();
                                mySoundInstance.IsLooped = true;
                                mySoundInstance.Volume = masterSound;
                                mySoundInstance.Play();
                            }

                        break;
                    }
                    case ActorType.SoundEffect:
                        PlaySfx(s);
                        break;
                }
        }

        public bool RemoveIf(Predicate<Sounds> predicate)
        {
            int position = list.FindIndex(predicate);

            if (position != -1)
            {
                list.RemoveAt(position);
                return true;
            }

            return false;
        }

        private void ResumeSong()
        {
            if (mySoundInstance != null && mySoundInstance.State == SoundState.Paused)
                mySoundInstance.Resume();
        }

        public void StopSong()
        {
            if (mySoundInstance != null && mySoundInstance.State == SoundState.Playing)
                mySoundInstance.Stop();
        }

        private void SwitchSong(int next)
        {
            activeSongIndex = next;

            StopSong();

            currentSong = list[next];
            mySoundInstance = currentSong.GetSfx().CreateInstance();
        }

        public void VolumeDown()
        {
            PauseSong();

            masterSound -= 0.2f;

            if (masterSound <= 0)
                masterSound = 0;

            mySoundInstance.Volume = masterSound;
            ResumeSong();
        }


        public void VolumeUp()
        {
            PauseSong();

            masterSound += 0.2f;

            if (masterSound >= 1.0)
                masterSound = 1;

            mySoundInstance.Volume = masterSound;
            ResumeSong();
        }

        #endregion
    }
}
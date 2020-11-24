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
        private List<Sounds> list;
        private int activeSongIndex;
        private Sounds currentSong;
        private SoundEffectInstance mySoundInstance;
        private float masterSound = 0.2f;

        /// <summary>
        /// Indexer for the camera manager
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// See <see href="www."/> for more info
        public Sounds this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

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

        public SoundManager(Microsoft.Xna.Framework.Game game) : base(game)
        {
            list = new List<Sounds>();
        }

        public void Add(Sounds newSong)
        {
            if(FindSound(newSong.ID) == null)
                list.Add(newSong);
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

        public Sounds FindSound(string id)
        {
            return list.FirstOrDefault(s => s.ID == id);
        }

        public void PlaySoundEffect(string id)
        {
            Sounds s = FindSound(id);
            currentSong = s;
            if (s != null)
            {
                switch (s.ActorType)
                {
                    case ActorType.MusicTrack:
                    {
                        if (currentSong != null)
                        {
                            if (s.ID != currentSong.ID)
                            {
                                mySoundInstance = s.GetSfx().CreateInstance();
                                mySoundInstance.IsLooped = true;
                                mySoundInstance.Volume = masterSound;
                                mySoundInstance.Play();
                            }
                        }

                        break;
                    }
                    case ActorType.SoundEffect:
                        PlaySfx(s);
                        break;
                }
            }
        }

        private void PlaySfx(Sounds s)
        {
            if (s.ActorType == ActorType.SoundEffect)
            {
                var instance = s.GetSfx().CreateInstance();
                instance.IsLooped = false;
                instance.Play();
            }
        }

        public void NextSong()
        {
            int next = activeSongIndex + 1;
            if (next >= list.Count)
                next = 0;

            while(list[next].ActorType != ActorType.MusicTrack)
            {
                next = (next + 1) % list.Count;
            }

            SwitchSong(next);
            mySoundInstance.Volume = masterSound;
            mySoundInstance.IsLooped = true;
            mySoundInstance.Play();
        }

        private void SwitchSong(int next)
        {
            activeSongIndex = next;

            StopSong();

            currentSong = list[next];
            mySoundInstance = currentSong.GetSfx().CreateInstance();
        }

        public void StopSong()
        {
            if (mySoundInstance != null && mySoundInstance.State == SoundState.Playing)
                mySoundInstance.Stop();
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

        private void PauseSong()
        {
            if (mySoundInstance != null && mySoundInstance.State == SoundState.Playing)
                mySoundInstance.Pause();
        }

        private void ResumeSong()
        {
            if (mySoundInstance != null && mySoundInstance.State == SoundState.Paused)
                mySoundInstance.Resume();
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

        public void VolumeDown()
        {
            PauseSong();

            masterSound -= 0.2f;

            if (masterSound <= 0)
                masterSound = 0;

            mySoundInstance.Volume = masterSound;
            ResumeSong();
        }
    }
}
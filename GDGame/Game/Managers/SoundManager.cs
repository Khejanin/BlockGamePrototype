using System;
using System.Collections.Generic;
using GDGame.Actors;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Managers
{
    public class SoundManager : GameComponent
    {
        private List<Sounds> list;
        private int activeSongIndex = 0;
        private Sounds currentSong;
        private SoundEffectInstance mySoundInstance;
        private float masterSound = 0.5f;

        /// <summary>
        /// Indexer for the camera manager
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// See <see href="www."/> for more info
        public Sounds this[int index]
        {
            get
            {
                return this.list[index];
            }
            set
            {
                this.list[index] = value;
            }
        }

        public Sounds ActiveSong
        {
            get
            {
                return this.list[this.activeSongIndex];
            }
        }
        public int ActiveSongIndex
        {
            get
            {
                return this.activeSongIndex;
            }
            set
            {
                value = value % this.list.Count;
                this.activeSongIndex = value; //bug!!! [0, list.size()-1]
            }
        }

        public SoundManager(Microsoft.Xna.Framework.Game game) : base(game)
        {
            this.list = new List<Sounds>();
        }

        public void Add(Sounds newSong)
        {
            if(FindSound(newSong.ID) == null)
                this.list.Add(newSong);
        }

        public bool RemoveIf(Predicate<Sounds> predicate)
        {
            int position = this.list.FindIndex(predicate);

            if (position != -1)
            {
                this.list.RemoveAt(position);
                return true;
            }
            return false;
        }

        public Sounds FindSound(string id)
        {
            foreach (Sounds s in this.list)
            {
                if (s.ID == id)
                    return s;
            }
            return null;
        }

        public void playSoundEffect(string id)
        {
            Sounds s = FindSound(id);
            currentSong = s;
            if (s != null)
            {
                if (s.ActorType == ActorType.MusicTrack)
                {
                    if (this.currentSong != null)
                    {
                        if (s.ID != currentSong.ID)
                        {
                            this.mySoundInstance = s.GetSfx().CreateInstance();
                            this.mySoundInstance.IsLooped = true;
                            this.mySoundInstance.Volume = this.masterSound;
                            this.mySoundInstance.Play();
                        }
                    }
                }
                else if (s.ActorType == ActorType.SoundEffect)
                    playSFX(s);
            }
        }

        public void playSFX(Sounds s)
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
            if (next >= this.list.Count)
                next = 0;

            while(list[next].ActorType != ActorType.MusicTrack)
            {
                next = (next + 1) % list.Count;
            }

            SwitchSong(next);
            this.mySoundInstance.Volume = (float)0.5;
            this.mySoundInstance.Play();
        }

        private void SwitchSong(int next)
        {
            this.activeSongIndex = next;

            StopSong();

            this.currentSong = list[next];
            this.mySoundInstance = currentSong.GetSfx().CreateInstance();
        }

        public void StopSong()
        {
            if (this.mySoundInstance != null && this.mySoundInstance.State == SoundState.Playing)
                this.mySoundInstance.Stop();
        }

        public void changeMusicState()
        {
            if (this.mySoundInstance.State == SoundState.Playing)
                pauseSong();
            else if (this.mySoundInstance.State == SoundState.Paused)
                resumeSong();
        }

        public void pauseSong()
        {
            if (this.mySoundInstance != null && this.mySoundInstance.State == SoundState.Playing)
                this.mySoundInstance.Pause();
        }

        public void resumeSong()
        {
            if (this.mySoundInstance != null && this.mySoundInstance.State == SoundState.Paused)
                this.mySoundInstance.Resume();
        }


        public void volumeUp()
        {
            pauseSong();

            this.masterSound = this.masterSound + 0.2f;

            if (this.masterSound >= 1.0)
                this.masterSound = 1;
            
            this.mySoundInstance.Volume = this.masterSound;
            resumeSong();
        }

        public void volumeDown()
        {
            pauseSong();

            this.masterSound = this.masterSound - 0.2f;

            if (this.masterSound <= 0)
                this.masterSound = 0;

            this.mySoundInstance.Volume = this.masterSound;
            resumeSong();
        }

        public override void Update(GameTime gameTime)
        {
            //if(this.mySoundInstance.State == SoundState.Stopped)
            //{
            //    this.mySoundInstance.Play();
            //}

            base.Update(gameTime);
        }
    }
}
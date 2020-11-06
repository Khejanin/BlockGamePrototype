using System;
using System.Collections.Generic;
using GDGame.Game.Actors.Audio;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Game.Managers
{
    public class SoundManager : GameComponent
    {
        private List<Sounds> list;
        private int activeSongIndex = 0;
        private Sounds currentSong;
        private SoundEffectInstance mySoundInstance;

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
                {
                    return s;
                }
            }
            return null;
        }

        public void PlaySoundEffect(string id)
        {
            Sounds s = FindSound(id);
            currentSong = s;
            if (s != null)
            {
                var instance = s.GetSfx().CreateInstance();
                if (s.ActorType == ActorType.MusicTrack)
                {
                    if(this.currentSong != null)
                    {
                        if (s.ID != currentSong.ID)
                        {
                            instance = s.GetSfx().CreateInstance();
                        }
                        else
                        {
                            //mySoundInstance.Stop();
                        }
                        instance.IsLooped = true;
                        instance.Play();
                        mySoundInstance = instance;
                    }
                }
                else if (s.ActorType == ActorType.SoundEffect)
                {
                    PlaySfx(s);
                }
            }
        }

        public void PlaySfx(Sounds s)
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
            {
                next = 0;
            }
            while(list[next].ActorType != ActorType.MusicTrack)
            {
                next = (next + 1) % list.Count;
            }
            this.activeSongIndex = next;
            this.currentSong = list[next];
            this.mySoundInstance = currentSong.GetSfx().CreateInstance();
            PlaySoundEffect(list[next].ID);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}

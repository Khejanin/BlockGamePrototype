using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using GDLibrary.Enums;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System.Security.Permissions;

namespace GDLibrary
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

        public Sounds activeSong
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

        public SoundManager(Game game) : base(game)
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

        public Sounds findSound(string id)
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

        public void playSoundEffect(string id)
        {
            Sounds s = findSound(id);
            currentSong = s;
            if (s != null)
            {
                var instance = s.getSFX().CreateInstance();
                if (s.ActorType == ActorType.MusicTrack)
                {
                    if(this.currentSong != null)
                    {
                        if (s.ID != currentSong.ID)
                        {
                            instance = s.getSFX().CreateInstance();
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
                    playSFX(s);
                }
            }
        }

        public void playSFX(Sounds s)
        {
            if (s.ActorType == ActorType.SoundEffect)
            {
                var instance = s.getSFX().CreateInstance();
                instance.IsLooped = false;
                instance.Play();
            }
        }

        public void nextSong()
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
            this.mySoundInstance = currentSong.getSFX().CreateInstance();
            playSoundEffect(list[next].ID);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}

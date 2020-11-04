using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using GDLibrary.Enums;
using Microsoft.Xna.Framework.Media;

namespace GDLibrary
{
    public class SoundManager : GameComponent
    {
        private List<Sounds> list;
        private List<Sounds> music;
        private int activeSongIndex = 0;

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
            this.music = new List<Sounds>();
        }

        public void Add(Sounds newSong)
        {
            this.list.Add(newSong);
            if(newSong.ActorType == ActorType.MusicTrack)
            {
                this.music.Add(newSong);
            }

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
            if (s != null)
            {
                var instance = s.getSFX().CreateInstance();
                if (s.ActorType == ActorType.MusicTrack)
                {
                    instance.IsLooped = true;
                }
                else if (s.ActorType == ActorType.SoundEffect)
                {
                    instance.IsLooped = false;
                }
                instance.Play();
            }
        }

        public void nextSong()
        {
            int next = activeSongIndex + 1;
            if (next >= this.music.Count)
            {
                next = 0;
            }
            playSoundEffect(music[next].ID);
            this.activeSongIndex = next;
        }

        public override void Update(GameTime gameTime)
        {
            //foreach (Song s in this.list)
            //{
            //    if ((s.StatusType & StatusType.Update) == StatusType.Update)
            //        s.Update(gameTime);
            //}
            base.Update(gameTime);
        }
    }
}

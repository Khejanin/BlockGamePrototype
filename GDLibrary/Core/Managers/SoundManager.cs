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

        //If we are cycling though a song we need to: Set old song to not playing. New one to playing and play new song.
        public void CycleActiveSong()
        {
            list[activeSongIndex].setPlaying(false);
            this.activeSongIndex++;
            list[activeSongIndex].setPlaying(true);
            this.activeSongIndex %= this.list.Count;
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
                if (s.ActorType == ActorType.MusicTrack)
                {
                    MediaPlayer.Play(s.getSong());
                }
                else if (s.ActorType == ActorType.SoundEffect)
                {
                    var instance = s.getSFX().CreateInstance();
                    instance.IsLooped = false;
                    instance.Play();
                }
            }
        }

        public void nextSong()
        {
            int next = activeSongIndex + 1;
            if (next >= this.list.Count)
            {
                next = 0;
            }
            playSoundEffect(list[next].ID);
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

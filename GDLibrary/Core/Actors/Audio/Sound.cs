using GDLibrary.Actors;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GDLibrary
{
    public class Sounds : Actor
    {
        #region Fields
        private Song song;
        private SoundEffect soundFx;
        private bool isPlaying;
        #endregion

        #region Properties
        public Song getSong() { return this.song; }
        public void setSong(Song song) { this.song = song; }
        public SoundEffect getSFX() { return this.soundFx; }
        public void setSFX(SoundEffect sfx) { this.soundFx = sfx; }
        public bool isSongPlaying() { return this.isPlaying; }
        public void setPlaying(bool playing) { this.isPlaying = playing; }

        public Sounds(Song song, SoundEffect sfx, string id, ActorType actorType, StatusType statusType) : base(id, actorType, statusType)
        {
            this.song = song;
            this.soundFx = sfx;
            this.isPlaying = false;
        }

        public override void Update(GameTime gameTime)
        {
            //check for keyboard input?
            //if input, then modify transform
            //  this.controller.Update(gameTime, this);

            base.Update(gameTime);
        }
    }
}
#endregion
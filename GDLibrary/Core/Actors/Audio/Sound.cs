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
        private SoundEffect soundFx;
        private bool isPlaying;
        #endregion

        #region Properties
        public SoundEffect getSFX() { return this.soundFx; }
        public void setSFX(SoundEffect sfx) { this.soundFx = sfx; }
        public bool isSongPlaying() { return this.isPlaying; }
        public void setPlaying(bool playing) { this.isPlaying = playing; }

        public Sounds(SoundEffect sfx, string id, ActorType actorType, StatusType statusType) : base(id, actorType, statusType)
        {
            this.soundFx = sfx;
            this.isPlaying = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
#endregion
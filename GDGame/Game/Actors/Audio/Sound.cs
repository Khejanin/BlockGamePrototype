using GDLibrary.Actors;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Actors
{
    public class Sounds : Actor
    {
        #region Private variables

        private bool isPlaying;

        private SoundEffect soundFx;

        #endregion

        #region Constructors

        public Sounds(SoundEffect sfx, string id, ActorType actorType, StatusType statusType) : base(id, actorType,
            statusType)
        {
            soundFx = sfx;
            isPlaying = false;
        }

        #endregion

        #region Override Methode

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion

        #region Methods

        public SoundEffect GetSfx()
        {
            return soundFx;
        }

        public bool IsSongPlaying()
        {
            return isPlaying;
        }

        public void SetPlaying(bool playing)
        {
            isPlaying = playing;
        }

        public void SetSfx(SoundEffect sfx)
        {
            soundFx = sfx;
        }

        #endregion
    }
}
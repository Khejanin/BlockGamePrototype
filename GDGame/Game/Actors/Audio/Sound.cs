using GDLibrary.Actors;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Actors
{
    public class Sounds : Actor
    {
        #region Fields

        private SoundEffect soundFx;
        private bool isPlaying;

        #endregion

        #region Properties

        public SoundEffect GetSfx()
        {
            return this.soundFx;
        }

        public void SetSfx(SoundEffect sfx)
        {
            this.soundFx = sfx;
        }

        public bool IsSongPlaying()
        {
            return this.isPlaying;
        }

        public void SetPlaying(bool playing)
        {
            this.isPlaying = playing;
        }

        public Sounds(SoundEffect sfx, string id, ActorType actorType, StatusType statusType) : base(id, actorType,
            statusType)
        {
            this.soundFx = sfx;
            this.isPlaying = false;
        }
    }
}

#endregion
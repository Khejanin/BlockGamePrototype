using System.Collections.Generic;
using GDGame.Actors;
using GDGame.Component;
using GDGame.Enums;
using GDGame.EventSystem;
using GDGame.Game.Parameters.Effect;
using GDGame.Utilities;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Actors
{
    /// <summary>
    ///     Custom implementation of the PathMoveTile to work for the Coffee.
    /// </summary>
    public class Coffee : PathMoveTile
    {
        #region Private variables

        private readonly PlayerTile player;

        private bool coffeeDanger;

        private List<CoffeeInfo> coffeeInfo;

        private CoffeeMovementComponent coffeeMovementComponent;

        #endregion

        #region Constructors

        public Coffee(string id, ActorType actorType, StatusType statusType, Transform3D transform,
            OurEffectParameters effectParameters, Model model, List<CoffeeInfo> coffeeInfo, PlayerTile player) : base(
            id, actorType,
            statusType, transform, effectParameters, model, false, ETileType.None)
        {
            this.player = player;
            CoffeeInfo = coffeeInfo;
            InitializeCollision(-Vector3.Up * coffeeInfo[0].y);
            InitializeTile();
            IsMoving = false;
        }

        #endregion

        #region Properties, Indexers

        private int CheckPointIndex { get; set; }

        public List<CoffeeInfo> CoffeeInfo
        {
            get => coffeeInfo;
            private set
            {
                Path.Clear();

                foreach (CoffeeInfo info in value) Path.Add(new Vector3(0, info.y, 0));

                coffeeInfo = value;
            }
        }

        public float TimeLeft { get; private set; } = -1;

        #endregion

        #region Initialization

        public override void InitializeCollision(Vector3 position, float factor = 1)
        {
            SetTranslation(position);
            AddPrimitive(new Box(Transform3D.Translation - Vector3.Up, Matrix.Identity, new Vector3(100, 0.01f, 100)),
                new MaterialProperties(1, 1, 1));
            Enable(true, 1);
        }


        public override void InitializeTile()
        {
            EventManager.RegisterListener<PlayerEventInfo>(CheckPointReached);
        }

        #endregion

        #region Override Method

        public override void SetTranslation(Vector3 translation)
        {
            base.SetTranslation(translation - Vector3.Up);
            Transform3D.Translation = translation;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            coffeeMovementComponent ??=
                ControllerList.Find(controller => controller.GetControllerType() == ControllerType.Movement) as
                    CoffeeMovementComponent;
            if (IsMoving)
                if (coffeeMovementComponent != null)
                {
                    TimeLeft = coffeeMovementComponent.GetTotalTimeLeft(player.Transform3D) / 1000f;

                    if (TimeLeft < 10f && !coffeeDanger)
                    {
                        EventManager.FireEvent(new CoffeeEventInfo {coffeeEventType = CoffeeEventType.CoffeeDanger});
                        coffeeDanger = true;
                    }

                    if (TimeLeft > 10f && coffeeDanger)
                    {
                        EventManager.FireEvent(new CoffeeEventInfo
                            {coffeeEventType = CoffeeEventType.CoffeeDangerStop});
                        coffeeDanger = false;
                    }
                }
        }

        #endregion

        #region Private Method

        private void CheckPointReached(PlayerEventInfo info)
        {
            if (info.type == PlayerEventType.SetCheckpoint)
            {
                if (CheckPointIndex == 0)
                {
                    EventManager.FireEvent(new CoffeeEventInfo {coffeeEventType = CoffeeEventType.CoffeeStartMoving});
                    EventManager.FireEvent(new SoundEventInfo
                    {
                        soundEventType = SoundEventType.PlaySfx, sfxType = SfxType.CoffeeStart
                    });
                    coffeeMovementComponent.Activate();
                    IsMoving = true;
                    TimeLeft = coffeeMovementComponent.GetTotalTimeLeft(player.Transform3D) / 1000f;
                    ++CheckPointIndex;
                }
                else
                {
                    float target = Transform3D.Translation.Y - coffeeInfo[CheckPointIndex++].setBackY;
                    coffeeMovementComponent.StartLowering(target, target / 500);
                }
            }
        }

        #endregion
    }
}
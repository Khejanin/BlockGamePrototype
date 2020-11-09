using GDGame.Enums;

namespace GDGame.EventSystem
{
    public class GameStateMessageEventInfo : EventInfo
    {
        public GameState gameState;

        public GameStateMessageEventInfo(GameState gameState)
        {
            this.gameState = gameState;
        }
    }
}
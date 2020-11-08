using GDGame.Game.EventSystem.Base;

namespace GDGame.Game.EventSystem
{
    public enum GameState
    {
        WON,
        LOST
    }
    
    public class GameStateMessageEventInfo : EventInfo
    {
        public GameState gameState;

        public GameStateMessageEventInfo(GameState gameState)
        {
            this.gameState = gameState;
        }
    }
}
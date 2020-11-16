namespace GDGame.Enums
{
    public enum ETileType
    {
        None,
        Static,
        Attachable,
        Trigger,
        PlayerStart,
        Win,
        Enemy,
        Button
    }
    
    public enum ColliderShape
    {
        Cube,
        Sphere
    }

    public enum ColliderType
    {
        CheckOnly,
        Blocking
    }
    
    public enum GameState
    {
        Won,
        Lost
    }

    public enum PlayerEventType
    {
        Move
    }
}
namespace GDGame.Enums
{
    public enum EDirection : sbyte
    {
        None,
        Right,
        Left,
        Up,
        Down,
        Forward,
        Back
    }
    
    public enum ETileType
    {
        None,
        Static,
        Attachable,
        Trigger,
        PlayerStart,
        Win
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
}
namespace GDGame.Game.Enums
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
}
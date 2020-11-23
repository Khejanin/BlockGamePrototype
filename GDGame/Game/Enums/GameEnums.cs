namespace GDGame.Enums
{
    public enum ETileType : sbyte
    {
        None,
        Static,
        Attachable,
        Trigger,
        PlayerStart,
        Win,
        Enemy,
        Button,
        Star,
        MovingPlatform,
        Spike,
        Checkpoint
    }
    
    public enum ColliderShape : sbyte
    {
        Cube,
        Sphere
    }

    public enum ColliderType : sbyte
    {
        CheckOnly,
        Blocking
    }
    
    public enum GameState : sbyte
    {
        Won,
        Lost
    }

    public enum PlayerEventType : sbyte
    {
        Move,
        Die,
        AttachedTileDie,
        SetCheckpoint
    }

    public enum TileEventType : sbyte
    {
        Reset
    }

    public enum ActivatorEventType : sbyte
    {
        Activate,
        Deactivate
    }
    
    public enum SceneActionType : sbyte
    {
        OnSceneChange,
        OnSceneLoaded
    }

    public enum Direction : sbyte
    {
        None,
        Right,
        Left,
        Up,
        Down,
        Forward,
        Backward
    }
}
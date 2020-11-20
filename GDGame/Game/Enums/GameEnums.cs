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
        MovingPlatform
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
        Die
    }
    
    public enum SceneActionType : sbyte
    {
        OnSceneChange,
        OnSceneLoaded
    }
}
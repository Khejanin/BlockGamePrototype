namespace GDGame.Enums
{
    public enum ETileType : sbyte
    {
        None,
        Static,
        Attachable,
        Trigger,
        Player,
        Win,
        Enemy,
        Button,
        Star,
        MovingPlatform,
        Spike,
        Checkpoint,
        FallingPlatform
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
        MovableTileDie,
        SetCheckpoint,
        PickupMug,
        OnMove,
        OnEnemyMove
    }

    public enum TileEventType : sbyte
    {
        Reset,
        PlayerKill,
        AttachableKill
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

    public enum MovementType
    {
        OnMove,
        OnPlayerMoved
    }

    public enum SfxType : sbyte
    {
        PlayerMove,
        PlayerAttach,
        EnemyMove
    }

    public enum SoundEventType : sbyte
    {
        PlaySfx,
        PlayMusic,
        PauseMusic,
        ResumeMusic,
        ToggleMusicPlayback,
        PlayNextMusic,
        IncreaseVolume,
        DecreaseVolume,
        MuteVolume
    }

    public enum SoundVolumeType : sbyte
    {
        Master,
        Sfx,
        Music
    }
}
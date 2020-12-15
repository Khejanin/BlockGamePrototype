/*
 * Collection of Enums
 */

namespace GDGame.Enums
{
    /// <summary>
    ///     Enums defining a Tile, also used in Level JSON
    /// </summary>
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
        FallingPlatform,
        Door
    }

    public enum ColliderShape : sbyte
    {
        Cube,
        Sphere
    }

    /// <summary>
    ///     Deprecated, now defined in OurCollidableObject.cs
    /// </summary>
    public enum ColliderType : sbyte
    {
        CheckOnly,
        Blocking
    }

    public enum GameState : sbyte
    {
        Won,
        Lost,
        Start,
        Resume
    }

    public enum PlayerEventType : sbyte
    {
        Move,
        Die,
        MovableTileDie,
        SetCheckpoint,
        SetCheckpointList,
        PickupMug,
        OnMove,
        OnEnemyMove,
        CheckSouroundings
    }

    public enum TileEventType : sbyte
    {
        Reset,
        PlayerKill,
        Consumed
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
        EnemyMove,
        CollectibleCollected,
        TrapDeath,
        MenuButtonClick,
        TrapDeathWater,
        CoffeeStart,
        PlayerDetach
    }

    public enum SoundCategory : sbyte
    {
        Gameplay,
        UI
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
        ToggleMute,
        SetListener,
        PauseAll,
        ResumeAll
    }

    public enum SoundVolumeType : sbyte
    {
        Master,
        Sfx,
        Music
    }

    public enum OptionsType
    {
        Toggle
    }
}
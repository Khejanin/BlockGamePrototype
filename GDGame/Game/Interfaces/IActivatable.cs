namespace GDGame.Interfaces
{
    /// <summary>
    ///     IActivatable is for anything that is Activatable.
    /// </summary>
    public interface IActivatable
    {
        #region Public Method

        void Activate();
        void Deactivate();
        void ToggleActivation();

        #endregion
    }
}
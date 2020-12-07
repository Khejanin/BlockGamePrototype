namespace GDGame.Interfaces
{
    /// <summary>
    /// IActivatable is for anything that is Activatable.
    /// </summary>
    public interface IActivatable
    {
        #region Methods

        void Activate();
        void Deactivate();
        void ToggleActivation();

        #endregion
    }
}
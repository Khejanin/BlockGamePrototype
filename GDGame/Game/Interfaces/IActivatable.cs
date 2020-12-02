namespace GDGame.Interfaces
{
    public interface IActivatable
    {
        #region Methods

        void Activate();
        void Deactivate();
        void ToggleActivation();

        #endregion
    }
}
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDLibrary
{

    //Here we extend all the managers
    namespace Managers
    {
        public partial class CameraManager<T> : GameComponent where T : IActor
        {
            public void Clear()
            {
                list.Clear();
            }
        }
    }

}
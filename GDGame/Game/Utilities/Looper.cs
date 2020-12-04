namespace GDGame.Utilities
{
    public enum LoopMethod
    {
        PlayOnce,
        Loop,
        PingPongLoop,
        PingPongOnce
    }
    
    public static class Looper
    {

        public static bool Loop(LoopMethod loopMethod, ref int x, ref int step, int max)
        {
            switch (loopMethod)
            {
                case LoopMethod.PlayOnce:
                    if (x > max)
                    {
                        x = max;
                        return true;
                    }
                    break;
                case LoopMethod.Loop:
                    if (x > max) x = 0;
                    break;
                case LoopMethod.PingPongOnce:
                    if (x > max)
                    {
                        step = -1;
                        x = max;
                    }
                    if (x < 0)
                    {
                        x = 0;
                        return true;
                    }
                    break;
                case LoopMethod.PingPongLoop:
                    if (x > max)
                    {
                        step = -1;
                        x = max;
                    }
                    if (x <= 0) step = 1;
                    break;
            }

            return false;
        }
        
    }
}
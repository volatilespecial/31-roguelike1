using UnityEngine;

namespace Roguelike.Utils
{
    public static class Math
    {
        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;  
        }
    }
}

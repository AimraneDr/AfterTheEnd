using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Administration
{
    public static class Game
    {
        static bool StopGame = false;

        public static bool GameIsStoped() { return StopGame; }
        public static void Stop() { StopGame = true; }
        public static void Continue() { StopGame = false; }
    }
   
}

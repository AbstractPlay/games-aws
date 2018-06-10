using System;
using System.Collections.Generic;
using System.Text;

namespace apgames.Games
{
    interface IGame
    {
        string Init(string[] players, string[] variants = null);
        string Move(string player, string move, string state);
        string Archive(string[] states);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace apgames
{
    class Ithaka : Game
    {
        public Ithaka()
        {
            this.version = "1.0.0";
            this.description = @"# Ithaka

A player can move any piece on the board. The winner is the player at the end of whose turn a row of three pieces of the same colour exists (either orthogonal or diagonal).";
            this.playercounts = new int[1] { 2 };
            this.changelog = "";
            this.state = this.version;
        }
    }
}

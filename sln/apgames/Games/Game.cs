﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace apgames.Games
{
    public class Game
    {
        /* The following should be declared in each derived class
        public static string meta_version;
        public static string meta_description;
        public static int[] meta_playercounts;
        public static string meta_changelog;
        public static string meta_state;
        */

        public string[] players;
        public int currplayer;
        public string lastmove;
        public bool gameover = false;
        public string winner;
        public List<string> chatmsgs;

        public string Whoseturn()
        {
            return this.players[this.currplayer];
        }
    }
}

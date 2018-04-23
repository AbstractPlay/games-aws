using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace apgames.Games
{
    public class Game
    {
        public static string meta_version;
        public static string meta_description;
        public static int[] meta_playercounts;
        public static string meta_changelog;
        public static string meta_state;

        public string[] players;
        public int currplayer;
        public string lastmove;
        public bool gameover = false;

        public string genState()
        {
            SHA256 mysha = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(Game.meta_version + Game.meta_description + Game.meta_changelog);
            byte[] hash = mysha.ComputeHash(bytes);
            return Encoding.UTF8.GetString(hash);
        }

        public string Whoseturn()
        {
            return this.players[this.currplayer];
        }
    }
}

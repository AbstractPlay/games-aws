using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace apgames
{
    public class Ithaka : Game
    {
        public new static string meta_version = "1.0.0";
        public new static string meta_description = @"# Ithaka

A player can move a piece any number of empty spaces in a line, but they can't move the piece that was just moved OR a piece that is not orthoganally adjacent to another piece of its own colour. The winner is the player at the end of whose turn a row of three pieces of the same colour exists (either orthogonal or diagonal). A player loses if they are unable to move on their turn.";
        public new static int[] meta_playercounts = new int[1] { 2 };
        public new static string meta_changelog = "";
        public new static string meta_state = Ithaka.meta_version;

        private string board;
        private int lastmoved;
        private static int[][] lines =
        {
            new int[] { 0,1,2,3 },
            new int[] { 4,5,6,7 },
            new int[] { 8,9,10,11 },
            new int[] { 12,13,14,15 },
            new int[] { 0,4,8,12 },
            new int[] { 1,5,9,13 },
            new int[] { 2,6,10,14 },
            new int[] { 3,7,11,15 },
            new int[] { 0,5,10,15 },
            new int[] { 3,6,9,12 },
        };
        private Dictionary<string, int> states;
        public struct Serialized
        {
            public string[] players;
            public int currplayer;
            public string board;
            public int lastmoved;
            public string lastmove;
            public Dictionary<string, int> states;
        };

        public Ithaka(string[] players)
        {
            if (players.Length != 2)
            {
                throw new System.ArgumentException("You must pass an array of exactly two strings representing the players.");
            }
            this.players = players;
            this.currplayer = 0;
            this.board = "YYRRY--RB--GBBGG";
            this.states = new Dictionary<string, int>();
            this.lastmoved = -1;
            this.lastmove = "";
        }

        public Ithaka(string json)
        {
            Serialized data = JsonConvert.DeserializeObject<Serialized>(json);
            this.players = data.players;
            this.currplayer = data.currplayer;
            this.board = data.board;
            this.lastmoved = data.lastmoved;
            this.lastmove = data.lastmove;
            this.states = data.states;
        }

        public string Serialize()
        {
            Serialized data = new Serialized()
            {
                players = this.players,
                currplayer = this.currplayer,
                board = this.board,
                lastmoved = this.lastmoved,
                lastmove = this.lastmove,
                states = this.states
            };
            return JsonConvert.SerializeObject(data);
        }
    }
}

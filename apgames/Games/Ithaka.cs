using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Diagnostics;
using apgames.Grids.Square;

namespace apgames.Games
{
    public class Ithaka : Game
    {
        public new static string meta_version = "1.0.0";
        public new static string meta_description = @"# Ithaka

A player can move a piece any number of empty spaces in a line, but they can't move the piece that was just moved OR a piece that is not orthoganally adjacent to another piece of its own colour. The winner is the player at the end of whose turn a row of three pieces of the same colour exists (either orthogonal or diagonal). A player loses if they are unable to move on their turn.";
        public new static int[] meta_playercounts = new int[1] { 2 };
        public new static string meta_changelog = "";
        public new static string meta_state = Ithaka.meta_version;

        private SquareFixed grid;
        private char[] board;
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
        public Regex re_validmove = new Regex(@"^([a-z]\d+)\-([a-z]\d+)$", RegexOptions.IgnoreCase);

        public Ithaka(string[] players)
        {
            if (players.Length != 2)
            {
                throw new System.ArgumentException("You must pass an array of exactly two strings representing the players.");
            }
            var set = new HashSet<string>(players);
            if (set.Count != players.Length)
            {
                throw new System.ArgumentException("The list of players must contain no duplicates.");
            }
            this.players = players;
            this.currplayer = 0;
            this.board = "YYRRY--RB--GBBGG".ToCharArray();
            this.states = new Dictionary<string, int>();
            this.lastmoved = -1;
            this.lastmove = "";
            this.grid = new SquareFixed(4, 4);
        }

        public Ithaka(string json)
        {
            Serialized data = JsonConvert.DeserializeObject<Serialized>(json);
            this.players = data.players;
            this.currplayer = data.currplayer;
            this.board = data.board.ToCharArray();
            this.lastmoved = data.lastmoved;
            this.lastmove = data.lastmove;
            this.states = data.states;
            this.grid = new SquareFixed(4, 4);
        }

        public string Serialize()
        {
            Serialized data = new Serialized()
            {
                players = this.players,
                currplayer = this.currplayer,
                board = new string(this.board),
                lastmoved = this.lastmoved,
                lastmove = this.lastmove,
                states = this.states
            };
            return JsonConvert.SerializeObject(data);
        }

        private bool CanMove(Face f)
        {
            if (!grid.ContainsFace(f))
            {
                throw new ArgumentOutOfRangeException("The face " + f.ToString() + " doesn't exist on this game board.");
            }
            char piece = board[grid.Face2FlatIdx(f)];
            if (piece == '-') { return false; }
            bool canmove = false;
            foreach (Face n in f.Neighbours())
            {
                if (! grid.ContainsFace(n)) { continue; }
                if (board[grid.Face2FlatIdx(n)] == piece)
                {
                    canmove = true;
                    break;
                }
            }
            return canmove;
        }

        public bool IsEmpty(Face f)
        {
            if (!grid.ContainsFace(f)) {
                //return false;
                throw new ArgumentOutOfRangeException("The face "+f.ToString()+" doesn't exist on this game board.");
            }
            return board[grid.Face2FlatIdx(f)] == '-';
        }

        public IEnumerable<string> LegalMoves()
        {
            foreach (Face f in grid.Faces())
            {
                if (CanMove(f))
                {
                    foreach (Face n in f.Neighbours(true))
                    {
                        if (!grid.ContainsFace(n)) { continue; }
                        if (board[grid.Face2FlatIdx(n)] == '-')
                        {
                            //compose move
                            yield return f.ToLabel() + "-" + n.ToLabel();
                        }
                    }
                }
            }
        }

        public Ithaka Move(string player, string move)
        {
            if (gameover)
            {
                throw new InvalidOperationException("The game is over, so no moves can be made.");
            }
            if (player != players[currplayer])
            {
                throw new ArgumentOutOfRangeException("Out of turn. Current player: " + players[currplayer] + ". Attempted: " + player + ".");
            }
            Match m = re_validmove.Match(move);
            if (! m.Success)
            {
                throw new ArgumentException("Invalid move format.");
            }
            string from = m.Groups[1].ToString();
            Face ffrom = new Face(from);
            int fromidx = grid.Face2FlatIdx(ffrom);
            string to = m.Groups[2].ToString();
            Face fto = new Face(to);
            int toidx = grid.Face2FlatIdx(fto);
            if ( (CanMove(ffrom)) && (IsEmpty(fto)) && (new HashSet<Face>(ffrom.Neighbours(true)).Contains(fto)) )
            {
                board[toidx] = board[fromidx];
                board[fromidx] = '-';
            } else
            {
                throw new ArgumentException("The move is invalid.");
            }

            //Store state
            //Check for EOG
            //Update currplayer
            currplayer = (currplayer + 1) % players.Length;
            return this;
        }
    }
}

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
        public static string meta_version = "1.0.0";
        public static string meta_description = @"# Ithaka

A player can move a piece any number of empty spaces in a line, but they can't move the piece that was just moved OR a piece that is not orthoganally adjacent to another piece of its own colour. The winner is the player at the end of whose turn a row of three pieces of the same colour exists (either orthogonal or diagonal). A player loses if they are unable to move on their turn.";
        public static int[] meta_playercounts = new int[1] { 2 };
        public static string meta_changelog = "";
        public static string meta_state = Ithaka.meta_version;

        private SquareFixed grid;
        public char[] board;
        public int lastmoved;
        private static int[][] lines =
        {
            new int[] { 0,1,2 },
            new int[] { 1,2,3 },
            new int[] { 4,5,6 },
            new int[] { 5,6,7 },
            new int[] { 8,9,10 },
            new int[] { 9,10,11 },
            new int[] { 12,13,14 },
            new int[] { 13,14,15 },
            new int[] { 0,4,8 },
            new int[] { 4,8,12 },
            new int[] { 1,5,9 },
            new int[] { 5,9,13 },
            new int[] { 2,6,10 },
            new int[] { 6,10,14 },
            new int[] { 3,7,11 },
            new int[] { 7,11,15 },
            new int[] { 0,5,10 },
            new int[] { 5,10,15 },
            new int[] { 3,6,9 },
            new int[] { 6,9,12 },
        };
        private Dictionary<string, int> states;
        public struct Serialized
        {
            public int[] players;
            public int currplayer;
            public string board;
            public int lastmoved;
            public string lastmove;
            public Dictionary<string, int> states;
            public bool gameover;
            public int winner;
        };
        public struct Rendered
        {
            public string spriteset;
            public Dictionary<string, string> legend;
            public string position;
            public int boardwidth;
            public string board;
        };
        private Regex re_validmove = new Regex(@"^([a-z][1-4])\-([a-z][1-4])$", RegexOptions.IgnoreCase);

        public Ithaka(int[] players)
        {
            if (players.Length != 2)
            {
                throw new System.ArgumentException("You must pass an array of exactly two strings representing the players.");
            }
            var set = new HashSet<int>(players);
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
            this.chatmsgs = new List<string>();
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
            this.winner = data.winner;
            this.gameover = data.gameover;
            this.chatmsgs = new List<string>();
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

        public string Render()
        {
            Rendered data = new Rendered()
            {
                spriteset = "generic",
                legend = new Dictionary<string, string>() { { "R", "piece-red" }, { "B", "piece-blue" }, { "G", "piece-green" }, { "Y", "piece-yellow" } },
                position = new string(board),
                boardwidth = 4,
                board = "checkered"
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

        private bool IsEmpty(Face f)
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
                    foreach (Dirs dir in Enum.GetValues(typeof(Dirs)))
                    {
                        for (var dist=1; dist<=3; dist++)
                        {
                            Face n = f.Neighbour(dir, dist);
                            if (!grid.ContainsFace(n)) { break; }
                            if (board[grid.Face2FlatIdx(n)] != '-') { break; }
                            yield return f.ToLabel() + "-" + n.ToLabel();
                        }
                    }
                }
            }
        }

        public Ithaka Move(int player, string move)
        {
            move = move.ToLower();
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
            if (board[fromidx] == '-')
            {
                throw new ArgumentException("There's no piece at "+from+".");
            }
            if (fromidx == lastmoved)
            {
                throw new ArgumentException("You can't move the piece that was last moved.");
            }
            if (!CanMove(ffrom))
            {
                throw new ArgumentException("The piece at " + from + " cannot move. It must be orthogonally adjacent to at least one other piece of its colour.");
            }
            string to = m.Groups[2].ToString();
            Face fto = new Face(to);
            int toidx = grid.Face2FlatIdx(fto);
            if (! IsEmpty(fto))
            {
                throw new ArgumentException("You can't move onto an occupied space.");
            }
            if ( (! ffrom.OrthTo(fto)) && (! ffrom.DiagTo(fto)) )
            {
                throw new ArgumentException("You can only move pieces in a straight line.");
            }
            foreach (Face f in ffrom.Between(fto))
            {
                if (board[grid.Face2FlatIdx(f)] != '-')
                {
                    throw new ArgumentException("You can't jump over other pieces.");
                }
            }
            //LEGAL MOVE!
            board[toidx] = board[fromidx];
            board[fromidx] = '-';

            //store lastmove and lastmoved
            lastmove = move;
            lastmoved = toidx;

            //Store state
            string state = new string(board);
            if (states.ContainsKey(state))
            {
                states[state]++;
            } else
            {
                states[state] = 1;
            }
            if (states[state] > 1) {
                chatmsgs.Add("The current board state has been seen "+states[state].ToString()+" times. The player who repeats a position for the third time loses.");
            }

            //Check for EOG

            //repeat states
            if (states[state] >= 3)
            {
                gameover = true;
                winner = players[(currplayer + 1) % players.Length];
                chatmsgs.Add("The game has ended for the following reason: REPEATED BOARD POSITION. The winner is [u " + winner + "]!");
            }
            //no more moves
            else if (new HashSet<string>(LegalMoves()).Count == 0)
            {
                gameover = true;
                winner = players[currplayer];
                chatmsgs.Add("The game has ended for the following reason: NO MOVES LEFT. The winner is [u " + winner + "]!");
            }
            //line of 3
            else
            {
                bool isline = false;
                foreach (var line in lines)
                {
                    if ( (board[line[0]] == board[line[1]]) && (board[line[1]] == board[line[2]]) && (board[line[0]] != '-') )
                    {
                        isline = true;
                        break;
                    }
                }
                if (isline)
                {
                    gameover = true;
                    winner = players[currplayer];
                    chatmsgs.Add("The game has ended for the following reason: THREE IN A ROW. The winner is [u " + winner + "]!");
                }
            }

            //Update currplayer 
            currplayer = (currplayer + 1) % players.Length;
            return this;
        }
    }
}

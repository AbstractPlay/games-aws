using System;
using System.Collections.Generic;
using System.Text;

namespace apgames.Grids.Square
{
    public enum Dirs : uint { N, NE, E, SE, S, SW, W, NW };

    public static class Common
    {
        public static Tuple<int, int> Label2Coords(string lbl)
        {
            if (lbl.Length < 2)
            {
                throw new System.ArgumentException("The label must be at least two characters long, consisting of a lowercase letter followed by one or more digits.");
            }
            char letter = lbl.ToLower().Substring(0, 1).ToCharArray()[0];
            int x = (int)letter - (int)'a';
            if ( (x < 0) || (x > 25) )
            {
                throw new System.ArgumentException("The first character must be the lowercase letter 'a' through 'z'.");
            }

            string number = lbl.Substring(1);
            int y = Convert.ToInt32(number)-1;
            if (y < 0)
            {
                throw new System.ArgumentException("The y coordinate may not be negative.");
            }

            return new Tuple<int, int>(x, y);
        }

        public static string Coords2Label(int x, int y)
        {
            if ( (x < 0) || (x > 25) )
            {
                throw new System.ArgumentException("The x coordinate must be between 0 and 25 inclusive.");
            }
            if (y < 0)
            {
                throw new System.ArgumentException("The y coordinate cannot be negative.");
            }
            char letter = (char)(x + (int)'a');
            return letter.ToString() + (y+1).ToString();
        }
    }

    public class Face
    {
        public static Dictionary<Dirs, Tuple<int, int>> offsets = new Dictionary<Dirs, Tuple<int, int>>()
        {
            {Dirs.N, new Tuple<int, int>(0,1) },
            {Dirs.NE, new Tuple<int, int>(1,1) },
            {Dirs.E, new Tuple<int, int>(1,0) },
            {Dirs.SE, new Tuple<int, int>(1,-1) },
            {Dirs.S, new Tuple<int, int>(0,-1) },
            {Dirs.SW, new Tuple<int, int>(-1,-1) },
            {Dirs.W, new Tuple<int, int>(-1,0) },
            {Dirs.NW, new Tuple<int, int>(-1,1) }
        };
        public static Dictionary<Dirs, Dirs> opps = new Dictionary<Dirs, Dirs>()
        {
            {Dirs.N, Dirs.S },
            {Dirs.NE, Dirs.SW },
            {Dirs.E, Dirs.W },
            {Dirs.SE, Dirs.NW },
            {Dirs.S, Dirs.N },
            {Dirs.SW, Dirs.NE },
            {Dirs.W, Dirs.E },
            {Dirs.NW, Dirs.SE }
        };
        public int x, y;

        public Face(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Face(string label)
        {
            var coords = Common.Label2Coords(label);
            this.x = coords.Item1;
            this.y = coords.Item2;
        }

        public HashSet<Face> Neighbours(bool diag=false)
        {
            HashSet<Face> ns = new HashSet<Face>();
            foreach (KeyValuePair<Dirs, Tuple<int,int>> entry in offsets) 
            {
                //Even-numbered Dirs are orth
                if ( ((uint)entry.Key % 2 == 0) || (diag) )
                {
                    ns.Add(new Face(this.x + entry.Value.Item1, this.y + entry.Value.Item2));
                }
            }
            return ns;
        }

        public Face Neighbour(Dirs dir, int dist=1)
        {
            if (! offsets.ContainsKey(dir))
            {
                throw new ArgumentException("The direction you requested is not recognized.");
            }
            Tuple<int, int> val = offsets[dir];
            return new Face(this.x + (val.Item1 * dist), this.y + (val.Item2 * dist));
        }

        public HashSet<Edge> Borders()
        {
            return new HashSet<Edge>()
            {
                new Edge(this.x, this.y, Dirs.W),
                new Edge(this.x, this.y, Dirs.S),
                new Edge(this.x+1, this.y, Dirs.W),
                new Edge(this.x, this.y+1, Dirs.S)
            };
        }

        public HashSet<Vertex> Corners()
        {
            return new HashSet<Vertex>()
            {
                new Vertex(this.x+1, this.y+1),
                new Vertex(this.x+1, this.y),
                new Vertex(this.x, this.y),
                new Vertex(this.x, this.y+1)
            };
        }

        public bool OrthTo(Face obj) => ((this.x == obj.x) || (this.y == obj.y));

        public bool DiagTo(Face obj) => Math.Abs(this.x - obj.x) == Math.Abs(this.y - obj.y);

        public Dirs? DirectionTo(Face obj)
        {
            if (obj == this)
            {
                return null;
            }
            if (obj.x == this.x)
            {
                if (obj.y > this.y)
                {
                    return Dirs.N;
                } else
                {
                    return Dirs.S;
                }
            } else if (obj.y == this.y)
            {
                if (obj.x > this.x)
                {
                    return Dirs.E;
                } else
                {
                    return Dirs.W;
                }
            } else if (obj.x > this.x)
            {
                if (obj.y > this.y)
                {
                    return Dirs.NE;
                } else
                {
                    return Dirs.SE;
                }
            } else //if (obj.x < this.x)
            {
                if (obj.y > this.y)
                {
                    return Dirs.NW;
                }
                else
                {
                    return Dirs.SW;
                }
            }
        }

        public List<Face> Between(Face obj)
        {
            if ( (this == obj) || ( (!this.OrthTo(obj)) && (!this.DiagTo(obj)) ) )
            {
                throw new ArgumentException("The two Faces must be directly orthogonal or diagonal from each other.");
            }
            List<Face> lst = new List<Face>();
            Dirs dir = (Dirs)this.DirectionTo(obj);
            Face next = this.Neighbour(dir);
            while (next != obj)
            {
                lst.Add(next);
                next = next.Neighbour(dir);
            }
            return lst;
        }

        //OVERRIDES
        public override string ToString() => "Face<(" + this.x + ", " + this.y + ")>";

        public static bool operator ==(Face lhs, Face rhs)
        {
            if ( (lhs.x == rhs.x) && (lhs.y == rhs.y))
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(Face lhs, Face rhs)
        {
            if ((lhs.x == rhs.x) && (lhs.y == rhs.y))
            {
                return false;
            }
            return true;
        }

        public override bool Equals(Object obj) => obj is Face && this == (Face)obj;

        public override int GetHashCode() => this.x ^ this.y;
    }

    public class Edge
    {
        public int x, y;
        public Dirs dir;

        public Edge(int x, int y, Dirs dir)
        {
            if ( (dir != Dirs.W) && (dir != Dirs.S) )
            {
                throw new ArgumentException("Edges must be West or South.");
            }
            this.x = x;
            this.y = y;
            this.dir = dir;
        }

        public HashSet<Face> Joins()
        {
            HashSet<Face> set = new HashSet<Face>();
            if (this.dir == Dirs.W)
            {
                set.Add(new Face(this.x, this.y));
                set.Add(new Face(this.x-1, this.y));
            } else
            {
                set.Add(new Face(this.x, this.y));
                set.Add(new Face(this.x, this.y-1));
            }
            return set;
        }

        public HashSet<Edge> Continues()
        {
            HashSet<Edge> set = new HashSet<Edge>();
            if (this.dir == Dirs.W)
            {
                set.Add(new Edge(this.x, this.y + 1, Dirs.W));
                set.Add(new Edge(this.x, this.y - 1, Dirs.W));
            } else
            {
                set.Add(new Edge(this.x + 1, this.y, Dirs.S));
                set.Add(new Edge(this.x - 1, this.y, Dirs.S));
            }
            return set;
        }

        public HashSet<Vertex> Endpoints()
        {
            HashSet<Vertex> set = new HashSet<Vertex>();
            if (this.dir == Dirs.W)
            {
                set.Add(new Vertex(this.x, this.y));
                set.Add(new Vertex(this.x, this.y+1));
            } else
            {
                set.Add(new Vertex(this.x, this.y));
                set.Add(new Vertex(this.x+1, this.y));
            }
            return set;
        }

        //Overrides
        public override string ToString() => "Edge<(" + this.x + ", " + this.y + ")>";

        public static bool operator ==(Edge lhs, Edge rhs)
        {
            if ((lhs.x == rhs.x) && (lhs.y == rhs.y) && (lhs.dir == rhs.dir))
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(Edge lhs, Edge rhs)
        {
            if ((lhs.x == rhs.x) && (lhs.y == rhs.y) && (lhs.dir == rhs.dir))
            {
                return false;
            }
            return true;
        }

        public override bool Equals(Object obj) => obj is Edge && this == (Edge)obj;

        public override int GetHashCode() => this.x ^ this.y ^ (int)this.dir;
    }

    public class Vertex : Face
    {
        public Vertex (int x, int y) : base(x,y) { }
        public override string ToString() => "Vertex<(" + this.x + ", " + this.y + ")>";

        public HashSet<Face> Touches()
        {
            return new HashSet<Face>()
            {
                new Face(this.x, this.y),
                new Face(this.x, this.y-1),
                new Face(this.x-1, this.y-1),
                new Face(this.x-1, this.y)
            };
        }

        public HashSet<Edge> Protrudes()
        {
            return new HashSet<Edge>()
            {
                new Edge(this.x, this.y, Dirs.W),
                new Edge(this.x, this.y, Dirs.S),
                new Edge(this.x, this.y-1, Dirs.W),
                new Edge(this.x-1, this.y, Dirs.S),
            };
        }

        public HashSet<Vertex> Adjacent()
        {
            return new HashSet<Vertex>()
            {
                new Vertex(this.x, this.y+1),
                new Vertex(this.x+1, this.y),
                new Vertex(this.x, this.y-1),
                new Vertex(this.x-1, this.y)
            };
        }
    }
}

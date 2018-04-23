using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using Newtonsoft.Json;

using apgames;
using apgames.Games;
using apgames.Grids.Square;

namespace apgames.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
        }

        [Fact]
        public void TestIthakaInitMethod()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions();

            //ping

            //metadata

            //init
            request = new APIGatewayProxyRequest();
            request.Body = "{\"mode\": \"init\", \"players\": [\"aaron\", \"adele\"]}";
            context = new TestLambdaContext();
            response = functions.ProcessIthaka(request, context);
            Assert.Equal(200, response.StatusCode);
            dynamic body = JsonConvert.DeserializeObject(response.Body);
            string state = (string)body.state.ToObject(typeof(string));
            Ithaka g = new Ithaka(state);
            Assert.Equal(new string[2] { "aaron", "adele" }, g.players);

            //move
        }

        [Fact]
        public void TestGrids()
        {
            //Common
            Assert.Equal(new Tuple<int, int>(0, 0), Common.Label2Coords("a1"));
            Assert.Equal(new Tuple<int, int>(25, 122), Common.Label2Coords("z123"));
            Assert.Equal("a1", Common.Coords2Label(0, 0));
            Assert.Equal("z123", Common.Coords2Label(25, 122));

            Action shortlbl = () => Common.Label2Coords("a");
            Assert.Throws<ArgumentException>(shortlbl);
            Action badlet = () => Common.Label2Coords("11");
            Assert.Throws<ArgumentException>(badlet);
            Action negy = () => Common.Label2Coords("a-20");
            Assert.Throws<ArgumentException>(negy);
            Action badnum = () => Common.Label2Coords("aa");
            Assert.Throws<FormatException>(badnum);
            Action smallx = () => Common.Coords2Label(-1, 0);
            Assert.Throws<ArgumentException>(smallx);
            Action bigx = () => Common.Coords2Label(100, 0);
            Assert.Throws<ArgumentException>(bigx);
            Action smally = () => Common.Coords2Label(0, -1);
            Assert.Throws<ArgumentException>(smally);

            //Face
            Assert.Equal(new Face(0, 0), new Face(0, 0));
            Assert.NotEqual(new Face(0, 0), new Face(0, 1));
            Assert.Equal(new Face(0, 0), new Face("a1"));
            HashSet<Face> nodiags = new HashSet<Face>()
            {
                new Face(0,1),
                new Face(1,0),
                new Face(0,-1),
                new Face(-1,0)
            };
            HashSet<Face> withdiags = new HashSet<Face>()
            {
                new Face(0,1),
                new Face(1,1),
                new Face(1,0),
                new Face(1,-1),
                new Face(0,-1),
                new Face(-1,-1),
                new Face(-1,0),
                new Face(-1, 1)
            };
            Face baseface = new Face(0, 0);
            Assert.Equal(baseface.Neighbours(false), nodiags);
            Assert.Equal(baseface.Neighbours(true), withdiags);
            Assert.Equal(baseface.Neighbour(Dirs.N), new Face(0, 1));
            Assert.Equal(baseface.Neighbour(Dirs.NE, 2), new Face(2, 2));
            Assert.Equal(baseface.Neighbour(Dirs.E, 2), new Face(2, 0));
            Assert.Equal(baseface.Neighbour(Dirs.SE, 1), new Face(1, -1));
            Assert.Equal(baseface.Neighbour(Dirs.S), new Face(0, -1));
            Assert.Equal(baseface.Neighbour(Dirs.SW, 2), new Face(-2, -2));
            Assert.Equal(baseface.Neighbour(Dirs.W, 2), new Face(-2, 0));
            Assert.Equal(baseface.Neighbour(Dirs.NW), new Face(-1, 1));
            Assert.True(baseface.OrthTo(new Face(0, 1)));
            Assert.True(baseface.OrthTo(new Face(1, 0)));
            Assert.False(baseface.OrthTo(new Face(2, 5)));
            Assert.True(baseface.DiagTo(new Face(1, 1)));
            Assert.True(baseface.DiagTo(new Face(-4, -4)));
            Assert.False(baseface.DiagTo(new Face(0, 1)));
            Assert.False(baseface.DiagTo(new Face(2, 5)));
            Assert.Null(baseface.DirectionTo(baseface));
            Assert.Equal(Dirs.N, baseface.DirectionTo(new Face(0, 10)));
            Assert.Equal(Dirs.NE, baseface.DirectionTo(new Face(2, 5)));
            Assert.Equal(Dirs.E, baseface.DirectionTo(new Face(5, 0)));
            Assert.Equal(Dirs.SE, baseface.DirectionTo(new Face(3, -3)));
            Assert.Equal(Dirs.S, baseface.DirectionTo(new Face(0, -3)));
            Assert.Equal(Dirs.SW, baseface.DirectionTo(new Face(-3, -5)));
            Assert.Equal(Dirs.W, baseface.DirectionTo(new Face(-5, 0)));
            Assert.Equal(Dirs.NW, baseface.DirectionTo(new Face(-1, 1)));

            //Action nullobj = () => baseface.Between(null);
            //Assert.Throws<ArgumentException>(nullobj);
            Action eqobj = () => baseface.Between(baseface);
            Assert.Throws<ArgumentException>(eqobj);
            Action eccentric = () => baseface.Between(new Face(1, 20));
            Assert.Throws<ArgumentException>(eccentric);
            List<Face> comp = new List<Face>()
            {
                new Face(1,1),
                new Face(2,2),
                new Face(3,3)
            };
            Assert.Equal(comp, baseface.Between(new Face(4, 4)));

            //Edge

            //Vertex

            //SquareFixed
            SquareFixed basegrid = new SquareFixed(2, 2);
            Assert.Equal(
                new HashSet<Face>() { new Face(0, 0), new Face(0, 1), new Face(1, 0), new Face(1, 1) },
                basegrid.Faces()
            );
            Assert.Equal(
                new HashSet<Vertex>() { new Vertex(0,0), new Vertex(1, 0), new Vertex(2, 0), new Vertex(0, 1), new Vertex(1, 1), new Vertex(2, 1), new Vertex(0, 2), new Vertex(1, 2), new Vertex(2, 2) },
                basegrid.Vertices()
            );
            Assert.Equal(
                new HashSet<Edge>() { new Edge(0, 0, Dirs.W), new Edge(0, 0, Dirs.S), new Edge(0, 1, Dirs.W), new Edge(0, 1, Dirs.S), new Edge(1, 0, Dirs.W), new Edge(1, 0, Dirs.S), new Edge(1, 1, Dirs.W), new Edge(1, 1, Dirs.S), new Edge(0, 2, Dirs.S), new Edge(1, 2, Dirs.S), new Edge(2, 1, Dirs.W), new Edge(2, 0, Dirs.W) },
                basegrid.Edges()
            );
            Assert.Equal((double)0, basegrid.Face2FlatIdx(new Face(0, 0)));
            Assert.Equal((double)1, basegrid.Face2FlatIdx(new Face(1, 0)));
            Assert.Equal((double)2, basegrid.Face2FlatIdx(new Face(0, 1)));
            Assert.Equal((double)3, basegrid.Face2FlatIdx(new Face(1, 1)));
            Action badface = () => basegrid.Face2FlatIdx(new Face(2, 2));
            Assert.Throws<ArgumentException>(badface);
            Assert.Equal(new Face(0, 0), basegrid.FlatIdx2Face(0));
            Assert.Equal(new Face(1, 0), basegrid.FlatIdx2Face(1));
            Assert.Equal(new Face(0, 1), basegrid.FlatIdx2Face(2));
            Assert.Equal(new Face(1, 1), basegrid.FlatIdx2Face(3));
            Assert.True(basegrid.ContainsFace(new Face(1, 1)));
            Assert.False(basegrid.ContainsFace(new Face(10, 10)));
        }

        [Fact]
        public void TestIthakaClassMethod()
        {
            //constructor
            Action a = () => new Ithaka(new string[2] { "aaron", "aaron" });
            Action b = () => new Ithaka(new string[1] { "aaron" });
            Assert.Throws<System.ArgumentException>(a);
            Assert.Throws<System.ArgumentException>(b);

            //LegalMoves
            Ithaka basegame = new Ithaka(new string[2] { "aaron", "adele" });
            HashSet<string> moves = new HashSet<string>()
            {
                "a1-b2",
                "b1-b2", "b1-c2",
                "c1-b2", "c1-c2",
                "d1-c2",
                "a2-b2", "a2-b3",
                "d2-c2", "d2-c3",
                "a3-b2", "a3-b3",
                "d3-c2", "d3-c3",
                "a4-b3", 
                "b4-b3", "b4-c3",
                "c4-b3", "c4-c3",
                "d4-c3"
            };
            HashSet<string> actual = new HashSet<string>(basegame.LegalMoves());
            Assert.Equal(moves, actual);

            //Move
            Action wrongplayer = () => basegame.Move("adele", "a1-b2");
            Assert.Throws<ArgumentOutOfRangeException>(wrongplayer);
            Action badmoveform = () => basegame.Move("aaron", "asdf");
            Assert.Throws<ArgumentException>(badmoveform);
            Action fromoor = () => basegame.Move("aaron", "z1-b2");
            Action fromempty = () => basegame.Move("aaron", "b2-b3");
            Assert.Throws<ArgumentException>(fromoor);
            Assert.Throws<ArgumentException>(fromempty);
            Action tooor = () => basegame.Move("aaron", "a1-z10");
            Action tofar = () => basegame.Move("aaron", "a1-c3");
            Action toocc = () => basegame.Move("aaron", "a1-b1");
            Assert.Throws<ArgumentException>(tooor);
            Assert.Throws<ArgumentException>(tofar);
            Assert.Throws<ArgumentException>(toocc);
            basegame.Move("aaron", "a1-b2");
        }
    }
}

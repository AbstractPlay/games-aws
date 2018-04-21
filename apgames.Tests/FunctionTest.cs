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
        }

        [Fact]
        public void TestIthakaClassMethod()
        {
            //constructor
            Action a = () => new Ithaka(new string[2] { "aaron", "aaron" });
            Action b = () => new Ithaka(new string[1] { "aaron" });
            Assert.Throws<System.ArgumentException>(a);
            Assert.Throws<System.ArgumentException>(b);
            //
        }
    }
}

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


            request = new APIGatewayProxyRequest();
            request.Body = "{\"mode\": \"init\", \"players\": [\"aaron\", \"adele\"]}";
            context = new TestLambdaContext();
            response = functions.ProcessIthaka(request, context);
            Assert.Equal(200, response.StatusCode);
            dynamic body = JsonConvert.DeserializeObject(response.Body);
            string state = (string)body.state.ToObject(typeof(string));
            Ithaka g = new Ithaka(state);
            Assert.Equal(new string[2] { "aaron", "adele" }, g.players);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace apgames
{
    public struct ResponsePing
    {
        public string state;
    }

    public struct Variants
    {
        public string name;
        public string note;
        public string group;
    }

    public struct ResponseMetadata
    {
        public string state;
        public string version;
        public string playercounts;
        public string description;
        public string changelog;
        public Variants[] variants;
    }

    public class Functions
    {
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
        }

        private string GetParms(IDictionary<string, string> qsParms, string parmName)
        {
            var result = string.Empty;
            if (qsParms != null)
            {
                if (qsParms.ContainsKey(parmName))
                {
                    result = qsParms[parmName];
                }
            }
            return result;
        }

        /// <summary>
        /// Processes POST requests for the game Ithaka
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of blogs</returns>
        public APIGatewayProxyResponse ProcessIthaka(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var mode = GetParms(request.QueryStringParameters, "mode");

            APIGatewayProxyResponse response;
            if (mode == "ping")
            {
                ResponsePing ret = new ResponsePing()
                {
                    state = new Ithaka().state
                };
                response = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonConvert.SerializeObject(ret),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            else
            {
                response = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = "Either you did not provide a 'mode' parameter or what you provided is either not recognized or not supported",
                    Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
                };
            }
            return response;
        }
    }
}

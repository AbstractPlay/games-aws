using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

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
        public int[] playercounts;
        public string description;
        public string changelog;
        public Variants[] variants;
    }

    public struct ResponseMove
    {
        public string state;
        public string whoseturn;
        public string chat;
        public string renderrep;
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
            dynamic body = JsonConvert.DeserializeObject(request.Body);
            string mode = (string)body.mode.ToObject(typeof(string));

            APIGatewayProxyResponse response = new APIGatewayProxyResponse();
            if (mode == "ping")
            {
                ResponsePing ret = new ResponsePing()
                {
                    state = Ithaka.meta_state
                };
                response = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonConvert.SerializeObject(ret),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            else if (mode == "metadata")
            {
                ResponseMetadata ret = new ResponseMetadata()
                {
                    state = Ithaka.meta_state,
                    version = Ithaka.meta_version,
                    playercounts = Ithaka.meta_playercounts,
                    description = Ithaka.meta_description,
                    changelog = Ithaka.meta_changelog
                };
                response = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonConvert.SerializeObject(ret),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            else if (mode == "init")
            {
                string[] players = (string[])body.players.ToObject(typeof(string[]));
                Ithaka g = new Ithaka(players);
                ResponseMove ret = new ResponseMove()
                {
                    state = g.Serialize(),
                    whoseturn = g.Whoseturn(),
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
                    Body = JsonConvert.SerializeObject(new Dictionary<string, string> { { "message", "Missing or invalid 'mode' parameter provided. It must be one of 'ping', 'metadata', or 'init'." } }),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            return response;
        }
    }
}

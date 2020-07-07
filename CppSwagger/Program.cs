using System;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CppSwagger
{
    class Program
    {
        static void Main(string[] args)
        {
            string swaggerJsonString = File.ReadAllText(ConfigurationManager.AppSettings["swagger_docker_json_path"]);
            JObject swaggerJson = JObject.Parse(swaggerJsonString);



        }
    }
}

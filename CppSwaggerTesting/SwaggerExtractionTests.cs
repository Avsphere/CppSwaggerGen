using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CppSwaggerTesting
{
    public class SwaggerExtractionTests
    {
        public JObject SwaggerJson { get; set; }

        [SetUp]
        public void Setup()
        {
            string jsonString = File.ReadAllText(@"C:\Users\avsph\source\repos\CppSwagger\CppSwagger\swagger_data\swagger_docker.json");
            this.SwaggerJson = JObject.Parse(jsonString);
        }

        [Test]
        public void HasDefinitionsProperty()
        {
            Assert.IsTrue(this.SwaggerJson["definitions"].ToString().Length > 1000); // 1000 bs number that I know it is greater than
        }
    }
}
using System.IO;
using CppSwagger;
using CppSwagger.DataContracts;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CppSwaggerTesting
{
    public class ProcessSwaggerDefinitionTest
    {
        public JObject SwaggerJson { get; set; }

        [SetUp]
        public void Setup()
        {
            string jsonString = File.ReadAllText(@"C:\Users\avsph\source\repos\CppSwagger\CppSwagger\swagger_data\swagger_docker.json");
            this.SwaggerJson = JObject.Parse(jsonString);
        }

        [Test]
        public void ProcessSimpleDefinition()
        {
            JObject simplePropertyOnlyObject = this.SwaggerJson["definitions"]["EndpointPortConfig"] as JObject;

            ProcessSwaggerDefinitions processor = new ProcessSwaggerDefinitions();

            PreprocessedSwaggerDefinition preprocessedSwaggerDefinition = processor.ProcessDefinitionKeyValuePair(new System.Collections.Generic.KeyValuePair<string, JObject>("EndpointPortConfig", simplePropertyOnlyObject));

            Assert.IsNotNull(preprocessedSwaggerDefinition.Id);
            Assert.IsTrue(preprocessedSwaggerDefinition.ResolvedProperties.Count == 5);

        }

        [Test]
        public void ProcessDefinitionWithNestedObjects()
        {
            JObject simplePropertyOnlyObject = this.SwaggerJson["definitions"]["Volume"] as JObject;

            ProcessSwaggerDefinitions processor = new ProcessSwaggerDefinitions();

            PreprocessedSwaggerDefinition preprocessedSwaggerDefinition = processor.ProcessDefinitionKeyValuePair(new System.Collections.Generic.KeyValuePair<string, JObject>("Volume", simplePropertyOnlyObject));

        }
    }
}
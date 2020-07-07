using System;
using System.Collections.Generic;
using System.IO;
using CppSwagger;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CppSwaggerTesting
{
    public class SwaggerPropertyClassificationTests
    {
        public JObject SwaggerJson { get; set; }

        [SetUp]
        public void Setup()
        {
            string jsonString = File.ReadAllText(@"C:\Users\avsph\source\repos\CppSwagger\CppSwagger\swagger_data\swagger_docker.json");
            this.SwaggerJson = JObject.Parse(jsonString);
        }

        [Test]
        public void CanClassifyBasicTypes()
        {
            JObject withOnlySimpleProperties = this.SwaggerJson["definitions"]["EndpointPortConfig"]["properties"] as JObject;

            foreach( KeyValuePair<string, JToken> keyValue in withOnlySimpleProperties)
            {
                Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(keyValue.Value as JObject));
            }

        }

        [Test]
        public void CanClassifyRef()
        {
            JObject resolvableRef = this.SwaggerJson["definitions"]["Config"]["properties"]["Version"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(resolvableRef));
        }

        [Test]
        public void CanClassifyResolvableMap()
        {
            JObject resolvableMap = this.SwaggerJson["definitions"]["RegistryServiceConfig"]["properties"]["IndexConfigs"] as JObject;
            JObject mapOfStrings = this.SwaggerJson["definitions"]["Volume"]["properties"]["Labels"] as JObject;
            JObject resolvableMapOfRefs = this.SwaggerJson["definitions"]["ContainerSummary"]["items"]["properties"]["NetworkSettings"]["properties"]["Networks"] as JObject;
            JObject anotherMapOfRefs = this.SwaggerJson["definitions"]["Network"]["properties"]["Containers"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(resolvableMap));
            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(resolvableMapOfRefs));
            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(anotherMapOfRefs));
            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(mapOfStrings));
        }

        [Test]
        public void SimpleClassifierFailsToClassifyNestedObject()
        {
            JObject nestedObject = this.SwaggerJson["definitions"]["Volume"]["properties"]["UsageData"] as JObject;

            Assert.IsFalse(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(nestedObject));
        }

        [Test]
        public void CanClassifyResolvableArray()
        {
            JObject resolvableArray = this.SwaggerJson["definitions"]["RegistryServiceConfig"]["properties"]["AllowNondistributableArtifactsHostnames"] as JObject;
            JObject anotherResolvableArray = this.SwaggerJson["definitions"]["RegistryServiceConfig"]["properties"]["InsecureRegistryCIDRs"] as JObject;
            JObject arrayOfRefs = this.SwaggerJson["definitions"]["SwarmInfo"]["properties"]["RemoteManagers"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(resolvableArray));
            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(anotherResolvableArray));
            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(arrayOfRefs));
        }

        [Test]
        public void CanClassifyResolvableArrayOfArrays()
        {
            JObject resolvableArrayOfArrays = this.SwaggerJson["definitions"]["SystemInfo"]["properties"]["SystemStatus"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(resolvableArrayOfArrays));
        }

        
        [Test]
        public void ConvienceFunction_CanClassifyResolvableArrayOfArrays()
        {
            JObject resolvableArrayOfArrays = this.SwaggerJson["definitions"]["SystemInfo"]["properties"]["SystemStatus"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.IsArrayOfResolvableArrays(resolvableArrayOfArrays));
        }

        [Test]
        public void ConvienceFunction_CanClassifyResolvableMapOfArrays()
        {
            JObject resolvableMapOfArrays = this.SwaggerJson["definitions"]["PortMap"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.IsMapOfResolvableArrays(resolvableMapOfArrays));
        }


        [Test]
        public void CanClassifiesNonResolvableObject()
        {
            JObject nonResolvable = this.SwaggerJson["definitions"]["EndpointPortConfig"] as JObject;
            
            Assert.IsFalse(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(nonResolvable));
        }
    }
}
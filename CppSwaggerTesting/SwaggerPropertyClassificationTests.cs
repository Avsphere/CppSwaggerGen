using System;
using System.Collections.Generic;
using System.IO;
using CppSwagger;
using CppSwagger.DataContracts;
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
        public void CanResolveRef()
        {
            JObject resolvableRef = this.SwaggerJson["definitions"]["Config"]["properties"]["Version"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(resolvableRef));
        }

        [Test]
        public void CanClassifyRef()
        {
            JObject resolvableRef = this.SwaggerJson["definitions"]["Config"]["properties"]["Version"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.GetResolvableType(resolvableRef) == ResolvableSwaggerType.Ref);
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
        public void CanClassifyPrimitive_ResolvableSwaggerType()
        {
            JObject withOnlySimpleProperties = this.SwaggerJson["definitions"]["EndpointPortConfig"]["properties"] as JObject;

            foreach (KeyValuePair<string, JToken> keyValue in withOnlySimpleProperties)
            {
                Assert.IsTrue(SwaggerPropertyClassifier.GetResolvableType(keyValue.Value as JObject) == ResolvableSwaggerType.Primitive);
            }
        }

        [Test]
        public void CanClassifyArrayOfPrimitives_ResolvableSwaggerType()
        {
            JObject resolvableArray = this.SwaggerJson["definitions"]["RegistryServiceConfig"]["properties"]["AllowNondistributableArtifactsHostnames"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.GetResolvableType(resolvableArray) == ResolvableSwaggerType.ArrayOfPrimitives);
        }

        [Test]
        public void CanClassifyArrayOfRefs_ResolvableSwaggerType()
        {
            JObject arrayOfRefs = this.SwaggerJson["definitions"]["SwarmInfo"]["properties"]["RemoteManagers"] as JObject;
            Assert.IsTrue(SwaggerPropertyClassifier.GetResolvableType(arrayOfRefs) == ResolvableSwaggerType.ArrayOfRefs);
        }

        [Test]
        public void CanClassifyArrayOfArrayPrimitives_ResolvableSwaggerType()
        {
            JObject arrayOfPrimitiveArrays = this.SwaggerJson["definitions"]["SystemInfo"]["properties"]["SystemStatus"] as JObject;

            Assert.IsTrue(SwaggerPropertyClassifier.GetResolvableType(arrayOfPrimitiveArrays) == ResolvableSwaggerType.ArrayArrayOfPrimitives);
        }

        [Test]
        public void CanClassifyPrimitiveMap_ResolvableSwaggerType()
        {
            JObject mapOfStrings = this.SwaggerJson["definitions"]["Volume"]["properties"]["Labels"] as JObject;
            Assert.IsTrue(SwaggerPropertyClassifier.GetResolvableType(mapOfStrings) == ResolvableSwaggerType.MapOfPrimitives);
        }

        [Test]
        public void CanClassifyRefMap_ResolvableSwaggerType()
        {
            JObject mapOfRefs = this.SwaggerJson["definitions"]["RegistryServiceConfig"]["properties"]["IndexConfigs"] as JObject;
            Assert.IsTrue(SwaggerPropertyClassifier.GetResolvableType(mapOfRefs) == ResolvableSwaggerType.MapOfRefs);
        }

    }
}
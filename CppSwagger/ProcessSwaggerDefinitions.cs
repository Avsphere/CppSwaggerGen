using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppSwagger.DataContracts;
using Newtonsoft.Json.Linq;

namespace CppSwagger
{
    using UnprocessedSwaggerDefinition = KeyValuePair<string, JObject>;


    public class ProcessSwaggerDefinitions
    {
        private Queue<IntermediateChildDefinition> NestedObjectProcessQueue { get; set; } = new Queue<IntermediateChildDefinition>();

        private IList<PreprocessedSwaggerDefinition> Processed { get; set; }

        private IntermediateChildDefinition CreateIntermediateChildDefinitionFromParent(Guid parentId, KeyValuePair<string, JObject> childObjectPropertyPair)
        {
            IntermediateChildDefinition childDefinition = new IntermediateChildDefinition()
            {
                ParentId = parentId,
                Key = childObjectPropertyPair.Key,
                ChildObject = childObjectPropertyPair.Value
            };


            return childDefinition;
        }

        private IList<IntermediateChildDefinition> ProcessNestedObjects(Guid parentId, IList<KeyValuePair<string, JObject>> childrenPropertyPairs)
        {
            IList<IntermediateChildDefinition> children = new List<IntermediateChildDefinition>();
            
            foreach (KeyValuePair<string, JObject> childPropPair in childrenPropertyPairs)
            {
                IntermediateChildDefinition intermediateChildDefinition = CreateIntermediateChildDefinitionFromParent(parentId, childPropPair);
                children.Add(intermediateChildDefinition);
                NestedObjectProcessQueue.Enqueue(intermediateChildDefinition);
            }

            return children;
        }


        private IList<KeyValuePair<string, JObject>> FilterProperties(JObject objectProperties, Predicate<JObject> propertyFilter)
        {
            IList<KeyValuePair<string, JObject>> resolvablePropertyPairs = new List<KeyValuePair<string, JObject>>();

            foreach( KeyValuePair<string, JToken> propPair in objectProperties)
            {
                JObject property = propPair.Value as JObject;
                if (propertyFilter(property))
                {
                    resolvablePropertyPairs.Add( new KeyValuePair<string, JObject>(propPair.Key, propPair.Value as JObject));
                }
            }

            return resolvablePropertyPairs;
        }

        private IList<KeyValuePair<string, JObject>> ExtractResolvableProperties(JObject objectProperties) => FilterProperties(objectProperties, (property) => SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(property));
        private IList<KeyValuePair<string, JObject>> ExtractNonImmediatelyResolvableProperties(JObject objectProperties) => FilterProperties(objectProperties, (property) => !SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(property));
        public PreprocessedSwaggerDefinition ProcessDefinitionKeyValuePair(UnprocessedSwaggerDefinition definitionKeyValue)
        {

            if (!SwaggerPropertyClassifier.IsNormalTopLevelSwaggerDefinition(definitionKeyValue.Value))
            {
                throw new Exception($"ProcessDefinitionKeyValuePair can only handle normal objects but was given {definitionKeyValue.Value.ToString()}");
            }

            PreprocessedSwaggerDefinition currentDefinition = new PreprocessedSwaggerDefinition() { TypeName = definitionKeyValue.Key };

            IList <KeyValuePair<string, JObject>> resolvableProperties = ExtractResolvableProperties(definitionKeyValue.Value["properties"] as JObject);
            currentDefinition.ResolvedProperties = resolvableProperties.Select(PropertyResolver.ResolveSimpleProperty).ToList();


            // these are the nested objects that are not resolvable because they need to be checked for name collisions once everything else has been processed
            IList<KeyValuePair<string, JObject>> nonResolvableNestedObjects = ExtractNonImmediatelyResolvableProperties(definitionKeyValue.Value["properties"] as JObject);

            currentDefinition.Children = ProcessNestedObjects(currentDefinition.ParentId, nonResolvableNestedObjects);

            return currentDefinition;
        }



    }
}

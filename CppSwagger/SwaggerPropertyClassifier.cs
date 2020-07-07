using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CppSwagger
{
    public enum SwaggerDefinitionClassification
    {
        NormalObject,

        /*
         * This is the case where the a swagger top level definition is something that does not have a type of object.
         * In this case it does not need a class in the header file but instead just a typedef
         * 
         * e.g. typedef ContainerSummaries = std::vector<ContainerSummary>
         * 
         */
        AbnormalNeedingTypedef
    }
    public static class SwaggerPropertyClassifier
    {

        private static IList<string> BasicTypes = new List<string>() { "string", "integer", "boolean" };

        private static bool HasType(JObject swaggerProperty) => swaggerProperty.ContainsKey("type");
        private static bool HasObjectType(JObject swaggerProperty) => HasType(swaggerProperty) && swaggerProperty["type"].ToString() == "object";
        private static bool HasArrayType(JObject swaggerProperty) => HasType(swaggerProperty) && swaggerProperty["type"].ToString() == "array";
        private static bool HasAdditionalProperties(JObject swaggerProperty) => HasObjectType(swaggerProperty) && swaggerProperty.ContainsKey("additionalProperties");
        private static bool HasItems(JObject swaggerProperty) => HasArrayType(swaggerProperty) && swaggerProperty.ContainsKey("items");

        public static bool IsBasicType(JObject swaggerProperty) => swaggerProperty.ContainsKey("type") && BasicTypes.Contains(swaggerProperty["type"].ToString());

        public static bool IsRef(JObject swaggerProperty) => swaggerProperty.ContainsKey("$ref");

        /*
         * FIX ME -- currently used to deal with additional properties with enums
         * Example : ContainerConfig : Volumes
         */ 
        public static bool IsMapOfEnums(JObject swaggerProperty)
        {
            if (!HasAdditionalProperties(swaggerProperty))
            {
                return false;
            }

            JObject additionalProperties = swaggerProperty["additionalProperties"] as JObject;
            return additionalProperties.ContainsKey("enum");
        }

        public static bool IsArrayOfResolvableArrays(JObject swaggerProperty)
        {
            if (!IsResolvableArray(swaggerProperty))
            {
                return false;
            }

            JObject items = swaggerProperty["items"] as JObject;
            
            return IsResolvableArray(items);
        }

        public static bool IsArrayOfResolvableMaps(JObject swaggerProperty)
        {
            if (!IsResolvableArray(swaggerProperty))
            {
                return false;
            }

            JObject items = swaggerProperty["items"] as JObject;

            return IsResolvableMap(items);
        }

        public static bool IsMapOfResolvableArrays(JObject swaggerProperty)
        {
            if (!IsResolvableMap(swaggerProperty))
            {
                return false;
            }

            JObject additionalProperties = swaggerProperty["additionalProperties"] as JObject;

            return IsResolvableArray(additionalProperties);
        }

        public static bool IsMapOfResolvableMaps(JObject swaggerProperty)
        {
            if (!IsResolvableMap(swaggerProperty))
            {
                return false;
            }

            JObject additionalProperties = swaggerProperty["additionalProperties"] as JObject;

            return IsResolvableMap(additionalProperties);
        }

        public static bool IsResolvableArray(JObject swaggerProperty)
        {
            if (HasItems(swaggerProperty) == false)
            {
                return false;
            }

            JObject items = swaggerProperty["items"] as JObject;
            bool isArrayOfRefs = IsRef(items);
            bool isArrayOfBasicTypes = IsBasicType(items);
            bool isArrayOfResolvableMaps = IsResolvableMap(items);
            bool isResolvableArrayOfArrays = IsResolvableArray(items);

            return isArrayOfRefs || isArrayOfBasicTypes || isArrayOfResolvableMaps || isResolvableArrayOfArrays;
        }

        public static bool IsResolvableMap(JObject swaggerProperty)
        {
            if (HasAdditionalProperties(swaggerProperty) == false)
            {
                return false;
            }

            JObject additionalProperties = swaggerProperty["additionalProperties"] as JObject;

            bool isMapOfBasicTypes = IsBasicType(additionalProperties);
            bool isMapContainingARef = IsRef(additionalProperties);
            bool isMapContainingABasicArray = IsResolvableArray(additionalProperties);
            bool isResolvableMapOfMaps = IsResolvableMap(additionalProperties);

            return isMapOfBasicTypes || isMapContainingARef || isMapContainingABasicArray || isResolvableMapOfMaps;
        }

        /*
         * Given a definition pulled from the top level of the swagger json determine if the header file needs a class, if it does, it is deemed normal
         * 
         * 
         */

        public static bool IsNormalTopLevelSwaggerDefinition(JObject potentiallyNormalSwaggerDefinition)
        {
            bool isAnObject = potentiallyNormalSwaggerDefinition.ContainsKey("type") && potentiallyNormalSwaggerDefinition["type"].ToString().Equals("object");
            bool hasProperties = potentiallyNormalSwaggerDefinition.ContainsKey("properties");

            return isAnObject && hasProperties;
        }

        public static bool IsSwaggerPropertyResolvable(JObject swaggerProperty)
        {
            return IsRef(swaggerProperty) || IsBasicType(swaggerProperty) || IsResolvableArray(swaggerProperty) || IsResolvableMap(swaggerProperty);
        }
    }
}

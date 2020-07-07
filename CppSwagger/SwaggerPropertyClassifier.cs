using System;
using System.Collections.Generic;
using System.Text;
using CppSwagger.DataContracts;
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


        public static ResolvableSwaggerType GetResolvableMapType(JObject swaggerProperty)
        {
            if (HasAdditionalProperties(swaggerProperty) == false)
            {
                throw new Exception($"GetResolvableMapType passed property has no additional properties {swaggerProperty.ToString()}");
            }

            JObject additionalProperties = swaggerProperty["additionalProperties"] as JObject;

            if (IsBasicType(additionalProperties))
            {
                return ResolvableSwaggerType.MapOfPrimitives;
            }
            else if (IsRef(additionalProperties))
            {
                return ResolvableSwaggerType.MapOfRefs;
            }
            else if (IsResolvableArray(additionalProperties))
            {
                ResolvableSwaggerType arrayType = GetResolvableArrayType(additionalProperties);
                
                if (arrayType == ResolvableSwaggerType.ArrayOfRefs)
                {
                    return ResolvableSwaggerType.MapOfRefArrays;
                }
                else if (arrayType == ResolvableSwaggerType.ArrayOfPrimitives)
                {
                    return ResolvableSwaggerType.MapOfPrimitiveArrays;
                }
                else
                {
                    throw new Exception($"GetResolvableMapType IsResolvableArray can't specify type: {swaggerProperty.ToString()}");
                }
            }
            else if (IsResolvableMap(additionalProperties))
            {
                ResolvableSwaggerType mapType = GetResolvableMapType(additionalProperties);

                if (mapType == ResolvableSwaggerType.MapOfPrimitives)
                {
                    return ResolvableSwaggerType.MapOfPrimitiveMaps;
                }
                else if (mapType == ResolvableSwaggerType.MapOfRefs)
                {
                    return ResolvableSwaggerType.MapOfRefMaps;
                }
                else
                {
                    throw new Exception($"GetResolvableMapType IsResolvableMap can't specify type: {swaggerProperty.ToString()}");
                }
            }
            else
            {
                throw new Exception($"GetResolvableMapType can't specify type: {swaggerProperty.ToString()}");
            }
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


        public static ResolvableSwaggerType GetResolvableArrayType(JObject swaggerProperty)
        {
            if (HasItems(swaggerProperty) == false)
            {
                throw new Exception($"GetResolvableArrayType passed property has no items {swaggerProperty.ToString()}");
            }
            
            JObject items = swaggerProperty["items"] as JObject;

            if (IsRef(items))
            {
                return ResolvableSwaggerType.ArrayOfRefs;
            }
            else if (IsBasicType(items))
            {
                return ResolvableSwaggerType.ArrayOfPrimitives;
            }
            else if (IsResolvableMap(items))
            {
                ResolvableSwaggerType mapType = GetResolvableMapType(items);
                if (mapType == ResolvableSwaggerType.MapOfPrimitives)
                {
                    return ResolvableSwaggerType.ArrayOfPrimitiveMaps;
                }
                else if (mapType == ResolvableSwaggerType.MapOfRefs)
                {
                    return ResolvableSwaggerType.ArrayOfMapOfRefs;
                }
                else
                {
                    throw new Exception($"GetResolvableArrayType IsResolvableMap can't specify type: {swaggerProperty.ToString()}");
                }
            }
            else if (IsResolvableArray(items))
            {
                ResolvableSwaggerType arrayType = GetResolvableArrayType(items);
                if (arrayType == ResolvableSwaggerType.ArrayOfPrimitives)
                {
                    return ResolvableSwaggerType.ArrayArrayOfPrimitives;
                }
                else if (arrayType == ResolvableSwaggerType.ArrayOfRefs)
                {
                    return ResolvableSwaggerType.ArrayArrayOfRefs;
                }
                else if (arrayType == ResolvableSwaggerType.ArrayOfPrimitiveMaps)
                {
                    return ResolvableSwaggerType.ArrayArrayOfPrimitiveMaps;
                }
                else
                {
                    throw new Exception($"GetResolvableArrayType IsResolvableArray can't specify type: {swaggerProperty.ToString()}");
                }
            }
            else
            {
                throw new Exception($"GetResolvableArrayType passed property can't be resolved {swaggerProperty.ToString()}");
            }
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

        public static ResolvableSwaggerType GetResolvableType(JObject swaggerProperty)
        {
            if (IsBasicType(swaggerProperty))
            {
                return ResolvableSwaggerType.Primitive;
            }
            else if (IsRef(swaggerProperty))
            {
                return ResolvableSwaggerType.Ref;
            }
            else if (IsResolvableArray(swaggerProperty))
            {
                return GetResolvableArrayType(swaggerProperty);
            }
            else if (IsResolvableMap(swaggerProperty))
            {
                return GetResolvableMapType(swaggerProperty);
            }
            else
            {
                throw new Exception($"GetResolvableType passed property can't be resolved {swaggerProperty.ToString()}");
            }
        }


        public static bool IsSwaggerPropertyResolvable(JObject swaggerProperty)
        {
            return IsRef(swaggerProperty) || IsBasicType(swaggerProperty) || IsResolvableArray(swaggerProperty) || IsResolvableMap(swaggerProperty);
        }

    }
}

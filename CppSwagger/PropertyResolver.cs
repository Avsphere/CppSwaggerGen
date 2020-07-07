using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppSwagger.DataContracts;
using Newtonsoft.Json.Linq;

namespace CppSwagger
{
    public static class PropertyResolver
    {
        private static string ConvertPrimitiveTypeToCppType(string type)
        {
            if (type == "string")
            {
                return "std::string";
            }
            else if (type == "integer")
            {
                return "int";
            }
            else if (type == "boolean")
            {
                return "bool";
            }
            else
            {
                throw new Exception($"ConvertBasicTypeToCppType did not recognize type {type}");
            }
        }

        private static string ConvertRefToCppType(string refString)
        {
            string[] refParts = refString.Split('/');
            return refParts[refParts.Length - 1];
        }

        private static PreprocessedProperty ResolveBasicType(string type, string name)
        {
            PreprocessedProperty preprocessedProperty = new PreprocessedProperty()
            {
                ResolvableSwaggerType = ResolvableSwaggerType.Primitive,
                Name = name,
                Type = ConvertPrimitiveTypeToCppType(type)
            };

            return preprocessedProperty;
        }


        private static PreprocessedProperty ResolveArray(string propertyName, JObject property)
        {
            PreprocessedProperty preprocessedProperty = new PreprocessedProperty()
            {
                Name = propertyName,
                ResolvableSwaggerType = SwaggerPropertyClassifier.GetResolvableArrayType(property)
            };

            if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.ArrayOfPrimitives)
            {
                preprocessedProperty.Type = ConvertPrimitiveTypeToCppType(property["items"]["type"].ToString());
            }
            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.ArrayArrayOfPrimitives)
            {
                preprocessedProperty.Type = ConvertPrimitiveTypeToCppType(property["items"]["items"]["type"].ToString());
            }

            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.ArrayOfRefs)
            {
                preprocessedProperty.Type = ConvertRefToCppType(property["items"]["$ref"].ToString());
            }
            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.ArrayArrayOfRefs)
            {
                preprocessedProperty.Type = ConvertRefToCppType(property["items"]["items"]["$ref"].ToString());
            }

            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.ArrayOfMapOfRefs)
            {
                preprocessedProperty.Type = ConvertRefToCppType(property["items"]["additionalProperties"]["$ref"].ToString());
            }

            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.ArrayOfPrimitiveMaps)
            {
                preprocessedProperty.Type = ConvertPrimitiveTypeToCppType(property["items"]["additionalProperties"]["type"].ToString());
            }

            else
            {
                throw new Exception($"ResolveArray cannot create preprocessed property given propertyName {propertyName} and ResolvableSwaggerType {preprocessedProperty.ResolvableSwaggerType.ToString()}");
            }

            return preprocessedProperty;
        }

        private static PreprocessedProperty ResolveMap(string propertyName, JObject property)
        {

            PreprocessedProperty preprocessedProperty = new PreprocessedProperty()
            {
                Name = propertyName,
                ResolvableSwaggerType = SwaggerPropertyClassifier.GetResolvableMapType(property)
            };

            if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.MapOfPrimitives)
            {
                preprocessedProperty.Type = ConvertPrimitiveTypeToCppType(property["additionalProperties"]["type"].ToString());
            }

            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.MapOfRefs)
            {
                preprocessedProperty.Type = ConvertRefToCppType(property["additionalProperties"]["$ref"].ToString());
            }

            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.MapOfRefArrays)
            {
                preprocessedProperty.Type = ConvertRefToCppType(property["additionalProperties"]["items"]["$ref"].ToString());
            }

            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.MapOfRefMaps)
            {
                preprocessedProperty.Type = ConvertRefToCppType(property["additionalProperties"]["additionalProperties"]["$ref"].ToString());
            }

            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.MapOfPrimitiveArrays)
            {
                preprocessedProperty.Type = ConvertPrimitiveTypeToCppType(property["additionalProperties"]["items"]["type"].ToString());
            }

            else if (preprocessedProperty.ResolvableSwaggerType == ResolvableSwaggerType.MapOfPrimitiveMaps)
            {
                preprocessedProperty.Type = ConvertPrimitiveTypeToCppType(property["additionalProperties"]["additionalProperties"]["type"].ToString());
            }

            else
            {
                throw new Exception($"ResolveMap cannot create preprocessed property given propertyName {propertyName} and ResolvableSwaggerType {preprocessedProperty.ResolvableSwaggerType.ToString()}");
            }

            return preprocessedProperty;
        }

        private static PreprocessedProperty ResolveRef(string propertyName, JObject property)
        {
            PreprocessedProperty preprocessedProperty = new PreprocessedProperty()
            {
                Name = propertyName,
                ResolvableSwaggerType = ResolvableSwaggerType.Ref,
                Type = ConvertRefToCppType(property["$ref"].ToString())
            };

            return preprocessedProperty;
        }

        public static PreprocessedProperty ResolveSimpleProperty(string propertyName, JObject resolvableProperty)
        {
            EnsureResolvable(resolvableProperty);

            if (SwaggerPropertyClassifier.IsBasicType(resolvableProperty))
            {
                return ResolveBasicType(resolvableProperty["type"].ToString(), propertyName);
            }
            else if (SwaggerPropertyClassifier.IsResolvableArray(resolvableProperty))
            {
                return ResolveArray(propertyName, resolvableProperty);
            }
            else if (SwaggerPropertyClassifier.IsResolvableMap(resolvableProperty))
            {
                return ResolveMap(propertyName, resolvableProperty);
            }
            else if (SwaggerPropertyClassifier.IsRef(resolvableProperty))
            {
                return ResolveRef(propertyName, resolvableProperty);
            }
            else
            {
                throw new Exception($"ResolveSimpleProperty did not recognize propertyName {propertyName}, property {resolvableProperty.ToString()}");
            }
        }

        public static PreprocessedProperty ResolveSimpleProperty(KeyValuePair<string, JObject> propertyPair)
        {
            return ResolveSimpleProperty(propertyPair.Key, propertyPair.Value);
        }



        private static void EnsureResolvable(JObject simpleProperty)
        {
            if (!SwaggerPropertyClassifier.IsSwaggerPropertyResolvable(simpleProperty))
            {
                throw new Exception($"EnsureSimpleResolvable could not ensure that property was resolvable: {simpleProperty.ToString()}");
            }
        }
    }
}

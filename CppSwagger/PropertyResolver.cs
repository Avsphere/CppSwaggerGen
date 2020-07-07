using System;
using System.Collections.Generic;
using System.Text;
using CppSwagger.DataContracts;
using Newtonsoft.Json.Linq;

namespace CppSwagger
{
    public static class PropertyResolver
    {
        private static string ConvertBasicTypeToCppType(string type)
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

        private static PreprocessedProperty ResolveBasicType(string type, string name)
        {
            PreprocessedProperty preprocessedProperty = new PreprocessedProperty()
            {
                EncapsulationType = EncapsulationType.None,
                Name = name,
                Type = ConvertBasicTypeToCppType(type)
            };

            return preprocessedProperty;
        }


        // IN PROGRESS
        private static PreprocessedProperty ResolveArray(string propertyName, JObject property)
        {
            PreprocessedProperty preprocessedProperty = new PreprocessedProperty()
            {
                Name = propertyName,
                EncapsulationType = EncapsulationType.Array
            };

            if (SwaggerPropertyClassifier.IsArrayOfResolvableArrays(property))
            {
                preprocessedProperty.EncapsulationType = EncapsulationType.ArrayOfArrays;
            }
            

            return preprocessedProperty;
        }

        // IN PROGRESS
        private static PreprocessedProperty ResolveMap(string propertyName, JObject property)
        {
            PreprocessedProperty preprocessedProperty = new PreprocessedProperty()
            {
                Name = propertyName,
                EncapsulationType = EncapsulationType.Map
            };

            return preprocessedProperty;
        }

        // IN PROGRESS
        private static PreprocessedProperty ResolveRef(string propertyName, JObject property)
        {
            PreprocessedProperty preprocessedProperty = new PreprocessedProperty()
            {
                Name = propertyName
            };

            return preprocessedProperty;
        }


        // IN PROGRESS
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

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CppSwagger
{

    public static class ExtractFromSwaggerJson
    {

        /*
         * Here normal is defined as a header file containing a class rather than a typedef
         * 
         */ 
        public static IList<KeyValuePair<string, JObject>> ExtractDefinitionKeyValuePairsByClassification(JObject definitions, SwaggerDefinitionClassification classification)
        {
            IList<KeyValuePair<string, JObject>> normalTopLevelObjects = new List<KeyValuePair<string, JObject>>();

            foreach (KeyValuePair<string, JToken> token in definitions)
            {
                if (SwaggerPropertyClassifier.IsNormalTopLevelSwaggerDefinition(token.Value as JObject))
                {
                    normalTopLevelObjects.Add(new KeyValuePair<string, JObject>(token.Key, token.Value as JObject));
                }
            }

            return normalTopLevelObjects;
        }

        public static IList<KeyValuePair<string, JObject>> ExtractNormalDefinitionKeyValuePairs(JObject definitions) => ExtractDefinitionKeyValuePairsByClassification(definitions, SwaggerDefinitionClassification.NormalObject);
        public static IList<KeyValuePair<string, JObject>> ExtractAbnormalDefinitionKeyValuePairs(JObject definitions) => ExtractDefinitionKeyValuePairsByClassification(definitions, SwaggerDefinitionClassification.AbnormalNeedingTypedef);
    }
}

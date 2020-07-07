using System;
using System.Collections.Generic;
using System.Text;

namespace CppSwagger.DataContracts
{
    public class PreprocessedProperty
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public ResolvableSwaggerType ResolvableSwaggerType { get; set; }
    }
}

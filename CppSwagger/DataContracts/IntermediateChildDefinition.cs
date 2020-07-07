using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CppSwagger.DataContracts
{
    // created when the parent swagger definition is being processed
    public class IntermediateChildDefinition
    {
        public string Key { get; set; } // the child object name
        public JObject ChildObject { get; set; } // the child object contents
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ParentId { get; set; } 

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CppSwagger.DataContracts
{
    public class PreprocessedSwaggerDefinition
    {
        public string TypeName { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ParentId { get; set; } // null if there is no parent
        public IList<PreprocessedProperty> ResolvedProperties { get; set; } = new List<PreprocessedProperty>();
        public IList<IntermediateChildDefinition> Children { get; set; } = new List<IntermediateChildDefinition>();

    }
}

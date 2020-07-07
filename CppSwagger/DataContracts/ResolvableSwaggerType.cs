using System;
using System.Collections.Generic;
using System.Text;

namespace CppSwagger.DataContracts
{
    public enum ResolvableSwaggerType
    {
        None,
        ArrayOfPrimitives,
        ArrayOfRefs,
        ArrayOfPrimitiveMaps,
        ArrayOfMapOfRefs,

        ArrayArrayOfPrimitives,
        ArrayArrayOfRefs,
        ArrayArrayOfPrimitiveMaps,
        ArrayArrayOfMapsOfRefs,

        MapOfPrimitives,
        MapOfRefs,
        MapOfRefArrays,
        MapOfPrimitiveArrays,

        MapOfPrimitiveMaps,
        MapOfRefMaps,

        Primitive,
        Ref
    }
}

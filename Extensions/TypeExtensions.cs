using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aquila360.Attendance.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetAllTypesInNamespace(this Type type)
        {
            return Assembly.GetAssembly(type)
                .GetExportedTypes()
                .Where(x => x.IsClass
                            && !x.IsAbstract
                            && x.Namespace != null
                            && x.Namespace.StartsWith(type.Namespace));
        }
    }
}

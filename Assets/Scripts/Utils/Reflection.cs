using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Managers;

namespace Utils
{
    /// <summary>
    /// Has some helper functions for using reflection on Unity Types.
    /// </summary>
    public static class Reflection
    {

        /// <summary>
        /// Uses Reflection to get all fields for a class.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }

        /// <summary>
        /// Uses reflection to return a field of an Object.
        /// </summary>
        /// <param name="type">The class type</param>
        /// <param name="fieldName">The field to get from the class.</param>
        /// <returns>The field as a fieldInfo. Use GetValue</returns>
        public static FieldInfo GetField(Type type, string fieldName)
        {
            var field = GetAllFields(type).ToList().First(
                fieldInfo =>
                {
                    if (fieldInfo.Name == fieldName)
                    {
                        return true;
                    }
                    return false;
                });
            return field;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SonoGame
{
    /// <summary>
    /// Query for interface types.
    /// Adapted from: https://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface
    /// </summary>
    public class TypeRegistry
    {
        private readonly string qualifiedInterfaceName;
        private readonly List<Type> typeRegistry;
        //const string namespacePrefix = "SonoGame.";
        //const string gameTypePrefix = "Game";

        public static string GetCanonicalTypeName(string typeName)
        {
            string result;

            if (typeName != null)
            {
                result = RemoveNamespaceFromType(typeName);
            }
            else
            {
                throw new Exception("Sonogame GameRegistry: typeName is null");
            }

            return result;
        }

        private static string RemoveNamespaceFromType(string typename)
        {
            var split = typename.Split('.');
            return split[split.Length - 1];

            //if (typename.IndexOf(namespacePrefix) == 0)
            //    return typename.Substring(namespacePrefix.Length);
            //else
            //    return typename;
        }

        /// <summary>
        /// Build a type registry from the current AppDomain.
        /// </summary>
        /// <param name="qualifiedInterfaceName">interface name with namespace prefix"</param>
        internal TypeRegistry(Type interfaceType)
        {
            this.qualifiedInterfaceName = interfaceType.FullName; // qualifiedInterfaceName;
            typeRegistry = new List<Type>();

            // get from current Appdomain
            typeRegistry = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
            //.Select(x => x.Name).ToList();

            //foreach (var t in typeRegistry)
            //    Debug.Log(t.Name + " (" + t.FullName + ")");
        }

        public Type GetByName(string name)
        {
            foreach (var t in typeRegistry)
            {
                if (t.Name == name || t.Name == "Game" + name)
                {
                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// Add types from assembly (DLL)
        /// </summary>
        /// <param name="assemblyFileInfo">FileInfo for assembly</param>
        public void AddFromAssembly(FileInfo assemblyFileInfo)
        {
            try
            {
                var nextAssembly = Assembly.ReflectionOnlyLoadFrom(assemblyFileInfo.FullName);
                QueryAssembly(nextAssembly);
            }
            catch (BadImageFormatException)
            {
                Debug.Log("MiniGameRegistry: Ignoring assembly (" + assemblyFileInfo.FullName + ")");
            }

        }

        public void QueryAssembly(Assembly assembly)
        {
            var interfaceFilter = new TypeFilter(InterfaceFilter);

            foreach (var type in assembly.GetTypes())
            {
                var myInterfaces = type.FindInterfaces(interfaceFilter, qualifiedInterfaceName);
                if (myInterfaces.Length > 0)
                {
                    typeRegistry.AddRange(myInterfaces);
                }
            }
        }

        public static bool InterfaceFilter(Type typeObj, System.Object criteriaObj)
        {
            return typeObj.ToString() == criteriaObj.ToString();
        }
    }
}

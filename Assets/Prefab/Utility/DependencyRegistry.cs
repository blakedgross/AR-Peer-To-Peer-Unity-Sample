using System;
using System.Collections.Generic;

namespace ARPeerToPeerSample.Utility
{
    public static class DependencyRegistry
    {
        private static Dictionary<Type, object> _dependencyMap = new Dictionary<Type, object>();

        public static void AddDependency<T>(T dependency)
        {
            _dependencyMap.Add(typeof(T), dependency);
        }

        public static T GetDependency<T>()
        {
            return (T)_dependencyMap[typeof(T)];
        }
    }
}
using System;
using System.Collections.Generic;

namespace BeardPhantom.RuntimeTypeCache.Serialized
{
    internal class CacheContext
    {
        public readonly ITypeCacheSource TypeCacheSource;

        private readonly HashSet<Type> _visitedTypes = new();

        public CacheContext(ITypeCacheSource typeCacheSource)
        {
            TypeCacheSource = typeCacheSource;
        }

        public bool TryVisit(Type type)
        {
            return _visitedTypes.Add(type);
        }
    }
}
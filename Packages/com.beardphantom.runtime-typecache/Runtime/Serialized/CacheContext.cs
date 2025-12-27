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

        /// <summary>
        /// If returns true then work needs to be done on this type. If false then work should be skipped as this type has been
        /// processed before.
        /// </summary>
        public bool TryVisitType(Type type)
        {
            return _visitedTypes.Add(type);
        }
    }
}
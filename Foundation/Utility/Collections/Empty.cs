// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;

namespace Std.Utility.Collections
{
	[PublicAPI]
	public static partial class Empty
    {
        public static readonly byte[] Bytes = Array<byte>();
        public static readonly object[] Objects = Array<object>();

        public static T[] Array<T>()
        {
            return Internal.InternalArray<T>.Instance;
        }

        public static IEnumerator<T> Enumerator<T>()
        {
            return Internal.InternalEnumerator<T>.Instance;
        }

        public static IEnumerable<T> Enumerable<T>()
        {
            return Internal.InternalList<T>.Instance;
        }

        public static ICollection<T> Collection<T>()
        {
            return Internal.InternalList<T>.Instance;
        }

        public static IList<T> List<T>()
        {
            return Internal.InternalList<T>.Instance;
        }

        public static IReadOnlyList<T> ReadOnlyList<T>()
        {
            return Internal.InternalList<T>.Instance;
        }

        public static ISet<T> Set<T>()
        {
            return Internal.InternalSet<T>.Instance;
        }

        public static IDictionary<TKey, TValue> Dictionary<TKey, TValue>()
        {
            return Internal.InternalDictionary<TKey, TValue>.Instance;
        }
    }
}

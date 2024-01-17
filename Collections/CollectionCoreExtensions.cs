namespace CommonHelpers.Collections
{
    /// <summary>
    /// The extended methods for collections
    /// </summary>
    public static class CollectionCoreExtensions
    {
        /// <summary>
        /// Put collection into dictionary by collection item key selector
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="values">The collection values</param>
        /// <param name="keySelector">The key selector</param>
        public static void PutAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TValue> values, Func<TValue, TKey> keySelector)
        {
            values.Each(value => dictionary.Put(keySelector(value), value));
        }

        /// <summary>
        /// Put collection into dictionary by collection key and value selectors 
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <typeparam name="TFinalValue">The final value type</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="values"></param>
        /// <param name="keySelector">The key selector</param>
        /// <param name="valueSelector">The value selector</param>
        public static void PutAll<TKey, TValue, TFinalValue>(this IDictionary<TKey, TFinalValue> dictionary, IEnumerable<TValue> values, Func<TValue, TKey> keySelector, Func<TValue, TFinalValue> valueSelector)
        {
            values.Each(value => dictionary.Put(keySelector(value), valueSelector(value)));
        }

        /// <summary>
        /// To dictionary by any key of the collection
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="collection">The collection</param>
        /// <param name="keySelector">The key selector</param>
        /// <returns>The dictionary by any key of the collection</returns>
        public static IDictionary<TKey, IList<TValue>> ToValueDictionary<TKey, TValue>(this IEnumerable<TValue> collection, Func<TValue, TKey> keySelector)
        {
            return collection.GroupBy(keySelector).ToDictionary(g => g.Key, g => (IList<TValue>)g.ToList());
        }

        /// <summary>
        /// Put the value inside collection by key (replace if exists)
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public static void Put<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            // if value is already there, just replace
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return;
            }

            // if key is not defined yet, add new value
            dictionary.Add(key, value);
        }

        /// <summary>
        /// Put the value inside collection by key if absent
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>The put value if the key is absent or the existing one in other cases</returns>
        public static TValue PutIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.TryGetValue(key, out TValue existing))
            {
                dictionary.Add(key, value);

                return value;
            }

            return existing;
        }

        /// <summary>
        /// Put the value inside collection if absent
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="collection">The collection</param>
        /// <param name="value">The value</param>
        /// <returns>The value</returns>
        public static TValue PutIfAbsent<TValue>(this ICollection<TValue> collection, TValue value)
        {
            if (!collection.Contains(value))
            {
                collection.Add(value);
            }

            return value;
        }

        /// <summary>
        /// Get the value from collection by key if exists or get default
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The value from the collection by key if exists or default value</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            // try get existing value
            return dictionary.TryGetValue(key, out var existing) ? existing : defaultValue;
        }

        /// <summary>
        /// Get the first value from collection by key if exists or get default
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The first value from the collection by key if exists or default value</returns>
        public static TValue GetFirstOrDefault<TKey, TValue>(this IDictionary<TKey, IEnumerable<TValue>> dictionary, TKey key, TValue defaultValue = default)
        {
            // try get existing value
            return dictionary.TryGetValue(key, out var existing) ? existing.FirstOrDefault() : defaultValue;
        }

        /// <summary>
        /// Get the value from collection by key if exists or get null
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <returns>The value from the collection by key if exists or null</returns>
        public static TValue GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            return GetOrDefault(dictionary, key);
        }

        /// <summary>
        /// Put all the values inside collection by key (replace if exists)
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="other">The other collection</param>
        public static void PutRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other)
        {
            // safety check
            if (other == null || other.Count == 0)
            {
                return;
            }

            // for each key value pair 
            foreach (var kv in other)
            {
                Put(dictionary, kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// Traverse all the elements of given dictionary
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="action">The action to apply</param>
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<KeyValuePair<TKey, TValue>> action)
        {
            // safety check
            if (action == null || dictionary == null)
            {
                return;
            }

            // for each key value pair 
            foreach (var kv in dictionary)
            {
                action.Invoke(kv);
            }
        }

        /// <summary>
        /// Traverse all the elements of given collection
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="collection">The target collection</param>
        /// <param name="action">The action to apply</param>
        public static void Each<TValue>(this IEnumerable<TValue> collection, Action<TValue> action)
        {
            // safety check
            if (action == null || collection == null)
            {
                return;
            }

            // for each key value pair 
            foreach (var item in collection)
            {
                action.Invoke(item);
            }
        }

        /// <summary>
        /// Add to value collection by key
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public static void AddValue<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dictionary, TKey key, TValue value)
        {
            // if collection exists add and finish
            if (dictionary.TryGetValue(key, out var collection))
            {
                collection.Add(value);
                return;
            }

            // if no collection yet create it
            dictionary.Add(key, new List<TValue> { value });
        }

        /// <summary>
        /// Add to value collection by key
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <typeparam name="TSecKey">The second key</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="sec">The second key</param>
        /// <param name="value">The value</param>
        public static void AddValue<TKey, TSecKey, TValue>(this IDictionary<TKey, IDictionary<TSecKey, TValue>> dictionary, TKey key, TSecKey sec, TValue value)
        {
            // if collection exists add and finish
            if (dictionary.TryGetValue(key, out var collection))
            {
                Put(collection, sec, value);
                return;
            }

            // if no collection yet create it
            dictionary.Add(key, new Dictionary<TSecKey, TValue> { { sec, value } });
        }

        /// <summary>
        /// Add to value collection by key
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <typeparam name="TSecKey">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <param name="sec">The value</param>
        public static void AddValue<TKey, TSecKey, TValue>(this IDictionary<TKey, IDictionary<TSecKey, IList<TValue>>> dictionary, TKey key, TSecKey sec, TValue value)
        {
            // if collection exists add and finish
            if (dictionary.TryGetValue(key, out var collection))
            {
                AddValue(collection, sec, value);
                return;
            }

            // if no collection yet create it
            dictionary.Add(key, new Dictionary<TSecKey, IList<TValue>> { { sec, new List<TValue> { value } } });
        }

        /// <summary>
        /// Check if value is in collection by key
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>True if value is in collection by key and false in other case</returns>
        public static bool ContainsValue<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dictionary, TKey key, TValue value)
        {
            // check if collection present and item is in collection
            return dictionary.TryGetValue(key, out var collection) && collection.Contains(value);
        }

        /// <summary>
        /// Add to value collection by key
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public static void AddValue<TKey, TValue>(this IDictionary<TKey, ISet<TValue>> dictionary, TKey key, TValue value)
        {
            // if collection exists add and finish
            if (dictionary.TryGetValue(key, out var collection))
            {
                collection.Add(value);
                return;
            }

            // if no collection yet create it
            dictionary.Add(key, new HashSet<TValue> { value });
        }

        /// <summary>
        /// Add to value collection by key
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="values">The values</param>
        public static void AddValues<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dictionary, TKey key, IEnumerable<TValue> values)
        {
            values.Each(value => AddValue(dictionary, key, value));
        }

        /// <summary>
        /// Add to value collection by key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="other"></param>
        public static void AddValues<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dictionary, IDictionary<TKey, IList<TValue>> other)
        {
            other.Each(kv => AddValues(dictionary, kv.Key, kv.Value));
        }

        /// <summary>
        /// Check if value is in collection by key
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>True if value is in collection by key and false in other case</returns>
        public static bool ContainsValue<TKey, TValue>(this IDictionary<TKey, ISet<TValue>> dictionary, TKey key, TValue value)
        {
            // check if collection present and item is in collection
            return dictionary.TryGetValue(key, out var collection) && collection.Contains(value);
        }

        /// <summary>
        /// Check if value is in dictionary by key
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="dictionary">The target collection</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>True if value is in dictionary by key and false in other case</returns>
        public static bool ContainsValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            // check if item is in dictionary
            return dictionary.TryGetValue(key, out var existingValue) && !existingValue.Equals(null) && existingValue.Equals(value);
        }

        /// <summary>
        /// Flattens sequence of sequences of the same element type
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="enumerables">The sequence of sequences</param>
        /// <returns>The flattens sequence of sequences of the same element type</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerables)
        {
            return enumerables.Aggregate((first, second) => first.Concat(second));
        }

        /// <summary>
        /// Add range of elements to given collection
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="collection">The target collection</param>
        /// <param name="items">The items to add</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }
                collection.Add(item);
            }
        }

        /// <summary>
        /// Checks if collection has items
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="collection">The collection to check</param>
        /// <returns>True if collection isn't null and has items and False in other case</returns>
        public static bool HasItems<TValue>(this ICollection<TValue> collection)
        {
            return collection != null && collection.Count > 0;
        }

        /// <summary>
        /// Checks if collection is null or empty
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="collection">The collection to check</param>
        /// <returns>True if collection is null or empty and False in other case</returns>
        public static bool NoItems<TValue>(this ICollection<TValue> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// Checks if enumerable has items
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="collection">The enumerable to check</param>
        /// <returns>True if enumerable isn't null and has items and False in other case</returns>
        public static bool HasItems<TValue>(this IEnumerable<TValue> collection)
        {
            return collection != null && collection.Any();
        }

        /// <summary>
        /// Checks if collection is null or empty
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="collection">The collection to check</param>
        /// <returns>True if collection is null or empty and False in other case</returns>
        public static bool NoItems<TValue>(this IEnumerable<TValue> collection)
        {
            if (collection == null)
            {
                return true;
            }

            return !collection.Any();
        }

        /// <summary>
        /// Get enumerable from values
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="items">The items</param>
        /// <returns>The enumerable from values</returns>
        public static IEnumerable<T> Of<T>(params T[] items)
        {
            return items;
        }

        /// <summary>
        /// Get an empty enumerable
        /// </summary>
        /// <typeparam name="T">Type of enumerable</typeparam>
        /// <returns>An empty enumerable</returns>
        public static IEnumerable<T> Of<T>()
        {
            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Converts an enum to a dictionary of integer key/string value pairs.
        /// </summary>
        /// <param name="enum">The Enum to convert.</param>
        /// <returns>Returns a dictionary implementation of an Enum list.</returns>
        public static IDictionary<int, string> ToDictionary(this Enum @enum)
        {
            return Enum.GetValues(@enum.GetType()).Cast<object>().ToDictionary(Convert.ToInt32, item => item.ToString());
        }
    }
}

namespace CommonExtensions
{
    /// <summary>
    /// Set of extension methods for objects in general
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Try cast given object to a given type
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="source">The object to cast</param>
        /// <param name="result">[out] The result of cast</param>
        /// <returns>True if cast is successful</returns>
        public static bool TryCast<T>(this object source, out T result) where T : class
        {
            // default value
            result = default;

            // if not of type T
            if (source is not T tSource)
            {
                return false;
            }

            // cast 
            result = tSource;

            // successful
            return true;
        }

        /// <summary>
        /// Cast value to the given type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="value">The value</param>
        /// <returns>The casted value</returns>
        public static T As<T>(this object value)
        {
            return value is not T tValue ? default : tValue;
        }

        /// <summary>
        /// Check value is of type T
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="value">The value</param>
        /// <returns>True if value is of type T and False in other case</returns>
        public static bool Is<T>(this object value)
        {
            return value is T;
        }

        /// <summary>
        /// Check value is not of type T
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="value">The value</param>
        /// <returns>True if value is not of type T and False in other case</returns>
        public static bool IsNot<T>(this object value)
        {
            return !Is<T>(value);
        }

        /// <summary>
        /// Check if values contains the target
        /// </summary>
        /// <typeparam name="TEnum">Target type</typeparam>
        /// <param name="target">The target</param>
        /// <param name="values">The values</param>
        /// <returns>True if values contains the target and False in other case</returns>
        public static bool OneOf<TEnum>(this TEnum target, params TEnum[] values) where TEnum : Enum
        {
            return values.Any(v => v.Equals(target));
        }

        /// <summary>
        /// Check if values contains the target
        /// </summary>
        /// <typeparam name="TEnum">Target type</typeparam>
        /// <param name="target">The target</param>
        /// <param name="values">The values</param>
        /// <returns>True if values contains the target and False in other case</returns>
        public static bool OneOf<TEnum>(this TEnum target, IEnumerable<TEnum> values) where TEnum : Enum
        {
            return values.Any(v => v.Equals(target));
        }

        /// <summary>
        /// Makes enumerable from Enum
        /// </summary>
        /// <typeparam name="TEnum">Target type</typeparam>
        /// <returns>Returns Key value representation of given Enum</returns>
        public static IEnumerable<object> ToEnumerable<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>().Select(value => (object)new
            {
                Key = Convert.ToInt32(value),
                Value = value.ToString()
            });
        }

        /// <summary>
        /// Gets the enum values of the given type.
        /// </summary>
        /// <typeparam name="TValue">The enum value type</typeparam>
        /// <returns></returns>
        public static TValue[] GetEnumValues<TValue>() where TValue : Enum
        {
            return Enum.GetValues(typeof(TValue)).Cast<TValue>().ToArray();
        }
    }
}

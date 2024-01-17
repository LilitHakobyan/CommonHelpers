using Newtonsoft.Json;

namespace CommonExtensions.Serialization
{
    public static class Json
    {
        /// <summary>
        /// Serialize generic object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
                // in line serialize use Formatting.None
            }
            catch(Exception) 
            {
                // failed
                return null;
            }
        }

        /// <summary>
        /// Deserialize using json path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string path)
        {
            try
            {
                // create stream reader
                using StreamReader sr = new StreamReader(path);

                // read json 
                var json  = sr.ReadToEnd();

                // deserialize json to given type
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// Deserialize using stream reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T Deserialize<T>(StreamReader reader)
        {
            try
            {
                // read json 
                var json = reader.ReadToEnd();

                // deserialize json to given type
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}

using System.Text;

namespace CommonExtensions
{
    /// <summary>
    /// Helpers to work with assemblies 
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the file content from assembly.
        /// </summary>
        /// <param name="assembly">The assembly to load.</param>
        /// <param name="file">The file to get content from.</param>
        /// <returns>The file content from assembly.</returns>
        public static string GetEmbeddedResource(string assembly, string file)
        {
            // load assembly
            var assemblyInstance = System.Reflection.Assembly.Load(assembly);

            // get resource stream and data 
            var templateStream = assemblyInstance.GetManifestResourceStream(file);
            if (templateStream == null)
            {
                throw new FileLoadException("Error while loading the file.");
            }

            var data = new BinaryReader(templateStream).ReadBytes((int)templateStream.Length);

            // close the stream
            templateStream.Close();

            return Encoding.UTF8.GetString(data);
        }
    }
}

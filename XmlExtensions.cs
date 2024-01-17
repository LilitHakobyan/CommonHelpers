using System.Xml;
using System.Xml.Linq;

namespace CommonExtensions
{
    /// <summary>
    /// Set of extension methods for Xml documents
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Get attribute values in dictionary for the Xml Element
        /// </summary>
        /// <param name="element">The target element</param>
        /// <returns>The  attribute values in dictionary for the Xml Element</returns>
        public static Dictionary<string, string> GetAttributeValues(this XElement element)
        {
            return element.Attributes().ToDictionary(attr => attr.Name.LocalName.ToUpper(), attr => attr.Value);
        }

        /// <summary>
        /// Converts Xml document to x document 
        /// </summary>
        /// <param name="xmlDocument">Xml document</param>
        /// <returns>The converted x document</returns>
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using var nodeReader = new XmlNodeReader(xmlDocument);
            nodeReader.MoveToContent();
            return XDocument.Load(nodeReader);
        }

        /// <summary>
        /// Gets scalar attribute values in dictionary for the Xml Element
        /// </summary>
        /// <param name="element">The target element</param>
        /// <returns>The scalar attribute values in dictionary for the Xml Element</returns>
        public static Dictionary<string, string> GetScalarAttributeValues(this XElement element)
        {
            var attrs = new Dictionary<string, string>();

            element.Elements().Each(childElement =>
            {
                if (childElement.HasElements || childElement.HasAttributes || childElement.IsEmpty)
                {
                    return;
                }

                attrs.Put(childElement.Name.LocalName.ToUpper(), childElement.Value);
            });

            return attrs;
        }
    }
}

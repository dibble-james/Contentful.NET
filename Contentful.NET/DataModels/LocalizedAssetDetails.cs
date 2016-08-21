using System.Collections.Generic;

namespace Contentful.NET.DataModels
{
    /// <summary>
    /// Describes the details for a given Contentful <see cref="Asset"/> in multiple languages.
    /// </summary>
    public class LocalizedAssetDetails
    {
        /// <summary>
        /// The title for the asset
        /// </summary>
        public IReadOnlyDictionary<string, string> Title { get; set; }
        /// <summary>
        /// A short description for the asset
        /// </summary>
        public IReadOnlyDictionary<string, string> Description { get; set; }
        /// <summary>
        /// The file details for the given asset
        /// </summary>
        public IReadOnlyDictionary<string, File> File { get; set; }
    }
}

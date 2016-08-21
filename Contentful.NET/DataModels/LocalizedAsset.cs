

using Newtonsoft.Json;

namespace Contentful.NET.DataModels
{
    /// <summary>
    /// Describes a Contentful Asset who's fields are in multiple languages.
    /// </summary>
    /// <see cref="https://www.contentful.com/developers/documentation/content-delivery-api/#assets"/>
    public class LocalizedAsset : ContentfulItemBase, ILocalizedContentfulItem
    {
        /// <summary>
        /// The details object for the given asset
        /// </summary>
        [JsonProperty("fields")]
        public LocalizedAssetDetails Details { get; set; }
    }
}

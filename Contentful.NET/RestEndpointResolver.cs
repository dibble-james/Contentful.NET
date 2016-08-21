using System;
using System.Collections.Generic;
using Contentful.NET.DataModels;

namespace Contentful.NET
{
    /// <summary>
    /// Internal class which handles resolving the URLs to the Contentful REST endpoints for 
    /// each individual ContentItem type
    /// </summary>
    internal static class RestEndpointResolver
    {
		private const string ContentfulProductionHost = "cdn";
		private const string ContentfulPreviewHost = "preview";

        // Base formatted URL to the Contentful CDN
		private const string ContentfulApiBase = "https://{0}.contentful.com/spaces/{1}";

        // Hardcoded mapping of IContentItems which have a corresponding endpoint URL
        private static readonly Dictionary<Type, string> EndpointDictionary = new Dictionary<Type, string>
        {
            { typeof(Asset), "/assets/"},
            { typeof(ContentType), "/content_types/"},
            { typeof(Entry), "/entries/"},
            { typeof(Space), ""}
        };

        // Hardcoded mapping of IContentItems which have a corresponding type name
        private static readonly Dictionary<Type, string> ContentfulItemTypeNameDictionary = new Dictionary<Type, string>
        {
            { typeof(Asset), "Asset"},
            { typeof(LocalizedAsset), "Asset"},
            { typeof(Entry), "Entrty"}
        };

        /// <summary>
        /// Gets the endpoint for a given IContentfulItem type
        /// </summary>
        /// <typeparam name="T">The type of request being made</typeparam>
        /// <param name="space">The ID of the Space used to generate the endpoint URL</param>
		/// <param name="preview">Whether to use the preview API, false by default</param>
        /// <returns>A generated URL to the REST Endpoint represented by T</returns>
		internal static string GetEndpointUrl<T>(string space, bool preview = false) where T : IContentfulItem
        {
			string host = preview ? ContentfulPreviewHost : ContentfulProductionHost;
            return string.Format(ContentfulApiBase + EndpointDictionary[typeof (T)], host, space);
        }

        /// <summary>
        /// Gets the name of the Contentful item type to use in a URI filter..
        /// </summary>
        /// <typeparam name="T">The type of Contentful item.</typeparam>
        /// <returns>The item type name.</returns>
        internal static string GetContentfulItemTypeName<T>() where T : IContentfulItem
        {
            return ContentfulItemTypeNameDictionary[typeof(T)];
        }
    }
}

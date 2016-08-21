using System;
using System.Collections.Generic;

namespace Contentful.NET.DataModels
{
    /// <summary>
    /// Representation of a response from a synchronization query of the Contentful API
    /// </summary>
    /// <typeparam name="T">The type of items as returned in the "items" field</typeparam>
    public class SynchronizationResult<T> where T : ILocalizedContentfulItem
    {
        /// <summary>
        /// An enumerable of all items returned by the query
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets or sets the sync token url.
        /// </summary>
        public string NextSyncUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL of the next set of results of this sync operation.
        /// </summary>
        public string NextPageUrl { get; set; }

        /// <summary>
        /// Extracts the sync token from the <see cref="NextSyncUrl"/> if one is present.
        /// </summary>
        public string SyncToken
        {
            get
            {
                if (string.IsNullOrEmpty(this.NextSyncUrl))
                {
                    return null;
                }

                return new Uri(this.NextSyncUrl).Query.Split('=')[1];
            }
        }

        /// <summary>
        /// Gets a value indicating whether this sync result was paged.
        /// </summary>
        public bool HasMoreResults => !string.IsNullOrEmpty(this.NextPageUrl);
    }
}
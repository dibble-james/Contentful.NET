using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contentful.NET.DataModels;
using Contentful.NET.Search;
using Contentful.NET.Search.Enums;

namespace Contentful.NET
{
    /// <summary>
    /// Contract for a Contentful HTTP Client
    /// </summary>
    public interface IContentfulClient
    {
        /// <summary>
        /// Retrieve a single IContentItem by specifying its ID.
        /// </summary>
        /// <typeparam name="T">The type of item expected to be returned</typeparam>
        /// <param name="cancellationToken">The cancellation token for the request</param>
        /// <param name="id">The ID of the content item</param>
        /// <returns>A new instance of the type T, populated with the data returned by Contentful</returns>
        /// <exception cref="ContentfulException">Thrown if the request to Contentful returned a status code other than 200 (OK)</exception>
        Task<T> GetAsync<T>(CancellationToken cancellationToken, string id)
            where T : IContentfulItem, new();

        /// <summary>
        /// Search for a range of IContentItems, given the provided filters & sorting
        /// </summary>
        /// <typeparam name="T">The type of item expected to be returned</typeparam>
        /// <param name="cancellationToken">The cancellation token for the request</param>
        /// <param name="searchFilters">(Optional) A collection of ISearchFilter implementations which specify what content to retrieve from the space</param>
        /// <param name="orderByProperty">(Optional) The name of the property to order the results on</param>
        /// <param name="orderByDirection">(Optional) The direction in which to sort the results</param>
        /// <param name="skip">(Optional) The number of results to skip</param>
        /// <param name="limit">(Optional) The number of results to return</param>
        /// <param name="includeLevels">(Optional) The number of levels of included Content to return</param>
        /// <returns>A SearchResult object containing the content items, a list of includes (if applicable) and the total number of results for the query</returns>
        /// <exception cref="ContentfulException">Thrown if the request to Contentful returned a status code other than 200 (OK)</exception>
        Task<SearchResult<T>> SearchAsync<T>(CancellationToken cancellationToken,
            IEnumerable<ISearchFilter> searchFilters = null, string orderByProperty = null,
            OrderByDirection? orderByDirection = null, int? skip = null, int? limit = null, int? includeLevels = null)
            where T : IContentfulItem, new();

        /// <summary>
        /// Perform an initial synchronization of the space for the given type. (Not supported by preview api).
        /// </summary>
        /// <typeparam name="T">The type of content to sync.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The content that was synchronized.</returns>
        /// <exception cref="System.NotSupportedException">An attempt was made to sync using a preview instance.</exception>
        Task<SynchronizationResult<T>> InitialSyncAsync<T>(CancellationToken cancellationToken) where T : ILocalizedContentfulItem, new();

        /// <summary>
        /// Use a synchronization token to get updated content.
        /// </summary>
        /// <typeparam name="T">The type of content to sync.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="syncToken">The synchronize token.</param>
        /// <returns>The content that was synchronized.</returns>
        /// <exception cref="System.NotSupportedException">An attempt was made to sync using a preview instance.</exception>
        Task<SynchronizationResult<T>> SyncAsync<T>(CancellationToken cancellationToken, string syncToken) where T : ILocalizedContentfulItem, new();

        /// <summary>
        /// Follow the <see cref="SynchronizationResult{T}.NextPageUrl"/>.
        /// </summary>
        /// <typeparam name="T">The type of content to sync.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="syncResult">The synchronize result.</param>
        /// <returns>The content that was synchronized.</returns>
        /// <exception cref="System.NotSupportedException">An attempt was made to sync using a preview instance.</exception>
        Task<SynchronizationResult<T>> GetNextSyncResultAsync<T>(CancellationToken cancellationToken, SynchronizationResult<T> syncResult) where T : ILocalizedContentfulItem, new();
    }
}

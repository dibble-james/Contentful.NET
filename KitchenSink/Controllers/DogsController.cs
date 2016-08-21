﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Contentful.NET;
using Contentful.NET.DataModels;
using Contentful.NET.Search;
using Contentful.NET.Search.Filters;
using KitchenSink.Models.Dogs;
using Newtonsoft.Json;

namespace KitchenSink.Controllers
{
    public class DogsController : AsyncController
    {
        // Injected by Unity App_Start/UnityConfig.cs
        private readonly IContentfulClient _contentfulClient;

        public DogsController(IContentfulClient contentfulClient)
        {
            _contentfulClient = contentfulClient;
        }

        // GET: All Dogs
        public async Task<ActionResult> AllAsync(CancellationToken cancellationToken)
        {

            var results = await _contentfulClient.SearchAsync<Entry>(cancellationToken, new[]
            {
                // Only search for the 'Dog' content type
                new EqualitySearchFilter(BuiltInProperties.ContentType, "3KzwWGrzry422cEkMCA2o6"),
            },
            includeLevels: 1 // Ensure we retrieve the linked assets inside this one request - we want to get the Images for the dogs too
            );

            return View(GetAllDogsFromContentfulResult(results));
        }

        [HttpPost]
        public async Task<ActionResult> SearchAsync(string criteria, CancellationToken cancellationToken)
        {
            var results = await _contentfulClient.SearchAsync<Entry>(cancellationToken, new ISearchFilter[]
            {
                // Only search for the 'Dog' content type
                new EqualitySearchFilter(BuiltInProperties.ContentType, "3KzwWGrzry422cEkMCA2o6"),
                new FullTextSearchFilter(criteria)
            },
            includeLevels: 1 // Ensure we retrieve the linked assets inside this one request - we want to get the Images for the dogs too
            );
            ViewBag.FromSearch = true;
            ViewBag.Criteria = criteria;
            return View("All", GetAllDogsFromContentfulResult(results));
        }
        
        // GET: /dogs/sync{?syncToken=}
        public async Task<ActionResult> SyncAsync(CancellationToken cancellationToken, string syncToken)
        {
            SynchronizationResult<LocalizedAsset> result;

            if (string.IsNullOrEmpty(syncToken))
            {
                result = await _contentfulClient.InitialSyncAsync<LocalizedAsset>(cancellationToken);
            }
            else
            {
                result = await this._contentfulClient.SyncAsync<LocalizedAsset>(cancellationToken, syncToken);
            }

            var items = result.Items;

            while (result.HasMoreResults)
            {
                result = await _contentfulClient.GetNextSyncResultAsync(cancellationToken, result);

                items.Concat(result.Items);
            }

            return Content(JsonConvert.SerializeObject(new { items, result.NextSyncUrl }), "application/json");
        }

        private static All GetAllDogsFromContentfulResult(SearchResult<Entry> results)
        {
            return new All
            {
                DogsItems = results.Items
                    // Retrieve the ImageId from the linked 'mainPicture' asset
                    // NOTE: We could merge all of these Select() statements into one, but this way we only have to call dog.GetLink() and dog.GetString()
                    //       once, which improves performance.
                    .Select(dog => new
                    {
                        dog.SystemProperties.Id,
                        ImageId = dog.GetLink("mainPicture").SystemProperties.Id,
                        Type = dog.GetString("dogType")
                    })
                    // Now find the included 'Asset' details from the corresponding ImageId
                    .Select(dog => new
                    {
                        dog.Id,
                        ImageUrl =
                            results.Includes.Assets.First(asset => asset.SystemProperties.Id == dog.ImageId)
                                .Details.File.Url,
                        dog.Type
                    })
                    // Now we map our calculated data to our model
                    .Select(dog => new DogItem
                    {
                        Id = dog.Id,
                        LargeImageUrl =
                            ImageHelper.GetResizedImageUrl(dog.ImageUrl, 500, 500, ImageHelper.ImageType.Jpg, 75),
                        ThumbnailImageUrl =
                            ImageHelper.GetResizedImageUrl(dog.ImageUrl, 150, 150, ImageHelper.ImageType.Png),
                        Type = dog.Type
                    })
            };
        }

        public async Task<ActionResult> DetailAsync(string id, CancellationToken cancellationToken)
        {
            var searchResults = await _contentfulClient.SearchAsync<Entry>(cancellationToken, new[]
            {
                // Only search for the ID of this dog
                new EqualitySearchFilter(BuiltInProperties.SysId, id),
            }, includeLevels: 1); // We COULD do a Get instead of a Search here, but only a Search includes linked assets
            if (searchResults.Total == 0) return HttpNotFound();
            var result = searchResults.Items.First();
            // Retrieve any linked pictures
            var allPictures = result.GetType<IEnumerable<Link>>("otherPictures");
            var model = new SingleDogItem
            {
                // Retrieve the necessary data from the Entry
                Type = result.GetString("dogType"),
                AverageCost = result.GetType<decimal>("averageCost"),
                Birthday = result.GetDateTime("birthDate"),
                Id = result.SystemProperties.Id,
                IsMale = result.GetBoolean("isMale") == true,
                NumberAvailable = result.GetType<int>("numberAvailable"),
                // Get a thumbnail AND a large image for each picture
                Pictures = allPictures == null ? null : allPictures
                    .Select(pic => new { pic.SystemProperties.Id })
                    .Select(
                        pic =>
                            new
                            {
                                ImageUrl =
                                    searchResults.Includes.Assets.First(asset => asset.SystemProperties.Id == pic.Id)
                                        .Details.File.Url
                            })
                    .Select(pic => new SingleDogItemPicture
                    {
                        LargeImageUrl =
                            ImageHelper.GetResizedImageUrl(pic.ImageUrl, 500, 500, ImageHelper.ImageType.Jpg, 75),
                        ThumbnailImageUrl =
                            ImageHelper.GetResizedImageUrl(pic.ImageUrl, 150, 150)
                    })
                /* If we don't care about resizing images, it becomes a bit easier:
                 * 
                 * Pictures = allPictures.Select(pic => new
                 * {
                 *     ImageUrl = searchResults.Includes.Assets.First(asset => asset.SystemProperties.Id == pic.SystemProperties.Id).Details.File.Url
                 * })
                 * 
                 */
            };

            return View(model);
        }
    }
}
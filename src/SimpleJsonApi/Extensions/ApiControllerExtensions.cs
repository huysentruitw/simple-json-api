using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Extensions
{
    /// <summary>
    /// SimpleJsonApi ApiController extension methods.
    /// </summary>
    public static class ApiControllerExtensions
    {
        /// <summary>
        /// Add a custom link to the JSON API response.
        /// </summary>
        /// <param name="controller">The <see cref="ApiController"/>.</param>
        /// <param name="name">The name of the link (self links are automatically generated).</param>
        /// <param name="uri">The URI of the link.</param>
        public static void AddResourceLink(this ApiController controller, string name, Uri uri)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            controller.Request.AddResourceLink(name, uri);
        }

        /// <summary>
        /// Add a custom link to the JSON API response.
        /// </summary>
        /// <param name="controller">The <see cref="ApiController"/>.</param>
        /// <param name="name">The name of the link (self links are automatically generated).</param>
        /// <param name="uri">The URI of the link.</param>
        /// <param name="metadata">The metadata of the link.</param>
        public static void AddResourceLink(this ApiController controller, string name, Uri uri, object metadata)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            controller.Request.AddResourceLink(name, uri, metadata);
        }

        internal static void AddResourceLink(this HttpRequestMessage request, string name, Uri uri, object metadata = null)
        {
            request.GetResourceLinksSet().Add(new ResourceLink
            {
                Name = name,
                Uri = uri,
                Metadata = metadata
            });
        }

        internal static ISet<ResourceLink> GetResourceLinksSet(this HttpRequestMessage request)
        {
            var properties = request.Properties;
            if (!properties.ContainsKey(Constants.Properties.ResourceLinks))
                properties.Add(Constants.Properties.ResourceLinks, new HashSet<ResourceLink>());
            return properties[Constants.Properties.ResourceLinks] as ISet<ResourceLink>;
        }
    }
}

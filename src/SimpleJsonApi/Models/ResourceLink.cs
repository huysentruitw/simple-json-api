using System;

namespace SimpleJsonApi.Models
{
    internal class ResourceLink
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }
        public object Metadata { get; set; }
    }
}

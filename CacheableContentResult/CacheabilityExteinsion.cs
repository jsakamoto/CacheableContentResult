using System;
using System.Web;
using System.Web.Mvc;

namespace Toolbelt.Web
{
    using Context = ControllerContext;
    using Cacheability = HttpCacheability;
    internal static class CacheabilityExteinsion
    {
        public static string ToCacheControlString(this Cacheability value)
        {
            switch (value)
            {
                case Cacheability.NoCache:
                case Cacheability.Server:
                    return "no-cache";
                case Cacheability.Private:
                case Cacheability.ServerAndPrivate:
                    return "private";
                case Cacheability.Public:
                    return "public";
                default:
                    throw new ArgumentException($"Unknown {typeof(Cacheability).Name} value: {value}");
            }
        }
    }
}

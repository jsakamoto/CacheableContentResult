using System;

namespace Toolbelt.Web
{
#if !NETSTANDARD
    using Context = System.Web.Mvc.ControllerContext;
    using Cacheability = System.Web.HttpCacheability;
#else
    using Context = Microsoft.AspNetCore.Mvc.ActionContext;
    using Cacheability = Microsoft.AspNetCore.Mvc.ResponseCacheLocation;
#endif

    internal static class CacheabilityExteinsion
    {
        public static string ToCacheControlString(this Cacheability value)
        {
#if !NETSTANDARD
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
#else
            switch (value)
            {
                case Cacheability.Any:
                    return "public";
                case Cacheability.Client:
                    return "private";
                case Cacheability.None:
                    return "no-cache";
                default:
                    throw new ArgumentException($"Unknown {typeof(Cacheability).Name} value: {value}");
            }
#endif
        }
    }
}

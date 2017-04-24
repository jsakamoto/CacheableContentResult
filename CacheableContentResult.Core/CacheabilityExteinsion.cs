using System;
using Microsoft.AspNetCore.Mvc;

namespace Toolbelt.Web
{
    using Context = ActionContext;
    using Cacheability = ResponseCacheLocation;

    internal static class CacheabilityExteinsion
    {
        public static string ToCacheControlString(this Cacheability value)
        {
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
        }
    }
}

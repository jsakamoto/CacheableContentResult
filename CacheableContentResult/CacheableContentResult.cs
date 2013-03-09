using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Toolbelt.Web
{
    public class CacheableContentResult : ActionResult
    {
        private enum CacheHint
        {
            None,
            Hit,
            Miss
        }

        public string ContentType { get; set; }

        public Func<byte[]> GetContent { get; set; }

        public DateTime? LastModified { get; set; }

        public string ETag { get; set; }

        public HttpCacheability? Cacheability { get; set; }

        public CacheableContentResult(string contentType, Func<byte[]> getContent, DateTime? lastModified = null, string etag = null, HttpCacheability? cacheability = null)
        {
            this.ContentType = contentType;
            this.GetContent = getContent;
            this.LastModified = lastModified;
            this.ETag = etag ?? "";
            this.Cacheability = cacheability;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            response.ContentType = this.ContentType;
            if (this.Cacheability.HasValue)
                response.Cache.SetCacheability(this.Cacheability.Value);

            if (this.LastModified.HasValue)
                response.Cache.SetLastModified(this.LastModified.Value);
            if (string.IsNullOrWhiteSpace(this.ETag) == false)
                response.Cache.SetETag(this.ETag);

            var cacheHintByDate = CacheHint.None;
            var ifModSinceStr = request.Headers["If-Modified-Since"];
            var ifModSince = default(DateTime);
            if (this.LastModified.HasValue && DateTime.TryParse(ifModSinceStr, out ifModSince))
            {
                cacheHintByDate = (this.LastModified.Value - ifModSince).Seconds <= 0 ? CacheHint.Hit : CacheHint.Miss;
            }

            var cacheHintByETag = CacheHint.None;
            var ifNoneMatch = request.Headers["If-None-Match"];
            if (!string.IsNullOrWhiteSpace(this.ETag) && !string.IsNullOrWhiteSpace(ifNoneMatch))
            {
                cacheHintByETag = this.ETag == ifNoneMatch ? CacheHint.Hit : CacheHint.Miss;
            }

            var cacheHints = new[] { cacheHintByDate, cacheHintByETag };
            if (cacheHints.Contains(CacheHint.Hit) && !cacheHints.Contains(CacheHint.Miss))
            {
                response.StatusCode = 304; // HTTP 304 Not Modified
            }
            else
            {
                response.BinaryWrite(this.GetContent());
            }
        }
    }
}
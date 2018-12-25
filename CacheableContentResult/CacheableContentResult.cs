using System;
using System.Linq;

namespace Toolbelt.Web
{
#if !NETSTANDARD
    using System.Web;
    using System.Web.Mvc;
    using Context = System.Web.Mvc.ControllerContext;
    using Cacheability = System.Web.HttpCacheability;
#else
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Context = Microsoft.AspNetCore.Mvc.ActionContext;
    using Cacheability = Microsoft.AspNetCore.Mvc.ResponseCacheLocation;
#endif

    public class CacheableContentResult : ActionResult
    {
        private enum CacheHint
        {
            None,
            Hit,
            Miss
        }

        public string ContentType { get; set; }

        /// <summary>
        /// Get or set the function that return content bytes, called lazy if needed.
        /// <para>If this function returned null, then this action result respond HTTP 404 Not Found status.</para>
        /// </summary>
        public Func<byte[]> GetContent { get; set; }

        public DateTime? LastModified { get; set; }

        public string ETag { get; set; }

        public Cacheability? Cacheability { get; set; }

        private CacheableContentResult(string contentType, DateTime? lastModified, string etag, Cacheability? cacheability)
        {
            this.ContentType = contentType;
            this.LastModified = lastModified;
            this.ETag = etag ?? "";
            this.Cacheability = cacheability;
        }

        public CacheableContentResult(string contentType, Func<byte[]> getContent, DateTime? lastModified = null, string etag = null, Cacheability? cacheability = null)
            : this(contentType, lastModified, etag, cacheability)
        {
            this.GetContent = getContent;
        }

        private bool RespondNotModified(Context context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            response.ContentType = this.ContentType;
            if (this.Cacheability.HasValue)
                response.Headers.Add("Cache-Control", this.Cacheability.Value.ToCacheControlString());
            if (this.LastModified.HasValue)
                response.Headers.Add("Last-Modified", this.LastModified.Value.ToUniversalTime().ToString("R"));
            if (string.IsNullOrWhiteSpace(this.ETag) == false)
                response.Headers.Add("ETag", this.ETag);

            var cacheHintByDate = CacheHint.None;
            var ifModSinceStr = request.Headers["If-Modified-Since"];
            if (this.LastModified.HasValue && DateTime.TryParse(ifModSinceStr, out var ifModSince))
            {
                var timeDiff = Math.Abs((ifModSince.ToUniversalTime() - this.LastModified.Value.ToUniversalTime()).TotalSeconds);
                cacheHintByDate = timeDiff < 1.0 ? CacheHint.Hit : CacheHint.Miss;
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
                return true;
            }
            return false;
        }

#if !NETSTANDARD
        public override void ExecuteResult(ControllerContext context)
        {
            if (this.RespondNotModified(context)) return;

            var response = context.HttpContext.Response;
            var contentBytes = this.GetContent();
            if (contentBytes == null)
            {
                response.StatusCode = 404; // HTTP 404 Not Found
            }
            else
            {
                response.BinaryWrite(contentBytes);
            }
        }
#else
        public Func<Task<byte[]>> GetContentAsync { get; set; }

        public CacheableContentResult(string contentType, Func<Task<byte[]>> getContentAsync, DateTime? lastModified = null, string etag = null, Cacheability? cacheability = null)
            : this(contentType, lastModified, etag, cacheability)
        {
            this.GetContentAsync = getContentAsync;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (this.GetContent == null && this.GetContentAsync == null)
            {
                return base.ExecuteResultAsync(context);
            }
            else
            {
                return ExecuteResultAsyncCore(context);
            }
        }

        private async Task ExecuteResultAsyncCore(ActionContext context)
        {
            if (this.RespondNotModified(context)) return;

            var contentBytes = default(byte[]);
            if (this.GetContent != null)
            {
                contentBytes = this.GetContent();
            }
            else
            {
                contentBytes = await this.GetContentAsync();
            }

            var response = context.HttpContext.Response;
            if (contentBytes == null)
            {
                response.StatusCode = 404; // HTTP 404 Not Found
                return;
            }
            await response.Body.WriteAsync(contentBytes, 0, contentBytes.Length);
        }
#endif
    }
}
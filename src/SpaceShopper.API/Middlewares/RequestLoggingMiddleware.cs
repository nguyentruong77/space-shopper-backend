using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace SpaceShopper.API.Middlewares
{
    /// <summary>
    /// Middleware log request/response theo correlationId.
    /// - Đọc correlationId từ header "X-Correlation-Id" nếu có, ngược lại tự sinh Guid.
    /// - Gắn correlationId vào HttpContext.Items để downstream dùng lại.
    /// - Log cơ bản: method, path, status code, thời gian xử lý (ms), correlationId.
    /// </summary>
    public sealed class RequestLoggingMiddleware : IMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-Id";
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationId = GetOrCreateCorrelationId(context);

            using (_logger.BeginScope(new Dictionary<string, object>
                   {
                       ["CorrelationId"] = correlationId
                   }))
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    await next(context);
                }
                finally
                {
                    stopwatch.Stop();

                    _logger.LogInformation(
                        "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms (CorrelationId: {CorrelationId})",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds,
                        correlationId);
                }
            }
        }

        private static string GetOrCreateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var existing) &&
                !string.IsNullOrWhiteSpace(existing))
            {
                var value = existing.ToString();
                context.Items[CorrelationIdHeaderName] = value;
                return value;
            }

            var correlationId = Guid.NewGuid().ToString("N");

            context.Request.Headers[CorrelationIdHeaderName] = correlationId;
            context.Response.Headers[CorrelationIdHeaderName] = correlationId;
            context.Items[CorrelationIdHeaderName] = correlationId;

            return correlationId;
        }
    }
}


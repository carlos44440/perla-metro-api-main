using System.Diagnostics;
using System.Text;

namespace parla_metro_api_main.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            // Log del request entrante
            await LogRequestAsync(context, requestId);

            // Capturar la respuesta original
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                // Log de la respuesta
                await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);

                // Copiar la respuesta de vuelta al stream original
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task LogRequestAsync(HttpContext context, string requestId)
        {
            try
            {
                var request = context.Request;
                
                var requestLog = new StringBuilder();
                requestLog.AppendLine($"🔵 [REQUEST {requestId}] {request.Method} {request.Path}{request.QueryString}");
                requestLog.AppendLine($"   Host: {request.Host}");
                requestLog.AppendLine($"   User-Agent: {request.Headers.UserAgent}");
                requestLog.AppendLine($"   Content-Type: {request.ContentType}");
                requestLog.AppendLine($"   Content-Length: {request.ContentLength}");
                requestLog.AppendLine($"   Remote IP: {context.Connection.RemoteIpAddress}");
                requestLog.AppendLine($"   Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

                // Log del body si es POST/PUT
                if ((request.Method == "POST" || request.Method == "PUT") && 
                    request.ContentLength > 0 && request.ContentLength < 1024)
                {
                    request.EnableBuffering();
                    var body = await new StreamReader(request.Body).ReadToEndAsync();
                    request.Body.Position = 0;
                    
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        requestLog.AppendLine($"   Body: {body}");
                    }
                }

                _logger.LogInformation(requestLog.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al registrar request {RequestId}", requestId);
            }
        }

        private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMs)
        {
            try
            {
                var response = context.Response;
                
                var responseLog = new StringBuilder();
                responseLog.AppendLine($"🟢 [RESPONSE {requestId}] Status: {response.StatusCode}");
                responseLog.AppendLine($"   Content-Type: {response.ContentType}");
                responseLog.AppendLine($"   Content-Length: {response.ContentLength}");
                responseLog.AppendLine($"   Duration: {elapsedMs}ms");

                // Log del body si es pequeño y es error
                if (response.StatusCode >= 400 && response.Body.Length > 0 && response.Body.Length < 1024)
                {
                    response.Body.Seek(0, SeekOrigin.Begin);
                    var body = await new StreamReader(response.Body).ReadToEndAsync();
                    response.Body.Seek(0, SeekOrigin.Begin);
                    
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        responseLog.AppendLine($"   Error Body: {body}");
                    }
                }

                // Log con nivel apropiado según status code
                if (response.StatusCode >= 500)
                {
                    _logger.LogError(responseLog.ToString());
                }
                else if (response.StatusCode >= 400)
                {
                    _logger.LogWarning(responseLog.ToString());
                }
                else
                {
                    _logger.LogInformation(responseLog.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al registrar response {RequestId}", requestId);
            }
        }
    }
}
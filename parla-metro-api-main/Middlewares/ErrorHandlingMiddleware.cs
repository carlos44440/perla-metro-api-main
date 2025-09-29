using parla_metro_api_main.Models.Responses;
using System.Net;
using System.Text.Json;

namespace parla_metro_api_main.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly JsonSerializerOptions _jsonOptions;

        public ErrorHandlingMiddleware(
            RequestDelegate next, 
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            _logger.LogError(exception, 
                "🔴 [ERROR {RequestId}] Excepción no manejada en {Method} {Path}: {Message}", 
                requestId, context.Request.Method, context.Request.Path, exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Success = false,
                Path = $"{context.Request.Method} {context.Request.Path}",
                Timestamp = DateTime.UtcNow
            };

            // Mapear excepciones a códigos HTTP y mensajes apropiados
            switch (exception)
            {
                case ArgumentNullException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Parámetro requerido faltante";
                    errorResponse.Details = _environment.IsDevelopment() ? argEx.Message : null;
                    break;

                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Parámetro inválido";
                    errorResponse.Details = _environment.IsDevelopment() ? argEx.Message : null;
                    break;

                case KeyNotFoundException keyEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Recurso no encontrado";
                    errorResponse.Details = _environment.IsDevelopment() ? keyEx.Message : null;
                    break;

                case UnauthorizedAccessException unAuthEx:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Acceso no autorizado";
                    errorResponse.Details = _environment.IsDevelopment() ? unAuthEx.Message : null;
                    break;

                case HttpRequestException httpEx:
                    response.StatusCode = (int)HttpStatusCode.BadGateway;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Error de comunicación con el servicio externo";
                    errorResponse.Details = _environment.IsDevelopment() ? httpEx.Message : null;
                    break;

                case TaskCanceledException timeoutEx when timeoutEx.InnerException is TimeoutException:
                    response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Timeout en la comunicación con el servicio";
                    errorResponse.Details = _environment.IsDevelopment() ? timeoutEx.Message : null;
                    break;

                case InvalidOperationException invOpEx:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Operación inválida";
                    errorResponse.Details = _environment.IsDevelopment() ? invOpEx.Message : null;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Error interno del servidor";
                    errorResponse.Details = _environment.IsDevelopment() ? exception.Message : null;
                    break;
            }

            // Agregar información adicional en desarrollo
            if (_environment.IsDevelopment())
            {
                errorResponse.Details += $" | RequestId: {requestId}";
                if (exception.StackTrace != null)
                {
                    _logger.LogDebug("🔍 [DEBUG {RequestId}] Stack Trace: {StackTrace}", requestId, exception.StackTrace);
                }
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, _jsonOptions);
            await response.WriteAsync(jsonResponse);

            _logger.LogInformation(
                "🟡 [HANDLED {RequestId}] Error response sent - Status: {StatusCode}, Message: {Message}",
                requestId, response.StatusCode, errorResponse.Message);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace parla_metro_api_main.Models.Responses
{
    /// <summary>
    /// Response estándar para todas las operaciones de la API
    /// </summary>
    /// <typeparam name="T">Tipo de datos retornados</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo de la operación
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Datos retornados por la operación
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Lista de errores
        /// </summary>
        public List<string>? ErrorData { get; set; }

        /// <summary>
        /// Constructor para respuesta exitosa
        /// </summary>
        public static ApiResponse<T> SuccessResult(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
            };
        }

        /// <summary>
        /// Constructor para respuesta de error
        /// </summary>
        public static ApiResponse<T> ErrorResult(string message, string? errorData = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorData = errorData != null ? new List<string> { errorData } : null,
            };
        }
    }
}

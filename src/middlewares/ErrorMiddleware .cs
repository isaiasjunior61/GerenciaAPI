using System.Net;
using System.Text.Json;


namespace GerenciaAPI.src.middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Executa a próxima etapa do pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Se ocorrer um erro, chama o método para tratá-lo
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Define a resposta como JSON
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                message = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
                details = exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

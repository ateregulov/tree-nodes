using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualBasic;
using System;
using System.Net;
using System.Text;
using TreesNodes.DAL;

namespace TreesNodes.Helpers
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is SecureException e)
            {
                var httpContext = context.HttpContext;
                var _context = context.HttpContext.RequestServices.GetService<MyContext>();

                var queryString = httpContext.Request.QueryString.HasValue
                    ? httpContext.Request.QueryString.Value
                    : "No query parameters";

                string requestBody = ReadRequestBodyAsync(httpContext).Result;

                var journalItem = new JournalItem
                {
                    CreatedAt = DateTime.UtcNow,
                    Type = "Secure",
                    Message = e.Message,
                    StackTrace = e.StackTrace,
                    Body = requestBody,
                    Query = queryString
                };

                 _context.Journal.Add(journalItem);
                 _context.SaveChanges();

                ExceptionMessage output = new ExceptionMessage
                {

                    Id = journalItem.Id,
                    Type = "Secure",
                    Data = new Data
                    {
                        Message = e.Message
                    }
                };

                context.Result = new JsonResult(output)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };

                context.ExceptionHandled = true;

                return;
            }

            if (context.Exception is Exception ex)
            {
                var httpContext = context.HttpContext;

                var queryString = httpContext.Request.QueryString.HasValue
                    ? httpContext.Request.QueryString.Value
                    : "No query parameters";

                string requestBody = ReadRequestBodyAsync(httpContext).Result;

                var _context = context.HttpContext.RequestServices.GetService<MyContext>();


                var journalItem = new JournalItem
                {
                    CreatedAt = DateTime.UtcNow,
                    Type = "Secure",
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Body = requestBody,
                    Query = queryString
                };
                _context.Journal.Add(journalItem);
                _context.SaveChanges();

                ExceptionMessage output = new ExceptionMessage
                {

                    Id = journalItem.Id,
                    Type = "Secure",
                    Data = new Data
                    {
                        Message = $"Internal server error ID = {journalItem.Id}"
                    }
                };

                context.Result = new JsonResult(output)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };

                context.ExceptionHandled = true;

                return;
            }
        }
    

        private async Task<string> ReadRequestBodyAsync(HttpContext context)
        {
            context.Request.EnableBuffering(); // Allow the request body to be read multiple times

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                string body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; // Reset the stream position to allow further processing
                return string.IsNullOrEmpty(body) ? "No request body" : body;
            }
        }

    }
}

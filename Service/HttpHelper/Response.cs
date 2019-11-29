using System;
using System.Net;

namespace BetSnooker.HttpHelper
{
    public class Response<T>
    {
        public Response(bool success, HttpStatusCode? status = null, T data = default)
        {
            Success = success;
            ErrorMessage = null;
            HttpStatus = status;
            Data = data;
        }

        public Response(bool success, string errorMessage, HttpStatusCode? status = null, T data = default)
        {
            Success = success;
            ErrorMessage = errorMessage;
            HttpStatus = status;
            Data = data;
        }

        public T Data { get; }
        
        public HttpStatusCode? HttpStatus { get; }
        
        public bool Success { get; }
        
        public string ErrorMessage { get; }

        public T GetDataIfSuccessOr(T dataIfFailure)
        {
            return Success ? Data : dataIfFailure;
        }

        public T GetDataIfSuccessOrThrow()
        {
            return Success ? Data : throw new Exception(ErrorMessage);
        }

        public T GetDataIfSuccessOrThrow(string customErrorMessage)
        {
            if (Success)
            {
                return Data;
            }

            var exception = new Exception(customErrorMessage);
            EnrichException(exception);
            throw exception;
        }

        private void EnrichException(Exception exception)
        {
            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                exception.Data.Add("responseErrorMessage", ErrorMessage);
            }

            if (HttpStatus != null)
            {
                exception.Data.Add("responseHttpStatus", HttpStatus);
            }

            exception.Data.Add("response", this);
        }
    }
}
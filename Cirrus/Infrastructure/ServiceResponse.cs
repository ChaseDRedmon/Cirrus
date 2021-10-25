using System;
using System.Threading.Tasks;

namespace Cirrus.Infrastructure
{
    public class ServiceResponse
    {
        /// <summary>
        /// Success status code for a successful operation
        /// </summary>
        public bool Success { get; }
        
        /// <summary>
        /// Error message from a failed operation
        /// </summary>
        public string ErrorMessage { get; }
        
        /// <summary>
        /// Failed status code
        /// </summary>
        public bool Failure => !Success;
        
        /// <summary>
        /// Do we have a value?
        /// </summary>
        public bool IsEmpty { get; }

        protected ServiceResponse(bool success, bool isEmpty, string errorMessage)
        {
            switch (success)
            {
                case true when !string.IsNullOrWhiteSpace(errorMessage):
                    throw new ArgumentException("Cannot be successful with error message");
                case false when string.IsNullOrWhiteSpace(errorMessage):
                    throw new ArgumentException("Cannot be failure with no error message");
                default:
                    Success = success;
                    ErrorMessage = errorMessage;
                    IsEmpty = isEmpty;
                    break;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ServiceResponse Fail(string message)
        {
            return new ServiceResponse(false, true, message);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ServiceResponse<T> Fail<T>(string message)
        {
            return new ServiceResponse<T>(default, false, true, message);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ServiceResponse Ok()
        {
            return new ServiceResponse(true, true, string.Empty);
        }
    
        /// <summary>
        /// Shows a successful service operation
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns a new <see cref="ServiceResponse{T}"/></returns>
        public static ServiceResponse<T> Ok<T>(T value)
        {
            return new ServiceResponse<T>(value, true, false, string.Empty);
        }

        public static ServiceResponse<T> EmptyResponse<T>()
        {
            return new ServiceResponse<T>(default, true, true, string.Empty);
        }

        public Task GetAction(Func<Task> onSuccessAction, Func<Task> onFailAction)
        {
            return Success ? onSuccessAction() : onFailAction();
        }
    }
    
    public sealed class ServiceResponse<T> : ServiceResponse
    {
        public T? Value { get; }

        internal ServiceResponse(T? value, bool success, bool isEmpty, string errorMessage) : base(success, isEmpty, errorMessage)
        {
            Value = value;
        }
    }
}
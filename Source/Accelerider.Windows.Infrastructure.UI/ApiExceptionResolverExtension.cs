using System;
using System.Net.Http;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using Prism.Ioc;
using Refit;

namespace Accelerider.Windows.Infrastructure
{
    public static class ApiExceptionResolverExtension
    {
        private class ApiExceptionResolver
        {
            private readonly ISnackbarMessageQueue _snackbarMessageQueue;

            public ApiExceptionResolver(ISnackbarMessageQueue snackbarMessageQueue)
            {
                _snackbarMessageQueue = snackbarMessageQueue;
            }

            public async Task RunApiInternal(Task task, Action onSuccessCallback)
            {
                try
                {
                    await task;
                    onSuccessCallback?.Invoke();
                }
                catch (ApiException e)
                {
                    _snackbarMessageQueue.Enqueue(JObject.Parse(e.Content).Value<string>("status"));
                }
                catch (HttpRequestException httpRequestException)
                {
                    _snackbarMessageQueue.Enqueue(httpRequestException.InnerException?.Message);
                }
            }

            public async Task<T> RunApiInternal<T>(Task<T> task) where T : new()
            {
                try
                {
                    return await task;
                }
                catch (ApiException apiException)
                {
                    var message = JObject.Parse(apiException.Content)["status"].ToString();
                    _snackbarMessageQueue.Enqueue(message);
                }
                catch (HttpRequestException httpRequestException)
                {
                    _snackbarMessageQueue.Enqueue(httpRequestException.InnerException?.Message);
                }

                return new T();
            }
        }

        private static IContainerProvider _container;

        public static void SetUnityContainer(IContainerProvider container) => _container = container;

        public static Task RunApi(this Task task, Action onSuccessCallback = null) => 
            _container.Resolve<ApiExceptionResolver>().RunApiInternal(task, onSuccessCallback);

        public static Task<T> RunApi<T>(this Task<T> task) where T : new() => 
            _container.Resolve<ApiExceptionResolver>().RunApiInternal(task);
    }

}

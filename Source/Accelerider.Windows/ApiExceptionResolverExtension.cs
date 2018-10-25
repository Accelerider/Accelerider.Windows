using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using Refit;

namespace Accelerider.Windows
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
                    _snackbarMessageQueue.Enqueue(JObject.Parse(e.Content).Value<string>("message"));
                }
                catch (HttpRequestException httpRequestException)
                {
                    _snackbarMessageQueue.Enqueue(httpRequestException.InnerException?.Message);
                }
            }

            public async Task<T> RunApiInternal<T>(Task<T> task)
            {
                try
                {
                    return await task;
                }
                catch (ApiException apiException)
                {
                    _snackbarMessageQueue.Enqueue(apiException.StatusCode);
                }
                catch (HttpRequestException httpRequestException)
                {
                    _snackbarMessageQueue.Enqueue(httpRequestException.InnerException?.Message);
                }

                return default(T);
            }
        }

        private static IContainer _container;

        public static void SetUnityContainer(IContainer container) => _container = container;

        public static Task RunApi(this Task task, Action onSuccessCallback) => _container.Resolve<ApiExceptionResolver>().RunApiInternal(task, onSuccessCallback);

        public static Task<T> RunApi<T>(this Task<T> task) => _container.Resolve<ApiExceptionResolver>().RunApiInternal(task);
    }

}

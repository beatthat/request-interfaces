#if NET_4_6
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BeatThat.Requests
{
    public static class AsyncExt
    {
        public static TaskAwaiter<TResult> GetAwaiter<TResult>(this Request<TResult> request)
        {
            return request.ToTask().GetAwaiter();
        }

        public static TaskAwaiter GetAwaiter(this Request request)
        {
            return request.ToTaskWithNoResult().GetAwaiter();
        }

        public static Task<TResult> ExecuteAsync<TResult>(this Request<TResult> request)
        {
            var task = request.ToTask();
            request.Execute();
            return task;
        }

        public static Task ExecuteAsyncTask(this Request request)
        {
            var task = request.ToTaskWithNoResult();
            request.Execute();
            return task;
        }

        private static void UpdateTask<TResult>(this Request<TResult> request, TaskCompletionSource<TResult> tcs)
        {
            switch (request.status)
            {
                case RequestStatus.CANCELLED:
                    tcs.SetCanceled();
                    break;
                case RequestStatus.DONE:
                    if (request.hasError)
                    {
                        tcs.SetException(new Exception(request.error));
                    }
                    else
                    {
                        tcs.SetResult(request.item);
                    }
                    break;
            }
        }

        private static void UpdateTaskWithNoResult(this Request request, TaskCompletionSource<object> tcs)
        {
            switch (request.status)
            {
                case RequestStatus.CANCELLED:
                    tcs.SetCanceled();
                    break;
                case RequestStatus.DONE:
                    if (request.hasError)
                    {
                        tcs.SetException(new Exception(request.error));
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }
                    break;
            }
        }


        private static Task<TResult> ToTask<TResult>(this Request<TResult> request)
        {
            var tcs = new TaskCompletionSource<TResult>();
            switch(request.status) {
                case RequestStatus.CANCELLED:
                case RequestStatus.DONE:
                    request.UpdateTask(tcs);
                    return tcs.Task;
            }

            request.UpdateTask(tcs);
            request.StatusUpdated += () =>
            {
                request.UpdateTask(tcs);
            };
            return tcs.Task;
        }

        private static Task ToTaskWithNoResult(this Request request)
        {
            var tcs = new TaskCompletionSource<object>();
            switch (request.status)
            {
                case RequestStatus.CANCELLED:
                case RequestStatus.DONE:
                    request.UpdateTaskWithNoResult(tcs);
                    return tcs.Task;
            }

            request.UpdateTaskWithNoResult(tcs);
            request.StatusUpdated += () =>
            {
                request.UpdateTaskWithNoResult(tcs);
            };
            return tcs.Task;
        }
    }
}


#endif
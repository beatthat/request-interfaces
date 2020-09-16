using System;

namespace BeatThat.Requests
{
    /// <summary>
    /// A Request is an async op that may succeed, fail or be cancelled
    /// </summary>
    public interface Request : IDisposable, HasError
    {
        event Action StatusUpdated;// TODO: replace with UnityEvent

        RequestStatus status { get; }

        bool isCancelled { get; }

        /// <summary>
        /// Progress of the request
        /// </summary>
        float progress { get; }

        bool hasError { get; }

        void Cancel();

        /// <summary>
        /// Execute the request and call the (optional) callback when the request terminates, successful or otherwise.
        /// </summary>
        void Execute(Action callback = null, bool callbackOnCancelled = false);

        ///<summary>
        /// often a web uri, but can be anything that helps identify the request
        ///</summary>
        string loggingName { get; }

        /// <summary>
        /// Signal to log debug info for a specific request. 
        /// Ususally more useful than a global flag, because there can be so many requests running at a time, 
        /// including multiples of the same type.
        /// </summary>
        bool debug { get; set; }
    }

    /// <summary>
    /// A request that retrieves an item
    /// </summary>
    public interface ItemRequest
    {
        object GetItem();
    }

    /// <summary>
    /// A request that retrieves a typed item
    /// </summary>
    public interface Request<T> : Request, ItemRequest
    {
        T item { get; }
    }

    public static class RequestExt
    {
        public static bool CanExecuteFromCurrentStatus(this Request r)
        {
            if (r == null)
            {
                return false;
            }
            return (
                r.status == RequestStatus.NONE
                || r.status == RequestStatus.QUEUED);
        }

        /// <summary>
        /// Utility for cases where a service stores the 'active' request,
        /// e.g. to prevent duplicate concurrent requests.
        /// If the passed request is the same ref as the passed ref, then nulls the ref
        /// If not, clears the request (this is why it's a ref arg)
        /// </summary>
        public static bool ClearIfMatches<T>(this Request r, ref T rRef) where T : class
        {
            if (r == null)
            {
                return false;
            }
            if (Object.ReferenceEquals(r, rRef))
            {
                rRef = null;
                return true;
            }
            return false;
        }


        public static bool IsDoneOrCancelled(this Request r)
        {
            if (r == null)
            {
                return false;
            }
            return (
                r.status == RequestStatus.DONE
                || r.status == RequestStatus.CANCELLED);
        }

        public static bool IsQueuedOrInProgress(this Request r)
        {
            return r.status == RequestStatus.QUEUED || r.status == RequestStatus.IN_PROGRESS;
        }

        public static T GetResult<T>(this Request r)
        {
            if (r == null)
            {
                return default(T);
            }
            var hasItem = r as ItemRequest;
            return hasItem != null ? (T)hasItem.GetItem() : default(T);

        }
    }
}


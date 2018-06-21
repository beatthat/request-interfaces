using System;

namespace BeatThat
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

		bool hasError { get;  }

		void Cancel();

		/// <summary>
		/// Execute the request and call the (optional) callback when the request terminates, successful or otherwise.
		/// </summary>
		void Execute(Action callback = null);

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

}


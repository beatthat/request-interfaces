namespace BeatThat.Requests
{
    public interface ListRequest<T> : Request
	{
		T[] items { get; }
	}
}

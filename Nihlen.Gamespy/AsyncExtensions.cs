namespace Nihlen.Gamespy;

public static class AsyncExtensions
{
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        // Useful for classes like UdpClient which have no timeouts built-in
        var tcs = new TaskCompletionSource<bool>();
        await using (cancellationToken.Register(s => (s as TaskCompletionSource<bool>)?.TrySetResult(true), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task))
            {
                throw new OperationCanceledException(cancellationToken);
            }
        }

        return task.Result;
    }
}

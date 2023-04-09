using B2BrokerTest.Interfaces;

namespace B2BrokerTest.BusWriters {
  internal class SemaphoreSlimBusWriter: IBusWriter {
    private const int MinLengthMsgBytes = 1000;
    private const int SemaphoreInitialCount = 1;
    private readonly IBusConnection _connection;
    private readonly CancellationToken _cancellationToken;
    private readonly SemaphoreSlim _semaphore;
    private MemoryStream _msBuffer;

    public SemaphoreSlimBusWriter(IBusConnection connection, CancellationToken cancellationToken) {
      _connection = connection;
      _cancellationToken = cancellationToken;
      _semaphore = new SemaphoreSlim(SemaphoreInitialCount);
      _msBuffer = new MemoryStream();

      //prevent case of missed messages after cancellation
      _cancellationToken.Register(async () => {
        if (_msBuffer.Length > 0) {
          await PushAsync(CancellationToken.None);
        }
      });
    }

    public async Task SendMessageAsync(byte[] nextMessage, CancellationToken cancellationToken) {
      await _semaphore.WaitAsync(cancellationToken);
      try {
        await BufferingAsync(nextMessage, cancellationToken);
        if (_msBuffer.Length > MinLengthMsgBytes) {
          cancellationToken.ThrowIfCancellationRequested();
          await PushAsync(cancellationToken);
        }
      } finally {
        _semaphore.Release();
      }
    }

    private async Task BufferingAsync(byte[] nextMessage, CancellationToken cancellationToken) {
      await _msBuffer.WriteAsync(nextMessage, 0, nextMessage.Length, cancellationToken);
    }

    private async Task PushAsync(CancellationToken cancellationToken) {
      using (_msBuffer) {
        await _connection.PublishAsync(_msBuffer.GetBuffer());
      }
      _msBuffer = new MemoryStream();
    }
  }
}

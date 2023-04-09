using B2BrokerTest.Interfaces;

namespace B2BrokerTest {
  internal class BusWriterStrategyWithBuffer {
    private const int MinLengthMsgBytes = 1000;
    private const int SemaphoreInitialCount = 1;
    private MemoryStream _msBuffer;
    private readonly SemaphoreSlim _semaphore;
    private readonly IBusWriter _busWriter;
    private readonly CancellationToken _cancellationToken;

    //There we can set IBusConnector, but maybe need some additional processing before Publish to Bus
    internal BusWriterStrategyWithBuffer(IBusWriter busWriter, CancellationToken cancellationToken) {
      _msBuffer = new MemoryStream();
      _busWriter = busWriter;
      _cancellationToken = cancellationToken;
      _semaphore = new SemaphoreSlim(SemaphoreInitialCount);

      //prevent case of missed messages after cancellation
      _cancellationToken.Register(async () => {
        if (_msBuffer.Length > 0) {
          await PushAsync(CancellationToken.None);
        }
      });
    }

    internal async Task SendMessageAsync(byte[] nextMessage, CancellationToken cancellationToken) {
      if (_busWriter == null) {
        throw new ArgumentNullException(nameof(_busWriter));
      }
      if (nextMessage == null) {
        throw new ArgumentNullException(nameof(nextMessage));
      }
      if (nextMessage.Length <= 0) {
        return;
      }
      //There possible add some addititonal logic or logging

      try {
        await _semaphore.WaitAsync(cancellationToken);
        await BufferingAsync(nextMessage, cancellationToken);
        if (_msBuffer.Length > MinLengthMsgBytes) {
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
        await _busWriter.SendMessageAsync(_msBuffer.ToArray(), cancellationToken);
      }
      _msBuffer = new MemoryStream();
    }
  }
}

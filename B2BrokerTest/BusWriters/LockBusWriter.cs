using B2BrokerTest.Interfaces;

namespace B2BrokerTest.BusWriters {
  // Not think that this good decision
  // We lost possibilility use async, but this is possible
  internal class LockBusWriter : IBusWriter {
    private readonly IBusConnection _connection;
    private readonly object _lock = new object();

    public LockBusWriter(IBusConnection connection) {
      _connection = connection;
    }

    private readonly MemoryStream _buffer = new();
    // how to make this method thread safe?
    public async Task SendMessageAsync(byte[] nextMessage, CancellationToken cancellationToken) {
      cancellationToken.ThrowIfCancellationRequested();
      lock (_lock) {
        _buffer.Write(nextMessage, 0, nextMessage.Length);
        if (_buffer.Length > 1000) {
          _connection.PublishAsync(_buffer.ToArray()).Wait();
          _buffer.SetLength(0);
        }
      }
    }
  }
}

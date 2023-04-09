using B2BrokerTest.Interfaces;

namespace B2BrokerTest.BusWriters {
  internal class SourceBusWriter: IBusWriter {
    private readonly IBusConnection _connection;

    public SourceBusWriter(IBusConnection connection) {
      _connection = connection;
    }

    private readonly MemoryStream _buffer = new();
    // how to make this method thread safe?
    public async Task SendMessageAsync(byte[] nextMessage, CancellationToken cancellationToken) {
      _buffer.Write(nextMessage, 0, nextMessage.Length);
      if (_buffer.Length > 1000) {
        await _connection.PublishAsync(_buffer.ToArray());
        _buffer.SetLength(0);
      }
    }
  }
}
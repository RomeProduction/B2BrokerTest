using B2BrokerTest.Interfaces;

namespace B2BrokerTest.BusWriters {
  internal class LightBusWriter: IBusWriter {
    private readonly IBusConnection _connection;

    public LightBusWriter(IBusConnection connection) {
      _connection = connection;
    }

    public async Task SendMessageAsync(byte[] nextMessage, CancellationToken cancellationToken) {
      await _connection.PublishAsync(nextMessage);
    }
  }
}

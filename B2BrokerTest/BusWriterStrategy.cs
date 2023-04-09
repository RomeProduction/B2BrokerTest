using B2BrokerTest.Interfaces;

namespace B2BrokerTest {
  internal class BusWriterStrategy {
    internal BusWriterStrategy() {
    }

    internal async Task ExecuteAsync(IBusWriter busWriter, byte[] nextMessage, CancellationToken cancellationToken) {
      if (busWriter is null) {
        throw new ArgumentNullException(nameof(busWriter));
      }
      if (nextMessage == null){ throw new ArgumentNullException(nameof(nextMessage)); }
      //There possible add some addititonal logic or logging

      await busWriter.SendMessageAsync(nextMessage, cancellationToken);
    }
  }
}

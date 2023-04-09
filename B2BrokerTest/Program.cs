using B2BrokerTest.BusWriters;
using B2BrokerTest.Interfaces;
using System.Text;

namespace B2BrokerTest {
  internal class Program {
    static async Task Main(string[] args) {
      var busConnection = new BusConnection();

      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

      List<IBusWriter> writers = new List<IBusWriter>();

      var sourceBW = new SourceBusWriter(busConnection);
      writers.Add(sourceBW);
      var lockBW = new LockBusWriter(busConnection);
      writers.Add(lockBW);
      var semaphoreBW = new SemaphoreSlimBusWriter(busConnection, cancellationTokenSource.Token);
      writers.Add(semaphoreBW);

      var strategy = new BusWriterStrategy();

      var messages = new List<string> {
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
        "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
        "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
        "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
        "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
        "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
      };

      //foreach (var msgStr in messages) {
      //  // converts a C# string to a byte array.
      //  byte[] msg = Encoding.ASCII.GetBytes(msgStr);
      //  foreach (var writer in writers) {
      //    await strategy.ExecuteAsync(writer, msg, cancellationTokenSource.Token);
      //  }
      //}

      var busBufferWriter = new BusWriterStrategyWithBuffer(new LightBusWriter(busConnection), cancellationTokenSource.Token);
      foreach (var msgStr in messages) {
        // converts a C# string to a byte array.
        byte[] msg = Encoding.ASCII.GetBytes(msgStr);
        await busBufferWriter.SendMessageAsync(msg, cancellationTokenSource.Token);
      }

      Console.WriteLine("Is complete");
      Console.ReadLine();
    }
  }
}
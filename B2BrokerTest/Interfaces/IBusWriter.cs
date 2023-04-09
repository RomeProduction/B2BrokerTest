namespace B2BrokerTest.Interfaces {
  internal interface IBusWriter {
    //The first I will want to add CancellationTolen it will be useful process of cancellation case
    Task SendMessageAsync(byte[] nextMessage, CancellationToken cancellationToken);
  }
}

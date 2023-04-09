namespace B2BrokerTest.Interfaces {
  internal interface IBusConnection {
    Task PublishAsync(byte[] bufMsg);
  }
}

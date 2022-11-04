namespace NFHelio.Services
{
  using nanoFramework.Device.Bluetooth.Spp;
  using NFCommon.Services;

  /// <summary>
  /// Implements <see cref="IAppMessageWriter"/> by sending message to a bluetooth app
  /// </summary>
  internal class AppMessageWriter : IAppMessageWriter
  {
    private readonly IBluetoothSpp bluetoothSpp;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppMessageWriter"/> class.
    /// </summary>
    /// <param name="bluetoothSpp">The bluetooth SPP.</param>
    public AppMessageWriter(IBluetoothSpp bluetoothSpp)
    {
      this.bluetoothSpp = bluetoothSpp;
    }

    /// <summary>
    /// Sends the string.
    /// </summary>
    /// <param name="message">The message.</param>
    public void SendString(string message)
    {
      this.bluetoothSpp.SendString(message);
    }
  }
}

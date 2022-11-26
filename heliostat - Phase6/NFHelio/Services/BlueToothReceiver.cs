namespace NFHelio.Services
{
  using nanoFramework.Device.Bluetooth.Spp;
  using nanoFramework.Hosting;
  using System;
  using System.Diagnostics;
  using System.Threading;

  /// <summary>
  /// A <see cref="BackgroundService"/> for handling Bluetooth events
  /// </summary>
  /// <seealso cref="nanoFramework.Hosting.BackgroundService" />
  internal class BlueToothReceiver : BackgroundService
  {
    private readonly IBluetoothSpp bluetoothSpp;

    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlueToothReceiver"/> class.
    /// </summary>
    /// <param name="bluetoothSpp">The bluetooth SPP.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public BlueToothReceiver(
      IBluetoothSpp bluetoothSpp,
      IServiceProvider serviceProvider)
    {
      this.bluetoothSpp = bluetoothSpp;
      this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// This method is called when the <see cref="T:nanoFramework.Hosting.IHostedService" /> starts.
    /// </summary>
    protected override void ExecuteAsync()
    {
      // Add event handles for received data and Connections 
      this.bluetoothSpp.ReceivedData += Spp_ReceivedData;
      this.bluetoothSpp.ConnectedEvent += Spp_ConnectedEvent;

      // Start Advertising SPP service
      this.bluetoothSpp.Start("HelioStat");

      while (!CancellationRequested)
      {
        Thread.Sleep(1);
      }

      this.bluetoothSpp.Stop();
    }

    private void Spp_ConnectedEvent(IBluetoothSpp sender, EventArgs e)
    {
      if (this.bluetoothSpp.IsConnected)
      {
        this.bluetoothSpp.SendString($"Welcome to HelioStat\n");
        this.bluetoothSpp.SendString($"Send 'help' for options\n");
      }

      Debug.WriteLine($"BlueTooth client connected:{sender.IsConnected}");
    }

    private void Spp_ReceivedData(IBluetoothSpp sender, SppReceivedDataEventArgs receivedDataEventArgs)
    {
      var commandHandlerService = (ICommandHandlerService)serviceProvider.GetService(typeof(ICommandHandlerService));

      commandHandlerService.HandleMessage(receivedDataEventArgs.DataString);
    }
  }
}

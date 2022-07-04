namespace NFHelio
{
  using System;
  using System.Diagnostics;
  using System.Threading;
  using nanoFramework.Device.Bluetooth.Spp;
  using nanoFramework.Hardware.Esp32;

  public static class Program
  {
    public static Context context = new Context();

    public static void Main()
    {
      Debug.WriteLine($"Starting HelioStat");

      SetupPins();

      context.BluetoothSpp = SetUpBlueTooth();

      Debug.WriteLine($"HelioStat is started, awaiting commands...");

      while (true)
      {
        Thread.Sleep(10000);
      }
    }

    private static void SetupPins()
    {
      Configuration.SetPinFunction((int)GPIOPort.ESP32_Onboard_Led, DeviceFunction.PWM1);
    }

    private static IBluetoothSpp SetUpBlueTooth()
    {
      // Create Instance of Bluetooth Serial profile
      var bluetoothSpp = new NordicSpp();

      // Add event handles for received data and Connections 
      bluetoothSpp.ReceivedData += Spp_ReceivedData;
      bluetoothSpp.ConnectedEvent += Spp_ConnectedEvent;

      // Start Advertising SPP service
      bluetoothSpp.Start("HelioStat");

      return bluetoothSpp;
    }

    private static void Spp_ConnectedEvent(IBluetoothSpp sender, EventArgs e)
    {
      if (context.BluetoothSpp.IsConnected)
      {
        context.BluetoothSpp.SendString($"Welcome to HelioStat\n");
        context.BluetoothSpp.SendString($"Send 'help' for options\n");
      }

      Debug.WriteLine($"BlueTooth client connected:{sender.IsConnected}");
    }

    private static void Spp_ReceivedData(IBluetoothSpp sender, SppReceivedDataEventArgs receivedDataEventArgs)
    {
      var commandHandler = new CommandHandler();
      commandHandler.HandleMessage(receivedDataEventArgs.DataString);
    }
  }
}
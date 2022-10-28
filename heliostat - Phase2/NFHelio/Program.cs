namespace NFHelio
{
  using System.Diagnostics;
  using nanoFramework.DependencyInjection;
  using nanoFramework.Device.Bluetooth.Spp;
  using nanoFramework.Hardware.Esp32;
  using nanoFramework.Hosting;
  using NFHelio.Services;

  /// <summary>
  /// The main program
  /// </summary>
  public static class Program
  {
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    public static void Main()
    {
      Debug.WriteLine($"Starting HelioStat");

      SetupPins();

      IHost host = CreateHostBuilder().Build();

      Debug.WriteLine($"HelioStat is started, awaiting commands...");

      // starts application and blocks the main calling thread 
      host.Run();
    }

    /// <summary>
    /// Creates the host builder.
    /// </summary>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder() =>
      Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
          services.AddSingleton(typeof(IBluetoothSpp), typeof(NordicSpp));
          services.AddSingleton(typeof(IAppMessageWriter), typeof(AppMessageWriter));
          services.AddTransient(typeof(ICommandHandlerService), typeof(CommandHandlerService));
          services.AddHostedService(typeof(BlueToothReceiver));
        });

    private static void SetupPins()
    {
      // onboard led
      Configuration.SetPinFunction((int)GPIOPort.ESP32_Onboard_Led, DeviceFunction.PWM16);
    }
  }
}
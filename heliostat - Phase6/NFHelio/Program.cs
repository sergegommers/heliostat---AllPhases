namespace NFHelio
{
  using System.Diagnostics;
  using nanoFramework.DependencyInjection;
  using nanoFramework.Device.Bluetooth.Spp;
  using nanoFramework.Hardware.Esp32;
  using nanoFramework.Hosting;
  using NFCommon.Services;
  using NFCommon.Storage;
  using NFHelio.Devices;
  using NFHelio.Services;
  using NFHelio.Storage;

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

      ReadSettingsFromStorage(host);

      WriteOutSetttings(host);

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
          services.AddSingleton(typeof(Settings));
          services.AddTransient(typeof(ISettingsStorage), typeof(JsonSettingsStorage));

          services.AddTransient(typeof(IEepromFactory), typeof(AT24C32EepromFactory));
          //services.AddTransient(typeof(IEepromFactory), typeof(InternalFlashEepromFactory));

          // IRealTimeClock defines the interface for a realtime clock.
          // As we want our application to be independent of the implementation of IRealTimeClock we let it be created by a factory.
          // Ds3231RealTimeClockFactory creates the IRealTimeClock instance and knows what parameters to pass in the constructor that are specific for this implementation.
          services.AddTransient(typeof(IRealTimeClockFactory), typeof(Ds3231RealTimeClockFactory));

          services.AddSingleton(typeof(IBluetoothSpp), typeof(NordicSpp));
          services.AddTransient(typeof(IAppMessageWriter), typeof(AppMessageWriter));
          services.AddTransient(typeof(ICommandHandlerService), typeof(CommandHandlerService));
          services.AddHostedService(typeof(BlueToothReceiver));

          services.AddHostedService(typeof(SunFollower));
        });

    private static void SetupPins()
    {
      // i2c pins
      Configuration.SetPinFunction((int)GPIOPort.I2C_Clock, DeviceFunction.I2C1_CLOCK);
      Configuration.SetPinFunction((int)GPIOPort.I2C_Data, DeviceFunction.I2C1_DATA);

      // onboard led
      Configuration.SetPinFunction((int)GPIOPort.ESP32_Onboard_Led, DeviceFunction.PWM16);

      // azimuth adc channel
      int pin = Configuration.GetFunctionPin(DeviceFunction.ADC1_CH0);
      if (pin != (int)GPIOPort.ADC_Azimuth)
      {
        Configuration.SetPinFunction((int)GPIOPort.ADC_Azimuth, DeviceFunction.ADC1_CH0);
      }

      // zenith adc channnel
      pin = Configuration.GetFunctionPin(DeviceFunction.ADC1_CH3);
      if (pin != (int)GPIOPort.ADC_Zenith)
      {
        Configuration.SetPinFunction((int)GPIOPort.ADC_Zenith, DeviceFunction.ADC1_CH3);
      }

      // azimuth motor control
      Configuration.SetPinFunction((int)GPIOPort.PWM_Azimuth_East_to_West, DeviceFunction.PWM1);
      Configuration.SetPinFunction((int)GPIOPort.PWM_Azimuth_West_to_East, DeviceFunction.PWM2);

      // zenith motor control
      Configuration.SetPinFunction((int)GPIOPort.PWM_Zenith_Up, DeviceFunction.PWM3);
      Configuration.SetPinFunction((int)GPIOPort.PWM_Zenith_Down, DeviceFunction.PWM4);
    }

    private static void ReadSettingsFromStorage(IHost host)
    {
      var settingsStorage = (ISettingsStorage)host.Services.GetService(typeof(ISettingsStorage));
      var newSettings = settingsStorage.ReadSettings() as Settings;

      // if we can't read the settings then we start from scratch
      if (newSettings == null)
      {
        newSettings = new Settings();
      }

      var currentSettings = (Settings)host.Services.GetService(typeof(Settings));
      currentSettings.Update(newSettings);
    }

    private static void WriteOutSetttings(IHost host)
    {
      var settings = (Settings)host.Services.GetService(typeof(Settings));
      var realTimeClockFactory = (IRealTimeClockFactory)host.Services.GetService(typeof(IRealTimeClockFactory));
      var realTimeClock = realTimeClockFactory.Create();

      var dt = realTimeClock.GetTime();

      Debug.WriteLine($"********************");
      Debug.WriteLine($"Latitude: {settings.Latitude}");
      Debug.WriteLine($"Longitude: {settings.Longitude}");
      Debug.WriteLine($"DateTime: {dt.ToString("yyyy / MM / dd HH: mm:ss")}");
      Debug.WriteLine($"********************");
      Debug.WriteLine($"Azimuth Adc Min: {settings.AzimuthAdcMin}");
      Debug.WriteLine($"Azimuth Adc Max: {settings.AzimuthAdcMax}");
      Debug.WriteLine($"Zenith Adc Min: {settings.ZenithAdcMin}");
      Debug.WriteLine($"Zenith Adc Maz: {settings.ZenithAdcMax}");
      Debug.WriteLine($"********************");
      for (int i = 0; i < settings.Aci.Length; i++)
      {
        Debug.WriteLine($"Azimuth calibration: {settings.Aci[i]} - {settings.Acv[i]}");
      }
      Debug.WriteLine($"********************");
      for (int i = 0; i < settings.Zci.Length; i++)
      {
        Debug.WriteLine($"Zenith calibration: {settings.Zci[i]} - {settings.Zcv[i]}");
      }
      Debug.WriteLine($"********************");
    }
  }
}
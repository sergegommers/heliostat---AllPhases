namespace NFHelio
{
  using NFCommon.Storage;
  using NFHelio.Devices;
  using NFSpa;
  using System;
  using System.Diagnostics;

  public class SunFollower : ConfigurableSchedulerService
  {
    private readonly IServiceProvider serviceProvider;

    public SunFollower(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public void Do()
    {
      var realTimeClockFactory = (IRealTimeClockFactory)this.serviceProvider.GetService(typeof(IRealTimeClockFactory));
      var realTimeClock = realTimeClockFactory.Create();

      var settingsStorage = (ISettingsStorage)this.serviceProvider.GetService(typeof(ISettingsStorage));

      Debug.WriteLine($"Follower: getting the time");
      var dt = realTimeClock.GetTime();

      Debug.WriteLine($"Follower: getting the position");
      Settings settings = settingsStorage.ReadSettings() as Settings;

      Debug.WriteLine($"Follower: calculating the angles");
      Spa_data spa = new()
      {
        year = dt.Year,
        month = dt.Month,
        day = dt.Day,
        hour = dt.Hour,
        minute = dt.Minute,
        second = dt.Second,
        longitude = settings.Longitude,
        latitude = settings.Latitude,
        elevation = 0,
        function = (int)Calculator.SpaOutputs.SPA_ZA
      };

      var calculator = new Calculator();
      var result = calculator.Spa_calculate(spa);
      if (result == 0)
      {
        short azimuth = (short)spa.azimuth;
        short zenith = (short)spa.zenith;

        Debug.WriteLine($"Follower: moving the mirror to azimuth {azimuth} and zenith {zenith}");

        var motorController = new MotorController(this.serviceProvider);
        motorController.MoveMotorToAngle(MotorPlane.Azimuth, azimuth);

        Debug.WriteLine($"Follower: mirrors moved");
      }
      else
      {
        Debug.WriteLine($"Follower: calculating the angles failed with error code {result}");
      }
    }

    protected override void ExecuteAsync()
    {
      this.Do();
    }
  }
}

namespace NFHelio
{
  using NFHelio.Devices;
  using NFSpa;
  using System;
  using System.Diagnostics;

  /// <summary>
  /// Projects the sun to a fixed spot
  /// It implements <see cref="ConfigurableSchedulerService"/>,
  /// so the ExecuteAsync method is called based on the interval specified in the RunEvery method of ConfigurableSchedulerService
  /// </summary>
  /// <seealso cref="NFHelio.ConfigurableSchedulerService" />
  public class HelioStat : ConfigurableSchedulerService
  {
    private readonly IServiceProvider serviceProvider;

    public HelioStat(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    protected override void ExecuteAsync()
    {
      var realTimeClockFactory = (IRealTimeClockFactory)this.serviceProvider.GetService(typeof(IRealTimeClockFactory));
      var realTimeClock = realTimeClockFactory.Create();
      var settings = (Settings)this.serviceProvider.GetService(typeof(Settings));

      Debug.WriteLine($"HelioStat: getting the time");
      var dt = realTimeClock.GetTime();

      Debug.WriteLine($"HelioStat: calculating the angles");
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
      if (result != 0)
      {
        Debug.WriteLine($"HelioStat: calculating the spa angles failed with error code {result}");

        return;
      }

      var azimuthAngle = (settings.AzimuthProjection - spa.azimuth) / 2.0 + spa.azimuth;
      var zenithAngle = (settings.ZenithProjection - spa.zenith) / 2.0 + spa.zenith;

      Debug.WriteLine($"HelioStat: moving the mirror to azimuth {azimuthAngle} and zenith {zenithAngle}");

      var motorController = new MotorController(this.serviceProvider);
      motorController.MoveMotorToAngle(MotorPlane.Azimuth, (short)azimuthAngle);
      motorController.MoveMotorToAngle(MotorPlane.Zenith, (short)zenithAngle);

      Debug.WriteLine($"HelioStat: mirrors moved");
    }
  }
}

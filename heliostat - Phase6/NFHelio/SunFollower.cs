namespace NFHelio
{
  using NFCommon.Services;
  using NFHelio.Devices;
  using NFSpa;
  using System;
  using System.Diagnostics;

  /// <summary>
  /// This class moves the mirror directly to the sun.
  /// It implements <see cref="ConfigurableSchedulerService"/>,
  /// so the ExecuteAsync method is called based on the interval specified in the RunEvery method of ConfigurableSchedulerService
  /// </summary>
  /// <seealso cref="NFHelio.ConfigurableSchedulerService" />
  public class SunFollower : ConfigurableSchedulerService
  {
    private readonly IServiceProvider serviceProvider;

    public SunFollower(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    protected override void ExecuteAsync()
    {
      var appMessageWriter = (IAppMessageWriter)serviceProvider.GetService(typeof(IAppMessageWriter));

      var selfCheck = new SelfCheck(this.serviceProvider);
      var issues = selfCheck.Check(
        SelfcheckReason.Basic |
        SelfcheckReason.MotorMovement);
      if (issues)
      {
        return;
      }

      var realTimeClockFactory = (IRealTimeClockFactory)this.serviceProvider.GetService(typeof(IRealTimeClockFactory));
      var realTimeClock = realTimeClockFactory.Create();
      var settings = (Settings)this.serviceProvider.GetService(typeof(Settings));

      Debug.WriteLine($"SunFollower: getting the time");
      var dt = realTimeClock.GetTime();

      Debug.WriteLine($"SunFollower: calculating the angles");
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

        appMessageWriter.SendString($"SunFollower: moving the mirror to azimuth {azimuth} and zenith {zenith}");

        var motorController = new MotorController(this.serviceProvider);
        motorController.MoveMotorToAngle(MotorPlane.Azimuth, azimuth);
        motorController.MoveMotorToAngle(MotorPlane.Zenith, zenith);

        appMessageWriter.SendString($"SunFollower: mirrors moved");
      }
      else
      {
        appMessageWriter.SendString($"SunFollower: calculating the angles failed with error code {result}");
      }
    }
  }
}

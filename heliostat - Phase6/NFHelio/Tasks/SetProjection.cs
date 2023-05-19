namespace NFHelio.Tasks
{
  using NFCommon;
  using NFCommon.Storage;
  using NFHelio.Devices;
  using NFSpa;
  using System;
  using System.Diagnostics;

  /// <summary>
  /// Stores the info about the point to project to
  /// </summary>
  internal class SetProjection : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "setproj";

    /// <inheritdoc />
    public override string Description => "Saves the place to project to";

    /// <inheritdoc />
    public override string Help => "No further info";

    /// <summary>
    /// Initializes a new instance of the <see cref="SetProjection" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public SetProjection(IServiceProvider serviceProvider)
    : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      var realTimeClockFactory = (IRealTimeClockFactory)this.GetServiceProvider().GetService(typeof(IRealTimeClockFactory));
      var realTimeClock = realTimeClockFactory.Create();

      var dt = realTimeClock.GetTime();

      var settings = (Settings)this.GetServiceProvider().GetService(typeof(Settings));

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
        function = (int)Calculator.SpaOutputs.SPA_ZA_RTS
      };

      var calculator = new Calculator();
      var result = calculator.Spa_calculate(spa);

      // check for SPA errors
      if (result != 0)
      {
        Debug.WriteLine($"Calculating the sun's position failed with error code {result}");

        return;
      }

      settings.AzimuthProjection = CalcForPlane(MotorPlane.Azimuth, spa.azimuth);
      settings.ZenithProjection = CalcForPlane(MotorPlane.Zenith, spa.zenith);

      var settingsStorage = (ISettingsStorage)this.GetServiceProvider().GetService(typeof(ISettingsStorage));
      settingsStorage.WriteSettings(settings);

      this.SendString($"Projecting Azimuth to {settings.AzimuthProjection}\nProjecting Zenith to {settings.ZenithProjection}\n");
    }

    private double CalcForPlane(MotorPlane plane, double sunAngle)
    {
      int adcChannel;
      CalibrationArray array;

      var settings = (Settings)this.GetServiceProvider().GetService(typeof(Settings));

      switch (plane)
      {
        case MotorPlane.Azimuth:
          adcChannel = Context.AzimuthAdcChannel;
          array = new CalibrationArray(settings.Acv, settings.Aci);
          break;

        default:
          adcChannel = Context.ZenithAdcChannel;
          array = new CalibrationArray(settings.Zcv, settings.Zci);
          break;
      }

      // get the current value of the motor position
      short value = (short)AdcReader.GetValue(adcChannel, Context.AdcSampleSize);

      // get the angle corresponding to the motor position
      array.GetCalibrationPoint(value, out short motorAngle);

      double projectionAngle = ((double)motorAngle - sunAngle) / 2.0 + sunAngle;

      return projectionAngle;
    }
  }
}

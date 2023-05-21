namespace NFHelio
{
  using NFCommon.Services;
  using NFHelio.Devices;
  using System;

  public enum SelfcheckReason
  {
    Basic,
    MotorMovement,
    HelioStat
  }

  /// <summary>
  /// A class to perform a self-check to see if everything is setup to move the mirror
  /// </summary>
  internal class SelfCheck
  {
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelfCheck"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public SelfCheck(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Checks this instance.
    /// </summary>
    /// <returns>true if there are issues</returns>
    public bool Check(SelfcheckReason reason)
    {
      var appMessageWriter = (IAppMessageWriter)serviceProvider.GetService(typeof(IAppMessageWriter));
      bool hasErrors = false;

      if ((reason & SelfcheckReason.Basic) == SelfcheckReason.Basic)
      {
        hasErrors = BasicChecks(appMessageWriter);
      }
      if ((reason & SelfcheckReason.MotorMovement) == SelfcheckReason.MotorMovement)
      {
        hasErrors = hasErrors | MotorMovementChecks(appMessageWriter, MotorPlane.Azimuth);
        hasErrors = hasErrors | MotorMovementChecks(appMessageWriter, MotorPlane.Zenith);
      }
      if ((reason & SelfcheckReason.HelioStat) == SelfcheckReason.HelioStat)
      {
        hasErrors = hasErrors | HelioStatChecks(appMessageWriter);
      }

      return hasErrors;
    }

    protected bool BasicChecks(IAppMessageWriter appMessageWriter)
    {
      bool hasErrors = false;
      var realTimeClockFactory = (IRealTimeClockFactory)this.serviceProvider.GetService(typeof(IRealTimeClockFactory));
      var realTimeClock = realTimeClockFactory.Create();

      var dt = realTimeClock.GetTime();
      if (dt.Year < 2022)
      {
        hasErrors = true;
        appMessageWriter.SendString("RTC clock does not have the correct time\n");
      }

      var settings = (Settings)this.serviceProvider.GetService(typeof(Settings));

      if (settings.Latitude == 0.0 || settings.Latitude == 0.0)
      {
        hasErrors = true;
        appMessageWriter.SendString("Geographical position is not set\n");
      }

      return hasErrors;
    }

    protected bool MotorMovementChecks(IAppMessageWriter appMessageWriter, MotorPlane plane)
    {
      bool hasErrors = false;
      var settings = (Settings)this.serviceProvider.GetService(typeof(Settings));

      switch (plane)
      {
        case MotorPlane.Azimuth:
          if (settings.Aci.Length < 2)
          {
            appMessageWriter.SendString("At least 2 calibrated points are needed for the Azimuth angles\n");
            hasErrors = true;
          }
          break;
        default:
          if (settings.Zci.Length < 2)
          {
            appMessageWriter.SendString("At least 2 calibrated points are needed for the Zenith angles\n");
            hasErrors = true;
          }
          break;
      }

      if (!settings.AreRangesSet(plane))
      {
        appMessageWriter.SendString($"Movement ranges are not set for plane {MotorPlaneNames.Name(plane)}\n");
        hasErrors = true;
      }

      return hasErrors;
    }

    protected bool HelioStatChecks(IAppMessageWriter appMessageWriter)
    {
      var settings = (Settings)this.serviceProvider.GetService(typeof(Settings));

      if (!settings.AreProjectionsSet())
      {
        appMessageWriter.SendString("Projection angles are not set\n");

        return true;
      }

      return false;
    }
  }
}

namespace NFHelio
{
  using NFHelio.Devices;
  using System;
  using System.Text;

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
    /// <returns>A non empty string if there are issues</returns>
    public string Check(MotorPlane plane)
    {
      var sb = new StringBuilder();

      var realTimeClockFactory = (IRealTimeClockFactory)this.serviceProvider.GetService(typeof(IRealTimeClockFactory));
      var realTimeClock = realTimeClockFactory.Create();

      var dt = realTimeClock.GetTime();
      if (dt.Year < 2022)
      {
        sb.AppendLine("RTC clock does not have the correct time");
      }

      var settings = (Settings)this.serviceProvider.GetService(typeof(Settings));

      if (settings.Latitude == 0.0 || settings.Latitude == 0.0)
      {
        sb.AppendLine("Position is not set");
      }

      switch (plane)
      {
        case MotorPlane.Azimuth:
          if (settings.Aci.Length < 2)
          {
            sb.AppendLine("At least 2 calibrated points are needed for the Azimuth angles");
          }
          break;
        default:
          if (settings.Zci.Length < 2)
          {
            sb.AppendLine("At least 2 calibrated points are needed for the Zenith angles\n");
          }
          break;
      }

      if (!settings.AreRangesSet(plane))
      {
        sb.AppendLine($"Movement ranges are not set for plane {plane.ToString()}");
      }

      return sb.ToString();
    }
  }
}

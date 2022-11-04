namespace NFHelio.Tasks
{
  using NFHelio.Devices;
  using NFSpa;
  using System;
  using System.Diagnostics;

  /// <summary>
  /// Calculates the position of the sun and some other parameters
  /// </summary>
  internal class CalcSpa : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "calcspa";

    /// <inheritdoc />
    public override string Description => "Calculates the position of the sun";

    /// <inheritdoc />
    public override string Help => "No further info";

    /// <summary>
    /// Initializes a new instance of the <see cref="CalcSpa" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public CalcSpa(IServiceProvider serviceProvider)
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

      double min, sec;

      var calculator = new Calculator();
      var result = calculator.Spa_calculate(spa);

      // check for SPA errors
      if (result == 0)
      {
        // display the results inside the SPA structure
        this.SendString(string.Format("Azimuth:       {0,12:F6}\n", spa.azimuth));
        this.SendString(string.Format("Zenith:        {0,12:F6}\n", spa.zenith));

        min = 60.0 * (spa.sunrise - (int)(spa.sunrise));
        sec = 60.0 * (min - (int)min);
        this.SendString(string.Format("Sunrise:       {0,2:D2}:{1,2:D2}:{2,2:D2}\n", (int)(spa.sunrise), (int)min, (int)sec));

        min = 60.0 * (spa.sunset - (int)(spa.sunset));
        sec = 60.0 * (min - (int)min);
        this.SendString(string.Format("Sunset:        {0,2:D2}:{1,2:D2}:{2,2:D2}\n", (int)(spa.sunset), (int)min, (int)sec));
      }
      else
      {
        Debug.WriteLine($"Calculating the sun's position failed with error code {result}");
      }
    }
  }
}

namespace NFHelio.Tasks
{
  using NFCommon.Storage;
  using NFHelio.Devices;
  using NFSpa;
  using System;
  using System.Diagnostics;
  using System.Threading;

  /// <summary>
  /// Follows the sun across the sky
  /// </summary>
  internal class FollowSun : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "followsun";

    /// <inheritdoc />
    public override string Description => "Follows the sun";

    /// <inheritdoc />
    public override string Help => "followsun <action>\nwhere action is start or stop";

    public FollowSun(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
    //  if (args.Length != 1)
    //  {
    //    this.SendString("Invalid number of arguments\n");
    //    return;
    //  }

    //  bool doStart = false;
    //  bool doStop = false;
    //  switch (args[0].ToLower())
    //  {
    //    case "start":
    //      doStart = true;
    //      break;
    //    case "stop":
    //      doStop = true;
    //      break;
    //  }

    //  if (doStart)
    //  {
    //    if (Program.context.SunFollowingThread != null)
    //    {
    //      this.SendString("Already running...\n");
    //      return;
    //    }

    //    Follower follower = new Follower(this.GetServiceProvider());

    //    Program.context.SunFollowingThread = new Thread(new ThreadStart(follower.Start));

    //    // Start the thread.
    //    Program.context.SunFollowingThread.Start();

    //    this.SendString("Following the sun...\n");

    //    return;
    //  }

    //  if (doStop)
    //  {
    //    if (Program.context.SunFollowingThread != null)
    //    {
    //      Program.context.SunFollowingThread.Abort();
    //      Program.context.SunFollowingThread = null;
    //    }
    //  }
    }
  }

  public class Follower
  {
    private readonly IServiceProvider serviceProvider;

    public Follower(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public void Start()
    {
      while (true)
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

        Thread.Sleep(10 * 1000);
      }
    }
  }
}

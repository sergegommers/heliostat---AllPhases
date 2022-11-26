namespace NFHelio.Tasks
{
  using nanoFramework.Hosting;
  using System;

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
      SunFollower sunFollower = null;

      // get the SunFollower from the DI container, note we have to search on IHostedService
      var serviceProvider = this.GetServiceProvider();
      var services = serviceProvider.GetService(new Type[] { typeof(IHostedService) });
      foreach (var service in services)
      {
        // is this IHostedService the SunFollower?
        SunFollower follower = service as SunFollower;

        if (follower != null)
        {
          sunFollower = follower;
        }
      }

      if (args.Length != 1)
      {
        this.SendString("Invalid number of arguments\n");
        return;
      }

      switch (args[0].ToLower())
      {
        case "start":
          sunFollower.RunEvery(TimeSpan.FromSeconds(60));
          this.SendString("Following the sun...\n");
          return;
        case "stop":
          sunFollower.Stop();
          return;
      }
    }
  }
}

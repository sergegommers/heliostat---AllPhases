﻿namespace NFHelio.Tasks
{
  using nanoFramework.Hosting;
  using NFCommon.Storage;
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
      HelioStat helioStat = null;

      if (args.Length != 1)
      {
        this.SendString("Invalid number of arguments\n");
        return;
      }

      // get the SunFollower from the DI container, note we have to search on IHostedService
      var serviceProvider = this.GetServiceProvider();
      var services = serviceProvider.GetService(new Type[] { typeof(IHostedService) });
      foreach (var service in services)
      {
        // is this IHostedService the SunFollower?
        if (service as SunFollower != null)
        {
          sunFollower = service as SunFollower;
        }

        // is this IHostedService the HelioStat?
        if (service as HelioStat != null)
        {
          helioStat = service as HelioStat;
        }
      }

      var settings = (Settings)this.GetServiceProvider().GetService(typeof(Settings));

      switch (args[0].ToLower())
      {
        case "start":
          helioStat.Stop();
          settings.ProjectSun = false;

          sunFollower.RunEvery(TimeSpan.FromSeconds(60));
          settings.FollowSun = true;

          this.SendString("Following the sun...\n");
          break;
        case "stop":
          sunFollower.Stop();
          settings.FollowSun = false;
          this.SendString("Stopped following the sun.\n");
          break;
      }

      var settingsStorage = (ISettingsStorage)this.GetServiceProvider().GetService(typeof(ISettingsStorage));
      settingsStorage.WriteSettings(settings);
    }
  }
}

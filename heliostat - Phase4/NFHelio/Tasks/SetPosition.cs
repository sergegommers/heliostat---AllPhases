namespace NFHelio.Tasks
{
  using NFCommon.Storage;
  using System;

  /// <summary>
  /// Stores the geographical position in the <see cref="Settings"/>
  /// </summary>
  internal class SetPosition : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "setpos";

    /// <inheritdoc />
    public override string Description => "Sets the position as latitude longitude";

    /// <inheritdoc />
    public override string Help => "setpos <latitude> <longitude>\nwith both values as doubles";

    /// <summary>
    /// Initializes a new instance of the <see cref="SetPosition"/> class.
    /// </summary>
    /// <param name="appMessageWriter">The application message writer.</param>
    public SetPosition(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      if (args.Length != 2)
      {
        this.SendString("To set the position, provide latitude longitude\n");
        return;
      }

      var settings = (Settings)this.GetServiceProvider().GetService(typeof(Settings));
      settings.Latitude = double.Parse(args[0]);
      settings.Longitude = double.Parse(args[1]);

      var settingsStorage = (ISettingsStorage)this.GetServiceProvider().GetService(typeof(ISettingsStorage));

      settingsStorage.WriteSettings(settings);

      var readBackSettings = settingsStorage.ReadSettings() as Settings;
      this.SendString($"Latitude longitude set to {readBackSettings.Latitude}, {readBackSettings.Longitude}\n");
    }
  }
}

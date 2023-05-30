namespace NFHelio.Tasks
{
  using NFCommon.Storage;
  using System;

  /// <summary>
  /// Sets the ranges of motion
  /// </summary>
  internal class ClearSettings : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "clear";

    /// <inheritdoc />
    public override string Description => "Clears the settings";

    /// <inheritdoc />
    public override string Help => "clear <action> where action is a (all) or c (calibration)";

    /// <summary>
    /// Initializes a new instance of the <see cref="SetPosition"/> class.
    /// </summary>
    /// <param name="appMessageWriter">The application message writer.</param>
    public ClearSettings(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      if (args.Length != 1)
      {
        this.SendString("Invalid number of arguments\n");
        return;
      }

      var currentSettings = (Settings)this.GetServiceProvider().GetService(typeof(Settings));

      switch (args[0].ToLower())
      {
        case "a":
          var settings = new Settings();
          currentSettings.Update(settings);
          break;
        case "c":
          currentSettings.ResetCalibration();
          break;
        default:
          this.SendString("Unknown action\n");
          return;
      }

      var settingsStorage = (ISettingsStorage)this.GetServiceProvider().GetService(typeof(ISettingsStorage));
      settingsStorage.WriteSettings(currentSettings);

      this.SendString($"OK\n");
    }
  }
}

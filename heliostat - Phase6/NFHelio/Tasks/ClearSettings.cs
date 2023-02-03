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
    public override string Description => "Clears all settings";

    /// <inheritdoc />
    public override string Help => "No further info";

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
      var settings = new Settings();

      var currentSettings = (Settings)this.GetServiceProvider().GetService(typeof(Settings));
      currentSettings.Update(settings);

      var settingsStorage = (ISettingsStorage)this.GetServiceProvider().GetService(typeof(ISettingsStorage));
      settingsStorage.WriteSettings(settings);

      this.SendString($"OK\n");
    }
  }
}

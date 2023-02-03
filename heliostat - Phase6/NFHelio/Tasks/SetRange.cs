namespace NFHelio.Tasks
{
  using NFCommon.Storage;
  using System;

  /// <summary>
  /// Sets the ranges of motion
  /// </summary>
  internal class SetRange : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "setrange";

    /// <inheritdoc />
    public override string Description => "Sets the min and max ranges of motion";

    /// <inheritdoc />
    public override string Help => "setrange <amin, amax, zmin, zmax>";

    /// <summary>
    /// Initializes a new instance of the <see cref="SetPosition"/> class.
    /// </summary>
    /// <param name="appMessageWriter">The application message writer.</param>
    public SetRange(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      if (args.Length != 1)
      {
        this.SendString("To set the range, provide the range specifier\n");
        return;
      }

      var settings = (Settings)this.GetServiceProvider().GetService(typeof(Settings));

      int adcChannel;
      switch (args[0])
      {
        case "amin":
          adcChannel = Context.AzimuthAdcChannel;
          settings.AzimuthAdcMin = (short)AdcReader.GetValue(adcChannel, Context.AdcSampleSize);
          break;
        case "amax":
          adcChannel = Context.AzimuthAdcChannel;
          settings.AzimuthAdcMax = (short)AdcReader.GetValue(adcChannel, Context.AdcSampleSize);
          break;
        case "zmin":
          adcChannel = Context.ZenithAdcChannel;
          settings.ZenithAdcMin = (short)AdcReader.GetValue(adcChannel, Context.AdcSampleSize);
          break;
        case "zmax":
          adcChannel = Context.ZenithAdcChannel;
          settings.ZenithAdcMax = (short)AdcReader.GetValue(adcChannel, Context.AdcSampleSize);
          break;
        default:
          this.SendString($"Unknown range specifier\n");
          return;
      }

      var settingsStorage = (ISettingsStorage)this.GetServiceProvider().GetService(typeof(ISettingsStorage));

      settingsStorage.WriteSettings(settings);

      this.SendString($"OK\n");
    }
  }
}

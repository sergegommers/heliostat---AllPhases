namespace NFHelio.Tasks
{
  using NFCommon;
  using NFCommon.Storage;
  using System;

  /// <summary>
  /// Calibrates the potentiometers that read out the azimuth and zenith values
  /// </summary>
  internal class Calibrate : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "cal";

    /// <inheritdoc />
    public override string Description => "Calibrates the azimuth and zenith angles";

    /// <inheritdoc />
    public override string Help => "cal <plane> <angle> where plane is a or z\nor r to reset existing values.\nAngle is the current angle.";

    public Calibrate(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      if (args.Length != 2)
      {
        this.SendString("Invalid number of arguments\n");
        return;
      }

      var settingsStorage = (ISettingsStorage)this.GetServiceProvider().GetService(typeof(ISettingsStorage));
      var settings = (Settings)this.GetServiceProvider().GetService(typeof(Settings));


      int channelNumber;
      switch (args[0].ToLower())
      {
        case "a":
          channelNumber = Context.AzimuthAdcChannel;
          break;
        case "z":
          channelNumber = Context.ZenithAdcChannel;
          break;
        case "r":
          settings.Aci = new short[0];
          settings.Acv = new short[0];
          settings.Zci = new short[0];
          settings.Zcv = new short[0];
          settingsStorage.WriteSettings(settings);

          this.SendString("Mirror calibration is cleared\n");
          return;
        default:
          this.SendString("Unknown plane to calibrate\n");
          return;
      }

      bool result = int.TryParse(args[1], out int angle);
      if (!result)
      {
        this.SendString("Can't convert given angle to an integer\n");
        return;
      }

      double value = AdcReader.GetValue(channelNumber, Context.AdcSampleSize);

      switch (args[0])
      {
        case "a":
          {
            var array = new CalibrationArray(settings.Aci, settings.Acv);
            array.AddCalibrationPoint((short)angle, (short)value);
            settings.Aci = array.GetIndexes();
            settings.Acv = array.GetValues();
            break;
          }
        case "z":
          {
            var array = new CalibrationArray(settings.Zci, settings.Zcv);
            array.AddCalibrationPoint((short)angle, (short)value);
            settings.Zci = array.GetIndexes();
            settings.Zcv = array.GetValues();
            break;
          }
      }

      settingsStorage.WriteSettings(settings);
      this.SendString($"Adc value for plane {args[0]}\nset to angle {(short)angle} and value {(short)value}\n");
    }
  }
}

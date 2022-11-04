namespace NFHelio.Storage
{
  using nanoFramework.Json;
  using NFCommon.Storage;
  using NFHelio.Devices;
  using System;
  using System.Diagnostics;
  using System.Text;

  /// <summary>
  /// An implementation of <see cref="ISettingsStorage"/> that read\writes the application settings to the configured <see cref="IEeprom"/> as a Json file
  /// </summary>
  /// <seealso cref="NFCommon.Storage.ISettingsStorage" />
  public class JsonSettingsStorage : ISettingsStorage
  {
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSettingsStorage"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public JsonSettingsStorage(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public SettingsBase ReadSettings()
    {
      try
      {
        var factory = (IEepromFactory)this.serviceProvider.GetService(typeof(IEepromFactory));
        var eeprom = factory.Create();

        ushort initAddress = 0x0;
        byte[] initBytes = eeprom.Read(initAddress, 2);
        if (initBytes == null || initBytes[0] != 0x55 || initBytes[1] != 0xAA)
        {
          Debug.WriteLine($"Settings can't be read, init bytes don't match");
          return null;
        }

        ushort lenghtAddress = 0x2;
        byte[] lenghtBytes = eeprom.Read(lenghtAddress, 4);
        var lenght = BitConverter.ToInt32(lenghtBytes, 0);

        ushort settingsAddress = 0x6;
        byte[] settingsBytes = eeprom.Read(settingsAddress, lenght);
        string serializedSetttings = Encoding.UTF8.GetString(settingsBytes, 0, lenght);

        Debug.WriteLine($"{serializedSetttings}");

        return (Settings)JsonConvert.DeserializeObject(serializedSetttings, typeof(Settings));
      }
      catch (Exception)
      {
        // This can happen when the settings contain new fields and we can't properly read\save them
        // To get around this, we clear all settings...
        var settings = new Settings();
        WriteSettings(settings);
        Debug.WriteLine($"Settings can't be read, resetting them.");

        return null;
      }
    }

    /// <inheritdoc />
    public void WriteSettings(SettingsBase settings)
    {
      var serializedSetttings = JsonConvert.SerializeObject(settings);
      var length = serializedSetttings.Length;

      var factory = (IEepromFactory)this.serviceProvider.GetService(typeof(IEepromFactory));
      var eeprom = factory.Create();

      ushort initAddress = 0x0;
      var initBytes = new byte[] { 0x55, 0xAA };
      eeprom.Write(initAddress, initBytes);

      ushort lenghtAddress = 0x2;
      byte[] lenghtBytes = BitConverter.GetBytes(length);
      eeprom.Write(lenghtAddress, lenghtBytes);

      ushort settingsAddress = 0x6;
      byte[] settingsBytes = Encoding.UTF8.GetBytes(serializedSetttings);
      eeprom.Write(settingsAddress, settingsBytes);
    }
  }
}

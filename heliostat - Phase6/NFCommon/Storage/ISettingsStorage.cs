namespace NFCommon.Storage
{
  /// <summary>
  /// An interface for storing the settings of an application
  /// </summary>
  public interface ISettingsStorage
  {
    /// <summary>
    /// Reads the settings.
    /// </summary>
    /// <returns>An instance of <see cref="SettingsBase"/></returns>
    public SettingsBase ReadSettings();

    /// <summary>
    /// Writes the settings.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public void WriteSettings(SettingsBase settings);
  }
}

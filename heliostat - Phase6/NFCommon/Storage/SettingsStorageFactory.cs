namespace NFCommon.Storage
{
  /// <summary>
  /// The abstract base class for a <see cref="ISettingsStorage"/> factory
  /// </summary>
  public abstract class SettingsStorageFactory
  {
    /// <summary>
    /// Gets the settings storage.
    /// </summary>
    /// <returns></returns>
    public abstract ISettingsStorage GetSettingsStorage();
  }
}

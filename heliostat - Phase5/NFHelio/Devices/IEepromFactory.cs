namespace NFHelio.Devices
{
  /// <summary>
  /// An interface for creating Eeprom factories
  /// </summary>
  public interface IEepromFactory
  {
    /// <summary>
    /// Creates an instance of <see cref="IEeprom"/>
    /// </summary>
    public IEeprom Create();
  }
}

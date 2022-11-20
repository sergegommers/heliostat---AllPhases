namespace NFHelio.Devices
{
  /// <summary>
  /// Implements the <see cref="IEepromFactory"/> for the AT24C32 Eeprom
  /// </summary>
  internal class AT24C32EepromFactory : IEepromFactory
  {
    /// <inheritdoc />
    public IEeprom Create()
    {
      return new AT24C32(Context.EepromAddress, 1);
    }
  }
}

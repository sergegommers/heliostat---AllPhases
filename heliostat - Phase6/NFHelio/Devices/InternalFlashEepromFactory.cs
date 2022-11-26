namespace NFHelio.Devices
{
  /// <summary>
  /// Implements the <see cref="IEepromFactory"/> for the internal flash
  /// </summary>
  public class InternalFlashEepromFactory : IEepromFactory
  {
    /// <inheritdoc />
    public IEeprom Create()
    {
      return new InternalFlash();
    }
  }
}

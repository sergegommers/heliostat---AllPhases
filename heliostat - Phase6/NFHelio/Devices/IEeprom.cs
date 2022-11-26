namespace NFHelio.Devices
{
  /// <summary>
  /// An interface for working with EEProms
  /// </summary>
  public interface IEeprom
  {
    /// <summary>
    /// Write at a specific address.
    /// </summary>
    /// <param name="memoryAddress">The address to write.</param>
    /// <param name="messageToSent">The byte buffer to write.</param>
    public void Write(ushort memoryAddress, byte[] messageToSent);

    /// <summary>
    /// Read a specific address.
    /// </summary>
    /// <param name="memoryAddress">The address to read.</param>
    /// <param name="numOfBytes">The number of bytes to read.</param>
    /// <returns>The read elements.</returns>
    public byte[] Read(ushort memoryAddress, int numOfBytes);
  }
}

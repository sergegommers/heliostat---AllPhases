namespace NFHelio.Devices
{
  using System;
  using System.Device.I2c;
  using System.Threading;

  /// <summary>
  /// Implements the <see cref="IEeprom"/> interface for the AT24C32 - I2C Eeprom
  /// </summary>
  public class AT24C32 : IEeprom
  {
    private readonly I2cDevice _memoryController;

    /// <summary>
    /// Creates a driver for the AT24C32.
    /// </summary>
    /// <param name="address">The I2C address of the device.</param>
    /// <param name="i2cBus">The I2C bus where the device is connected to.</param>
    public AT24C32(int address, int i2cBus)
    {
      var settings = new I2cConnectionSettings(i2cBus, address);

      // Instantiate I2C controller
      _memoryController = I2cDevice.Create(settings);
    }

    /// <summary>
    /// Write at a specific address.
    /// </summary>
    /// <param name="memoryAddress">The address to write.</param>
    /// <param name="messageToSent">The byte buffer to write.</param>
    public void Write(ushort memoryAddress, byte[] messageToSent)
    {
      // This chip allows to write 32 bytes in 1 go,
      // but each write must fit in an address range where the lower addresses are in the 0 - 31 range.
      // So if you write 32 bytes to address 1, the last byte will be put in address 0 instead of 32.
      // Given that our messageToSent can be of any lenght, and the starting address is completely random,
      // we must chop the messageToSent in pieces that fit into the 0 - 31 range.

      int segmentStart = memoryAddress & 0x1F;
      int messageIndex = 0;
      int messageLenght = messageToSent.Length;

      int bytesToCopy = Math.Min(32 - segmentStart, messageToSent.Length);
      while (bytesToCopy > 0)
      {
        byte[] txBuffer = new byte[bytesToCopy];

        for (int i = 0; i < bytesToCopy; i++)
        {
          txBuffer[i] = messageToSent[messageIndex + i];
        }

        InternalWrite(memoryAddress, txBuffer);

        messageIndex += bytesToCopy;
        memoryAddress += (ushort)bytesToCopy;
        messageLenght -= bytesToCopy;
        bytesToCopy = Math.Min(32, messageLenght);
      }

      Thread.Sleep(100);
    }

    /// <summary>
    /// Read a specific address.
    /// </summary>
    /// <param name="memoryAddress">The address to read.</param>
    /// <param name="numOfBytes">The number of bytes to read.</param>
    /// <returns>The read elements.</returns>
    public byte[] Read(ushort memoryAddress, int numOfBytes)
    {
      Thread.Sleep(100);

      byte[] rxBuffer = new byte[numOfBytes];
      // Device address is followed by the memory address (two words)
      // and must be sent over the I2C bus before data reception
      byte[] txBuffer = new byte[2];
      txBuffer[0] = (byte)(memoryAddress >> 8 & 0xFF);
      txBuffer[1] = (byte)(memoryAddress & 0xFF);
      _memoryController.WriteRead(txBuffer, rxBuffer);

      return rxBuffer;
    }

    protected void InternalWrite(ushort memoryAddress, byte[] messageToSent)
    {
      byte[] txBuffer = new byte[2 + messageToSent.Length];
      txBuffer[0] = (byte)(memoryAddress >> 8 & 0xFF);
      txBuffer[1] = (byte)(memoryAddress & 0xFF);
      messageToSent.CopyTo(txBuffer, 2);
      _memoryController.Write(txBuffer);
    }
  }
}

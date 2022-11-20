namespace NFHelio.Devices
{
  using System.IO;

  /// <summary>
  /// An implementation of <see cref="IEeprom"/> that mimics an Eeprom by writing files to internal flash
  /// </summary>
  internal class InternalFlash : IEeprom
  {
    // roothpath 'I:' means we want to work with the internal flash
    private string rootPath = "I:";

    /// <inheritdoc />
    public byte[] Read(ushort memoryAddress, int numOfBytes)
    {
      string path = this.GetPath(memoryAddress);

      if (!File.Exists(path))
      {
        return null;
      }

      var fs = new FileStream(path, FileMode.Open, FileAccess.Read);

      byte[] buffer = new byte[fs.Length];
      fs.Read(buffer, 0, (int)fs.Length);

      return buffer;
    }

    /// <inheritdoc />
    public void Write(ushort memoryAddress, byte[] buffer)
    {
      string path = this.GetPath(memoryAddress);

      File.Create(path);
      var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
      fs.Write(buffer, 0, buffer.Length);
      fs.Dispose();
    }

    private string GetPath(ushort memoryAddress)
    {
      string path = $"{rootPath}\\{memoryAddress}.txt";

      return path;
    }
  }
}

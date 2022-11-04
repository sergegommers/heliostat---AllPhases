namespace NFCommon.Services
{
  /// <summary>
  /// An interface for writing application level messages
  /// </summary>
  public interface IAppMessageWriter
  {
    /// <summary>
    /// Sends the string.
    /// </summary>
    /// <param name="message">The message.</param>
    void SendString(string message);
  }
}

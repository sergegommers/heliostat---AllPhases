namespace NFHelio.Devices
{
  /// <summary>
  /// An interface for creating real time clock factories
  /// </summary>
  public interface IRealTimeClockFactory
  {
    /// <summary>
    /// Creates an instance of <see cref="IRealTimeClock"/>
    /// </summary>
    /// <returns></returns>
    public IRealTimeClock Create();
  }
}

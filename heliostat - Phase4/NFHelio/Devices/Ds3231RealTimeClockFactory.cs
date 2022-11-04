namespace NFHelio.Devices
{
  /// <summary>
  /// Implements the <see cref="IRealTimeClockFactory"/> for the Ds3231 real time clock
  /// </summary>
  public class Ds3231RealTimeClockFactory : IRealTimeClockFactory
  {
    /// <inheritdoc />
    public IRealTimeClock Create()
    {
      return new Ds3231RealTimeClock(Context.RtcAddress, 1);
    }
  }
}

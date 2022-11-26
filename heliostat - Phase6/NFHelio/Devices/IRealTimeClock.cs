namespace NFHelio.Devices
{
  using System;

  /// <summary>
  /// An interface for working with real time clocks
  /// </summary>
  public interface IRealTimeClock
  {
    /// <summary>
    /// Sets the time.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <param name="second">The second.</param>
    public void SetTime(int year, int month, int day, int hour, int minute, int second);

    /// <summary>
    /// Gets the time.
    /// </summary>
    public DateTime GetTime();
  }
}

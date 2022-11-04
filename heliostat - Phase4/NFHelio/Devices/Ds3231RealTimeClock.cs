namespace NFHelio.Devices
{
  using Iot.Device.Rtc;
  using System;
  using System.Device.I2c;

  /// <summary>
  /// Implements the <see cref="IRealTimeClock"/> interface for the Ds3231 real time clock
  /// </summary>
  public class Ds3231RealTimeClock : IRealTimeClock
  {
    private readonly Ds3231 ds3231;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ds3231RealTimeClock"/> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="i2cBus">The i2c bus.</param>
    public Ds3231RealTimeClock(int address, int i2cBus)
    {
      var settings = new I2cConnectionSettings(i2cBus, address);
      var device = I2cDevice.Create(settings);

      ds3231 = new Ds3231(device);
    }

    /// <inheritdoc />
    public void SetTime(int year, int month, int day, int hour, int minute, int second)
    {
      var dt = new DateTime(year, month, day, hour, minute, second);

      ds3231.DateTime = dt;
    }

    /// <inheritdoc />
    public DateTime GetTime()
    {
      var dt = ds3231.DateTime;

      return dt;
    }
  }
}

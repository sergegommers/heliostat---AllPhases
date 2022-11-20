namespace NFHelio
{
  using NFCommon.Storage;

  /// <summary>
  /// The settings for this application
  /// </summary>
  /// <seealso cref="NFCommon.Storage.SettingsBase" />
  public class Settings : SettingsBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/> class.
    /// </summary>
    public Settings()
    {
    }

    /// <summary>
    /// Updates the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    public void Update(Settings other)
    {
      this.Longitude = other.Longitude;
      this.Latitude = other.Latitude;
    }

    // Observer longitude (negative west of Greenwich)
    // valid range: -180 to 180 degrees
    public double Longitude
    {
      get;
      set;
    }

    // Observer latitude (negative south of equator)
    // valid range: -90 to 90 degrees
    public double Latitude
    {
      get;
      set;
    }
  }
}

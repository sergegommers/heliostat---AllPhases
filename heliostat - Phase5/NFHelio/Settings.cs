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
      this.Aci = new short[0];
      this.Acv = new short[0];
      this.Zci = new short[0];
      this.Zcv = new short[0];
    }

    /// <summary>
    /// Updates the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    public void Update(Settings other)
    {
      this.Aci = other.Aci;
      this.Acv = other.Acv;
      this.Zci = other.Zci;
      this.Zcv = other.Zcv;
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

    /// <summary>
    /// Gets or sets the array with azimuth indexes
    /// </summary>
    public short[] Aci
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the array with azimuth values
    /// </summary>
    public short[] Acv
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the array with zenith indexes
    /// </summary>
    public short[] Zci
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the array with zenith values
    /// </summary>
    public short[] Zcv
    {
      get;
      set;
    }
  }
}

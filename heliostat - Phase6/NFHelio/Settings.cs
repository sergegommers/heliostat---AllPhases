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
      this.AzimuthAdcMin = other.AzimuthAdcMin;
      this.AzimuthAdcMax = other.AzimuthAdcMax;
      this.ZenithAdcMin = other.ZenithAdcMin;
      this.ZenithAdcMax = other.ZenithAdcMax;
      this.FollowSun = other.FollowSun;
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

    /// <summary>
    /// Gets or sets the minimum Adc azimuth value.
    /// </summary>
    public short AzimuthAdcMin
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the maximum Adc azimuth value.
    /// </summary>
    public short AzimuthAdcMax
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the minimum Adc zenith value.
    /// </summary>
    public short ZenithAdcMin
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the maximum Adc zenith value.
    /// </summary>
    public short ZenithAdcMax
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether we follow the sun or not
    /// </summary>
    public bool FollowSun
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether we need to project the sun or not
    /// </summary>
    public bool ProjectSun
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the azimuth angle to project to.
    /// </summary>
    public double AzimuthProjection
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the zenith angle to project to.
    /// </summary>
    public double ZenithProjection
    {
      get;
      set;
    }

    /// <summary>
    /// Returns true if the movement ranges are set
    /// </summary>
    public bool AreRangesSet(MotorPlane plane)
    {
      switch (plane)
      {
        case MotorPlane.Azimuth:
          return AzimuthAdcMin != AzimuthAdcMax;
        default:
          return ZenithAdcMin != ZenithAdcMax;
      }
    }

    /// <summary>
    /// Returns true if the projection angles are set
    /// </summary>
    public bool AreProjectionsSet()
    {
      return AzimuthProjection != ZenithProjection;
    }
  }
}

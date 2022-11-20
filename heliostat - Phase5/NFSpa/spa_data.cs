namespace NFSpa
{
  public class Spa_data
  {
    //----------------------INPUT VALUES------------------------

    public int year;            // 4-digit year,      valid range: -2000 to 6000, error code: 1
    public int month;           // 2-digit month,         valid range: 1 to  12,  error code: 2
    public int day;             // 2-digit day,           valid range: 1 to  31,  error code: 3
    public int hour;            // Observer local hour,   valid range: 0 to  24,  error code: 4
    public int minute;          // Observer local minute, valid range: 0 to  59,  error code: 5
    public double second;       // Observer local second, valid range: 0 to <60,  error code: 6

    public double delta_ut1;    // Fractional second difference between UTC and UT which is used
                                // to adjust UTC for earth's irregular rotation rate and is derived
                                // from observation only and is reported in this bulletin:
                                // http://maia.usno.navy.mil/ser7/ser7.dat,
                                // where delta_ut1 = DUT1
                                // valid range: -1 to 1 second (exclusive), error code 17

    public double delta_t;      // Difference between earth rotation time and terrestrial time
                                // It is derived from observation only and is reported in this
                                // bulletin: http://maia.usno.navy.mil/ser7/ser7.dat,
                                // where delta_t = 32.184 + (TAI-UTC) - DUT1
                                // valid range: -8000 to 8000 seconds, error code: 7

    public double timezone;     // Observer time zone (negative west of Greenwich)
                                // valid range: -18   to   18 hours,   error code: 8

    public double longitude;    // Observer longitude (negative west of Greenwich)
                                // valid range: -180  to  180 degrees, error code: 9

    public double latitude;     // Observer latitude (negative south of equator)
                                // valid range: -90   to   90 degrees, error code: 10

    public double elevation;    // Observer elevation [meters]
                                // valid range: -6500000 or higher meters,    error code: 11

    public double pressure;     // Annual average local pressure [millibars]
                                // valid range:    0 to 5000 millibars,       error code: 12

    public double temperature;  // Annual average local temperature [degrees Celsius]
                                // valid range: -273 to 6000 degrees Celsius, error code; 13

    public double slope;        // Surface slope (measured from the horizontal plane)
                                // valid range: -360 to 360 degrees, error code: 14

    public double azm_rotation; // Surface azimuth rotation (measured from south to projection of
                                //     surface normal on horizontal plane, negative east)
                                // valid range: -360 to 360 degrees, error code: 15

    public double atmos_refract;// Atmospheric refraction at sunrise and sunset (0.5667 deg is typical)
                                // valid range: -5   to   5 degrees, error code: 16

    public int function;        // Switch to choose functions for desired output (from enumeration)

    //-----------------Intermediate OUTPUT VALUES--------------------

    public double jd;          //Julian day
    public double jc;          //Julian century

    public double jde;         //Julian ephemeris day
    public double jce;         //Julian ephemeris century
    public double jme;         //Julian ephemeris millennium

    public double l;           //earth heliocentric longitude [degrees]
    public double b;           //earth heliocentric latitude [degrees]
    public double r;           //earth radius vector [Astronomical Units, AU]

    public double theta;       //geocentric longitude [degrees]
    public double beta;        //geocentric latitude [degrees]

    public double x0;          //mean elongation (moon-sun) [degrees]
    public double x1;          //mean anomaly (sun) [degrees]
    public double x2;          //mean anomaly (moon) [degrees]
    public double x3;          //argument latitude (moon) [degrees]
    public double x4;          //ascending longitude (moon) [degrees]

    public double del_psi;     //nutation longitude [degrees]
    public double del_epsilon; //nutation obliquity [degrees]
    public double epsilon0;    //ecliptic mean obliquity [arc seconds]
    public double epsilon;     //ecliptic true obliquity  [degrees]

    public double del_tau;     //aberration correction [degrees]
    public double lamda;       //apparent sun longitude [degrees]
    public double nu0;         //Greenwich mean sidereal time [degrees]
    public double nu;          //Greenwich sidereal time [degrees]

    public double alpha;       //geocentric sun right ascension [degrees]
    public double delta;       //geocentric sun declination [degrees]

    public double h;           //observer hour angle [degrees]
    public double xi;          //sun equatorial horizontal parallax [degrees]
    public double del_alpha;   //sun right ascension parallax [degrees]
    public double delta_prime; //topocentric sun declination [degrees]
    public double alpha_prime; //topocentric sun right ascension [degrees]
    public double h_prime;     //topocentric local hour angle [degrees]

    public double e0;          //topocentric elevation angle (uncorrected) [degrees]
    public double del_e;       //atmospheric refraction correction [degrees]
    public double e;           //topocentric elevation angle (corrected) [degrees]

    public double eot;         //equation of time [minutes]
    public double srha;        //sunrise hour angle [degrees]
    public double ssha;        //sunset hour angle [degrees]
    public double sta;         //sun transit altitude [degrees]

    //---------------------Final OUTPUT VALUES------------------------

    public double zenith;       //topocentric zenith angle [degrees]
    public double azimuth_astro;//topocentric azimuth angle (westward from south) [for astronomers]
    public double azimuth;      //topocentric azimuth angle (eastward from north) [for navigators and solar radiation]
    public double incidence;    //surface incidence angle [degrees]

    public double suntransit;   //local sun transit time (or solar noon) [fractional hour]
    public double sunrise;      //local sunrise time (+/- 30 seconds) [fractional hour]
    public double sunset;       //local sunset time (+/- 30 seconds) [fractional hour]

    public Spa_data()
    {
      timezone = 0.0f;
      delta_ut1 = 0;
      delta_t = 67;
      elevation = 0;
      pressure = 1013;
      temperature = 11;
      slope = 0;
      azm_rotation = 0;
      atmos_refract = 0.5667f;
      function = (int)Calculator.SpaOutputs.SPA_ZA;
    }

    public Spa_data(Spa_data other)
    {
      year = other.year;
      month = other.month;
      day = other.day;
      hour = other.hour;
      minute = other.minute;
      second = other.second;
      delta_ut1 = other.delta_ut1;
      delta_t = other.delta_t;
      timezone = other.timezone;
      longitude = other.longitude;
      latitude = other.latitude;
      elevation = other.elevation;
      pressure = other.pressure;
      temperature = other.temperature;
      slope = other.slope;
      azm_rotation = other.azm_rotation;
      atmos_refract = other.atmos_refract;
      function = other.function;
      jd = other.jd;
      jc = other.jc;
      jde = other.jde;
      jce = other.jce;
      jme = other.jme;
      l = other.l;
      b = other.b;
      r = other.r;
      theta = other.theta;
      beta = other.beta;
      x0 = other.x0;
      x1 = other.x1;
      x2 = other.x2;
      x3 = other.x3;
      x4 = other.x4;
      del_psi = other.del_psi;
      del_epsilon = other.del_epsilon;
      epsilon0 = other.epsilon0;
      epsilon = other.epsilon;
      del_tau = other.del_tau;
      lamda = other.lamda;
      nu0 = other.nu0;
      nu = other.nu;
      alpha = other.alpha;
      delta = other.delta;
      h = other.h;
      xi = other.xi;
      del_alpha = other.del_alpha;
      delta_prime = other.delta_prime;
      alpha_prime = other.alpha_prime;
      h_prime = other.h_prime;
      e0 = other.e0;
      del_e = other.del_e;
      e = other.e;
      eot = other.eot;
      srha = other.srha;
      ssha = other.ssha;
      sta = other.sta;
      zenith = other.zenith;
      azimuth_astro = other.azimuth_astro;
      azimuth = other.azimuth;
      incidence = other.incidence;
      suntransit = other.suntransit;
      sunrise = other.sunrise;
      sunset = other.sunset;
    }
  };
}

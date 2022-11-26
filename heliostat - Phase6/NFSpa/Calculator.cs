namespace NFSpa
{
  using System;

  public class Calculator
  {
    const double PI = 3.1415926535897932384626433832795028841971;
    const double SUN_RADIUS = 0.26667;

    //enumeration for function codes to select desired final outputs from SPA
    public enum SpaOutputs
    {
      SPA_ZA,           //calculate zenith and azimuth
      SPA_ZA_INC,       //calculate zenith, azimuth, and incidence
      SPA_ZA_RTS,       //calculate zenith, azimuth, and sun rise/transit/set values
      SPA_ALL,          //calculate all SPA output values
    };

    public enum Terms { TERM_A, TERM_B, TERM_C, TERM_COUNT };
    public enum Term { TERM_X0, TERM_X1, TERM_X2, TERM_X3, TERM_X4, TERM_X_COUNT };
    public enum Term2 { TERM_PSI_A, TERM_PSI_B, TERM_EPS_C, TERM_EPS_D, TERM_PE_COUNT };
    public enum Julian { JD_MINUS, JD_ZERO, JD_PLUS, JD_COUNT };
    public enum Sun { SUN_TRANSIT, SUN_RISE, SUN_SET, SUN_COUNT };

    const int TERM_Y_COUNT = (int)Term.TERM_X_COUNT;
    readonly int[] l_subcount = { 64, 34, 20, 7, 3, 1 };
    readonly int[] b_subcount = { 5, 2 };
    readonly int[] r_subcount = { 40, 10, 6, 2, 1 };

    double Rad2deg(double radians)
    {
      return (180.0 / PI) * radians;
    }

    double Deg2rad(double degrees)
    {
      return (PI / 180.0) * degrees;
    }

    int Integer(double value)
    {
      return (int)(Math.Floor(value));
    }

    double Limit_degrees(double degrees)
    {
      double limited;

      degrees /= 360.0;
      limited = 360.0 * (degrees - Math.Floor(degrees));
      if (limited < 0) limited += 360.0;

      return limited;
    }
    double Limit_degrees180pm(double degrees)
    {
      double limited;

      degrees /= 360.0;
      limited = 360.0 * (degrees - Math.Floor(degrees));
      if (limited < -180.0) limited += 360.0;
      else if (limited > 180.0) limited -= 360.0;

      return limited;
    }

    double Limit_degrees180(double degrees)
    {
      double limited;

      degrees /= 180.0;
      limited = 180.0 * (degrees - Math.Floor(degrees));
      if (limited < 0) limited += 180.0;

      return limited;
    }

    double Limit_zero2one(double value)
    {
      double limited;

      limited = value - Math.Floor(value);
      if (limited < 0) limited += 1.0;

      return limited;
    }

    double Limit_minutes(double minutes)
    {
      double limited = minutes;

      if (limited < -20.0) limited += 1440.0;
      else if (limited > 20.0) limited -= 1440.0;

      return limited;
    }

    double Dayfrac_to_local_hr(double dayfrac, double timezone)
    {
      return 24.0 * Limit_zero2one(dayfrac + timezone / 24.0);
    }

    double Third_order_polynomial(double a, double b, double c, double d, double x)
    {
      return ((a * x + b) * x + c) * x + d;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    int Validate_inputs(Spa_data spa)
    {
      if ((spa.year < -2000) || (spa.year > 6000)) return 1;
      if ((spa.month < 1) || (spa.month > 12)) return 2;
      if ((spa.day < 1) || (spa.day > 31)) return 3;
      if ((spa.hour < 0) || (spa.hour > 24)) return 4;
      if ((spa.minute < 0) || (spa.minute > 59)) return 5;
      if ((spa.second < 0) || (spa.second >= 60)) return 6;
      if ((spa.pressure < 0) || (spa.pressure > 5000)) return 12;
      if ((spa.temperature <= -273) || (spa.temperature > 6000)) return 13;
      if ((spa.delta_ut1 <= -1) || (spa.delta_ut1 >= 1)) return 17;
      if ((spa.hour == 24) && (spa.minute > 0)) return 5;
      if ((spa.hour == 24) && (spa.second > 0)) return 6;

      if (Math.Abs(spa.delta_t) > 8000) return 7;
      if (Math.Abs(spa.timezone) > 18) return 8;
      if (Math.Abs(spa.longitude) > 180) return 9;
      if (Math.Abs(spa.latitude) > 90) return 10;
      if (Math.Abs(spa.atmos_refract) > 5) return 16;
      if (spa.elevation < -6500000) return 11;

      if ((spa.function == (int)SpaOutputs.SPA_ZA_INC) || (spa.function == (int)SpaOutputs.SPA_ALL))
      {
        if (Math.Abs(spa.slope) > 360) return 14;
        if (Math.Abs(spa.azm_rotation) > 360) return 15;
      }

      return 0;
    }

    double Julian_day(int year, int month, int day, int hour, int minute, double second, double dut1, double tz)
    {
      double day_decimal, julian_day, a;

      day_decimal = day + (hour - tz + (minute + (second + dut1) / 60.0) / 60.0) / 24.0;

      if (month < 3)
      {
        month += 12;
        year--;
      }

      double v3 = 365.25 * (year + 4716.0);

      var v1 = Integer(v3);
      var v2 = Integer(30.6001 * (month + 1));

      julian_day = v1 + v2 + day_decimal - 1524.5;

      if (julian_day > 2299160.0)
      {
        a = Integer(year / 100);
        julian_day += (2 - a + Integer(a / 4));
      }

      return julian_day;
    }
    double Julian_century(double jd)
    {
      return (jd - 2451545.0) / 36525.0;
    }

    double Julian_ephemeris_day(double jd, double delta_t)
    {
      return jd + delta_t / 86400.0;
    }

    double Julian_ephemeris_century(double jde)
    {
      return (jde - 2451545.0) / 36525.0;
    }

    double Julian_ephemeris_millennium(double jce)
    {
      return (jce / 10.0);
    }

    double Earth_periodic_term_summation(float[][] terms, int count, double jme)
    {
      int i;
      double sum = 0;

      for (i = 0; i < count; i++)
        sum += terms[i][(int)Terms.TERM_A] * Math.Cos(terms[i][(int)Terms.TERM_B] + terms[i][(int)Terms.TERM_C] * jme);

      return sum;
    }

    double Earth_values(double[] term_sum, int count, double jme)
    {
      int i;
      double sum = 0;

      for (i = 0; i < count; i++)
        sum += term_sum[i] * Math.Pow(jme, i);

      sum /= 1.0e8;

      return sum;
    }

    double Earth_heliocentric_longitude(double jme)
    {
      double[] sum = new double[PeriodicTerms.L_COUNT];
      int i;

      for (i = 0; i < PeriodicTerms.L_COUNT; i++)
        sum[i] = Earth_periodic_term_summation(PeriodicTerms.L_TERMS[i], l_subcount[i], jme);

      return Limit_degrees(Rad2deg(Earth_values(sum, PeriodicTerms.L_COUNT, jme)));

    }
    double Earth_heliocentric_latitude(double jme)
    {
      double[] sum = new double[PeriodicTerms.B_COUNT];
      int i;

      for (i = 0; i < PeriodicTerms.B_COUNT; i++)
        sum[i] = Earth_periodic_term_summation(PeriodicTerms.B_TERMS[i], b_subcount[i], jme);

      return Rad2deg(Earth_values(sum, PeriodicTerms.B_COUNT, jme));

    }

    double Earth_radius_vector(double jme)
    {
      double[] sum = new double[PeriodicTerms.R_COUNT];
      int i;

      for (i = 0; i < PeriodicTerms.R_COUNT; i++)
        sum[i] = Earth_periodic_term_summation(PeriodicTerms.R_TERMS[i], r_subcount[i], jme);

      return Earth_values(sum, PeriodicTerms.R_COUNT, jme);

    }

    double Geocentric_longitude(double l)
    {
      double theta = l + 180.0;

      if (theta >= 360.0) theta -= 360.0;

      return theta;
    }

    double Geocentric_latitude(double b)
    {
      return -b;
    }

    double Mean_elongation_moon_sun(double jce)
    {
      return Third_order_polynomial(1.0 / 189474.0, -0.0019142, 445267.11148, 297.85036, jce);
    }

    double Mean_anomaly_sun(double jce)
    {
      return Third_order_polynomial(-1.0 / 300000.0, -0.0001603, 35999.05034, 357.52772, jce);
    }

    double Mean_anomaly_moon(double jce)
    {
      return Third_order_polynomial(1.0 / 56250.0, 0.0086972, 477198.867398, 134.96298, jce);
    }

    double Argument_latitude_moon(double jce)
    {
      return Third_order_polynomial(1.0 / 327270.0, -0.0036825, 483202.017538, 93.27191, jce);
    }

    double Ascending_longitude_moon(double jce)
    {
      return Third_order_polynomial(1.0 / 450000.0, 0.0020708, -1934.136261, 125.04452, jce);
    }

    double Xy_term_summation(int i, double[] x)
    {
      int j;
      double sum = 0;

      for (j = 0; j < TERM_Y_COUNT; j++)
        sum += x[j] * PeriodicTerms.Y_TERMS[i][j];

      return sum;
    }

    void Nutation_longitude_and_obliquity(double jce, double[] x, ref double del_psi,
        ref double del_epsilon)
    {
      int i;
      double xy_term_sum, sum_psi = 0, sum_epsilon = 0;

      for (i = 0; i < PeriodicTerms.Y_COUNT; i++)
      {
        xy_term_sum = Deg2rad(Xy_term_summation(i, x));
        sum_psi += (PeriodicTerms.PE_TERMS[i][(int)Term2.TERM_PSI_A] + jce * PeriodicTerms.PE_TERMS[i][(int)Term2.TERM_PSI_B]) * Math.Sin(xy_term_sum);
        sum_epsilon += (PeriodicTerms.PE_TERMS[i][(int)Term2.TERM_EPS_C] + jce * PeriodicTerms.PE_TERMS[i][(int)Term2.TERM_EPS_D]) * Math.Cos(xy_term_sum);
      }

      del_psi = sum_psi / 36000000.0;
      del_epsilon = sum_epsilon / 36000000.0;
    }

    double Ecliptic_mean_obliquity(double jme)
    {
      double u = jme / 10.0;

      return 84381.448 + u * (-4680.93 + u * (-1.55 + u * (1999.25 + u * (-51.38 + u * (-249.67 +
          u * (-39.05 + u * (7.12 + u * (27.87 + u * (5.79 + u * 2.45)))))))));
    }

    double Ecliptic_true_obliquity(double delta_epsilon, double epsilon0)
    {
      return delta_epsilon + epsilon0 / 3600.0;
    }

    double Aberration_correction(double r)
    {
      return -20.4898 / (3600.0 * r);
    }

    double Apparent_sun_longitude(double theta, double delta_psi, double delta_tau)
    {
      return theta + delta_psi + delta_tau;
    }

    double Greenwich_mean_sidereal_time(double jd, double jc)
    {
      return Limit_degrees(280.46061837 + 360.98564736629 * (jd - 2451545.0) +
          jc * jc * (0.000387933 - jc / 38710000.0));
    }

    double Greenwich_sidereal_time(double nu0, double delta_psi, double epsilon)
    {
      return nu0 + delta_psi * Math.Cos(Deg2rad(epsilon));
    }

    double Geocentric_right_ascension(double lamda, double epsilon, double beta)
    {
      double lamda_rad = Deg2rad(lamda);
      double epsilon_rad = Deg2rad(epsilon);

      return Limit_degrees(Rad2deg(Math.Atan2(Math.Sin(lamda_rad) * Math.Cos(epsilon_rad) -
          Math.Tan(Deg2rad(beta)) * Math.Sin(epsilon_rad), Math.Cos(lamda_rad))));
    }

    double Geocentric_declination(double beta, double epsilon, double lamda)
    {
      double beta_rad = Deg2rad(beta);
      double epsilon_rad = Deg2rad(epsilon);

      return Rad2deg(Math.Asin(Math.Sin(beta_rad) * Math.Cos(epsilon_rad) +
          Math.Cos(beta_rad) * Math.Sin(epsilon_rad) * Math.Sin(Deg2rad(lamda))));
    }

    double Observer_hour_angle(double nu, double longitude, double alpha_deg)
    {
      return Limit_degrees(nu + longitude - alpha_deg);
    }

    double Sun_equatorial_horizontal_parallax(double r)
    {
      return 8.794 / (3600.0 * r);
    }

    void Right_ascension_parallax_and_topocentric_dec(double latitude, double elevation,
        double xi, double h, double delta, ref double delta_alpha, ref double delta_prime)
    {
      double delta_alpha_rad;
      double lat_rad = Deg2rad(latitude);
      double xi_rad = Deg2rad(xi);
      double h_rad = Deg2rad(h);
      double delta_rad = Deg2rad(delta);
      double u = Math.Atan(0.99664719 * Math.Tan(lat_rad));
      double y = 0.99664719 * Math.Sin(u) + elevation * Math.Sin(lat_rad) / 6378140.0;
      double x = Math.Cos(u) + elevation * Math.Cos(lat_rad) / 6378140.0;

      delta_alpha_rad = Math.Atan2(-x * Math.Sin(xi_rad) * Math.Sin(h_rad),
          Math.Cos(delta_rad) - x * Math.Sin(xi_rad) * Math.Cos(h_rad));

      delta_prime = Rad2deg(Math.Atan2((Math.Sin(delta_rad) - y * Math.Sin(xi_rad)) * Math.Cos(delta_alpha_rad),
          Math.Cos(delta_rad) - x * Math.Sin(xi_rad) * Math.Cos(h_rad)));

      delta_alpha = Rad2deg(delta_alpha_rad);
    }

    double Topocentric_right_ascension(double alpha_deg, double delta_alpha)
    {
      return alpha_deg + delta_alpha;
    }

    double Topocentric_local_hour_angle(double h, double delta_alpha)
    {
      return h - delta_alpha;
    }

    double Topocentric_elevation_angle(double latitude, double delta_prime, double h_prime)
    {
      double lat_rad = Deg2rad(latitude);
      double delta_prime_rad = Deg2rad(delta_prime);

      return Rad2deg(Math.Asin(Math.Sin(lat_rad) * Math.Sin(delta_prime_rad) +
          Math.Cos(lat_rad) * Math.Cos(delta_prime_rad) * Math.Cos(Deg2rad(h_prime))));
    }

    double Atmospheric_refraction_correction(double pressure, double temperature,
        double atmos_refract, double e0)
    {
      double del_e = 0;

      if (e0 >= -1 * (SUN_RADIUS + atmos_refract))
        del_e = (pressure / 1010.0) * (283.0 / (273.0 + temperature)) *
        1.02 / (60.0 * Math.Tan(Deg2rad(e0 + 10.3 / (e0 + 5.11))));

      return del_e;
    }

    double Topocentric_elevation_angle_corrected(double e0, double delta_e)
    {
      return e0 + delta_e;
    }

    double Topocentric_zenith_angle(double e)
    {
      return 90.0 - e;
    }

    double Topocentric_azimuth_angle_astro(double h_prime, double latitude, double delta_prime)
    {
      double h_prime_rad = Deg2rad(h_prime);
      double lat_rad = Deg2rad(latitude);

      return Limit_degrees(Rad2deg(Math.Atan2(Math.Sin(h_prime_rad),
          Math.Cos(h_prime_rad) * Math.Sin(lat_rad) - Math.Tan(Deg2rad(delta_prime)) * Math.Cos(lat_rad))));
    }

    double Topocentric_azimuth_angle(double azimuth_astro)
    {
      return Limit_degrees(azimuth_astro + 180.0);
    }

    double Surface_incidence_angle(double zenith, double azimuth_astro, double azm_rotation,
        double slope)
    {
      double zenith_rad = Deg2rad(zenith);
      double slope_rad = Deg2rad(slope);

      return Rad2deg(Math.Acos(Math.Cos(zenith_rad) * Math.Cos(slope_rad) +
          Math.Sin(slope_rad) * Math.Sin(zenith_rad) * Math.Cos(Deg2rad(azimuth_astro - azm_rotation))));
    }

    double Sun_mean_longitude(double jme)
    {
      return Limit_degrees(280.4664567 + jme * (360007.6982779 + jme * (0.03032028 +
          jme * (1 / 49931.0 + jme * (-1 / 15300.0 + jme * (-1 / 2000000.0))))));
    }

    double Eot(double m, double alpha, double del_psi, double epsilon)
    {
      return Limit_minutes(4.0 * (m - 0.0057183 - alpha + del_psi * Math.Cos(Deg2rad(epsilon))));
    }

    double Approx_sun_transit_time(double alpha_zero, double longitude, double nu)
    {
      return (alpha_zero - longitude - nu) / 360.0;
    }

    double Sun_hour_angle_at_rise_set(double latitude, double delta_zero, double h0_prime)
    {
      double h0 = -99999;
      double latitude_rad = Deg2rad(latitude);
      double delta_zero_rad = Deg2rad(delta_zero);
      double argument = (Math.Sin(Deg2rad(h0_prime)) - Math.Sin(latitude_rad) * Math.Sin(delta_zero_rad)) /
          (Math.Cos(latitude_rad) * Math.Cos(delta_zero_rad));

      if (Math.Abs(argument) <= 1) h0 = Limit_degrees180(Rad2deg(Math.Acos(argument)));

      return h0;
    }

    void Approx_sun_rise_and_set(double[] m_rts, double h0)
    {
      double h0_dfrac = h0 / 360.0;

      m_rts[(int)Sun.SUN_RISE] = Limit_zero2one(m_rts[(int)Sun.SUN_TRANSIT] - h0_dfrac);
      m_rts[(int)Sun.SUN_SET] = Limit_zero2one(m_rts[(int)Sun.SUN_TRANSIT] + h0_dfrac);
      m_rts[(int)Sun.SUN_TRANSIT] = Limit_zero2one(m_rts[(int)Sun.SUN_TRANSIT]);
    }

    double Rts_alpha_delta_prime(double[] ad, double n)
    {
      double a = ad[(int)Julian.JD_ZERO] - ad[(int)Julian.JD_MINUS];
      double b = ad[(int)Julian.JD_PLUS] - ad[(int)Julian.JD_ZERO];

      if (Math.Abs(a) >= 2.0) a = Limit_zero2one(a);
      if (Math.Abs(b) >= 2.0) b = Limit_zero2one(b);

      return ad[(int)Julian.JD_ZERO] + n * (a + b + (b - a) * n) / 2.0;
    }

    double Rts_sun_altitude(double latitude, double delta_prime, double h_prime)
    {
      double latitude_rad = Deg2rad(latitude);
      double delta_prime_rad = Deg2rad(delta_prime);

      return Rad2deg(Math.Asin(Math.Sin(latitude_rad) * Math.Sin(delta_prime_rad) +
          Math.Cos(latitude_rad) * Math.Cos(delta_prime_rad) * Math.Cos(Deg2rad(h_prime))));
    }

    double Sun_rise_and_set(double[] m_rts, double[] h_rts, double[] delta_prime, double latitude,
        double[] h_prime, double h0_prime, int sun)
    {
      return m_rts[sun] + (h_rts[sun] - h0_prime) /
          (360.0 * Math.Cos(Deg2rad(delta_prime[sun])) * Math.Cos(Deg2rad(latitude)) * Math.Sin(Deg2rad(h_prime[sun])));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Calculate required SPA parameters to get the right ascension (alpha) and declination (delta)
    // Note: JD must be already calculated and in structure
    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Calculate_geocentric_sun_right_ascension_and_declination(Spa_data spa)
    {
      double[] x = new double[(int)Term.TERM_X_COUNT];

      spa.jc = Julian_century(spa.jd);

      spa.jde = Julian_ephemeris_day(spa.jd, spa.delta_t);
      spa.jce = Julian_ephemeris_century(spa.jde);
      spa.jme = Julian_ephemeris_millennium(spa.jce);

      spa.l = Earth_heliocentric_longitude(spa.jme);
      spa.b = Earth_heliocentric_latitude(spa.jme);
      spa.r = Earth_radius_vector(spa.jme);

      spa.theta = Geocentric_longitude(spa.l);
      spa.beta = Geocentric_latitude(spa.b);

      x[(int)Term.TERM_X0] = spa.x0 = Mean_elongation_moon_sun(spa.jce);
      x[(int)Term.TERM_X1] = spa.x1 = Mean_anomaly_sun(spa.jce);
      x[(int)Term.TERM_X2] = spa.x2 = Mean_anomaly_moon(spa.jce);
      x[(int)Term.TERM_X3] = spa.x3 = Argument_latitude_moon(spa.jce);
      x[(int)Term.TERM_X4] = spa.x4 = Ascending_longitude_moon(spa.jce);

      Nutation_longitude_and_obliquity(spa.jce, x, ref spa.del_psi, ref spa.del_epsilon);

      spa.epsilon0 = Ecliptic_mean_obliquity(spa.jme);
      spa.epsilon = Ecliptic_true_obliquity(spa.del_epsilon, spa.epsilon0);

      spa.del_tau = Aberration_correction(spa.r);
      spa.lamda = Apparent_sun_longitude(spa.theta, spa.del_psi, spa.del_tau);
      spa.nu0 = Greenwich_mean_sidereal_time(spa.jd, spa.jc);
      spa.nu = Greenwich_sidereal_time(spa.nu0, spa.del_psi, spa.epsilon);

      spa.alpha = Geocentric_right_ascension(spa.lamda, spa.epsilon, spa.beta);
      spa.delta = Geocentric_declination(spa.beta, spa.epsilon, spa.lamda);
    }

    ////////////////////////////////////////////////////////////////////////
    // Calculate Equation of Time (EOT) and Sun Rise, Transit, & Set (RTS)
    ////////////////////////////////////////////////////////////////////////
    void Calculate_eot_and_sun_rise_transit_set(Spa_data spa)
    {
      double nu, m, h0, n;
      double[] alpha = new double[(int)Julian.JD_COUNT];
      double[] delta = new double[(int)Julian.JD_COUNT];
      double[] m_rts = new double[(int)Sun.SUN_COUNT];
      double[] nu_rts = new double[(int)Sun.SUN_COUNT];
      double[] h_rts = new double[(int)Sun.SUN_COUNT];
      double[] alpha_prime = new double[(int)Sun.SUN_COUNT];
      double[] delta_prime = new double[(int)Sun.SUN_COUNT];
      double[] h_prime = new double[(int)Sun.SUN_COUNT];
      double h0_prime = -1 * (SUN_RADIUS + spa.atmos_refract);
      int i;

      Spa_data sun_rts = new(spa);

      m = Sun_mean_longitude(spa.jme);
      spa.eot = Eot(m, spa.alpha, spa.del_psi, spa.epsilon);

      sun_rts.hour = sun_rts.minute = 0;
      sun_rts.second = 0.0;
      sun_rts.delta_ut1 = sun_rts.timezone = 0.0;

      sun_rts.jd = Julian_day(sun_rts.year, sun_rts.month, sun_rts.day, sun_rts.hour,
          sun_rts.minute, sun_rts.second, sun_rts.delta_ut1, sun_rts.timezone);

      Calculate_geocentric_sun_right_ascension_and_declination(sun_rts);
      nu = sun_rts.nu;

      sun_rts.delta_t = 0;
      sun_rts.jd--;
      for (i = 0; i < (int)Julian.JD_COUNT; i++)
      {
        Calculate_geocentric_sun_right_ascension_and_declination(sun_rts);
        alpha[i] = sun_rts.alpha;
        delta[i] = sun_rts.delta;
        sun_rts.jd++;
      }

      m_rts[(int)Sun.SUN_TRANSIT] = Approx_sun_transit_time(alpha[(int)Julian.JD_ZERO], spa.longitude, nu);
      h0 = Sun_hour_angle_at_rise_set(spa.latitude, delta[(int)Julian.JD_ZERO], h0_prime);

      if (h0 >= 0)
      {

        Approx_sun_rise_and_set(m_rts, h0);

        for (i = 0; i < (int)Sun.SUN_COUNT; i++)
        {

          nu_rts[i] = nu + 360.985647 * m_rts[i];

          n = m_rts[i] + spa.delta_t / 86400.0;
          alpha_prime[i] = Rts_alpha_delta_prime(alpha, n);
          delta_prime[i] = Rts_alpha_delta_prime(delta, n);

          h_prime[i] = Limit_degrees180pm(nu_rts[i] + spa.longitude - alpha_prime[i]);

          h_rts[i] = Rts_sun_altitude(spa.latitude, delta_prime[i], h_prime[i]);
        }

        spa.srha = h_prime[(int)Sun.SUN_RISE];
        spa.ssha = h_prime[(int)Sun.SUN_SET];
        spa.sta = h_rts[(int)Sun.SUN_TRANSIT];

        spa.suntransit = Dayfrac_to_local_hr(m_rts[(int)Sun.SUN_TRANSIT] - h_prime[(int)Sun.SUN_TRANSIT] / 360.0,
            spa.timezone);

        spa.sunrise = Dayfrac_to_local_hr(Sun_rise_and_set(m_rts, h_rts, delta_prime,
            spa.latitude, h_prime, h0_prime, (int)Sun.SUN_RISE), spa.timezone);

        spa.sunset = Dayfrac_to_local_hr(Sun_rise_and_set(m_rts, h_rts, delta_prime,
            spa.latitude, h_prime, h0_prime, (int)Sun.SUN_SET), spa.timezone);

      }
      else spa.srha = spa.ssha = spa.sta = spa.suntransit = spa.sunrise = spa.sunset = -99999;

    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    // Calculate all SPA parameters and put into structure
    // Note: All inputs values (listed in header file) must already be in structure
    ///////////////////////////////////////////////////////////////////////////////////////////
    public int Spa_calculate(Spa_data spa)
    {
      int result;

      result = Validate_inputs(spa);

      if (result == 0)
      {
        spa.jd = Julian_day(spa.year, spa.month, spa.day, spa.hour,
            spa.minute, spa.second, spa.delta_ut1, spa.timezone);

        Calculate_geocentric_sun_right_ascension_and_declination(spa);

        spa.h = Observer_hour_angle(spa.nu, spa.longitude, spa.alpha);
        spa.xi = Sun_equatorial_horizontal_parallax(spa.r);

        Right_ascension_parallax_and_topocentric_dec(spa.latitude, spa.elevation, spa.xi,
            spa.h, spa.delta, ref spa.del_alpha, ref spa.delta_prime);

        spa.alpha_prime = Topocentric_right_ascension(spa.alpha, spa.del_alpha);
        spa.h_prime = Topocentric_local_hour_angle(spa.h, spa.del_alpha);

        spa.e0 = Topocentric_elevation_angle(spa.latitude, spa.delta_prime, spa.h_prime);
        spa.del_e = Atmospheric_refraction_correction(spa.pressure, spa.temperature,
            spa.atmos_refract, spa.e0);
        spa.e = Topocentric_elevation_angle_corrected(spa.e0, spa.del_e);

        spa.zenith = Topocentric_zenith_angle(spa.e);
        spa.azimuth_astro = Topocentric_azimuth_angle_astro(spa.h_prime, spa.latitude,
            spa.delta_prime);
        spa.azimuth = Topocentric_azimuth_angle(spa.azimuth_astro);

        if ((spa.function == (int)SpaOutputs.SPA_ZA_INC) || (spa.function == (int)SpaOutputs.SPA_ALL))
          spa.incidence = Surface_incidence_angle(spa.zenith, spa.azimuth_astro,
              spa.azm_rotation, spa.slope);

        if ((spa.function == (int)SpaOutputs.SPA_ZA_RTS) || (spa.function == (int)SpaOutputs.SPA_ALL))
          Calculate_eot_and_sun_rise_transit_set(spa);
      }

      return result;
    }
  }
}

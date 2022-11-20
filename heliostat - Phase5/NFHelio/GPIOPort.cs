namespace NFHelio
{
  public enum GPIOPort
  {
    // The i2c clock
    I2C_Clock = 22,

    // The i2c data
    I2C_Data = 21,

    // The onboard Led on the ESP Devkit
    ESP32_Onboard_Led = 2,

    // The ADC for reading out the current Azimuth angle
    ADC_Azimuth = 36,

    // The ADC for reading out the current Zenith angle
    ADC_Zenith = 39,

    // Azimuth east to west
    PWM_Azimuth_East_to_West = 18,

    // Azimuth west to east
    PWM_Azimuth_West_to_East = 19,

    // Zenith moving up
    PWM_Zenith_Up = 13,

    // Zenith moving down
    PWM_Zenith_Down = 12,
  }
}

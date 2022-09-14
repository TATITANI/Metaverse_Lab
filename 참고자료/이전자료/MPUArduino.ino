#include <Wire.h>

void setup() {
  // Serial init
  Serial.begin(115200);
  // Wire init
  Wire.begin();
  // Power Management
  Wire.beginTransmission(0x68);
  Wire.write(0x6B);
  Wire.write(0);
  Wire.endTransmission();

  /* Register 26 for setting Frame Sync & DLPF */
  Wire.beginTransmission(0x68);
  Wire.write(26);
  Wire.write(0x03);
  Wire.endTransmission();
  
  /* Register 27 for setting Gyroscope Config
   * 0 << 3 for  250 deg/s, LSB means 131
   * 1 << 3 for  500 deg/s, LSB means  65.5
   * 2 << 3 for 1000 deg/s, LSB means  32.8
   * 3 << 3 for 2000 deg/s, LSB means  16.4 */
  Wire.beginTransmission(0x68);
  Wire.write(27);
  Wire.write(3 << 3);
  Wire.endTransmission();
  
  /* Register 28 for setting Accelerometer Config
   * 0 << 3 for  2g, LSB means 16384
   * 1 << 3 for  4g, LSB means  8192
   * 2 << 3 for  8g, LSB means  4096
   * 3 << 3 for 16g, LSB means  2048 */
  Wire.beginTransmission(0x68);
  Wire.write(28);
  Wire.write(1 << 3);
  Wire.endTransmission();
}

struct MPUrawData {
  uint8_t HEADER = 0xAA;
  
  int16_t dt = 0;
  
  int16_t acc_X = 0;
  int16_t acc_Y = 0;
  int16_t acc_Z = 0;
  int16_t gyr_X = 0;
  int16_t gyr_Y = 0;
  int16_t gyr_Z = 0;

  int16_t flex = 0;  
};

unsigned long p = 0;
unsigned long n = 0;
  
void loop() {
  uint8_t i;
  MPUrawData BUF;

  //get dT
  n = micros();
  BUF.dt = n - p;
  p = n;
  
  for(uint8_t i = 2; i <= 7; i++)
  {
    Wire.beginTransmission(0x68);
    Wire.write(26);
    Wire.write(i << 3 | 0x03);
    Wire.endTransmission();
  }
  
  // Get Accel
  Wire.beginTransmission(0x68);
  Wire.write(59);
  Wire.endTransmission();
  Wire.requestFrom(0x68, 6);
  BUF.acc_X = (Wire.read() << 8) | Wire.read();
  BUF.acc_Y = (Wire.read() << 8) | Wire.read();
  BUF.acc_Z = (Wire.read() << 8) | Wire.read();
  
  // Get Gyro
  Wire.beginTransmission(0x68);
  Wire.write(67);
  Wire.endTransmission();
  Wire.requestFrom(0x68, 6);
  BUF.gyr_X = (Wire.read() << 8) | Wire.read();
  BUF.gyr_Y = (Wire.read() << 8) | Wire.read();
  BUF.gyr_Z = (Wire.read() << 8) | Wire.read();

  BUF.flex = analogRead(A5);
  
  // Serial print
  Serial.write((byte*)&BUF, sizeof(BUF));
  /*
  char str[100] = "";moon
  sprintf(str, "Acc X: %d Y: %d Z: %d / Gyro X: %d Y: %d Z: %d",
  BUF.acc_raw[0], BUF.acc_raw[1], BUF.acc_raw[2],
  BUF.gyr_raw[0], BUF.gyr_raw[1], BUF.gyr_raw[2]);
  Serial.println(str);
  */
}

#include <SoftwareSerial.h>
int blueTx = 6;  //2
int blueRx = 7;  //3
SoftwareSerial mySerial(blueTx, blueRx);

const char startMarker = '<';
const char endMarker = '>';
int Poten_1_Threshold = 1023;
int Poten_2_Threshold = 1023;
int Poten_3_Threshold = 1023;

int potenTest[3] = { 10, 10, 10 };
////// 모터제어
#include <Servo.h>  // 서보모터 라이브러리 포함


///////EMG
#if defined(ARDUINO) && ARDUINO >= 100
#include "Arduino.h"
#else
#include "WProgram.h"
#endif
#include "EMGFilters.h"
#define TIMING_DEBUG 1
#define SensorInputPin A0    // input pin number
#define SensorInputPin_1 A1  // input pin number

EMGFilters myFilter;
// discrete filters must works with fixed sample frequence
// our emg filter only support "SAMPLE_FREQ_500HZ" or "SAMPLE_FREQ_1000HZ"
// other sampleRate inputs will bypass all the EMG_FILTER
int sampleRate = 500;  //SAMPLE_FREQ_500HZ;
// For countries where power transmission is at 50 Hz
// For countries where power transmission is at 60 Hz, need to change to
// "NOTCH_FREQ_60HZ"
// our emg filter only support 50Hz and 60Hz input
// other inputs will bypass all the EMG_FILTER
int humFreq = NOTCH_FREQ_50HZ;

// Calibration:
// put on the sensors, and release your muscles;
// wait a few seconds, and select the max value as the threshold;
// any value under threshold will be set to zero
static int Threshold_Min = 0;      //유효 최소값
static int Threshold_Max = 700;    //200000; //유효 최대값
static int Threshold_Min_1 = 0;    //유효 최소값
static int Threshold_Max_1 = 700;  //200000; //유효 최대값 //공 쥐는 거 : 700 / 집는거 손등 85


unsigned long timeStamp;
unsigned long timeBudget;
unsigned long timeMillis;
unsigned long startMillis;
unsigned long currentMillis;
unsigned long previousMillis;
const long interval = 10000;  //120,000=2분(120,000ms)
int count = 1;
unsigned long sum = 0;  // 합
unsigned long ave = 0;  // 평균
unsigned long maxEMG = 0;

char result[8];
char* ch1;

String sendData, recvData;
int pressure[3];
int elasticity = -1;  // 0: 강체, 1~99 탄성체, 100 : 자유

void parseData() {

  int first = recvData.indexOf(",");              // 첫 번째 콤마 위치
  int second = recvData.indexOf(",", first + 1);  // 두 번째 콤마 위치
  int third = recvData.indexOf(",", second + 1);  // 세 번째 콤마 위치
  int length = recvData.length();                 // 문자열 길이

  pressure[0] = recvData.substring(0, first).toInt();
  pressure[1] = recvData.substring(first + 1, second).toInt();
  pressure[2] = recvData.substring(second + 1, third).toInt();
  elasticity = recvData.substring(third + 1, length).toInt();

  //Serial.println("revc pressure  : " +  String(pressure[0]) + "," + String(pressure[1]) + "," + String(pressure[2]));
}

void setup() {
  Serial.begin(115200);
  mySerial.begin(115200);

  myFilter.init((SAMPLE_FREQUENCY)sampleRate, (NOTCH_FREQUENCY)humFreq, true, true, true);
  // setup for time cost measure
  // using micros()
  timeBudget = 1e6 / sampleRate;
  timeMillis = 0;
  startMillis = millis();
}

//////////////

float Clamp(float value, float low, float high) {
  if (value > high) {
    value = high;
  }
  if (value < low) {
    value = low;
  }
  return value;
}


float Map(float currentValue, float currentStart, float currentEnd, float targetStart, float targetEnd) {
  float ratio = abs((currentValue - currentStart) / (currentEnd - currentStart));

  float mappedValue = targetStart + ratio * (targetEnd - targetStart);

  return mappedValue;
}

//////////////////// 모터 제어 ///////////////////////
int testPressID = 0;
Servo servo[3];  // 서보모터를 배열로 정의
unsigned long lastMotorUpdatedTime = 0;
bool isPress[3] = { false, false, false };
int pressFirstAngle[3] = { 0, 0, 0 };          // 눌렀을 떄 최초 나사각도
int currentServoAngle[3] = { 180, 180, 180 };  // 현재 서보모터 각도
int prevPressure[3] = { 0, 0, 0 };
const int delayMotor = 200;  // milliseconds
const int angelOffset = 25;
void MotorControl() {

  if (millis() - lastMotorUpdatedTime > delayMotor) {
    
    // 가변저항
    int potentiometer_value[3];  // 가변저항 값
    // analogdRead) : 가변저항 0~1023범위
    potentiometer_value[0] = Clamp(analogRead(A2), 374, 1023);  // (엄지: A2), 손가락을 최대한 위로 땅겼을 때,손가락을 최대한 쥐었을 때
    potentiometer_value[1] = Clamp(analogRead(A3), 303, 970);   // (검지: A3)
    potentiometer_value[2] = Clamp(analogRead(A4), 437, 1023);  // (중지: A4)

    int potentAngle[3];                                               // 가변저항 나사 각도
    potentAngle[0] = Map(potentiometer_value[0], 1023, 374, 0, 180);  // 손풀었을 때 가변저항, 쥐었을 때 가변저항, 풀었을 때 각도, 쥐었을 때 각도
    potentAngle[1] = Map(potentiometer_value[1], 970, 303, 0, 180);
    potentAngle[2] = Map(potentiometer_value[2], 1023, 477, 20, 170);

    String msg = "";
    msg += String(analogRead(A2)) + ",";
    msg += String(analogRead(A3)) + ",";
    msg += String(analogRead(A4)) + "/";


    int servoRange = elasticity * 5;  // 서보모터 이동가능 범위
    for (int i = 0; i < 3; i++) {

      bool prevIsPress = isPress[i];
      isPress[i] = pressure[i] > 0;
      bool isFirstPress = (!prevIsPress && isPress[i]);

      if (isFirstPress)  // 물체를 처음 쥐었을 때 나사각도 저장
      {
        currentServoAngle[i] = pressFirstAngle[i] = potentAngle[i];
      }

      if (isPress[i]) {
        int diffPressure = pressure[i] - prevPressure[i];
        if (isFirstPress) {
          diffPressure = 4;
        }
        prevPressure[i] = pressure[i];
        int targetAngle = Clamp(currentServoAngle[i] + diffPressure * 0.7, pressFirstAngle[i], pressFirstAngle[i] + servoRange);
        targetAngle = Clamp(targetAngle, 10, 170);
        servo[i].write(targetAngle);

      } else {
        prevPressure[i] = 0;
        servo[i].write(180);
      }
    }
    lastMotorUpdatedTime = millis();
  }
}


void InputTest() {
  while (Serial.available()) {
    char x = Serial.read();
    //패킷 시작 문자열 초기화
    if (x == startMarker) {
      recvData = "";
    }
    // 패킷 종료시 전송
    else if (x == endMarker) {
      parseData();

    } else if (x == 'e') {
      //Serial.println("exit");
      exit(0);
    } else {
      recvData += x;
    }
  }
}


const int delayEMG_Micro = 50;
int lastTimeEMG = 0;
// EMG 블루투스 전송
void SendData() {

  //  if (micros() - lastTimeEMG < delayEMG_Micro)
  //  {
  //    return;
  //  }

  // In order to make sure the ADC sample frequence on arduino,
  // the time cost should be measured each loop
  /*------------start here-------------------*/
  timeStamp = micros();

  int Value = analogRead(SensorInputPin);
  //SensorInputPin_1
  int Value_1 = analogRead(SensorInputPin_1);

  // filter processing
  int DataAfterFilter = myFilter.update(Value);
  int DataAfterFilter_1 = myFilter.update(Value_1);

  int envlope = sq(DataAfterFilter);
  int envlope_1 = sq(DataAfterFilter_1);

  // any value under threshold will be set to zero
  envlope = (envlope > Threshold_Min) ? envlope : 0;
  envlope_1 = (envlope_1 > Threshold_Min_1) ? envlope_1 : 0;

  timeStamp = micros() - timeStamp;

  //if(envlope !=0 && envlope<Threshold_Max){
  if (envlope < Threshold_Max || envlope_1 < Threshold_Max_1) {
    //Serial.println("DATA,TIME,TIMER," + String(millis()*0.001));

    String str_envlope = String(envlope);
    String str_envlope_1 = String(envlope_1);

    //sendData = str_envlope + ',' + str_envlope_1;
    sendData = "#, " + str_envlope + ',' + str_envlope_1;  //유니티용
    char* a = new char[sendData.length() + 1];
    strcpy(a, sendData.c_str());
    //delay(200);  //excels
    if (maxEMG < envlope) {
      maxEMG = envlope;  //최대값 maxEMG
    }
    count++;
    sum = sum + envlope;

    Serial.println(a);

    delete a;
    lastTimeEMG = micros();

    // Serial.println("Filters cost time: " + String(timeStamp));
    // the filter cost average around 520 us
  }
}

bool isInit = false;
void Init() {

  const int angleInit = 170;
  servo[0].attach(9);   // 엄지
  servo[1].attach(10);  // 검지
  //servo[2].attach(11);  // 중지
  for (int i = 0; i < 2; i++) {
    servo[i].write(angleInit);
  }
  while (servo[0].read() != angleInit || servo[1].read() != angleInit /*|| servo[2].read() != angleInit*/) {
    delay(100);
  }
  //Serial.println("init finish");
}
void RecvData() {
  while (Serial.available()) {
    char x = Serial.read();
    //패킷 시작 문자열 초기화
    if (x == startMarker) {
      recvData = "";
    }
    // 패킷 종료시 전송
    else if (x == endMarker) {
      parseData();
    } else {
      recvData += x;
    }
  }
  Serial.flush();
}

unsigned long loopLastMilliTime = 0;

void loop() {
  
  if (!isInit) {
    Init();
    isInit = true;
  }

  if (millis() - loopLastMilliTime>  20) {

  //InputTest();
  RecvData();
  MotorControl();
  //SendData();

  loopLastMilliTime = millis();
  }
}
//서보 : 가변저항 : <180,180,180> : 600 450 477
//서보 : 가변저항 : <0,0,0> :  1013  1000 1023

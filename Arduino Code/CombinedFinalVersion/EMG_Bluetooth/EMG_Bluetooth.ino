#include <SoftwareSerial.h>
int blueTx = 6;  //2
int blueRx = 7;  //3
SoftwareSerial mySerial(blueTx, blueRx);

const char startMarker = '<';
const char endMarker = '>';
int Poten_1_Threshold = 1023;
int Poten_2_Threshold = 1023;
int Poten_3_Threshold = 1023;

int potenTest[3];
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
int sampleRate = 500;//SAMPLE_FREQ_500HZ;
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

void parseData() {

  int first = recvData.indexOf(",");              // 첫 번째 콤마 위치
  int second = recvData.indexOf(",", first + 1);  // 두 번째 콤마 위치
  int length = recvData.length();                 // 문자열 길이

  pressure[0] = recvData.substring(0, first).toInt();
  pressure[1] = recvData.substring(first + 1, second).toInt();
  pressure[2] = recvData.substring(second + 1, length).toInt();
  Serial.println("revc pressure  : " +  String(pressure[0]) + "," + String(pressure[1]) + "," + String(pressure[2]));
}

///////

void SetupMotor() {
  // 서보 모터 연결된 핀을 설정
  // servo[0].attach(2);  // 엄지
  // servo[1].attach(3);  // 검지
  // servo[2].attach(4);  // 중지

  // 입력 값이 없을 때는 모터의 각도를 180도로 설정(손이 마음대로 움직일 수 있는 상태)
  // for (int i = 0; i < 3; i++) {
  //   currentServoAngles[i] = targetServoAngles[i] = 180
  // }
}

void setup() {
  Serial.begin(9600);
  mySerial.begin(9600);

  myFilter.init(sampleRate, humFreq, true, true, true);
  // setup for time cost measure
  // using micros()
  timeBudget = 1e6 / sampleRate;
  timeMillis = 0;
  startMillis = millis();
  SetupMotor();
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
  float ratio = abs( (currentValue - currentStart) / (currentEnd - currentStart));
  float mappedValue = targetStart + ratio * (targetEnd - targetStart);
  return mappedValue;
}

//////////////////// 모터 제어 ///////////////////////
Servo servo[3];  // 서보모터를 배열로 정의
unsigned long lastMotorUpdatedTime = 0;
int testPressID = 0;
bool isPress = false;
int pressAngle; // 눌렀을 떄 최초 나사각도
void MotorControl() {

  //테스트코드
  int testPress[10] = { 30, 30, 30, 50, 50, 50, 0, 0, 20, 20 };

  // 가변저항
  int potentiometer_value[3];                                // 가변저항 값
  // analogdRead) : 가변저항 0~1023범위
  potentiometer_value[0] = Clamp(analogRead(A2), 600, 1013);  // (엄지: A2), 손가락을 최대한 위로 땅겼을 때,손가락을 최대한 쥐었을 때
  potentiometer_value[1] = Clamp(analogRead(A3), 450, 1023);  // (검지: A3)
  potentiometer_value[2] = Clamp(analogRead(A4), 477, 1023);  // (중지: A4)

  int potentAngle[3];  // 가변저항 나사 각도
  potentAngle[0] = Map(potentiometer_value[0], 1013, 600, 0, 180); // 손풀었을 때 가변저항, 쥐었을 때 가변저항, 풀었을 때 각도, 쥐었을 때 각도
  potentAngle[1] = Map(potentiometer_value[1], 1023, 450, 0, 180);
  potentAngle[2] = Map(potentiometer_value[2], 1023, 477, 0, 180);

  String msg = "";
  msg += String(analogRead(A2))  + "," ;
  msg += String(analogRead(A3)) + ",";
  msg += String(analogRead(A4)) + "/";

  // write 서보모터의 각도 입력값을 서서히 증가시키는 방식으로 탄성력 구현해야함
  int delay_value = 200;  // milliseconds

  int targetServoAngles[3];   // 목표 각도
  int currentServoAngles[3];  // 서보모터의 현재 각도
  // delay 함수 사용시 근전도 전송도 지연되므로 사용x


  if (millis() - lastMotorUpdatedTime > delay_value) {
    //test

    // pressure[0] = pressure[1] = pressure[2] = testPress[testPressID];
    //testPressID = testPressID == 9 ? 0 : testPressID + 1;
    ///testEnd



    //    for (int i = 0; i < 3; i++) {
    for (int i = 0; i < 3; i++) { // 엄지만 우선 사용
      int servoSpeed = pressure[i] * 0.1;
      targetServoAngles[i] = potentAngle[i];//+ (100 - pressure[i]) * 0.05;  // 압력이 높을 수록 서보모터를 나사에 붙임. 수정 필요
      // 서보모터 목표각도로 현재 각도이동
      currentServoAngles[i] += (targetServoAngles[i] > currentServoAngles[i]) ? servoSpeed : -servoSpeed;
      currentServoAngles[i] = Clamp(currentServoAngles[i], 0, 180);
      //      servo[i].attach(i + 2);                 // 서보모터 떨림 방지 : attach-detach
      //      servo[i].write(potentAngle[i]);  // 서보에 현재 각도를 반영

   

      if (pressure[i] > 0 && !isPress)
      {
        isPress = true;
        pressAngle = potentAngle[i];
        potenTest[i]=potentAngle[i];
      }
      if (pressure[i] == 0 && isPress)
      {
        isPress = false;
      }
      
      servo[i].write(isPress ? pressAngle + 50 : 180);
      //msg += String(pressure[i]) + ",";
      //msg += analogRead(A2);
      //      msg += String(servo[i].read()) + ", " + String(potentAngle[i]) + " / ";
    }
    for(int i = 0; i<3; i++){
      msg+= "poten: "+String(potenTest[i])+", servo: "+String(servo[i].read()-50+", ");
    }
    Serial.println(msg);
    //Serial.println(currentServoAngles[0]);

    lastMotorUpdatedTime = millis();

  } else if (millis() - lastMotorUpdatedTime > 40) {
    for (int i = 0; i < 3; i++) {
      //      servo[i].detach();
    }
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
      Serial.println("exit");
      exit(0);
    } else {
      recvData += x;
    }
  }
}

//receive data through bluetooth
void RecvData() {
  while (mySerial.available()) {
    char x = mySerial.read();
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

// EMG 블루투스 전송
void SendData() {
  // In order to make sure the ADC sample frequence on arduino,
  // the time cost should be measured each loop
  /*------------start here-------------------*/
  timeStamp = micros();

  int Value = analogRead(SensorInputPin);
  //SensorInputPin_1
  int Value_1 = 0; //analogRead(SensorInputPin_1);

  // filter processing
  int DataAfterFilter = myFilter.update(Value);
  int DataAfterFilter_1 = myFilter.update(Value_1);

  int envlope = sq(DataAfterFilter);
  int envlope_1 = sq(DataAfterFilter_1);

  // any value under threshold will be set to zero
  envlope = (envlope > Threshold_Min) ? envlope : 0;
  envlope_1 = (envlope_1 > Threshold_Min_1) ? envlope_1 : 0;

  timeStamp = micros() - timeStamp;
  if (TIMING_DEBUG) {

    //if(envlope !=0 && envlope<Threshold_Max){
    if (envlope < Threshold_Max || envlope_1 < Threshold_Max_1) {
      //Serial.println("DATA,TIME,TIMER," + String(millis()*0.001));

      String str_envlope = String(envlope);
      String str_envlope_1 = String(envlope_1);

      sendData = str_envlope + ',' + str_envlope_1;

      char* a = new char[sendData.length() + 1];
      strcpy(a, sendData.c_str());
      //delay(200);  //excels
      if (maxEMG < envlope) {
        maxEMG = envlope;  //최대값 maxEMG
      }
      count++;
      sum = sum + envlope;
      mySerial.println(a);  // 전송
      delete a;
    }

    // Serial.println("Filters cost time: " + String(timeStamp));
    // the filter cost average around 520 us
  }
}

bool isInit = false;
void Init() {

  servo[0].attach(9);  // 엄지
  servo[1].attach(10);  // 검지
  servo[2].attach(11);  // 중지
  for (int i = 0; i < 3; i++) {
    servo[i].write(180);
  }
  //  while (servo[0].read() < 180 || servo[0].read() < 180 || servo[0].read() < 180)
  while (servo[0].read() < 180 || servo[0].read() < 180 || servo[0].read() < 180)
  {
    delay(100);
  }
  Serial.println("init finish");
  for (int i = 0; i < 3; i++) {
    //    servo[i].detach();
  }
}

void loop() {
  if (!isInit) {
    Init();
    isInit = true;
  }

  InputTest();
  //RecvData();
  MotorControl();

  //SendData();

  // put your main code here, to run repeatedly:
  delayMicroseconds(100);
}

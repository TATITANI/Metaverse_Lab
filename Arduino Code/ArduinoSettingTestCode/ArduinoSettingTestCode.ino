////// 모터제어
#include <Servo.h>  // 서보모터 라이브러리 포함

unsigned long timeStamp;
unsigned long timeBudget;
unsigned long timeMillis;
unsigned long startMillis;
unsigned long currentMillis;
unsigned long previousMillis;
bool isInit = false;
char result[8];
char* ch1;

const char startMarker = '<';
const char endMarker = '>';
String sendData, recvData;
int pressure[3];
//////////////////// 모터 제어 ///////////////////////
Servo servo[3];  // 서보모터를 배열로 정의
// 가변저항
int potentiometer_value[3];  
int potentAngle[3];  // 가변저항 나사 각도
  
unsigned long lastMotorUpdatedTime = 0;

void MotorControl() {
  //potentiometer_value[0] = Clamp(analogRead(A2), 0, 830);  // (엄지: A2), 손가락을 최대한 쥐었을 때, 손가락을 최대한 위로 땅겼을 때

  potentAngle[0] = Map(potentiometer_value[0], 1023, 0, 0, 180); // 손풀었을 때 가변저항, 쥐었을 때 가변저항, 풀었을 때 각도, 쥐었을 때 각도
  potentAngle[1] = Map(potentiometer_value[1], 1023, 0, 0, 180); // 손풀었을 때 가변저항, 쥐었을 때 가변저항, 풀었을 때 각도, 쥐었을 때 각도
  potentAngle[2] = Map(potentiometer_value[2], 1023, 0, 0, 180); // 손풀었을 때 가변저항, 쥐었을 때 가변저항, 풀었을 때 각도, 쥐었을 때 각도

  // String msg = "";
  // msg += String(analogRead(A2))  + "," ;
  // msg += String(analogRead(A3)) + ",";
  // msg += String(analogRead(A4)) + "/";

    for (int i = 0; i < 3; i++) {
    servo[i].write(pressure[i]);
  }

  Serial.print("Servo[D2],[D3],[D4]: "+String(servo[0].read())+"/");
  Serial.print(String(servo[1].read())+"/");
  Serial.println(String(servo[2].read()));
  delay(100);
}

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  //timeBudget = 1e6 / sampleRate;
  timeMillis = 0;
  startMillis = millis();  
}
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
  float ratio = (currentValue - currentStart) / (currentEnd - currentStart);
  float mappedValue = targetStart + ratio * (targetEnd - targetStart);
  return mappedValue;
}


void loop() {
  // put your main code here, to run repeatedly:
  if (!isInit) {
    Init();
    isInit = true;
  }

  InputTest();
  //RecvData();
  //MotorControl();
  PrintPotentialmeters();
  //SendData();

  // put your main code here, to run repeatedly:
  delayMicroseconds(100);
}

void PrintPotentialmeters(){
  //if(potentiometer_value[0] != analogRead(A2) ||potentiometer_value[1] != analogRead(A3)||potentiometer_value[2] != analogRead(A4))  
  {
    potentiometer_value[0] = analogRead(A2);
    potentiometer_value[1] = analogRead(A3);
    potentiometer_value[2] = analogRead(A4);
    Serial.print("Poten[A2],[A3],[A4]: "+String(potentiometer_value[0])+"/ ");
    Serial.print(String(potentiometer_value[1])+"/ ");
    Serial.println(String(potentiometer_value[2]));
    Serial.println("----------------------------------");
    delay(100);
  }
  
}
//서보 : 가변저항 : <180,180,180> : 600 450 477
//서보 : 가변저항 : <0,0,0> :  1013  1000 1023
//<0,0,0>       873 1023
//<90,90,90>
void Init() {

  //servo[0].attach(9);  // 엄지
  servo[1].attach(10);  // 검지
  servo[2].attach(11);  // 중지
  for (int i = 0; i < 1; i++) {
    servo[i].write(180);
  }
  //  while (servo[0].read() < 180 || servo[0].read() < 180 || servo[0].read() < 180)
  // while (servo[0].read() < 180)
  // {
  //   delay(100);
  // } //331 950
  Serial.println("init finish");
  for (int i = 0; i < 3; i++) {
    //    servo[i].detach();
  }
}

void parseData() {

  int first = recvData.indexOf(",");              // 첫 번째 콤마 위치
  int second = recvData.indexOf(",", first + 1);  // 두 번째 콤마 위치
  int length = recvData.length();                 // 문자열 길이

  pressure[0] = recvData.substring(0, first).toInt();
  pressure[1] = recvData.substring(first + 1, second).toInt();
  pressure[2] = recvData.substring(second + 1, length).toInt();
  Serial.println("revc pressure  : " +  String(pressure[0]) + "," + String(pressure[1]) + "," + String(pressure[2]));
  
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
    MotorControl();
  }
}

//서보 : 가변저항 : <180,180,180> : 600 450 477
//서보 : 가변저항 : <0,0,0> :  1013  1000 1023
//<0,0,0>       873 1023
//<90,90,90>


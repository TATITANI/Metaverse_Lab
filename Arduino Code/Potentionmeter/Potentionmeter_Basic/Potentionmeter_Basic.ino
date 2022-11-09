#include <Servo.h>
Servo microServo;
int servoPin = 6;
int angle=0;
int value;
int touch=7;
int state=0;



void setup() {
  // put your setup code here, to run once:
  microServo.attach(servoPin);
  pinMode(touch,INPUT);
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  //state = digitalRead(touch);
  
  value = analogRead(A0);
  Serial.println(value);
value = map(value,0,1023,0,180);
  microServo.write(value);
/*
  
  if(state == 1){
    for(int i =0; i<90; i++){
        angle++;
    microServo.write(angle);
        delay(value);  
      if(angle>=180) angle=180;
    }
  }
    for(int i = 90; i>0; i--){ 
      angle--;
    microServo.write(angle);
      delay(value);
      if(angle<=0) angle=0;
    }
  Serial.println(state);
*/
}  

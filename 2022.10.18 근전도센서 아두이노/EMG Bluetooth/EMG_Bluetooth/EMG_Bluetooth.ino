#include <SoftwareSerial.h>
int blueTx=2;
int blueRx=3;
SoftwareSerial mySerial(blueTx, blueRx);

//EMG
#if defined(ARDUINO) && ARDUINO >= 100
#include "Arduino.h"
#else
#include "WProgram.h"
#endif

#include "EMGFilters.h"

#define TIMING_DEBUG 1

#define SensorInputPin A0 // input pin number
#define SensorInputPin_1 A1 // input pin number

EMGFilters myFilter;
// discrete filters must works with fixed sample frequence
// our emg filter only support "SAMPLE_FREQ_500HZ" or "SAMPLE_FREQ_1000HZ"
// other sampleRate inputs will bypass all the EMG_FILTER
int sampleRate = SAMPLE_FREQ_500HZ;
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
static int Threshold_Min = 0; //유효 최소값
static int Threshold_Max = 200000; //유효 최대값
static int Threshold_Min_1 = 0; //유효 최소값
static int Threshold_Max_1 = 200000; //유효 최대값 //공 쥐는 거 : 700 / 집는거 손등 85


unsigned long timeStamp;
unsigned long timeBudget;
unsigned long timeMillis;
unsigned long startMillis;
unsigned long currentMillis;
unsigned long previousMillis;
const long interval = 10000; //120,000=2분(120,000ms)
 int count=1;
 unsigned long sum = 0; // 합
  unsigned long ave = 0; // 평균
  unsigned long maxEMG = 0;

String sendData;
char result[8];
char* ch1;

void setup() {
  Serial.begin(9600);
  mySerial.begin(9600);

  myFilter.init(sampleRate, humFreq, true, true, true);
  // setup for time cost measure
    // using micros()
    timeBudget = 1e6 / sampleRate;
    // micros will overflow and auto return to zero every 70 minutes

    //Serial.println("CLEARDATA");
    //Serial.println("LABEL,Time,Timer, EMG,EMG_1,T");//, Counting,timeMillis
    
    timeMillis=0;
    startMillis = millis();
}

void loop() {
  if(mySerial.available()) //receive data through bluetooth
  {
    Serial.write(mySerial.read());
    //int value1 = mySerial.parseInt();
    //int value2 = mySerial.parseInt();
    //int value3 = mySerial.parseInt();
    //Serial.write("value 1 : ");
    //Serial.println(value1);
    //Serial.write("value 2 : ");
    //Serial.println(value2);
    //Serial.write("value 3 : ");
    //Serial.println(value3);
    //parse string -> int
  }
  Serial.flush();
  delay(100);


  ////////////////////////////////
/* add main program code here */
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
    envlope = (envlope > Threshold_Min ) ? envlope : 0;
    envlope_1 = (envlope_1 > Threshold_Min_1 ) ? envlope_1 : 0;
    
    timeStamp = micros() - timeStamp;
    if (TIMING_DEBUG) {
        // Serial.print("Read Data: "); Serial.println(Value);
        // Serial.print("Filtered Data: ");Serial.println(DataAfterFilter);
        //Serial.print("Squared Data: ");
        //Serial.println(0);
        //if(envlope !=0 && envlope<Threshold_Max){
        if(envlope<Threshold_Max || envlope_1<Threshold_Max_1){ 
          //Serial.print("DATA,TIME,TIMER,");
          //Serial.print(millis()*0.001);
          //Serial.print(",");
          //Serial.print("Squared Data: ");
          //Serial.println(envlope-Threshold);
          
          //Serial.print("envlope : ");
          
          //Serial.print(envlope);
          //Serial.print(", ");

          //Serial.print(envlope_1);
          //Serial.print(", ");
          
          //Serial.print(millis()*0.001);
          //Serial.println(",");
          //Serial.print("count : ");
          //Serial.println(count);

          String str_envlope = String(envlope);
          String str_envlope_1 = String(envlope_1);
         
          sendData = str_envlope +','+str_envlope_1;
          
          char * a = new char[sendData.length() +1];
          strcpy(a, sendData.c_str());
          //Serial.println(a); 
          delay(200); //excels 
          if(maxEMG<envlope){
            maxEMG=envlope; //최대값 maxEMG
          }
          count++;
          sum = sum + envlope;
          //Serial.println(a); 
          mySerial.println(a);
          delete a;
        }
        
        // Serial.print("Filters cost time: "); Serial.println(timeStamp);
        // the filter cost average around 520 us
    }
    ////////////////////////////////

  // put your main code here, to run repeatedly:
  delayMicroseconds(1000);
}

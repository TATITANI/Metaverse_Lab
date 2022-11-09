/*
* Copyright 2017, OYMotion Inc.
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions
* are met:
*
* 1. Redistributions of source code must retain the above copyright
*    notice, this list of conditions and the following disclaimer.
*
* 2. Redistributions in binary form must reproduce the above copyright
*    notice, this list of conditions and the following disclaimer in
*    the documentation and/or other materials provided with the
*    distribution.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
* "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
* LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
* FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
* COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
* INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
* BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
* OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
* AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF
* THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
* DAMAGE.
*
*/

#if defined(ARDUINO) && ARDUINO >= 100
#include "Arduino.h"
#else
#include "WProgram.h"
#endif

#include "EMGFilters.h"

#define TIMING_DEBUG 1

#define SensorInputPin A0 // input pin number

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
static int Threshold = 2000; //공 쥐는 거 : 700 / 집는거 손등 85

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
void setup() {
    /* add setup code here */
    myFilter.init(sampleRate, humFreq, true, true, true);

    // open serial
    Serial.begin(115200);

    // setup for time cost measure
    // using micros()
    timeBudget = 1e6 / sampleRate;
    // micros will overflow and auto return to zero every 70 minutes

    Serial.println("CLEARDATA");
    Serial.println("LABEL,Time,EMG,Counting,timeMillis");
    
    timeMillis=0;
    startMillis = millis();
}

void loop() {
    
    /* add main program code here */
    // In order to make sure the ADC sample frequence on arduino,
    // the time cost should be measured each loop
    /*------------start here-------------------*/
    timeStamp = micros();

    int Value = analogRead(SensorInputPin);

    // filter processing
    int DataAfterFilter = myFilter.update(Value);

    int envlope = sq(DataAfterFilter);
    // any value under threshold will be set to zero
    envlope = (envlope > Threshold) ? envlope : 0;

    timeStamp = micros() - timeStamp;
    if (TIMING_DEBUG) {
        // Serial.print("Read Data: "); Serial.println(Value);
        // Serial.print("Filtered Data: ");Serial.println(DataAfterFilter);
        //Serial.print("Squared Data: ");
        //Serial.println(0);
        if(envlope !=0 && envlope<100000){
          /*
          Serial.print("DATA,");
          Serial.print(millis());
          Serial.print(",");
          //Serial.print("Squared Data: ");
          //Serial.println(envlope-Threshold);
          
          Serial.print("envlope : ");
         Serial.print(envlope);
          Serial.print(", ");
          Serial.print("count : ");
         Serial.println(count);
         */
          delay(100); //excel 
          if(maxEMG<envlope){
            maxEMG=envlope; //최대값 maxEMG
          }
          count++;
          sum = sum + envlope;
        }
        
        // Serial.print("Filters cost time: "); Serial.println(timeStamp);
        // the filter cost average around 520 us
    }
    currentMillis = millis();
    timeMillis = currentMillis-startMillis;
    
    
    if (currentMillis - previousMillis >= interval) {
         previousMillis = currentMillis;
         timeMillis++; //0.1초 증가
         
         ave = sum/count;
         Serial.print("sum : ");
         Serial.println(sum);
         Serial.print("ave : ");
         Serial.println(ave);
         Serial.print("maxEMG : ");
         Serial.println(maxEMG);
         count = 1;
         sum=0;
         maxEMG=0;
      }
    /*------------end here---------------------*/
    // if less than timeBudget, then you still have (timeBudget - timeStamp) to
    // do your work
    delayMicroseconds(500);
    // if more than timeBudget, the sample rate need to reduce to
    // SAMPLE_FREQ_500HZ
}

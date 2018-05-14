#include<Servo.h>
#include<Adafruit_MotorShield.h>
#include<AccelStepper.h>
#include "utility/Adafruit_MS_PWMServoDriver.h"
#include<SoftwareSerial.h>
 
SoftwareSerial XBee(2,3);
Servo servoBack;
Adafruit_MotorShield AFMS(0x60);
Adafruit_StepperMotor *stepper1 = AFMS.getStepper(200, 1);
Adafruit_StepperMotor *stepper2 = AFMS.getStepper(200, 2);
int resetPin = 12;
int pos;
String received;
 
//Unloads the vehicle
void Unload()
{
    //Moves unloading servo 90 degrees.
    for(pos = 6; pos <= 90; pos += 1)
    {
        servoBack.write(pos);
        delay(20);
    }
 
    //Resets unloading servo
    for(pos = 90; pos > 6; pos -= 1)
    {
        servoBack.write(pos);
        delay(20);
    }
 
    XBee.write("Done/");
}
 
//Methods for various movements
void ForwardSingleStep1()
{
    stepper1->onestep(FORWARD, SINGLE);
}
 
void BackwardSingleStep1()
{
    stepper1->onestep(BACKWARD, SINGLE);
}
 
void ForwardSingleStep2()
{
    stepper2->onestep(FORWARD, SINGLE);
}
 
void BackwardSingleStep2()
{
    stepper2->onestep(BACKWARD, SINGLE);
}
 
void ForwardDoubleStep1()
{
    stepper1->onestep(FORWARD, DOUBLE);
}
 
void BackwardDoubleStep1()
{
    stepper1->onestep(BACKWARD, DOUBLE);
}
 
void ForwardDoubleStep2()
{
    stepper2->onestep(FORWARD, DOUBLE);
}
 
void BackwardDoubleStep2()
{
    stepper2->onestep(BACKWARD, DOUBLE);
}
 
void ForwardInterleaveStep1()
{
    stepper1->onestep(FORWARD, INTERLEAVE);
}
 
void BackwardInterleaveStep1()
{
    stepper1->onestep(BACKWARD, INTERLEAVE);
}
 
void ForwardInterleaveStep2()
{
    stepper2->onestep(FORWARD, INTERLEAVE);
}
 
void BackwardInterleaveStep2()
{
    stepper2->onestep(BACKWARD, INTERLEAVE);
}
void ForwardMicrostep1()
{
    stepper1->onestep(FORWARD, MICROSTEP);
}
 
void BackwardMicrostep1()
{
    stepper1->onestep(BACKWARD, MICROSTEP);
}
 
void ForwardMicrostep2()
{
    stepper2->onestep(FORWARD, MICROSTEP);
}
 
void BackwardMicrostep2()
{
    stepper2->onestep(BACKWARD, MICROSTEP);
}
 
//create an AccelStepper object for every mode the motor use
AccelStepper s1s(BackwardSingleStep1, ForwardSingleStep1);
AccelStepper s2s(ForwardSingleStep2, BackwardSingleStep2);
AccelStepper s1d(BackwardDoubleStep1, ForwardDoubleStep1);
AccelStepper s2d(ForwardDoubleStep2, BackwardDoubleStep2);
AccelStepper s1i(BackwardInterleaveStep1, ForwardInterleaveStep1);
AccelStepper s2i(ForwardInterleaveStep2, BackwardInterleaveStep2);
AccelStepper s1m(BackwardMicrostep1, ForwardMicrostep1);
AccelStepper s2m(ForwardMicrostep2, BackwardMicrostep2);
 
 
void setup()
{
    AFMS.begin();
    XBee.begin(9600);
    servoBack.attach(10);
    servoBack.write(6);
    digitalWrite(resetPin, HIGH);
    pinMode(resetPin, OUTPUT);
}

void CheckForStop()
{
    if(XBee.available())
    {
        if(XBee.readStringUntil(';') == "Stop")
        {
            XBee.write("Emergency stop succesfully completed/");
            delay(3000);
            digitalWrite(resetPin, LOW);
        }
    }
}
 
void loop()
{
    if(XBee.available())
    {
        received = XBee.readStringUntil(';');

        if(received == "Command")
        {
            XBee.read();
            String mode = XBee.readStringUntil(';');
            XBee.read();
            String accel1 = XBee.readStringUntil(';');
            XBee.read();
            String speed1 = XBee.readStringUntil(';');
            XBee.read();
            String steps1 = XBee.readStringUntil(';');
            XBee.read();
            String accel2 = XBee.readStringUntil(';');
            XBee.read();
            String speed2 = XBee.readStringUntil(';');
            XBee.read();
            String steps2 = XBee.readStringUntil(';');

            //Convert to long
            long Steps1 = steps1.toInt();
            long Steps2 = steps2.toInt();

            while(mode == "2")
            {
                s1d.setMaxSpeed(speed1.toFloat());
                s1d.setAcceleration(accel1.toFloat());
                s2d.setMaxSpeed(speed2.toFloat());
                s2d.setAcceleration(accel2.toFloat());
                s1d.moveTo(Steps1);
                s2d.moveTo(Steps2);
                s1d.run();
                s2d.run();
 
                if(s1d.distanceToGo() == 0)
                {
                    s1d.setCurrentPosition(0);
                    s2d.setCurrentPosition(0);

                    XBee.write("Done/");
                    mode = "0";
                }
 
            }
       
            while(mode == "3")
            {
                s1i.setMaxSpeed(speed1.toFloat());
                s1i.setAcceleration(accel1.toFloat());
                s2i.setMaxSpeed(speed2.toFloat());
                s2i.setAcceleration(accel2.toFloat());
                s1i.moveTo(Steps1);
                s2i.moveTo(Steps2);
                s1i.run();
                s2i.run();
 
                if(s1i.distanceToGo() == 0)
                {
                    s1i.setCurrentPosition(0);
                    s2i.setCurrentPosition(0);

                    XBee.write("Done/");
                    mode = "0";
                }
            }
        }
        else
        {
            if(received == "Connect")
            {
                XBee.write("Connected to Arduino Vehicle/");
            }
            else if(received == "Unload")
            {
                Unload();
            }
        }
    }
}

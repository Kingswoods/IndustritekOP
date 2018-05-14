#include <SoftwareSerial.h>
#include <Wire.h>
#include <PN532_I2C.h>
#include <PN532.h>
#include <NfcAdapter.h>

SoftwareSerial XBee(1, 0);
PN532_I2C pn532_i2c(Wire);
NfcAdapter nfc = NfcAdapter(pn532_i2c);

String UID = "";

String message;
bool isBusy = false;

int loopCounter = 0;

void setup() 
{
    XBee.begin(9600);
    nfc.begin();
}

void Listen()
{
    message = XBee.readStringUntil(';');

    if(message == "Connect")
    {
        isBusy = false;
        XBee.write("Connected to Arduino Reader/");
    }
    else if(message == "Tag Received")
    {
        isBusy = true;
    }
    else if(message == "Task complete")
    {
        isBusy = false;
    }
}

void ReadTag()
{
    NfcTag tag = nfc.read();
    UID = tag.getUidString();

    //Remove spaces
    UID.replace(" ", "");

    UID.remove(14);

    //Print to Xbee
    XBee.print(UID + "/");

}

void loop() 
{
    Listen();

    if(nfc.tagPresent() && !isBusy)
    {
        ReadTag();
    }
}

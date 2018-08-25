<div align="center">
	<img src="docs/images/full-logo.png" 
         alt="Ardity: Arduino + Unity"3
         title="Ardity: Arduino + Unity">
    <h1>Arduino + Unity communication made easy</h1>
</div>

And not just Arduino: any hardware/software that communicates over serial (COM) ports !

WebSite URL: https://ardity.dwilches.com

***(Previously known as: SerialCommUnity)***

Instructions
============

There are three scenes that show how to read/write data from/to a serial device.
There is a prefab that you need to add to your scene, this prefab will do all the thread management, queue synchronization and exception handling for you.

If you need a program to test your Unity scene I have created the following Arduino program that sends a heartbeat message each 2 seconds.
It also recognizes two input messages and reacts to them, this is for bidirectional communication for the scene "DemoSceneUserPoll_ReadWrite".
So if you want to use that scene just run it and press the keys 'A' or 'Z' to see what you Arduino has to say about it.


Sample Arduino Program
======================
This sample communicates with any of the scenes called: `DemoScene_AutoPoll*` or `DemoScene_UserPoll*`.

```cs
unsigned long last_time = 0;

void setup()
{
    Serial.begin(9600);
}

void loop()
{
    // Print a heartbeat
    if (millis() > last_time + 2000)
    {
        Serial.println("Arduino is alive!!");
        last_time = millis();
    }

    // Send some message when I receive an 'A' or a 'Z'.
    switch (Serial.read())
    {
        case 'A':
            Serial.println("That's the first letter of the abecedarium.");
            break;
        case 'Z':
            Serial.println("That's the last letter of the abecedarium.");
            break;
    }
}
```

Sample with Tear-Down function
======================
This sample has a tear-down function (use it with the scene `DemoScene_SampleTearDown`),
it will be executed when the Unity program stops. This sample expects you to be using an Arduino UNO,
if not, change the number of the pin to which the LED is connected.

```cs
unsigned long last_time = 0;
int ledPin = 13;

void setup()
{
    Serial.begin(9600);
    pinMode(ledPin, OUTPUT);
    digitalWrite(ledPin, LOW);
}

void loop()
{
    // Print a heartbeat
    if (millis() > last_time + 2000)
    {
        Serial.println("Arduino is alive!!");
        last_time = millis();
    }

    // Send some message when I receive an 'A' or a 'Z'.
    switch (Serial.read())
    {
        case '1':
            digitalWrite(ledPin, HIGH);
            Serial.println("Lights are ON");
            break;
        case '2':
            digitalWrite(ledPin, LOW);
            Serial.println("Lights are OFF");
            break;
        
        // Execute tear-down functionality
        case 'X':
            for (int i = 0; i < 10; i++)
            {
                digitalWrite(ledPin, HIGH);
                delay(100);
                digitalWrite(ledPin, LOW);
                delay(100);
            }
            
            // This message won't arrive at Unity, as it is already in the
            // process of closing the port
            Serial.println("Tearing down some work inside Arduino");
            break;
    }
}
```

Sample with custom delimiter
======================

```cs
// This is the separator configured in Unity
#define SEPARATOR 255

unsigned long last_time = 0;
int ledPin = 13;

byte responseMessage[] = { 65, 66, 67, SEPARATOR };
byte aliveMessage[] = { 65, 76, 73, 86, 69, 33, SEPARATOR };

void setup()
{
    Serial.begin(9600);

    aliveMessage[8] = SEPARATOR;
}

void loop()
{
    // Print a heartbeat
    if (millis() > last_time + 2000)
    {
        Serial.write(aliveMessage, sizeof(aliveMessage));
        Serial.flush();
        last_time = millis();
    }
    
    // React to "commands"
    if (Serial.read() == ' ')
    {
        Serial.write(responseMessage, sizeof(responseMessage));
        Serial.flush();
    }
}
```

COM port names
==
To open a COM port from `Ardity` you need to use one of these naming conventions:
* `COM1`, `COM2`, ... for COM1 through COM9
* `\\.\COM10`, `\\.\COM11`, ... for COM10 and up


Possible Problem When Importing This Package
============================================

If you get this error message:

> Assets/Ardity/Scripts/SerialThread.cs(9,17): error CS0234: The type or namespace name 'Ports' does not exist in the namespace 'System.IO'. Are you missing an assembly reference?

It's because the current "API Compatibility Level" of your Unity project is set to ".NET 2.0 Subset", which doesn't contain the classes necessary for serial communication. Do this to solve the problem:

Go to Edit -> Project Settings -> Player, and under "Other Settings" find an option that reads "Api Compatibility Level" and change it from ".NET 2.0 Subset" to ".NET 2.0".

    
License
=======

This work is released under the [Creative Commons Attributions](https://creativecommons.org/licenses/by/2.0/) license.

If you use this library please let me know, so I know my work has been useful to you :)

![CC Attribution](docs/images/CC-BY_icon.png?raw=true)

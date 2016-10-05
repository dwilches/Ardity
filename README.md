# SerialCommUnity
Assets for integrating Arduino and Unity (or Unity and any hardware that communicates over a COM port)



Instructions
============

There are three scenes that show how to read/write data from/to a serial device.
There is a prefab that you need to add to your scene, this prefab will do all the thread management, queue synchronization and exception handling for you.

If you need a program to test your Unity scene I have created the following Arduino program that sends a heartbeat message each 2 seconds.
It also recognizes two input messages and reacts to them, this is for bidirectional communication for the scene "DemoSceneUserPoll_ReadWrite".
So if you want to use that scene just run it and press the keys 'A' or 'Z' to see what you Arduino has to say about it.



Possible Problem When Importing This Package
============================================

If you get this error message:

> Assets/SerialComm/Scripts/SerialThread.cs(9,17): error CS0234: The type or namespace name 'Ports' does not exist in the namespace 'System.IO'. Are you missing an assembly reference?

It's because the current "API Compatibility Level" of your Unity project is set to ".NET 2.0 Subset", which doesn't contain the classes necessary for serial communication. Do this to solve the problem:

Go to Edit -> Project Settings -> Player, and under "Other Settings" find an option that reads "Api Compatibility Level" and change it from ".NET 2.0 Subset" to ".NET 2.0".



Sample Arduino Program
======================

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

Another sample Arduino Program
======================
This sample has a tear-down function (check the scene "DemoSceneUserPoll_ReadWrite_TearDown"),
it will be executed when the Unity program stops. This sample expects you to be using an Arduino UNO,
if not, change the number of the pin to which the LED is connected.

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
                break;
            case '2':
                digitalWrite(ledPin, LOW);
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
                break;
        }
    }
    
    
License
=======

This work is released under the [Creative Commons Attributions](https://creativecommons.org/licenses/by/2.0/) license.

If you use this library please let me know, so I know my work has been useful to you :)

![CC Attribution](docs/images/CC-BY_icon.png?raw=true)

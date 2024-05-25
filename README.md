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

Installation
============

You can download the code from GitHub (Code > Download ZIP) and after uncompressing, import the assets into your Unity scene.


Alternative installation using Package Manager
============

1. Open the Unity Package Manager by navigating to **Packages > Package Manager**.
2. Click on **Packages** in the top left corner and select **Add package from git URL...**.
3. Paste the following URL into the input field and click **Install**:

```
https://github.com/dwilches/Ardity.git?path=UnityProject/Assets/Ardity
```

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

Documentation
==

Ardity is quite simple to use. You can find the setup guide in PDF format [here](https://github.com/dwilches/Ardity/raw/master/UnityProject/Ardity%20-%20Setup%20Guide.pdf).

There is also a series of in-depth tutorials by the [Interface Lab](https://medium.com/interface-lab) from the University [NYU Shanghai](https://shanghai.nyu.edu/), please take a look here:
- [Connecting to Unity](https://medium.com/interface-lab/arduino-tutorial-series-connecting-to-unity-eedc48e77087)
- [Working with many sensors](https://medium.com/interface-lab/working-with-many-sensors-9029556ad3c0)
- [Bidirectional communication](https://medium.com/interface-lab/bidirectional-communication-a1c11ff62a25)

COM port names
==
To open a COM port from `Ardity` you need to use one of these naming conventions:
* `COM1`, `COM2`, ... for COM1 through COM9
* `\\.\COM10`, `\\.\COM11`, ... for COM10 and up

Can Ardity be used with Bluetooth devices?
==

Yes, it's possible.
You need to configure your device to be seen in your operating system as a COM port.
Instructions to do so vary from device to device and from OS to OS.
Once you have it configured, use that COM port with Ardity, which will treat it as just another serial device.

Common Issues
============================================

###  The type or namespace name 'Ports' does not exist in the namespace 'System.IO'

If you get this error:
> Assets/Ardity/Scripts/SerialThread.cs(9,17): error CS0234: The type or namespace name 'Ports' does not exist in the namespace 'System.IO'. Are you missing an assembly reference?

Check the current "API Compatibility Level" of your Unity project. Go to `Edit -> Project Settings -> Player`, and under `Other Settings` find an option that reads "Api Compatibility Level" and change it to `.NET 4.0` (or `.NET 2.0` if you have a version older than Unity 2018).

Also, some users have reported needing to manually add `System.IO.dll` to the project. If the above solution doesn't work for you, in Visual Studio go to `Project -> Add Reference -> Browse` and then select the file `System.IO.dll` from inside your .NET framework's folder. This file may be located at `C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework.NETFramework\v4.5\Facades\System.IO.dll`.

### Communication with some devices doesn't work out of the box

Some users have reported needing to enable RtsEnable and DtsEnable in order to get Ardity to work with their devices. So if communication is not working for you, try enabling these options in [AbstractSerialThread](https://github.com/dwilches/Ardity/blob/master/UnityProject/Assets/Ardity/Scripts/Threads/AbstractSerialThread.cs#L198) just before the `serialPort.Open()` invocation:

```
serialPort.DtrEnable = true;
serialPort.RtsEnable = true;
```

### If Ardity works in the Editor but not outside of it

Try changing the scripting backend from Mono to [IL2CPP](https://docs.unity3d.com/Manual/IL2CPP.html).
    
License
=======

This work is released under the [Creative Commons Attributions](https://creativecommons.org/licenses/by/2.0/) license.

If you use this library please let me know, so I know my work has been useful to you :)

![CC Attribution](docs/images/CC-BY_icon.png?raw=true)

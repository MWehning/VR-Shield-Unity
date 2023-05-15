# VR-Shield-Unity
Unity SDK needed to access data from VR-Shield board over Bluetooth

https://www.openhardware.io/view/33050

# Installation:

1.  clone the required assets(5 scripts) into your projects /Assets/ folder 
2.  boot Unity.
3.  Create an empty game object called "PortManager"
4.  drag and drop the script "Port Manager" into that object
5.  File->Build Settings->Player Settings...->Configuration->Player->Api Compatibility Level: Set to ".NET Framework" (Not .NET Standard 2.x)
6.  Save your project
7.  Congratulation, the SDK is installed now

# Device Management:

- To manage and create MCU boards or individual sensors click on the "Ports Manager" game object and use the dropdown menus.
- Each board is adressed by a number between 00 and 99
- Each sensor is adressed by the type(specified by three capital letters) and a number between 00 and 03(e.g. MPU01)
- Additionally custom designations can be assigned to that fixed adress for both the board and sensors


# API features:

Since information has to be returned if some functions are called they have to be provided with a sufficient **buffer** array. This also means that if you request 6 values but only provide an array with length 3, only the first 3 values will be filled in.

# device.Get();**

Gets the value of device based on the data type. IMPORTANT!: Filtered values are calculated using the gliding average method and only for currently active devices. If a previously inactive device is asked for a filtered value it will first return an unfiltered value and activate the filter task. If a second value is requested before the task times out(currently a 10 second window) an actually filtered value will be returned. 
**Example:** 

**short[] buffer = {0,0,0};**

**gyro1.Get(buffer,"second-filtered");**

This would return the 3rd to 6th value the sensor provides, in this case the accelerometer values of an IMU

Data types:
- all-filtered:    as many data values as the sensor provides but filtered through gliding average
- all-raw:         as many data values as the sensor provides but unfiltered straight from the sensor(could have a higher delay since fresh value has to be                        fetched)
- first-filtered:  first three data values from the sensor, filtered in the same manner as "all-filtered"
- second-filtered: second three data...
- third-filtered:  ...

# device.Set();**

Sets a value to an actuator and confirms the interaction through a reaction message

**relay0.Set(255)**
This device is set to its max. value(0xff)

# device.Scan();

Scans for sister/brother devices on the same board as the device mentioned. Then returns their current virtual IDs in Byte format(see table below)

**short[] buffer;**

**magneto2.Scan(buffer);**

Returns the byte address of every interactable connected to the same MCU as this device


# Virtual ID Table:

AdressByte = DeviceNibble << 4 +  NumberNibble

DeviceNibble:

MPU | QMC | BHL | REL

 0  |  1  |  2  |  3
 
Number Nibble:

 0  |  1  |  2  |  3

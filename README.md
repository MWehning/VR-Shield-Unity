# VR-Shield-Unity
Unity SDK needed to access data from VR-Shield board over Bluetooth

# Installation:
1.  clone the game object into your -missing- folder 
2.  boot Unity.
3.  drag and drop the folder into your scene. 
4.  Switch from ... to ...(Thing to make ports work)
5.  Save your project
6.  Congratulation, the SDK is installed now

# Device Management:
- To manage and create MCU Board or individual sensors click on the "Ports Manager" game object and use the dropdown menus.
- Each board is adressed by a number between 00 and 99
- Each sensor is adressed by the type(specified by three capital letters) and a number between 00 and 03(e.g. MPU01)
- Additionally custom designations can be assigned to that fixed adress for both the board and sensors


# API features:
Since information has to be returned if some functions are called they have to be provided with a sufficient **buffer** array. This also means that if you request 6 values but only provide an array with length 3, only the first 3 values will be filled in.
**device.Get();**
Gets the value of device based on the data type. IMPORTANT!: Filtered values are calculated using the gliding average method and only for currently active devices. If a previously inactive device is asked for a filtered value it will first return an unfiltered value and activate the filter task. If a second value is requested before the task times out(currently a 10 second window) an actually filtered value will be returned. 
**device.Set();**
Sets a value to an actuator and confirms the interaction through a reaction message
**device.Scan();**
Scans for sister/brother devices on the same board as the device mentioned. Then returns their current virtual IDs in Byte format(see table below)


# Virtual ID Table:
AdressByte = DeviceNibble << 4 +  NumberNibble

DeviceNibble:
MPU | QMC | BHL | REL
 0  |  1  |  2  |  3
 
Number Nibble:
 0  |  1  |  2  |  3

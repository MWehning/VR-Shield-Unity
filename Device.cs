using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Device 
{
    private byte[] setbuffer = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private Port port;
    public string deviceName;
    private byte deviceAddress;

    public Device(Port port,string deviceName, string stringDeviceAddress)
    {
        byte type = 0xff;
        if (stringDeviceAddress.Length>4)
        {
            Debug.LogError("Check Adress of " + deviceName + " on Port " + 
                            port.GetPortName() + "(" + port.GetPortNumber() + ")");

        }
        switch (stringDeviceAddress.Substring(0,3))     // get a number according to 3 letter tag 
        {
            case "MPU":
                type = 0x00;
                break;
            case "QMC":
                type = 0x10;
                break;
            case "BHL":
                type = 0x20;
                break;
            case "REL":
                type = 0x30;
                break;
            default:
                Debug.LogError("The device type for " + deviceName + " on Port " +
                                port.GetPortName() + "(" + port.GetPortNumber() + ") isn't a thing - yet");
                break;
        }
        byte count = (byte)(stringDeviceAddress[3]-48); // convert from char number to byte number 
        if (count > 4)
        {
            Debug.LogError("Sorry, but devices only go up to 4. " + deviceName + " on Port " + 
                            port.GetPortName() + "(" + port.GetPortNumber() + ") violates this policy");
        }
        byte combined = (byte)(type + count);           // combine those into a single adress byte

        this.port= port;
        this.deviceName= deviceName;
        this.deviceAddress = combined;
    }

    public string GetDeviceName()
    {
        return deviceName;
    }
    public bool Get(short[] buffer,string data_type)
    {
        byte numeric_data_type = 0xff;
        switch (data_type)
        {
            case "all-filtered":
                numeric_data_type = 0;
                break;
            case "all-raw":
                numeric_data_type = 1;
                break;
            case "first-filtered":
                numeric_data_type = 2;
                break;
            case "second-filtered":
                numeric_data_type = 3;
                break;
            case "third-filtered":
                numeric_data_type = 4;
                break;
            default:
                Debug.LogError("Tried a data type for " + deviceName + " on Port " + 
                                port.GetPortName() + "(" + port.GetPortNumber() + ") that isn't a thing");
                return false;
        }
        byte[] raw_data = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        if (port.Request(port.mcuID, deviceAddress, numeric_data_type, raw_data))
        {
            // Decode data
            for(int data_position = 0; data_position < buffer.Length; data_position++)
            {
                short highByte = raw_data[data_position*2];
                short lowByte = raw_data[(data_position*2)+1];
                buffer[data_position] = (short)( (highByte<<8) + lowByte);
            }
            return true;
        }
        return false;
    }

    public bool Set(byte value)
    {
;       port.Request(port.mcuID, deviceAddress, value, setbuffer);
        Debug.Log("Set Value");
        // TODO: Look at setbuffer to confirm
        return true;
    }

    public bool Scan(byte[] buffer)
    {
        port.Request(port.mcuID, 0xff, 0xff, buffer);
        //Debug.Log("Scanned for other devices parallel to this one");
        return true;
    }
}

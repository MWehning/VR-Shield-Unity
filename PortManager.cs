/*
	*-VR-Shield Project-*

	hard and software system that simplifies linking i2c sensors to other systems over bluetooth

	*Required hardware: ESP32-Wroom32 Dev Board C, Custom VR-Shield PCB (v03 or up)
	*Supported sensors: MPU6050, QMC5883L, BH1750 (Attached using VR-Dapter board)

	@author MWehning
	@version 3.0 22/04/2023

*/

using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortManager : MonoBehaviour
{

    [SerializeField]
    private List<PortDescr> portDescriptions;
	//private bool scanPort = false;

    private static List<Port> ports;
	private static List<Device> devices;


    private void Awake()
    {
		ports = new List<Port>();
		devices = new List<Device>();
		foreach (PortDescr portDescription in portDescriptions)
		{
			Port port = new Port(portDescription.portName, portDescription.portNumber, portDescription.mcuID);
			ports.Add(port);

			foreach (DeviceDescr deviceDescription in portDescription.devices)
			{
				Device device = new Device(port,deviceDescription.deviceName, deviceDescription.stringDeviceAddress);
				devices.Add(device);
			}
		}
    }

	public static Device GetDevice(string deviceName)
	{
		foreach (Device device in devices)
		{
			if (device.deviceName == deviceName)
			{
				return device;
			}
		}
		return null;
	}
}

using System.Collections.Generic;

[System.Serializable]
public class PortDescr
{
    public string portName;
    public int portNumber;
    public byte mcuID;
    public List<DeviceDescr> devices;
}


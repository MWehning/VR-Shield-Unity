using System;
using System.Collections;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Port
{
    private int portNumber;
    private string portName;
    public byte mcuID;
    private static System.IO.Ports.SerialPort serialPort;

    private byte state;                
    private byte pointer;

    // Sets up necessary ports for communication with a single shield
    // @param Port      Serial Port number(can be USB or Bluetooth port)
    public Port(string portName,int portNumber, byte mcuID)
    {
        this.portNumber = portNumber;
        this.portName = portName;
        this.mcuID = mcuID;
        serialPort = new System.IO.Ports.SerialPort();
        serialPort.BaudRate = 115200;                   // has to be the same as baud rate of shield
        String PortStr = "///.//COM" + portNumber;
        serialPort.PortName = PortStr;
        state = 0;                                      // reset state machine values
        pointer = 0;

        serialPort.ReadTimeout = 1;                     // ! Might be important in the future
        try
        {
            serialPort.Open();
            if (!serialPort.IsOpen)
                Debug.LogError("Could not open Serial Port");
            else
                Debug.Log("Serial port is open!");
        }
        catch (Exception exc)
        {
            Debug.LogException(exc);
        }
    }

    public int GetPortNumber()
    {
        return portNumber;
    }

    public string GetPortName()
    {
        return portName;
    }

    // Requests some type or action from a hardware device over the connected port
    // @param s_id      Shield ID, every MCU has its own
    // @param v_id      Virtual ID attached to a specific sensor
    // @param val       either the type of data(sensor) or the setvalue(actuator)
    // @param received  buffer array to be filled with response data bytes
    //
    // @return true if there was a valid answer
    public bool Request(byte s_id, byte v_id, byte val, byte[] received)
    {
        byte[] PKG = { 0xaa, s_id, v_id, val, 0xaa };

        String Outgoing = "";
        for (int i = 0; i < PKG.Length;i++){
            Outgoing += PKG[i].ToString("X");
            Outgoing += " ";
        }
        Debug.Log(Outgoing);

        pointer = 0;                                                // reset pointer to start writing at start of array
        serialPort.ReadExisting();                                  // clear all leftover bytes
        serialPort.Write(PKG, 0, PKG.Length);                       // Send out request over port
        int startByte = WaitForMessage();
        return ReadAndDecode(startByte,PKG, received);


        //byte[] test_array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        //for (byte i=0;i<received.Length;i++)
        //{
        //    received[i] = test_array[i];
        //}
        //return true;

    }

    private int WaitForMessage()
    {
        int byteRead = -1;
        int timeout = 2;
        while (true)
        {
            try
            {
                Debug.Log("Waiting for Message");
                byteRead = serialPort.ReadByte();
                if (byteRead != -1)
                {
                    System.Threading.Thread.Sleep(30);
                    return byteRead;
                }
            }
            catch
            {
                System.Threading.Thread.Sleep(30);
                timeout--;
                // supress timeout
            }

        }
    }


    private bool ReadAndDecode(int startByte, byte[] echo, byte[] buffer)  // provided echo, written to buffer
    {
        int byteRead = startByte;
        for (int b = 0; b < echo.Length-1;b++)
        {
            if ( byteRead != echo[b])
            {
                Debug.LogError("echo != request");
                return false;
            }
            byteRead = serialPort.ReadByte();
        }


        for (int i = 0; byteRead != -1; i++)            // Go through messages until registers dont have more to offer
        {
            //Debug.Log("Next byte "+byteRead.ToString("X"));
            if (StateMachine(buffer, byteRead))
            {
                break;                                  // Read complete
            }
            else
            {
                byteRead = serialPort.ReadByte();   
            }
        }
        return true;
    }

    // Takes a single byte and puts it into storage array depending on current state and pointer        
    // @param storage[] buffer array that gets filled with data bytes
    // @param current_byte current byte fed into the state machine
    //
    // @return true if a message was completed and false every other case
    private bool StateMachine(byte[] storage, int current_byte)
    {

        byte block = (byte)current_byte;
        bool e = false;
        if (block == 0xaa)   // escapement bit
        {
            e = true;
        }
  
        switch (state)
        {
            case 0:
                
                switch (e)
                {
                    case true:
                        state = 1;                  // escapement triggered, waiting for second one
                        break;
                    default:
                        storage[pointer] = block;   // enqueue data bit 
                        pointer++;
                        state = 0;
                        break;
                }
                break;
            case 1:
                switch (e)
                {
                    case true:
                        storage[pointer] = block;   // enqueue data bit equal to escapement
                        pointer++;
                        state = 2;
                        break;
                    case false:
                        state = 0;                  // message is completed by escapement followed by non escapement
                        
                        if(pointer == 0)
                        {
                            Debug.LogError("A device on Port " + portName + " does not answer");
                                return false;
                        }
                        pointer = 0;
                        return true;
                }
                break;
            default:
                return false;
            
        }
        //Debug.Log($"Block :{block}\tState :{state}\tPointer :{pointer}\tCondition : {e}");
        return false;

    }

}

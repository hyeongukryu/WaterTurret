using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using WaterTurret.Module.Models;

namespace WaterTurret.Module.Services
{
    public class SerialService : ISerialService
    {
        private readonly IConfigService _configService;
        private SerialPort _serialPort;

        public SerialService(IConfigService configService)
        {
            _configService = configService;

            _serialPort = new SerialPort();
            _serialPort.PortName = _configService.DeviceConfig.SerialPortName;
            _serialPort.BaudRate = _configService.DeviceConfig.SerialBaudRate;
            _serialPort.ReadTimeout = _configService.DeviceConfig.ReadTimeOut;
            _serialPort.WriteTimeout = _configService.DeviceConfig.WriteTimeOut;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
        }
        
        static object _lock = new object();

        public void ReceivePacketSync(DeviceMessage message)
        {
            lock (_lock)
            {
                try
                {
                    byte[] buffer1 = new byte[4];
                    _serialPort.Read(buffer1, 0, 4);

                    var sequence_high = buffer1[0];
                    var sequence_low = buffer1[1];
                    short sequence = (short)(sequence_high << 8 | sequence_low);
                    byte type = buffer1[2];
                    byte length = buffer1[3];

                    if (sequence != message.Sequence)
                    {
                        throw new IOException("Sequence");
                    }

                    if (length > 0)
                    {
                        byte[] buffer2 = new byte[length * 2];
                        _serialPort.Read(buffer2, 0, length * 2);

                        short[] data = new short[length];

                        for (int i = 0; i < length; i++)
                        {
                            // Big Endian
                            var index = i * 2;
                            var high = (short)buffer2[index];
                            var low = buffer2[index + 1];
                            data[i] = (short)(high << 8 | low);
                        }
                        message.ReceiveData = data;
                    }

                    message.ReceiveType = (DeviceMessageReceiveType)type;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void Open()
        {
            _serialPort.Open();
        }

        public void Close()
        {
            _serialPort.Close();
        }

        public void SendAndReceivePacketSync(DeviceMessage message)
        {
            lock (_lock)
            {
                SendPacketSync(message);
                ReceivePacketSync(message);
            }
        }
        
        public void SendPacketSync(DeviceMessage message)
        {
            lock (_lock)
            {
                try
                {
                    List<byte> dataList = new List<byte>();

                    short sequence = message.Sequence;
                    byte type = (byte)message.SendType;

                    // Big Endian
                    dataList.Add((byte)((sequence >> 8) & 0xFF));
                    dataList.Add((byte)(sequence & 0xFF));
                    dataList.Add(type);

                    if (message.SendData != null)
                    {
                        byte length = (byte)message.SendData.Length;
                        dataList.Add(length);

                        foreach (var b in message.SendData)
                        {
                            var high = (byte)((b >> 8) & 0xFF);
                            var low = (byte)(b & 0xFF);

                            // Big Endian
                            dataList.Add(high);
                            dataList.Add(low);
                        }
                    }
                    else
                    {
                        byte length = 0;
                        dataList.Add(length);
                    }

                    var data = dataList.ToArray();
                    _serialPort.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}

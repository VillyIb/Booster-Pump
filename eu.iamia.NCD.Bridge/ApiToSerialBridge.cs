using System;
using System.Collections.Generic;
using EnsureThat;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Serial.Contract;
using eu.iamia.NCD.Shared;

namespace eu.iamia.NCD.Bridge
{
    /// <summary>
    /// Translates ICommand to 
    /// </summary>
    public class ApiToSerialBridge : IBridge
    {
        private readonly IGateway Gateway;

        public ApiToSerialBridge(IGateway gateway)
        {
            Ensure.That(gateway).IsNotNull();
            Gateway = gateway;
        }

        internal I2CCommandCode GetI2CCommandCode(ICommand command)
        {
            return command switch
            {
                ICommandControllerBusScan => I2CCommandCode.DeviceBusScan,
                ICommandControllerHardReboot => I2CCommandCode.DeviceConverterCommand,
                ICommandControllerReboot => I2CCommandCode.DeviceConverterCommand,
                ICommandControllerStop => I2CCommandCode.DeviceConverterCommand,
                ICommandControllerTest2WayCommunication => I2CCommandCode.DeviceConverterCommand,
                ICommandRead => I2CCommandCode.DeviceRead,
                ICommandWrite => I2CCommandCode.DeviceWrite,
                ICommandWriteRead => I2CCommandCode.DeviceWriteRead,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        internal INcdApiProtocol GetI2CCommand(ICommand command)
        {
            var payload = new List<byte> {(byte) GetI2CCommandCode(command)};
            payload.AddRange(command.I2C_Data());
            var result = new NcdApiProtocol(payload);
            return result;
        }

        public IDataFromDevice Execute(ICommand command)
        {
            var i2CCommand = GetI2CCommand(command);
            var i2CResponse = Gateway.Execute(i2CCommand);
            return i2CResponse is null
                ? null
                : new DataFromDevice(i2CResponse.Header, i2CResponse.ByteCount, i2CResponse.Payload,
                    i2CResponse.Checksum);
        }

        public void Dispose()
        {
            Gateway?.Dispose();
        }
    }
}
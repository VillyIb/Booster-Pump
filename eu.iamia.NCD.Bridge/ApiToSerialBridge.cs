namespace eu.iamia.NCD.Bridge
{
    using System.Collections.Generic;
    using EnsureThat;
    using API.Contract;
    using Shared;

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

        internal static INcdApiProtocol GetI2CCommand(ICommand command)
        {
            var payload = new List<byte> { (byte)command.GetI2CCommandCode };
            payload.AddRange(command.I2C_Data());
            INcdApiProtocol result = new NcdApiProtocol(payload);
            return result;
        }

        public INcdApiProtocol Execute(ICommand command)
        {
            var i2CCommand = GetI2CCommand(command);
            var i2CResponse = Gateway.Execute(i2CCommand);

            return i2CResponse ?? NcdApiProtocol.NoResponse;
        }

        public void Dispose()
        {
            Gateway?.Dispose();
        }
    }
}
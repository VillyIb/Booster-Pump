using System.Collections.Generic;

namespace eu.iamia.NCD.DeviceCommunication.Contract
{
    /// <summary>
    /// Raw I2C Command Request.
    /// </summary>
    public interface ICommand
    {
        IEnumerable<byte> I2C_Data();
    }


    public interface ICommandDevice : ICommand
    {
        byte DeviceAddress { get; }
    }

    // Device commands - actually this might all be serial converter commands - see NCD API...

    public interface IDeviceCommand : ICommand
    { }

    public interface ICommandRead : IDeviceCommand
    { }

    public interface ICommandWrite : IDeviceCommand
    { }

    public interface ICommandWriteRead : IDeviceCommand
    { }

    // Controller commands - might not be relevant for non serial port -> USB -> I2C_Controller.

    public interface IControllerCommand : ICommand
    { }

    // TODO is this a Controller- or a Gateway(bus) Command ? it might be relevant on a DirectGateway.
    public interface ICommandControllerBusScan : IControllerCommand
    { }

    public interface ICommandControllerStop : IControllerCommand
    { }

    public interface ICommandControllerReboot : IControllerCommand
    { }

    public interface ICommandControllerHardReboot : IControllerCommand
    { }

    public interface ICommandControllerTest2WayCommunication : IControllerCommand
    { }
}
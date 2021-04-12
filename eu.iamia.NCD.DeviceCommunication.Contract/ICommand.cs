using System.Collections.Generic;

namespace eu.iamia.NCD.DeviceCommunication.Contract
{
    public interface ICommand
    {
        IEnumerable<byte> I2C_Data();
    }


    public interface ICommandDevice : ICommand
    {
        byte DeviceAddress { get; }
    }

    public interface ICommandRead : ICommandDevice
    { }

    public interface ICommandWrite : ICommandDevice
    { }

    public interface ICommandWriteRead : ICommandDevice
    { }


    public interface ICommandControllerBusScan : ICommand
    { }

    public interface ICommandControllerStop : ICommand
    { }

    public interface ICommandControllerReboot : ICommand
    { }

    public interface ICommandControllerHardReboot : ICommand
    { }

    public interface ICommandControllerTest2WayCommunication : ICommand
    { }
}
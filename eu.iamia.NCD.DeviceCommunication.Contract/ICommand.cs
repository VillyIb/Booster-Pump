using System.Collections.Generic;

namespace eu.iamia.NCD.DeviceCommunication.Contract
{
    public interface ICommand
    {
        byte DeviceAddress { get; set; }

        IEnumerable<byte> I2C_Data();
    }

    public interface ICommandRead : ICommand
    { }

    public interface ICommandWrite : ICommand
    { }

    public interface ICommandWriteRead : ICommand
    { }

    public interface ICommandConverter : ICommand
    { }

    public interface ICommandBusScan : ICommandConverter
    { }

    public interface ICommandStop : ICommandConverter
    { }

    public interface ICommandReboot : ICommandConverter
    { }

    public interface ICommandHardReboot : ICommandConverter
    { }

    public interface ICommandTest2WayCommunication : ICommandConverter
    { }

    public interface ICommandTestStop : ICommandConverter
    { }
}
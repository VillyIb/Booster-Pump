using BoosterPumpLibrary.Modules;
using NCD_API_SerialConverter;

namespace BoosterPumpApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var serialPort = new SerialPortDecorator("COM4");
            var serialConverter = new SerialConverter(serialPort);

            var displayModule = new AS1115_Module(serialConverter);

            try
            {
                serialPort.Open();

                displayModule.Init();

                for (float count = 0.1f; count < 1000f; count += 0.1f)
                {
                    displayModule.SetBcdValue(count);
                    displayModule.Send();
                }
            }
            finally
            {
                serialPort.Dispose();
            }
        }
    }
}

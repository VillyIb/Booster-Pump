using BoosterPumpLibrary.Modules;
using NCD_API_SerialConverter;

namespace BoosterPumpApplication1
{
    public class Program
    {
        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            var serialPort = new SerialPortDecorator("COM4");
            var serialConverter = new SerialConverter(serialPort);

            var displayModule = new As1115Module(serialConverter);

            try
            {
                serialPort.Open();

                displayModule.Init();

                for (var count = 0.1f; count < 1000f; count += 0.1f)
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

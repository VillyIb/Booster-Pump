namespace eu.iamia.NCD.Serial.Contract
{
    public interface IGateway
    {
        /// <summary>
        /// Send Command to - and receive Response from I2C Device.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        IDataFromDevice Execute(IDataToDevice command);
    }
}

namespace eu.iamia.I2CContract
{
    public interface I2CCommunication
    {
        IDataFromDevice Execute(IDataToDevice command);
    }
}

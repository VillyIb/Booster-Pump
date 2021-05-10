namespace eu.iamia.NCD.API.Contract
{
    public interface IBridge
    {
        public IDataFromDevice Execute(ICommand command);
    }
}
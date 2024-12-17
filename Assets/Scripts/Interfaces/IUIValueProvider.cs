public interface IUIValueProvider<T> : IInterfaceIdentity
{
    public T BaseValue();
    public T CurrentValue();
}
namespace FilterOrder.Service.Logger;
internal sealed class NullScope : IDisposable
{
    public static NullScope Instance { get; } = new NullScope();
    public void Dispose() { }
}

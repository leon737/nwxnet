namespace NWXNet
{
    public interface IRequestData
    {
        RequestTypes Type { get; }
        bool IsValid { get; }
        string Id { get; }
    }
}
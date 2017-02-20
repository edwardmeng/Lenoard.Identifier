namespace Lenoard.Identifier.AspNetCore
{
    public interface ICorrelationFeature
    {
        string CorrelationIdentity { get; set; }
    }
}

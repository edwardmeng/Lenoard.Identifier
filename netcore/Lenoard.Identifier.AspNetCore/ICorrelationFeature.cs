using System;

namespace Lenoard.Identifier.AspNetCore
{
    public interface ICorrelationFeature
    {
        Guid CorrelationID { get; }
    }
}

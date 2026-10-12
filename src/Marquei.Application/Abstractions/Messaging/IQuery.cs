namespace Marquei.Application.Abstractions.Messaging;

public interface IQuery<TResponse>
{
}

public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query);
}
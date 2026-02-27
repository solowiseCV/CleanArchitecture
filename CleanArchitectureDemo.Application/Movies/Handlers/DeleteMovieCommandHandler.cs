using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Movies.Commands;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Movies.Handlers;

public class DeleteMovieCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<DeleteMovieCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMovieCommand command, CancellationToken cancellationToken)
    {
        var movie = await unitOfWork.Movies.GetByIdAsync(command.Id, cancellationToken) ?? throw new NotFoundException(nameof(Movie), command.Id);
        unitOfWork.Movies.Delete(movie);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Unit.Value;
    }
}

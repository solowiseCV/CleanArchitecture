using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Movies.Commands;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Movies.Handlers;

public class UpdateMovieCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<UpdateMovieCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMovieCommand command, CancellationToken cancellationToken)
    {
        var movie = await unitOfWork.Movies.GetByIdAsync(command.Id, cancellationToken);
        
        if (movie == null)
        {
            throw new NotFoundException(nameof(Movie), command.Id);
        }

        mapper.Map(command.Request, movie);
        unitOfWork.Movies.Update(movie);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Unit.Value;
    }
}

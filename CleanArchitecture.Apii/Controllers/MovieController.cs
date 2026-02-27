using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Movies.Commands;
using CleanArchitecture.Application.Movies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/movies/")]
    public class MovieController(IMediator mediator) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<MovieResponse>>> GetAll()
        {
            var result = await mediator.Send(new GetAllMoviesQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MovieResponse>> Create(CreateMovieRequest request)
        {
            var result = await mediator.Send(new CreateMovieCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = result.MovieId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateMovieRequest request)
        {
            await mediator.Send(new UpdateMovieCommand(id, request));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await mediator.Send(new DeleteMovieCommand(id));
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieResponse>> GetById(Guid id)
        {
            var result = await mediator.Send(new GetMovieByIdQuery(id));
            return Ok(result); 
        }
    }
}
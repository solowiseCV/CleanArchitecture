using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Movies.Commands;
using CleanArchitecture.Application.Movies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchitecture.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/movies/")]
    public class MovieController(IMediator mediator) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [SwaggerOperation(Summary = "Get all movies", Tags = new[] { "Movies" })]
        [ProducesResponseType(typeof(List<MovieResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<MovieResponse>>> GetAll()
        {
            var result = await mediator.Send(new GetAllMoviesQuery());
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new movie", Tags = new[] { "Movies" })]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MovieResponse>> Create(CreateMovieRequest request)
        {
            var result = await mediator.Send(new CreateMovieCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = result.MovieId }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing movie", Tags = new[] { "Movies" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, UpdateMovieRequest request)
        {
            await mediator.Send(new UpdateMovieCommand(id, request));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a movie", Tags = new[] { "Movies" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await mediator.Send(new DeleteMovieCommand(id));
            return NoContent();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get movie by id", Tags = new[] { "Movies" })]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieResponse>> GetById(Guid id)
        {
            var result = await mediator.Send(new GetMovieByIdQuery(id));
            return Ok(result); 
        }
    }
}
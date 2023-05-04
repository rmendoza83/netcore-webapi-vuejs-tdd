namespace TodoCrud.Api.Controllers;

using TodoCrud.Business.Services;
using TodoCrud.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseController<TEntity> : ControllerBase 
    where TEntity : class, IEntity, new()
{
    private readonly IBaseService<TEntity> _service;
    private readonly ILogger<IBaseService<TEntity>> _logger;

    protected BaseController(IBaseService<TEntity> service, ILogger<IBaseService<TEntity>> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TEntity>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _service.GetByIdAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(new EventId(), ex, "GetByIdAsync Error: {Message}", ex.Message);
            return StatusCode(500);
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TEntity>>> GetAllAsync()
    {
        try
        {
            var entities = await _service.GetAllAsync();

            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(new EventId(), ex, "GetAllAsync Error: {Message}", ex.Message);
            return Ok();
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InsertAsync(TEntity entity)
    {
        try
        {
            await _service.InsertAsync(entity);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(new EventId(), ex, "InsertAsync Error: {Message}", ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync(Guid id, TEntity entity)
    {
        try
        {
            if (id == entity.Id)
            {
                await _service.UpdateAsync(entity);

                return NoContent();
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(new EventId(), ex, "UpdateAsync Error: {Message}", ex.Message);
            return StatusCode(500);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(new EventId(), ex, "UpdateAsync Error: {Message}", ex.Message);
            return StatusCode(500);
        }
    }
}
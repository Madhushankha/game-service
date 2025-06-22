using GameService.Model;
using GameService.Services;
using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _service;

    public GamesController(IGameService service)
        => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create(GameModel dto)
    {
        var game = await _service.CreateAsync(dto);
        return game is not null ? Ok(game) : NoContent();
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetById(string id)
    {
        var game = await _service.GetByIdDetachedAsync(id);
        return game is not null ? Ok(game) : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> ListAll()
        => Ok(await _service.ListAllDetachedAsync());

    [HttpPut("id")]
    public async Task<IActionResult> Update(string id, GameModel dto)
    {
        if (id != dto.Id) return BadRequest();
        var updated = await _service.UpdateAsync(dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("id")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
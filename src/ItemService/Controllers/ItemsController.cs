using ItemService.Common.Interfaces;
using ItemService.Interfaces;
using ItemService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItemService.Controllers;

[ApiController]
public class ItemsController : ControllerBase
{
    private readonly IItemService _repository;

    public ItemsController(IItemService repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IEnumerable<ItemResponse>> GetItemsAsync()
    {
        var items = (await _repository.GetItemsAsync())
            .Select(item => item.AsDto());

        return items;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemResponse>> GetItemAsync(Guid id)
    {
        var item = await _repository.GetItemAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return item.AsDto();
    }

    [HttpPost]
    public async Task<ActionResult<ItemResponse>> CreateItemAsync(ItemRequest itemDto)
    {
        Item item = new()
        {
            Id = Guid.NewGuid(),
            Name = itemDto.Name,
            Price = itemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await _repository.CreateItemAsync(item);

        return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
    {
        var existingItem = await _repository.GetItemAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        Item updatedItem = existingItem with
        {
            Name = itemDto.Name,
            Price = itemDto.Price
        };

        await _repository.UpdateItemAsync(updatedItem);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItemAsync(Guid id)
    {
        var existingItem = await _repository.GetItemAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        await _repository.DeleteItemAsync(id);

        return NoContent();
    }
}
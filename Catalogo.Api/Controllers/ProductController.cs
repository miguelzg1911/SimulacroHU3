using Catalogo.Application.Interfaces;
using Catalogo.Application.Services;
using Catalogo.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService productService)
    {
        _service = productService;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var products = await _service.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var products = await _service.GetByIdAsync(id);
        return products is not null ?  Ok(products) : NotFound(); 
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Product product)
    {
        var created = await _service.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] Product product)
    {
        var updated = await _service.UpdateAsync(id, product);
        return updated ?  NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ?  NoContent() : NotFound();
    }
}
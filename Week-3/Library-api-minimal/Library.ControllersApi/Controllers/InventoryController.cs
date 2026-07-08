using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using AutoMapper;
using Library.ControllersApi.DTOs;
using Library.ControllersApi.Services;
using Library.Data;
using Microsoft.AspNetCore.Mvc; // ControllerBase lives here

[ApiController] // this annotation tells ASP.NET to map this controller during app.MapControllers()
[Route("api/[controller]")] // Pretty sure this will be localhost:5051/api/Inventory as the route base
public class InventoryController : ControllerBase
{
    // This will be removed tomorrow for sure
    private readonly IInventoryService _service;
    private readonly IMapper _mapper; // automapper object

    public InventoryController(IInventoryService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    // Let's write our first GET endpoint
    [HttpGet] // IActionResult just represents possible HTTP response actions
    public async Task<ActionResult<IEnumerable<InventoryDTO>>> Get()
    {
        var items =await _service.AllAsync();


        var mappedItems = _mapper.Map<List<InventoryDTO>>(items);

        return Ok(mappedItems);
        // // As is this creates an infinite loop when we try to serialize to JSON
        // //return Ok(await _repo.GetAllAsync());

        // // The fix is using DTO - Data Transfer Object. In general it is bad practice
        // // to send models as returns (or take them as arguments) to/from controller methods
        // // models are for your API, not for the front end

        // var items = await _repo.GetAllAsync(); // Get all items

        // // This is what we will send back once we populate it
        // EntireInventoryDTO response = new();

        // // Now we need to map to those DTO's
        // foreach(var item in items)
        // {
        //    InventoryReturnDTO i = new InventoryReturnDTO
        //     {
        //         Name = item.Product.Name,
        //         Sku = item.Product.Sku,
        //         CurrentStock = item.CurrentStock
        //     };
        //     response.EntireInventory.Add(i);
        // }

        // // Returning
        // return Ok(response);
    }

    // localhost:5137/api/Inventory/{sku} - sku is passed in by the user
    // We can add routing info right on the annotation
    [HttpGet("{sku}")] // I can parameterize the route itself
    public async Task<ActionResult<InventoryDTO>> GetBySku(string sku)
    {

        var item = await _service.BySkuAsync(sku);

        if(item is null)
        {
            return NotFound(); // 404 not found
        }
        else
        {
            var mappedItems = _mapper.Map<InventoryDTO>(item);
            return Ok(mappedItems);
        }
        // var item = await _repo.GetInventoryItemBySkuAsync(sku);

        // var response = new InventoryReturnDTO
        // {
        //     Name = item.Product.Name,
        //     Sku = item.Product.Sku,
        //     CurrentStock = item.CurrentStock
        // };

        // Then we check what to return based on item being null or not
        //return Ok(response); // 200 - found something - sent back to front end
    }
}
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using AutoMapper;
using Library.ControllersApi.DTOs;
using Library.ControllersApi.Services;
using Library.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // ControllerBase lives here

[ApiController] // this annotation tells ASP.NET to map this controller during app.MapControllers()
[Route("api/[controller]")] // Pretty sure this will be localhost:5051/api/Inventory as the route base
public class InventoryController : ControllerBase
{
    // This will be removed tomorrow for sure
    private readonly IInventoryService _service;
    private readonly IMapper _mapper; // automapper object

    // One share instance for the whole app - singleton.
    private readonly IMemoryCache _cache; // server side cache

    private  readonly ISupplierClient _supplier;

    public InventoryController(IInventoryService service, IMapper mapper, IMemoryCache cache, ISupplierClient supplier)
    {
        _service = service;
        _mapper = mapper;
        _cache = cache;
        _supplier = supplier;
    }

    // Let's write our first GET endpoint
    [HttpGet] // IActionResult just represents possible HTTP response actions
    [ResponseCache(Duration = 30)] // adding response cache-ing, now that we've set it up in Program.cs
    public async Task<ActionResult<IEnumerable<InventoryDTO>>> Get()
    {
        // Let's add server side cache-ing - still straightforward but we have to think a little harder
        // We have to think about when/where to add the logic to add something to the cache - and also
        // when to invalidates it.

        // First check the cache. If it's there AND valid, pull from it. Otherwise,
        // we will add whatever we get during this method to the cache
        var dtos = await _cache.GetOrCreateAsync("inventory:all", async entry =>
        {
            // Setting things about our cache entry - like "expire no matter what after 2 minutes"
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            // Actually get the items from DB
            var items = await _service.AllAsync();

            // Return to front end (and also add to cache, since we´re wrapped by _cache.GetOrCreateAsync)
            return _mapper.Map<List<InventoryDTO>>(items);
        });

        return Ok(dtos);

        // var items =await _service.AllAsync();

        // var mappedItems = _mapper.Map<List<InventoryDTO>>(items);

        // return Ok(mappedItems);
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

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<InventoryDTO>> Create(InventoryCreateDto newInv)
    {
        var created = await _service.AddAsync(newInv);
        var response = _mapper.Map<InventoryDTO>(created);

        // CreatedAt (201) works a little differently from our other response ActionResults
        // Created at needs to know how to find the newly created resource - so we tell it
        // Use the GetBySku controller method (literally the one above) and use the information
        // in response to build the URI string


        return CreatedAtAction(nameof(GetBySku), new {sku = response.Sku}, response);
    }

    [HttpDelete("sku")]
    [Authorize(Roles = "admin")] 
    public async Task<ActionResult> Delete(string sku)
    {
        bool isDeleted = await _service.RemoveAsync(sku);

        if (isDeleted)
        {
            
        // Invalidating whatever is in cache - because DB state has changed

            _cache.Remove("inventory:all");
            return NoContent(); // 204 No content - it WAS there , not anymore
        }
        else
        {
            return NotFound(); // 404 - Couldn't delete it because your sku was wrong
            // return StatusCode(404, "Not found");
        }
    }

    // New GET that uses that SupplierClient to call an outside API
    // Localhost:5173/api/Inventory/{sku}/supplier-price
    [HttpGet("{sku}/supplier-price")]
    [Authorize] // No valid token returns a 401. No code ever runs. Any role but "admin" = 403.
    public async Task<IActionResult> GetSupplierPrice(string sku)
    {
        // Call our supplier with the httpclient code
        var price = await _supplier.GetListPriceAsync(sku);

        if(price is null)
        {
            return NotFound(); // No price found
        }

        //Returning an inline object for now, no DTO
        return Ok(new{sku, supplierPrice=price});

    }
}
namespace Library.ControllersApi.DTOs;

// This DTO is for taking in info to then create a new row in my DB
// I can use Data Annotation to enforce constraints on the information
// If the front end/user violates the rules I setup, ASP.NET bounces back a 400 automatically (Bad Request)
public record InventoryCreateDto(
    
);
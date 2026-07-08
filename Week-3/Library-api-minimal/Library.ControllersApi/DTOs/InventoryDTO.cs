namespace Library.ControllersApi.DTOs;

// I won't need to add methods or a constructor to this - its only job
// is passing info to <-> from the frontend (swagger, React website, etc)
// this solves the JSON loop - as well as saves the front end from having to pass
// massive objects for no reason.
public record InventoryDTO(string Sku, string Name, int CurrentStock);
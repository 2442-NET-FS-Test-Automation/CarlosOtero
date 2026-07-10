namespace HospitalApi.Services.Infrastructure;

public interface ISeederService
{
    public Task ResetDatabaseAsync();
}
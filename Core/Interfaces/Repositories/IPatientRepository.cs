using Core.Models;

namespace Core.Interfaces.Repositories
{
    public interface IPatientRepository : IBaseRepository<Patient>
    {
        Task<Patient?> GetPatientByPatientId(string patientId);
    }
}

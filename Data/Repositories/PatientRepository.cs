using Core.Interfaces.Repositories;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class PatientRepository(AppDbContext context) : BaseRepository<Patient>(context), IPatientRepository
    {
        public async Task<Patient?> GetPatientByPatientId(string patientId) =>
            await _context!.Patients.FirstOrDefaultAsync(pt => pt.PatientId == patientId);

    }
}

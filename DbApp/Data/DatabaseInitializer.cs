using DbApp.Models;
using DbApp.Configuration;
using Npgsql;

namespace DbApp.Data;

/// <summary>
/// Handles database initialization, including doctor seeding on first run.
/// </summary>
public static class DatabaseInitializer
{
    private const string InitializationFlagKey = "DatabaseInitialized";

    /// <summary>
    /// Initializes the database with default doctors if this is the first run.
    /// </summary>
    /// <param name="context">The database context.</param>
    public static void InitializeDoctors(MedicalContext context)
    {
        // Check if doctors already exist
        var existingDoctors = context.Doctors.ToList();
        if (existingDoctors.Count > 0)
        {
            return; // Already initialized
        }


        // Seed default doctors
        var doctors = new List<Doctor>
        {
            new Doctor { FirstName = "Ivan", LastName = "Horvat", Specialization = "Cardiology" },
            new Doctor { FirstName = "Ana", LastName = "Kovač", Specialization = "Neurology" },
            new Doctor { FirstName = "Marko", LastName = "Novak", Specialization = "Radiology" },
            new Doctor { FirstName = "Petra", LastName = "Babić", Specialization = "Dermatology" },
            new Doctor { FirstName = "Tomislav", LastName = "Jurić", Specialization = "Ophthalmology" },
            new Doctor { FirstName = "Maja", LastName = "Matić", Specialization = "Dentistry" },
            new Doctor { FirstName = "Josip", LastName = "Knežević", Specialization = "Mammography" },
            new Doctor { FirstName = "Sara", LastName = "Lovrić", Specialization = "General Medicine" }
        };

        // Add and save each doctor individually to avoid primary key conflicts
        foreach (var doctor in doctors)
        {
            try
            {
                context.Doctors.Add(doctor);
                context.SaveChanges(); // Save immediately so the ID is assigned before adding the next one

                context.ChangeTracker.Clear();
            }
            catch (PostgresException ex) when (ex.SqlState == "23502") // NOT NULL constraint violation
            {
                throw new InvalidOperationException(
                    "The 'doctor' table was not created with auto-increment. " +
                    "Please drop the table and let migrations recreate it, or run migrations to fix the schema.", ex);
            }
        }
    }
}


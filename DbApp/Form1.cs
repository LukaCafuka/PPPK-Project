using DbApp.Configuration;
using DbApp.Data;

namespace DbApp;

public partial class Form1 : Form
{
    private MedicalContext? _context;

    public Form1()
    {
        InitializeComponent();
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            _context = new MedicalContext(DatabaseConfig.ConnectionString);
            
            // Check for pending migrations
            var migration = _context.GenerateMigration();
            if (migration != null)
            {
                var result = MessageBox.Show(
                    "Database schema needs to be updated. Would you like to apply migrations now?",
                    "Database Migration Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _context.Migrations.ExecuteUp(migration);
                    MessageBox.Show("Database migrations applied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // Initialize doctors on first run
            DatabaseInitializer.InitializeDoctors(_context);

            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing database: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadData()
    {
        if (_context == null) return;

        try
        {
            // Load data into each tab
            patientListForm1?.LoadPatients(_context);
            diseaseHistoryForm1?.LoadDiseaseHistories(_context);
            medicationForm1?.LoadMedications(_context);
            examinationForm1?.LoadExaminations(_context);
            doctorListForm1?.LoadDoctors(_context);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Refreshes all data in all forms. Call this after CRUD operations.
    /// </summary>
    public void RefreshAllData()
    {
        LoadData();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _context?.Dispose();
        base.OnFormClosing(e);
    }
}

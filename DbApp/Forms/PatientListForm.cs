using DbApp.Data;
using DbApp.Models;
using DbApp.Forms.Dialogs;
using DbApp;

namespace DbApp.Forms;

public partial class PatientListForm : UserControl
{
    private DataGridView dataGridViewPatients;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnViewDetails;
    private TextBox txtSearch;
    private Button btnSearch;
    private Label lblSearch;

    public PatientListForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.dataGridViewPatients = new DataGridView();
        this.btnAdd = new Button();
        this.btnEdit = new Button();
        this.btnDelete = new Button();
        this.btnViewDetails = new Button();
        this.txtSearch = new TextBox();
        this.btnSearch = new Button();
        this.lblSearch = new Label();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatients)).BeginInit();
        this.SuspendLayout();
        // 
        // lblSearch
        // 
        this.lblSearch.AutoSize = true;
        this.lblSearch.Location = new Point(12, 15);
        this.lblSearch.Name = "lblSearch";
        this.lblSearch.Size = new Size(45, 15);
        this.lblSearch.TabIndex = 0;
        this.lblSearch.Text = "Search:";
        // 
        // txtSearch
        // 
        this.txtSearch.Location = new Point(63, 12);
        this.txtSearch.Name = "txtSearch";
        this.txtSearch.Size = new Size(300, 23);
        this.txtSearch.TabIndex = 1;
        // 
        // btnSearch
        // 
        this.btnSearch.Location = new Point(369, 12);
        this.btnSearch.Name = "btnSearch";
        this.btnSearch.Size = new Size(75, 23);
        this.btnSearch.TabIndex = 2;
        this.btnSearch.Text = "Search";
        this.btnSearch.UseVisualStyleBackColor = true;
        this.btnSearch.Click += BtnSearch_Click;
        // 
        // btnAdd
        // 
        this.btnAdd.Location = new Point(12, 50);
        this.btnAdd.Name = "btnAdd";
        this.btnAdd.Size = new Size(100, 30);
        this.btnAdd.TabIndex = 3;
        this.btnAdd.Text = "Add Patient";
        this.btnAdd.UseVisualStyleBackColor = true;
        this.btnAdd.Click += BtnAdd_Click;
        // 
        // btnEdit
        // 
        this.btnEdit.Location = new Point(118, 50);
        this.btnEdit.Name = "btnEdit";
        this.btnEdit.Size = new Size(100, 30);
        this.btnEdit.TabIndex = 4;
        this.btnEdit.Text = "Edit";
        this.btnEdit.UseVisualStyleBackColor = true;
        this.btnEdit.Click += BtnEdit_Click;
        // 
        // btnDelete
        // 
        this.btnDelete.Location = new Point(224, 50);
        this.btnDelete.Name = "btnDelete";
        this.btnDelete.Size = new Size(100, 30);
        this.btnDelete.TabIndex = 5;
        this.btnDelete.Text = "Delete";
        this.btnDelete.UseVisualStyleBackColor = true;
        this.btnDelete.Click += BtnDelete_Click;
        // 
        // btnViewDetails
        // 
        this.btnViewDetails.Location = new Point(330, 50);
        this.btnViewDetails.Name = "btnViewDetails";
        this.btnViewDetails.Size = new Size(100, 30);
        this.btnViewDetails.TabIndex = 6;
        this.btnViewDetails.Text = "View Details";
        this.btnViewDetails.UseVisualStyleBackColor = true;
        this.btnViewDetails.Click += BtnViewDetails_Click;
        // 
        // dataGridViewPatients
        // 
        this.dataGridViewPatients.AllowUserToAddRows = false;
        this.dataGridViewPatients.AllowUserToDeleteRows = false;
        this.dataGridViewPatients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewPatients.Location = new Point(12, 90);
        this.dataGridViewPatients.Name = "dataGridViewPatients";
        this.dataGridViewPatients.ReadOnly = true;
        this.dataGridViewPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewPatients.Size = new Size(1176, 564);
        this.dataGridViewPatients.TabIndex = 7;
        this.dataGridViewPatients.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.dataGridViewPatients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // 
        // PatientListForm
        // 
        this.Controls.Add(this.dataGridViewPatients);
        this.Controls.Add(this.btnViewDetails);
        this.Controls.Add(this.btnDelete);
        this.Controls.Add(this.btnEdit);
        this.Controls.Add(this.btnAdd);
        this.Controls.Add(this.btnSearch);
        this.Controls.Add(this.txtSearch);
        this.Controls.Add(this.lblSearch);
        this.Name = "PatientListForm";
        this.Size = new Size(1200, 700);
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatients)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    public void LoadPatients(MedicalContext context)
    {
        try
        {
            // Use ORM query capabilities for sorting
            var patients = context.Patients.AsQueryable()
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToList();
            
            dataGridViewPatients.DataSource = patients.Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.Oib,
                DateOfBirth = p.DateOfBirth.ToString("yyyy-MM-dd"),
                p.Gender,
                ResidenceAddress = p.ResidenceAddress ?? "",
                PermanentAddress = p.PermanentAddress ?? ""
            }).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading patients: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        using var form = new PatientDetailForm(null, context);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadPatients(context);
            // Notify parent form to refresh all data
            if (ParentForm is Form1 mainForm)
            {
                mainForm.RefreshAllData();
            }
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (dataGridViewPatients.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a patient to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedId = (long)dataGridViewPatients.SelectedRows[0].Cells["Id"].Value;
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        var patient = context.Patients.Find(selectedId);

        if (patient != null)
        {
            using var form = new PatientDetailForm(patient, context);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadPatients(context);
                // Notify parent form to refresh all data
                if (ParentForm is Form1 mainForm)
                {
                    mainForm.RefreshAllData();
                }
            }
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (dataGridViewPatients.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a patient to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this patient?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes) return;

        var selectedId = (long)dataGridViewPatients.SelectedRows[0].Cells["Id"].Value;
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        
        try
        {
            context.Patients.Remove(selectedId);
            context.SaveChanges();
            MessageBox.Show("Patient deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadPatients(context);
            // Notify parent form to refresh all data
            if (ParentForm is Form1 mainForm)
            {
                mainForm.RefreshAllData();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting patient: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnViewDetails_Click(object? sender, EventArgs e)
    {
        if (dataGridViewPatients.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a patient to view.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedId = (long)dataGridViewPatients.SelectedRows[0].Cells["Id"].Value;
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        
        // Use eager loading to load patient with related data
        var patient = context.Patients.AsQueryable()
            .Include(p => p.DiseaseHistories)
            .Include(p => p.Medications)
            .Include(p => p.Examinations)
            .Where(p => p.Id == selectedId)
            .FirstOrDefault();

        if (patient != null)
        {
            using var form = new PatientDetailForm(patient, context, readOnly: true);
            form.ShowDialog();
        }
    }

    private void BtnSearch_Click(object? sender, EventArgs e)
    {
        var searchText = txtSearch.Text.Trim();
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        
        if (string.IsNullOrEmpty(searchText))
        {
            LoadPatients(context);
            return;
        }

        try
        {
            // Use ORM query capabilities for filtering
            var patients = context.Patients.AsQueryable()
                .Where(p => p.FirstName.Contains(searchText) || 
                           p.LastName.Contains(searchText) || 
                           p.Oib.Contains(searchText))
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToList();

            dataGridViewPatients.DataSource = patients.Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.Oib,
                DateOfBirth = p.DateOfBirth.ToString("yyyy-MM-dd"),
                p.Gender,
                ResidenceAddress = p.ResidenceAddress ?? "",
                PermanentAddress = p.PermanentAddress ?? ""
            }).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching patients: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}



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
        dataGridViewPatients = new DataGridView();
        btnAdd = new Button();
        btnEdit = new Button();
        btnDelete = new Button();
        btnViewDetails = new Button();
        txtSearch = new TextBox();
        btnSearch = new Button();
        lblSearch = new Label();
        ((System.ComponentModel.ISupportInitialize)dataGridViewPatients).BeginInit();
        SuspendLayout();
        // 
        // dataGridViewPatients
        // 
        dataGridViewPatients.AllowUserToAddRows = false;
        dataGridViewPatients.AllowUserToDeleteRows = false;
        dataGridViewPatients.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridViewPatients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewPatients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewPatients.Location = new Point(12, 90);
        dataGridViewPatients.Name = "dataGridViewPatients";
        dataGridViewPatients.ReadOnly = true;
        dataGridViewPatients.RowHeadersWidth = 51;
        dataGridViewPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewPatients.Size = new Size(1176, 564);
        dataGridViewPatients.TabIndex = 7;
        // 
        // btnAdd
        // 
        btnAdd.Location = new Point(12, 50);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(100, 30);
        btnAdd.TabIndex = 3;
        btnAdd.Text = "Add Patient";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += BtnAdd_Click;
        // 
        // btnEdit
        // 
        btnEdit.Location = new Point(118, 50);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(100, 30);
        btnEdit.TabIndex = 4;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += BtnEdit_Click;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(224, 50);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(100, 30);
        btnDelete.TabIndex = 5;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += BtnDelete_Click;
        // 
        // btnViewDetails
        // 
        btnViewDetails.Location = new Point(330, 50);
        btnViewDetails.Name = "btnViewDetails";
        btnViewDetails.Size = new Size(100, 30);
        btnViewDetails.TabIndex = 6;
        btnViewDetails.Text = "View Details";
        btnViewDetails.UseVisualStyleBackColor = true;
        btnViewDetails.Click += BtnViewDetails_Click;
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(63, 12);
        txtSearch.Name = "txtSearch";
        txtSearch.Size = new Size(300, 27);
        txtSearch.TabIndex = 1;
        // 
        // btnSearch
        // 
        btnSearch.Location = new Point(369, 12);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(75, 27);
        btnSearch.TabIndex = 2;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += BtnSearch_Click;
        // 
        // lblSearch
        // 
        lblSearch.AutoSize = true;
        lblSearch.Location = new Point(12, 15);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(56, 20);
        lblSearch.TabIndex = 0;
        lblSearch.Text = "Search:";
        // 
        // PatientListForm
        // 
        Controls.Add(dataGridViewPatients);
        Controls.Add(btnViewDetails);
        Controls.Add(btnDelete);
        Controls.Add(btnEdit);
        Controls.Add(btnAdd);
        Controls.Add(btnSearch);
        Controls.Add(txtSearch);
        Controls.Add(lblSearch);
        Name = "PatientListForm";
        Size = new Size(1200, 700);
        ((System.ComponentModel.ISupportInitialize)dataGridViewPatients).EndInit();
        ResumeLayout(false);
        PerformLayout();
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



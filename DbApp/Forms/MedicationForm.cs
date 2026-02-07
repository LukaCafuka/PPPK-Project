using DbApp.Data;
using DbApp.Models;
using DbApp.Forms.Dialogs;
using DbApp;

namespace DbApp.Forms;

public partial class MedicationForm : UserControl
{
    private DataGridView dataGridViewMedications;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private ComboBox cmbPatientFilter;
    private Label lblPatientFilter;

    public MedicationForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.dataGridViewMedications = new DataGridView();
        this.btnAdd = new Button();
        this.btnEdit = new Button();
        this.btnDelete = new Button();
        this.cmbPatientFilter = new ComboBox();
        this.lblPatientFilter = new Label();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMedications)).BeginInit();
        this.SuspendLayout();
        // 
        // lblPatientFilter
        // 
        this.lblPatientFilter.AutoSize = true;
        this.lblPatientFilter.Location = new Point(12, 15);
        this.lblPatientFilter.Name = "lblPatientFilter";
        this.lblPatientFilter.Size = new Size(48, 15);
        this.lblPatientFilter.TabIndex = 0;
        this.lblPatientFilter.Text = "Patient:";
        // 
        // cmbPatientFilter
        // 
        this.cmbPatientFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbPatientFilter.Location = new Point(66, 12);
        this.cmbPatientFilter.Name = "cmbPatientFilter";
        this.cmbPatientFilter.Size = new Size(300, 23);
        this.cmbPatientFilter.TabIndex = 1;
        this.cmbPatientFilter.SelectedIndexChanged += CmbPatientFilter_SelectedIndexChanged;
        // 
        // btnAdd
        // 
        this.btnAdd.Location = new Point(12, 50);
        this.btnAdd.Name = "btnAdd";
        this.btnAdd.Size = new Size(100, 30);
        this.btnAdd.TabIndex = 2;
        this.btnAdd.Text = "Add Medication";
        this.btnAdd.UseVisualStyleBackColor = true;
        this.btnAdd.Click += BtnAdd_Click;
        // 
        // btnEdit
        // 
        this.btnEdit.Location = new Point(118, 50);
        this.btnEdit.Name = "btnEdit";
        this.btnEdit.Size = new Size(100, 30);
        this.btnEdit.TabIndex = 3;
        this.btnEdit.Text = "Edit";
        this.btnEdit.UseVisualStyleBackColor = true;
        this.btnEdit.Click += BtnEdit_Click;
        // 
        // btnDelete
        // 
        this.btnDelete.Location = new Point(224, 50);
        this.btnDelete.Name = "btnDelete";
        this.btnDelete.Size = new Size(100, 30);
        this.btnDelete.TabIndex = 4;
        this.btnDelete.Text = "Delete";
        this.btnDelete.UseVisualStyleBackColor = true;
        this.btnDelete.Click += BtnDelete_Click;
        // 
        // dataGridViewMedications
        // 
        this.dataGridViewMedications.AllowUserToAddRows = false;
        this.dataGridViewMedications.AllowUserToDeleteRows = false;
        this.dataGridViewMedications.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewMedications.Location = new Point(12, 90);
        this.dataGridViewMedications.Name = "dataGridViewMedications";
        this.dataGridViewMedications.ReadOnly = true;
        this.dataGridViewMedications.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewMedications.Size = new Size(1176, 564);
        this.dataGridViewMedications.TabIndex = 5;
        this.dataGridViewMedications.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.dataGridViewMedications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // 
        // MedicationForm
        // 
        this.Controls.Add(this.dataGridViewMedications);
        this.Controls.Add(this.btnDelete);
        this.Controls.Add(this.btnEdit);
        this.Controls.Add(this.btnAdd);
        this.Controls.Add(this.cmbPatientFilter);
        this.Controls.Add(this.lblPatientFilter);
        this.Name = "MedicationForm";
        this.Size = new Size(1200, 700);
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMedications)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    public void LoadMedications(MedicalContext context)
    {
        try
        {
            var patients = context.Patients.ToList();
            cmbPatientFilter.Items.Clear();
            cmbPatientFilter.Items.Add("All Patients");
            foreach (var patient in patients)
            {
                cmbPatientFilter.Items.Add($"{patient.FirstName} {patient.LastName} (ID: {patient.Id})");
            }
            if (cmbPatientFilter.Items.Count > 0)
                cmbPatientFilter.SelectedIndex = 0;

            LoadMedicationData(context);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading medications: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadMedicationData(MedicalContext context)
    {
        try
        {
            var query = context.Medications.AsQueryable();
            
            if (cmbPatientFilter.SelectedIndex > 0)
            {
                var selectedText = cmbPatientFilter.SelectedItem?.ToString() ?? "";
                var patientIdStr = selectedText.Split("(ID: ")[1].TrimEnd(')');
                if (long.TryParse(patientIdStr, out var patientId))
                {
                    // Use ORM query capabilities for filtering
                    query = query.Where(m => m.PatientId == patientId);
                }
            }

            var medications = query
                .OrderByDescending(m => m.PrescribedDate)
                .ToList();

            dataGridViewMedications.DataSource = medications.Select(m => new
            {
                m.Id,
                PatientId = m.PatientId,
                m.MedicationName,
                m.Dose,
                m.DoseUnit,
                m.Frequency,
                Condition = m.Condition ?? "",
                PrescribedDate = m.PrescribedDate.ToString("yyyy-MM-dd")
            }).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading medication data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CmbPatientFilter_SelectedIndexChanged(object? sender, EventArgs e)
    {
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        LoadMedicationData(context);
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        using var form = new MedicationDetailForm(null, context);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadMedications(context);
            // Notify parent form to refresh all data
            if (ParentForm is Form1 mainForm)
            {
                mainForm.RefreshAllData();
            }
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (dataGridViewMedications.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a medication to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedId = (long)dataGridViewMedications.SelectedRows[0].Cells["Id"].Value;
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        var medication = context.Medications.Find(selectedId);

        if (medication != null)
        {
            using var form = new MedicationDetailForm(medication, context);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadMedications(context);
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
        if (dataGridViewMedications.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a medication to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this medication?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes) return;

        var selectedId = (long)dataGridViewMedications.SelectedRows[0].Cells["Id"].Value;
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        
        try
        {
            context.Medications.Remove(selectedId);
            context.SaveChanges();
            MessageBox.Show("Medication deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadMedications(context);
            // Notify parent form to refresh all data
            if (ParentForm is Form1 mainForm)
            {
                mainForm.RefreshAllData();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting medication: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}



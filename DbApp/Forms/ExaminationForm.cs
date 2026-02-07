using DbApp.Data;
using DbApp.Models;
using DbApp.Forms.Dialogs;
using DbApp;

namespace DbApp.Forms;

public partial class ExaminationForm : UserControl
{
    private DataGridView dataGridViewExaminations;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private ComboBox cmbPatientFilter;
    private Label lblPatientFilter;

    public ExaminationForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.dataGridViewExaminations = new DataGridView();
        this.btnAdd = new Button();
        this.btnEdit = new Button();
        this.btnDelete = new Button();
        this.cmbPatientFilter = new ComboBox();
        this.lblPatientFilter = new Label();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewExaminations)).BeginInit();
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
        this.btnAdd.Text = "Schedule Exam";
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
        // dataGridViewExaminations
        // 
        this.dataGridViewExaminations.AllowUserToAddRows = false;
        this.dataGridViewExaminations.AllowUserToDeleteRows = false;
        this.dataGridViewExaminations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewExaminations.Location = new Point(12, 90);
        this.dataGridViewExaminations.Name = "dataGridViewExaminations";
        this.dataGridViewExaminations.ReadOnly = true;
        this.dataGridViewExaminations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewExaminations.Size = new Size(1176, 564);
        this.dataGridViewExaminations.TabIndex = 5;
        this.dataGridViewExaminations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.dataGridViewExaminations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // 
        // ExaminationForm
        // 
        this.Controls.Add(this.dataGridViewExaminations);
        this.Controls.Add(this.btnDelete);
        this.Controls.Add(this.btnEdit);
        this.Controls.Add(this.btnAdd);
        this.Controls.Add(this.cmbPatientFilter);
        this.Controls.Add(this.lblPatientFilter);
        this.Name = "ExaminationForm";
        this.Size = new Size(1200, 700);
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewExaminations)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    public void LoadExaminations(MedicalContext context)
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

            LoadExaminationData(context);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading examinations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadExaminationData(MedicalContext context)
    {
        try
        {
            var query = context.Examinations.AsQueryable();
            
            if (cmbPatientFilter.SelectedIndex > 0)
            {
                var selectedText = cmbPatientFilter.SelectedItem?.ToString() ?? "";
                var patientIdStr = selectedText.Split("(ID: ")[1].TrimEnd(')');
                if (long.TryParse(patientIdStr, out var patientId))
                {
                    // Use ORM query capabilities for filtering
                    query = query.Where(e => e.PatientId == patientId);
                }
            }

            var examinations = query
                .OrderBy(e => e.ScheduledDate)
                .ToList();

            dataGridViewExaminations.DataSource = examinations.Select(e => new
            {
                e.Id,
                PatientId = e.PatientId,
                DoctorId = e.DoctorId,
                e.ExaminationType,
                ScheduledDate = e.ScheduledDate.ToString("yyyy-MM-dd HH:mm"),
                Status = e.Status ?? "Scheduled"
            }).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading examination data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CmbPatientFilter_SelectedIndexChanged(object? sender, EventArgs e)
    {
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        LoadExaminationData(context);
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        using var form = new ExaminationDetailForm(null, context);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadExaminations(context);
            // Notify parent form to refresh all data
            if (ParentForm is Form1 mainForm)
            {
                mainForm.RefreshAllData();
            }
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (dataGridViewExaminations.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select an examination to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedId = (long)dataGridViewExaminations.SelectedRows[0].Cells["Id"].Value;
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        var examination = context.Examinations.Find(selectedId);

        if (examination != null)
        {
            using var form = new ExaminationDetailForm(examination, context);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadExaminations(context);
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
        if (dataGridViewExaminations.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select an examination to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this examination?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes) return;

        var selectedId = (long)dataGridViewExaminations.SelectedRows[0].Cells["Id"].Value;
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        
        try
        {
            context.Examinations.Remove(selectedId);
            context.SaveChanges();
            MessageBox.Show("Examination deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadExaminations(context);
            // Notify parent form to refresh all data
            if (ParentForm is Form1 mainForm)
            {
                mainForm.RefreshAllData();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting examination: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}



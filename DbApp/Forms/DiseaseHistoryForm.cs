using DbApp.Data;
using DbApp.Models;
using DbApp.Forms.Dialogs;
using DbApp;

namespace DbApp.Forms;

public partial class DiseaseHistoryForm : UserControl
{
    private DataGridView dataGridViewDiseaseHistory;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private ComboBox cmbPatientFilter;
    private Label lblPatientFilter;

    public DiseaseHistoryForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.dataGridViewDiseaseHistory = new DataGridView();
        this.btnAdd = new Button();
        this.btnEdit = new Button();
        this.btnDelete = new Button();
        this.cmbPatientFilter = new ComboBox();
        this.lblPatientFilter = new Label();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDiseaseHistory)).BeginInit();
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
        this.btnAdd.Text = "Add History";
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
        // dataGridViewDiseaseHistory
        // 
        this.dataGridViewDiseaseHistory.AllowUserToAddRows = false;
        this.dataGridViewDiseaseHistory.AllowUserToDeleteRows = false;
        this.dataGridViewDiseaseHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewDiseaseHistory.Location = new Point(12, 90);
        this.dataGridViewDiseaseHistory.Name = "dataGridViewDiseaseHistory";
        this.dataGridViewDiseaseHistory.ReadOnly = true;
        this.dataGridViewDiseaseHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewDiseaseHistory.Size = new Size(1176, 564);
        this.dataGridViewDiseaseHistory.TabIndex = 5;
        this.dataGridViewDiseaseHistory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.dataGridViewDiseaseHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // 
        // DiseaseHistoryForm
        // 
        this.Controls.Add(this.dataGridViewDiseaseHistory);
        this.Controls.Add(this.btnDelete);
        this.Controls.Add(this.btnEdit);
        this.Controls.Add(this.btnAdd);
        this.Controls.Add(this.cmbPatientFilter);
        this.Controls.Add(this.lblPatientFilter);
        this.Name = "DiseaseHistoryForm";
        this.Size = new Size(1200, 700);
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDiseaseHistory)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    public void LoadDiseaseHistories(MedicalContext context)
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

            LoadDiseaseHistoryData(context);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading disease histories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadDiseaseHistoryData(MedicalContext context)
    {
        try
        {
            var query = context.DiseaseHistories.AsQueryable();
            
            if (cmbPatientFilter.SelectedIndex > 0)
            {
                var selectedText = cmbPatientFilter.SelectedItem?.ToString() ?? "";
                var patientIdStr = selectedText.Split("(ID: ")[1].TrimEnd(')');
                if (long.TryParse(patientIdStr, out var patientId))
                {
                    // Use ORM query capabilities for filtering
                    query = query.Where(h => h.PatientId == patientId);
                }
            }

            var histories = query
                .OrderByDescending(h => h.StartDate)
                .ToList();

            dataGridViewDiseaseHistory.DataSource = histories.Select(h => new
            {
                h.Id,
                PatientId = h.PatientId,
                h.DiseaseName,
                StartDate = h.StartDate.ToString("yyyy-MM-dd"),
                EndDate = h.EndDate?.ToString("yyyy-MM-dd") ?? "Ongoing"
            }).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading disease history data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CmbPatientFilter_SelectedIndexChanged(object? sender, EventArgs e)
    {
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        LoadDiseaseHistoryData(context);
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        using var form = new DiseaseHistoryDetailForm(null, context);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadDiseaseHistories(context);
            // Notify parent form to refresh all data
            if (ParentForm is Form1 mainForm)
            {
                mainForm.RefreshAllData();
            }
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (dataGridViewDiseaseHistory.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a disease history to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedId = (long)dataGridViewDiseaseHistory.SelectedRows[0].Cells["Id"].Value;
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        var history = context.DiseaseHistories.Find(selectedId);

        if (history != null)
        {
            using var form = new DiseaseHistoryDetailForm(history, context);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDiseaseHistories(context);
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
        if (dataGridViewDiseaseHistory.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a disease history to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = MessageBox.Show("Are you sure you want to delete this disease history?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes) return;

        var selectedId = (long)dataGridViewDiseaseHistory.SelectedRows[0].Cells["Id"].Value;
        using var context = new MedicalContext(DbApp.Configuration.DatabaseConfig.ConnectionString);
        
        try
        {
            context.DiseaseHistories.Remove(selectedId);
            context.SaveChanges();
            MessageBox.Show("Disease history deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadDiseaseHistories(context);
            // Notify parent form to refresh all data
            if (ParentForm is Form1 mainForm)
            {
                mainForm.RefreshAllData();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting disease history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}



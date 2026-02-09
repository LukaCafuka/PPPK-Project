using DbApp.Data;
using DbApp.Models;

namespace DbApp.Forms.Dialogs;

public partial class DiseaseHistoryDetailForm : Form
{
    private DiseaseHistory? _history;
    private MedicalContext _context;

    private ComboBox cmbPatient;
    private TextBox txtDiseaseName;
    private DateTimePicker dtpStartDate;
    private DateTimePicker dtpEndDate;
    private CheckBox chkOngoing;
    private Button btnSave;
    private Button btnCancel;
    private Label lblPatient;
    private Label lblDiseaseName;
    private Label lblStartDate;
    private Label lblEndDate;

    public DiseaseHistoryDetailForm(DiseaseHistory? history, MedicalContext context)
    {
        _history = history;
        _context = context;
        InitializeComponent();
        this.Text = _history == null ? "Add Disease History" : "Edit Disease History";
        LoadPatientComboBox();
        LoadDiseaseHistoryData();
    }

    private void InitializeComponent()
    {
        this.cmbPatient = new ComboBox();
        this.txtDiseaseName = new TextBox();
        this.dtpStartDate = new DateTimePicker();
        this.dtpEndDate = new DateTimePicker();
        this.chkOngoing = new CheckBox();
        this.btnSave = new Button();
        this.btnCancel = new Button();
        this.lblPatient = new Label();
        this.lblDiseaseName = new Label();
        this.lblStartDate = new Label();
        this.lblEndDate = new Label();
        this.SuspendLayout();
        // 
        // lblPatient
        // 
        this.lblPatient.AutoSize = true;
        this.lblPatient.Location = new Point(12, 15);
        this.lblPatient.Name = "lblPatient";
        this.lblPatient.Size = new Size(48, 15);
        this.lblPatient.TabIndex = 0;
        this.lblPatient.Text = "Patient:";
        // 
        // cmbPatient
        // 
        this.cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbPatient.Location = new Point(120, 12);
        this.cmbPatient.Name = "cmbPatient";
        this.cmbPatient.Size = new Size(300, 23);
        this.cmbPatient.TabIndex = 1;
        // 
        // lblDiseaseName
        // 
        this.lblDiseaseName.AutoSize = true;
        this.lblDiseaseName.Location = new Point(12, 45);
        this.lblDiseaseName.Name = "lblDiseaseName";
        this.lblDiseaseName.Size = new Size(81, 15);
        this.lblDiseaseName.TabIndex = 2;
        this.lblDiseaseName.Text = "Disease Name:";
        // 
        // txtDiseaseName
        // 
        this.txtDiseaseName.Location = new Point(120, 42);
        this.txtDiseaseName.Name = "txtDiseaseName";
        this.txtDiseaseName.Size = new Size(300, 23);
        this.txtDiseaseName.TabIndex = 3;
        // 
        // lblStartDate
        // 
        this.lblStartDate.AutoSize = true;
        this.lblStartDate.Location = new Point(12, 75);
        this.lblStartDate.Name = "lblStartDate";
        this.lblStartDate.Size = new Size(61, 15);
        this.lblStartDate.TabIndex = 4;
        this.lblStartDate.Text = "Start Date:";
        // 
        // dtpStartDate
        // 
        this.dtpStartDate.Location = new Point(120, 72);
        this.dtpStartDate.Name = "dtpStartDate";
        this.dtpStartDate.Size = new Size(300, 23);
        this.dtpStartDate.TabIndex = 5;
        // 
        // lblEndDate
        // 
        this.lblEndDate.AutoSize = true;
        this.lblEndDate.Location = new Point(12, 105);
        this.lblEndDate.Name = "lblEndDate";
        this.lblEndDate.Size = new Size(57, 15);
        this.lblEndDate.TabIndex = 6;
        this.lblEndDate.Text = "End Date:";
        // 
        // dtpEndDate
        // 
        this.dtpEndDate.Location = new Point(120, 102);
        this.dtpEndDate.Name = "dtpEndDate";
        this.dtpEndDate.Size = new Size(300, 23);
        this.dtpEndDate.TabIndex = 7;
        // 
        // chkOngoing
        // 
        this.chkOngoing.AutoSize = true;
        this.chkOngoing.Location = new Point(120, 131);
        this.chkOngoing.Name = "chkOngoing";
        this.chkOngoing.Size = new Size(70, 19);
        this.chkOngoing.TabIndex = 8;
        this.chkOngoing.Text = "Ongoing";
        this.chkOngoing.CheckedChanged += ChkOngoing_CheckedChanged;
        // 
        // btnSave
        // 
        this.btnSave.Location = new Point(264, 160);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new Size(75, 30);
        this.btnSave.TabIndex = 9;
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += BtnSave_Click;
        // 
        // btnCancel
        // 
        this.btnCancel.Location = new Point(345, 160);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new Size(75, 30);
        this.btnCancel.TabIndex = 10;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += BtnCancel_Click;
        // 
        // DiseaseHistoryDetailForm
        // 
        this.ClientSize = new Size(450, 200);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.chkOngoing);
        this.Controls.Add(this.dtpEndDate);
        this.Controls.Add(this.lblEndDate);
        this.Controls.Add(this.dtpStartDate);
        this.Controls.Add(this.lblStartDate);
        this.Controls.Add(this.txtDiseaseName);
        this.Controls.Add(this.lblDiseaseName);
        this.Controls.Add(this.cmbPatient);
        this.Controls.Add(this.lblPatient);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "DiseaseHistoryDetailForm";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "DiseaseHistoryDetailForm";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void LoadPatientComboBox()
    {
        var patients = _context.Patients.ToList();
        cmbPatient.Items.Clear();
        foreach (var patient in patients)
        {
            cmbPatient.Items.Add(new { Id = patient.Id, Display = $"{patient.FirstName} {patient.LastName} (OIB: {patient.Oib})" });
        }
        cmbPatient.DisplayMember = "Display";
        cmbPatient.ValueMember = "Id";
    }

    private void LoadDiseaseHistoryData()
    {
        if (_history != null)
        {
            txtDiseaseName.Text = _history.DiseaseName;
            dtpStartDate.Value = _history.StartDate;
            if (_history.EndDate.HasValue)
            {
                dtpEndDate.Value = _history.EndDate.Value;
                chkOngoing.Checked = false;
            }
            else
            {
                chkOngoing.Checked = true;
            }

            // Select patient
            for (int i = 0; i < cmbPatient.Items.Count; i++)
            {
                var item = cmbPatient.Items[i];
                var idProperty = item.GetType().GetProperty("Id");
                if (idProperty != null && (long)idProperty.GetValue(item)! == _history.PatientId)
                {
                    cmbPatient.SelectedIndex = i;
                    break;
                }
            }
        }
        else
        {
            chkOngoing.Checked = true;
        }
    }

    private void ChkOngoing_CheckedChanged(object? sender, EventArgs e)
    {
        dtpEndDate.Enabled = !chkOngoing.Checked;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;

        try
        {
            if (_history == null)
            {
                _history = new DiseaseHistory();
            }

            var selectedPatient = cmbPatient.SelectedItem;
            var idProperty = selectedPatient?.GetType().GetProperty("Id");
            _history.PatientId = idProperty != null ? (long)idProperty.GetValue(selectedPatient)! : 0;

            _history.DiseaseName = txtDiseaseName.Text.Trim();
            _history.StartDate = dtpStartDate.Value;
            _history.EndDate = chkOngoing.Checked ? null : dtpEndDate.Value;

            if (_history.Id == 0)
            {
                _context.DiseaseHistories.Add(_history);
            }
            else
            {
                _context.DiseaseHistories.Update(_history);
            }

            _context.SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving disease history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool ValidateInput()
    {
        if (cmbPatient.SelectedItem == null)
        {
            MessageBox.Show("Please select a patient.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtDiseaseName.Text))
        {
            MessageBox.Show("Disease name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (!chkOngoing.Checked && dtpEndDate.Value < dtpStartDate.Value)
        {
            MessageBox.Show("End date must be after start date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}



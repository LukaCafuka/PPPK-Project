using DbApp.Data;
using DbApp.Models;

namespace DbApp.Forms.Dialogs;

public partial class PatientDetailForm : Form
{
    private Patient? _patient;
    private MedicalContext _context;
    private bool _readOnly;

    private TextBox txtFirstName;
    private TextBox txtLastName;
    private TextBox txtOib;
    private DateTimePicker dtpDateOfBirth;
    private TextBox txtGender;
    private TextBox txtResidenceAddress;
    private TextBox txtPermanentAddress;
    private Button btnSave;
    private Button btnCancel;
    private Label lblFirstName;
    private Label lblLastName;
    private Label lblOib;
    private Label lblDateOfBirth;
    private Label lblGender;
    private Label lblResidenceAddress;
    private Label lblPermanentAddress;
    private TabControl tabControlDetails;
    private TabPage tabPageBasicInfo;
    private TabPage tabPageDiseaseHistory;
    private TabPage tabPageMedications;
    private TabPage tabPageExaminations;
    private DataGridView dataGridViewDiseaseHistories;
    private DataGridView dataGridViewMedications;
    private DataGridView dataGridViewExaminations;

    public PatientDetailForm(Patient? patient, MedicalContext context, bool readOnly = false)
    {
        _patient = patient;
        _context = context;
        _readOnly = readOnly;
        InitializeComponent();
        LoadPatientData();
    }

    private void InitializeComponent()
    {
        this.txtFirstName = new TextBox();
        this.txtLastName = new TextBox();
        this.txtOib = new TextBox();
        this.dtpDateOfBirth = new DateTimePicker();
        this.txtGender = new TextBox();
        this.txtResidenceAddress = new TextBox();
        this.txtPermanentAddress = new TextBox();
        this.btnSave = new Button();
        this.btnCancel = new Button();
        this.lblFirstName = new Label();
        this.lblLastName = new Label();
        this.lblOib = new Label();
        this.lblDateOfBirth = new Label();
        this.lblGender = new Label();
        this.lblResidenceAddress = new Label();
        this.lblPermanentAddress = new Label();
        this.tabControlDetails = new TabControl();
        this.tabPageBasicInfo = new TabPage();
        this.tabPageDiseaseHistory = new TabPage();
        this.tabPageMedications = new TabPage();
        this.tabPageExaminations = new TabPage();
        this.dataGridViewDiseaseHistories = new DataGridView();
        this.dataGridViewMedications = new DataGridView();
        this.dataGridViewExaminations = new DataGridView();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDiseaseHistories)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMedications)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewExaminations)).BeginInit();
        this.tabControlDetails.SuspendLayout();
        this.tabPageDiseaseHistory.SuspendLayout();
        this.tabPageMedications.SuspendLayout();
        this.tabPageExaminations.SuspendLayout();
        this.SuspendLayout();
        // 
        // lblFirstName
        // 
        this.lblFirstName.AutoSize = true;
        this.lblFirstName.Location = new Point(12, 15);
        this.lblFirstName.Name = "lblFirstName";
        this.lblFirstName.Size = new Size(67, 15);
        this.lblFirstName.TabIndex = 0;
        this.lblFirstName.Text = "First Name:";
        // 
        // txtFirstName
        // 
        this.txtFirstName.Location = new Point(120, 12);
        this.txtFirstName.Name = "txtFirstName";
        this.txtFirstName.Size = new Size(300, 23);
        this.txtFirstName.TabIndex = 1;
        this.txtFirstName.ReadOnly = _readOnly;
        // 
        // lblLastName
        // 
        this.lblLastName.AutoSize = true;
        this.lblLastName.Location = new Point(12, 45);
        this.lblLastName.Name = "lblLastName";
        this.lblLastName.Size = new Size(66, 15);
        this.lblLastName.TabIndex = 2;
        this.lblLastName.Text = "Last Name:";
        // 
        // txtLastName
        // 
        this.txtLastName.Location = new Point(120, 42);
        this.txtLastName.Name = "txtLastName";
        this.txtLastName.Size = new Size(300, 23);
        this.txtLastName.TabIndex = 3;
        this.txtLastName.ReadOnly = _readOnly;
        // 
        // lblOib
        // 
        this.lblOib.AutoSize = true;
        this.lblOib.Location = new Point(12, 75);
        this.lblOib.Name = "lblOib";
        this.lblOib.Size = new Size(30, 15);
        this.lblOib.TabIndex = 4;
        this.lblOib.Text = "OIB:";
        // 
        // txtOib
        // 
        this.txtOib.Location = new Point(120, 72);
        this.txtOib.Name = "txtOib";
        this.txtOib.Size = new Size(300, 23);
        this.txtOib.TabIndex = 5;
        this.txtOib.MaxLength = 11;
        this.txtOib.ReadOnly = _readOnly;
        // 
        // lblDateOfBirth
        // 
        this.lblDateOfBirth.AutoSize = true;
        this.lblDateOfBirth.Location = new Point(12, 105);
        this.lblDateOfBirth.Name = "lblDateOfBirth";
        this.lblDateOfBirth.Size = new Size(80, 15);
        this.lblDateOfBirth.TabIndex = 6;
        this.lblDateOfBirth.Text = "Date of Birth:";
        // 
        // dtpDateOfBirth
        // 
        this.dtpDateOfBirth.Location = new Point(120, 102);
        this.dtpDateOfBirth.Name = "dtpDateOfBirth";
        this.dtpDateOfBirth.Size = new Size(300, 23);
        this.dtpDateOfBirth.TabIndex = 7;
        this.dtpDateOfBirth.Enabled = !_readOnly;
        // 
        // lblGender
        // 
        this.lblGender.AutoSize = true;
        this.lblGender.Location = new Point(12, 135);
        this.lblGender.Name = "lblGender";
        this.lblGender.Size = new Size(48, 15);
        this.lblGender.TabIndex = 8;
        this.lblGender.Text = "Gender:";
        // 
        // txtGender
        // 
        this.txtGender.Location = new Point(120, 132);
        this.txtGender.Name = "txtGender";
        this.txtGender.Size = new Size(300, 23);
        this.txtGender.TabIndex = 9;
        this.txtGender.MaxLength = 1;
        this.txtGender.ReadOnly = _readOnly;
        // 
        // lblResidenceAddress
        // 
        this.lblResidenceAddress.AutoSize = true;
        this.lblResidenceAddress.Location = new Point(12, 165);
        this.lblResidenceAddress.Name = "lblResidenceAddress";
        this.lblResidenceAddress.Size = new Size(110, 15);
        this.lblResidenceAddress.TabIndex = 10;
        this.lblResidenceAddress.Text = "Residence Address:";
        // 
        // txtResidenceAddress
        // 
        this.txtResidenceAddress.Location = new Point(120, 162);
        this.txtResidenceAddress.Multiline = true;
        this.txtResidenceAddress.Name = "txtResidenceAddress";
        this.txtResidenceAddress.Size = new Size(300, 60);
        this.txtResidenceAddress.TabIndex = 11;
        this.txtResidenceAddress.ReadOnly = _readOnly;
        // 
        // lblPermanentAddress
        // 
        this.lblPermanentAddress.AutoSize = true;
        this.lblPermanentAddress.Location = new Point(12, 235);
        this.lblPermanentAddress.Name = "lblPermanentAddress";
        this.lblPermanentAddress.Size = new Size(111, 15);
        this.lblPermanentAddress.TabIndex = 12;
        this.lblPermanentAddress.Text = "Permanent Address:";
        // 
        // txtPermanentAddress
        // 
        this.txtPermanentAddress.Location = new Point(120, 232);
        this.txtPermanentAddress.Multiline = true;
        this.txtPermanentAddress.Name = "txtPermanentAddress";
        this.txtPermanentAddress.Size = new Size(300, 60);
        this.txtPermanentAddress.TabIndex = 13;
        this.txtPermanentAddress.ReadOnly = _readOnly;
        // 
        // btnSave
        // 
        this.btnSave.Location = new Point(264, _readOnly ? 470 : 370);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new Size(75, 30);
        this.btnSave.TabIndex = 14;
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += BtnSave_Click;
        this.btnSave.Visible = !_readOnly;
        // 
        // btnCancel
        // 
        this.btnCancel.Location = new Point(345, _readOnly ? 470 : 370);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new Size(75, 30);
        this.btnCancel.TabIndex = 15;
        this.btnCancel.Text = _readOnly ? "Close" : "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += BtnCancel_Click;
        // 
        // tabPageBasicInfo
        // 
        this.tabPageBasicInfo.Controls.Add(this.txtPermanentAddress);
        this.tabPageBasicInfo.Controls.Add(this.lblPermanentAddress);
        this.tabPageBasicInfo.Controls.Add(this.txtResidenceAddress);
        this.tabPageBasicInfo.Controls.Add(this.lblResidenceAddress);
        this.tabPageBasicInfo.Controls.Add(this.txtGender);
        this.tabPageBasicInfo.Controls.Add(this.lblGender);
        this.tabPageBasicInfo.Controls.Add(this.dtpDateOfBirth);
        this.tabPageBasicInfo.Controls.Add(this.lblDateOfBirth);
        this.tabPageBasicInfo.Controls.Add(this.txtOib);
        this.tabPageBasicInfo.Controls.Add(this.lblOib);
        this.tabPageBasicInfo.Controls.Add(this.txtLastName);
        this.tabPageBasicInfo.Controls.Add(this.lblLastName);
        this.tabPageBasicInfo.Controls.Add(this.txtFirstName);
        this.tabPageBasicInfo.Controls.Add(this.lblFirstName);
        this.tabPageBasicInfo.Location = new Point(4, 24);
        this.tabPageBasicInfo.Name = "tabPageBasicInfo";
        this.tabPageBasicInfo.Padding = new Padding(3);
        this.tabPageBasicInfo.Size = new Size(792, 420);
        this.tabPageBasicInfo.TabIndex = 0;
        this.tabPageBasicInfo.Text = "Basic Information";
        this.tabPageBasicInfo.UseVisualStyleBackColor = true;
        // 
        // tabPageDiseaseHistory
        // 
        this.tabPageDiseaseHistory.Controls.Add(this.dataGridViewDiseaseHistories);
        this.tabPageDiseaseHistory.Location = new Point(4, 24);
        this.tabPageDiseaseHistory.Name = "tabPageDiseaseHistory";
        this.tabPageDiseaseHistory.Padding = new Padding(3);
        this.tabPageDiseaseHistory.Size = new Size(792, 420);
        this.tabPageDiseaseHistory.TabIndex = 1;
        this.tabPageDiseaseHistory.Text = "Disease History";
        this.tabPageDiseaseHistory.UseVisualStyleBackColor = true;
        // 
        // dataGridViewDiseaseHistories
        // 
        this.dataGridViewDiseaseHistories.AllowUserToAddRows = false;
        this.dataGridViewDiseaseHistories.AllowUserToDeleteRows = false;
        this.dataGridViewDiseaseHistories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewDiseaseHistories.Dock = DockStyle.Fill;
        this.dataGridViewDiseaseHistories.Location = new Point(3, 3);
        this.dataGridViewDiseaseHistories.Name = "dataGridViewDiseaseHistories";
        this.dataGridViewDiseaseHistories.ReadOnly = true;
        this.dataGridViewDiseaseHistories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewDiseaseHistories.Size = new Size(786, 414);
        this.dataGridViewDiseaseHistories.TabIndex = 0;
        this.dataGridViewDiseaseHistories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // 
        // tabPageMedications
        // 
        this.tabPageMedications.Controls.Add(this.dataGridViewMedications);
        this.tabPageMedications.Location = new Point(4, 24);
        this.tabPageMedications.Name = "tabPageMedications";
        this.tabPageMedications.Padding = new Padding(3);
        this.tabPageMedications.Size = new Size(792, 420);
        this.tabPageMedications.TabIndex = 2;
        this.tabPageMedications.Text = "Medications";
        this.tabPageMedications.UseVisualStyleBackColor = true;
        // 
        // dataGridViewMedications
        // 
        this.dataGridViewMedications.AllowUserToAddRows = false;
        this.dataGridViewMedications.AllowUserToDeleteRows = false;
        this.dataGridViewMedications.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewMedications.Dock = DockStyle.Fill;
        this.dataGridViewMedications.Location = new Point(3, 3);
        this.dataGridViewMedications.Name = "dataGridViewMedications";
        this.dataGridViewMedications.ReadOnly = true;
        this.dataGridViewMedications.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewMedications.Size = new Size(786, 414);
        this.dataGridViewMedications.TabIndex = 0;
        this.dataGridViewMedications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // 
        // tabPageExaminations
        // 
        this.tabPageExaminations.Controls.Add(this.dataGridViewExaminations);
        this.tabPageExaminations.Location = new Point(4, 24);
        this.tabPageExaminations.Name = "tabPageExaminations";
        this.tabPageExaminations.Padding = new Padding(3);
        this.tabPageExaminations.Size = new Size(792, 420);
        this.tabPageExaminations.TabIndex = 3;
        this.tabPageExaminations.Text = "Examinations";
        this.tabPageExaminations.UseVisualStyleBackColor = true;
        // 
        // dataGridViewExaminations
        // 
        this.dataGridViewExaminations.AllowUserToAddRows = false;
        this.dataGridViewExaminations.AllowUserToDeleteRows = false;
        this.dataGridViewExaminations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewExaminations.Dock = DockStyle.Fill;
        this.dataGridViewExaminations.Location = new Point(3, 3);
        this.dataGridViewExaminations.Name = "dataGridViewExaminations";
        this.dataGridViewExaminations.ReadOnly = true;
        this.dataGridViewExaminations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewExaminations.Size = new Size(786, 414);
        this.dataGridViewExaminations.TabIndex = 0;
        this.dataGridViewExaminations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // 
        // tabControlDetails
        // 
        this.tabControlDetails.Controls.Add(this.tabPageBasicInfo);
        if (_readOnly)
        {
            this.tabControlDetails.Controls.Add(this.tabPageDiseaseHistory);
            this.tabControlDetails.Controls.Add(this.tabPageMedications);
            this.tabControlDetails.Controls.Add(this.tabPageExaminations);
        }
        this.tabControlDetails.Location = new Point(12, 12);
        this.tabControlDetails.Name = "tabControlDetails";
        this.tabControlDetails.SelectedIndex = 0;
        this.tabControlDetails.Size = new Size(800, _readOnly ? 448 : 350);
        this.tabControlDetails.TabIndex = 16;
        // 
        // PatientDetailForm
        // 
        this.ClientSize = new Size(824, _readOnly ? 500 : 420);
        this.Controls.Add(this.tabControlDetails);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSave);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "PatientDetailForm";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = _readOnly ? "Patient Details" : (_patient == null ? "Add Patient" : "Edit Patient");
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDiseaseHistories)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMedications)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewExaminations)).EndInit();
        this.tabControlDetails.ResumeLayout(false);
        this.tabPageDiseaseHistory.ResumeLayout(false);
        this.tabPageMedications.ResumeLayout(false);
        this.tabPageExaminations.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void LoadPatientData()
    {
        if (_patient != null)
        {
            txtFirstName.Text = _patient.FirstName;
            txtLastName.Text = _patient.LastName;
            txtOib.Text = _patient.Oib;
            dtpDateOfBirth.Value = _patient.DateOfBirth;
            txtGender.Text = _patient.Gender;
            txtResidenceAddress.Text = _patient.ResidenceAddress ?? "";
            txtPermanentAddress.Text = _patient.PermanentAddress ?? "";

            // Load related data using navigation properties (loaded via eager loading)
            if (_readOnly && _patient.DiseaseHistories != null)
            {
                dataGridViewDiseaseHistories.DataSource = _patient.DiseaseHistories.Select(dh => new
                {
                    dh.Id,
                    dh.DiseaseName,
                    StartDate = dh.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = dh.EndDate?.ToString("yyyy-MM-dd") ?? "Ongoing"
                }).ToList();
            }

            if (_readOnly && _patient.Medications != null)
            {
                dataGridViewMedications.DataSource = _patient.Medications.Select(m => new
                {
                    m.Id,
                    m.MedicationName,
                    m.Dose,
                    m.DoseUnit,
                    m.Frequency,
                    Condition = m.Condition ?? "",
                    PrescribedDate = m.PrescribedDate.ToString("yyyy-MM-dd")
                }).ToList();
            }

            if (_readOnly && _patient.Examinations != null)
            {
                dataGridViewExaminations.DataSource = _patient.Examinations.Select(e => new
                {
                    e.Id,
                    e.ExaminationType,
                    ScheduledDate = e.ScheduledDate.ToString("yyyy-MM-dd HH:mm"),
                    Status = e.Status ?? "Scheduled"
                }).ToList();
            }
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;

        try
        {
            if (_patient == null)
            {
                _patient = new Patient();
            }

            _patient.FirstName = txtFirstName.Text.Trim();
            _patient.LastName = txtLastName.Text.Trim();
            _patient.Oib = txtOib.Text.Trim();
            _patient.DateOfBirth = dtpDateOfBirth.Value;
            _patient.Gender = txtGender.Text.Trim();
            _patient.ResidenceAddress = string.IsNullOrWhiteSpace(txtResidenceAddress.Text) ? null : txtResidenceAddress.Text.Trim();
            _patient.PermanentAddress = string.IsNullOrWhiteSpace(txtPermanentAddress.Text) ? null : txtPermanentAddress.Text.Trim();

            if (_patient.Id == 0)
            {
                _context.Patients.Add(_patient);
            }
            else
            {
                _context.Patients.Update(_patient);
            }

            _context.SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving patient: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtFirstName.Text))
        {
            MessageBox.Show("First name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtLastName.Text))
        {
            MessageBox.Show("Last name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtOib.Text) || txtOib.Text.Length != 11)
        {
            MessageBox.Show("OIB must be exactly 11 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtGender.Text))
        {
            MessageBox.Show("Gender is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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



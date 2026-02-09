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
        ApplyDynamicLayout();
        LoadPatientData();
    }

    private void InitializeComponent()
    {
        txtFirstName = new TextBox();
        txtLastName = new TextBox();
        txtOib = new TextBox();
        dtpDateOfBirth = new DateTimePicker();
        txtGender = new TextBox();
        txtResidenceAddress = new TextBox();
        txtPermanentAddress = new TextBox();
        btnSave = new Button();
        btnCancel = new Button();
        lblFirstName = new Label();
        lblLastName = new Label();
        lblOib = new Label();
        lblDateOfBirth = new Label();
        lblGender = new Label();
        lblResidenceAddress = new Label();
        lblPermanentAddress = new Label();
        tabControlDetails = new TabControl();
        tabPageBasicInfo = new TabPage();
        tabPageDiseaseHistory = new TabPage();
        dataGridViewDiseaseHistories = new DataGridView();
        tabPageMedications = new TabPage();
        dataGridViewMedications = new DataGridView();
        tabPageExaminations = new TabPage();
        dataGridViewExaminations = new DataGridView();
        tabControlDetails.SuspendLayout();
        tabPageBasicInfo.SuspendLayout();
        tabPageDiseaseHistory.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewDiseaseHistories).BeginInit();
        tabPageMedications.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewMedications).BeginInit();
        tabPageExaminations.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewExaminations).BeginInit();
        SuspendLayout();
        // 
        // txtFirstName
        // 
        txtFirstName.Location = new Point(160, 11);
        txtFirstName.Name = "txtFirstName";
        txtFirstName.Size = new Size(300, 27);
        txtFirstName.TabIndex = 1;
        // 
        // txtLastName
        // 
        txtLastName.Location = new Point(160, 41);
        txtLastName.Name = "txtLastName";
        txtLastName.Size = new Size(300, 27);
        txtLastName.TabIndex = 3;
        txtLastName.TextChanged += txtLastName_TextChanged;
        // 
        // txtOib
        // 
        txtOib.Location = new Point(157, 71);
        txtOib.MaxLength = 11;
        txtOib.Name = "txtOib";
        txtOib.Size = new Size(300, 27);
        txtOib.TabIndex = 5;
        txtOib.TextChanged += txtOib_TextChanged;
        // 
        // dtpDateOfBirth
        // 
        dtpDateOfBirth.Location = new Point(157, 103);
        dtpDateOfBirth.Name = "dtpDateOfBirth";
        dtpDateOfBirth.Size = new Size(300, 27);
        dtpDateOfBirth.TabIndex = 7;
        // 
        // txtGender
        // 
        txtGender.Location = new Point(157, 131);
        txtGender.MaxLength = 1;
        txtGender.Name = "txtGender";
        txtGender.Size = new Size(300, 27);
        txtGender.TabIndex = 9;
        // 
        // txtResidenceAddress
        // 
        txtResidenceAddress.Location = new Point(157, 165);
        txtResidenceAddress.Multiline = true;
        txtResidenceAddress.Name = "txtResidenceAddress";
        txtResidenceAddress.Size = new Size(300, 60);
        txtResidenceAddress.TabIndex = 11;
        // 
        // txtPermanentAddress
        // 
        txtPermanentAddress.Location = new Point(160, 231);
        txtPermanentAddress.Multiline = true;
        txtPermanentAddress.Name = "txtPermanentAddress";
        txtPermanentAddress.Size = new Size(300, 60);
        txtPermanentAddress.TabIndex = 13;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(341, 370);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(75, 30);
        btnSave.TabIndex = 14;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += BtnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(422, 370);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(75, 30);
        btnCancel.TabIndex = 15;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += BtnCancel_Click;
        // 
        // lblFirstName
        // 
        lblFirstName.AutoSize = true;
        lblFirstName.Location = new Point(15, 18);
        lblFirstName.Name = "lblFirstName";
        lblFirstName.Size = new Size(83, 20);
        lblFirstName.TabIndex = 0;
        lblFirstName.Text = "First Name:";
        // 
        // lblLastName
        // 
        lblLastName.AutoSize = true;
        lblLastName.Location = new Point(15, 48);
        lblLastName.Name = "lblLastName";
        lblLastName.Size = new Size(82, 20);
        lblLastName.TabIndex = 2;
        lblLastName.Text = "Last Name:";
        // 
        // lblOib
        // 
        lblOib.AutoSize = true;
        lblOib.Location = new Point(15, 78);
        lblOib.Name = "lblOib";
        lblOib.Size = new Size(36, 20);
        lblOib.TabIndex = 4;
        lblOib.Text = "OIB:";
        // 
        // lblDateOfBirth
        // 
        lblDateOfBirth.AutoSize = true;
        lblDateOfBirth.Location = new Point(15, 108);
        lblDateOfBirth.Name = "lblDateOfBirth";
        lblDateOfBirth.Size = new Size(97, 20);
        lblDateOfBirth.TabIndex = 6;
        lblDateOfBirth.Text = "Date of Birth:";
        // 
        // lblGender
        // 
        lblGender.AutoSize = true;
        lblGender.Location = new Point(15, 138);
        lblGender.Name = "lblGender";
        lblGender.Size = new Size(60, 20);
        lblGender.TabIndex = 8;
        lblGender.Text = "Gender:";
        // 
        // lblResidenceAddress
        // 
        lblResidenceAddress.AutoSize = true;
        lblResidenceAddress.Location = new Point(15, 168);
        lblResidenceAddress.Name = "lblResidenceAddress";
        lblResidenceAddress.Size = new Size(136, 20);
        lblResidenceAddress.TabIndex = 10;
        lblResidenceAddress.Text = "Residence Address:";
        // 
        // lblPermanentAddress
        // 
        lblPermanentAddress.AutoSize = true;
        lblPermanentAddress.Location = new Point(15, 238);
        lblPermanentAddress.Name = "lblPermanentAddress";
        lblPermanentAddress.Size = new Size(139, 20);
        lblPermanentAddress.TabIndex = 12;
        lblPermanentAddress.Text = "Permanent Address:";
        // 
        // tabControlDetails
        // 
        tabControlDetails.Controls.Add(tabPageBasicInfo);
        tabControlDetails.Location = new Point(12, 12);
        tabControlDetails.Name = "tabControlDetails";
        tabControlDetails.SelectedIndex = 0;
        tabControlDetails.Size = new Size(489, 350);
        tabControlDetails.TabIndex = 16;
        // 
        // tabPageBasicInfo
        // 
        tabPageBasicInfo.Controls.Add(txtPermanentAddress);
        tabPageBasicInfo.Controls.Add(lblPermanentAddress);
        tabPageBasicInfo.Controls.Add(txtResidenceAddress);
        tabPageBasicInfo.Controls.Add(lblResidenceAddress);
        tabPageBasicInfo.Controls.Add(txtGender);
        tabPageBasicInfo.Controls.Add(lblGender);
        tabPageBasicInfo.Controls.Add(dtpDateOfBirth);
        tabPageBasicInfo.Controls.Add(lblDateOfBirth);
        tabPageBasicInfo.Controls.Add(txtOib);
        tabPageBasicInfo.Controls.Add(lblOib);
        tabPageBasicInfo.Controls.Add(txtLastName);
        tabPageBasicInfo.Controls.Add(lblLastName);
        tabPageBasicInfo.Controls.Add(txtFirstName);
        tabPageBasicInfo.Controls.Add(lblFirstName);
        tabPageBasicInfo.Location = new Point(4, 29);
        tabPageBasicInfo.Name = "tabPageBasicInfo";
        tabPageBasicInfo.Padding = new Padding(3);
        tabPageBasicInfo.Size = new Size(481, 317);
        tabPageBasicInfo.TabIndex = 0;
        tabPageBasicInfo.Text = "Basic Information";
        tabPageBasicInfo.UseVisualStyleBackColor = true;
        // 
        // tabPageDiseaseHistory
        // 
        tabPageDiseaseHistory.Controls.Add(dataGridViewDiseaseHistories);
        tabPageDiseaseHistory.Location = new Point(4, 24);
        tabPageDiseaseHistory.Name = "tabPageDiseaseHistory";
        tabPageDiseaseHistory.Padding = new Padding(3);
        tabPageDiseaseHistory.Size = new Size(792, 420);
        tabPageDiseaseHistory.TabIndex = 1;
        tabPageDiseaseHistory.Text = "Disease History";
        tabPageDiseaseHistory.UseVisualStyleBackColor = true;
        // 
        // dataGridViewDiseaseHistories
        // 
        dataGridViewDiseaseHistories.AllowUserToAddRows = false;
        dataGridViewDiseaseHistories.AllowUserToDeleteRows = false;
        dataGridViewDiseaseHistories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewDiseaseHistories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewDiseaseHistories.Dock = DockStyle.Fill;
        dataGridViewDiseaseHistories.Location = new Point(3, 3);
        dataGridViewDiseaseHistories.Name = "dataGridViewDiseaseHistories";
        dataGridViewDiseaseHistories.ReadOnly = true;
        dataGridViewDiseaseHistories.RowHeadersWidth = 51;
        dataGridViewDiseaseHistories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewDiseaseHistories.Size = new Size(786, 414);
        dataGridViewDiseaseHistories.TabIndex = 0;
        // 
        // tabPageMedications
        // 
        tabPageMedications.Controls.Add(dataGridViewMedications);
        tabPageMedications.Location = new Point(4, 24);
        tabPageMedications.Name = "tabPageMedications";
        tabPageMedications.Padding = new Padding(3);
        tabPageMedications.Size = new Size(792, 420);
        tabPageMedications.TabIndex = 2;
        tabPageMedications.Text = "Medications";
        tabPageMedications.UseVisualStyleBackColor = true;
        // 
        // dataGridViewMedications
        // 
        dataGridViewMedications.AllowUserToAddRows = false;
        dataGridViewMedications.AllowUserToDeleteRows = false;
        dataGridViewMedications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewMedications.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewMedications.Dock = DockStyle.Fill;
        dataGridViewMedications.Location = new Point(3, 3);
        dataGridViewMedications.Name = "dataGridViewMedications";
        dataGridViewMedications.ReadOnly = true;
        dataGridViewMedications.RowHeadersWidth = 51;
        dataGridViewMedications.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewMedications.Size = new Size(786, 414);
        dataGridViewMedications.TabIndex = 0;
        // 
        // tabPageExaminations
        // 
        tabPageExaminations.Controls.Add(dataGridViewExaminations);
        tabPageExaminations.Location = new Point(4, 24);
        tabPageExaminations.Name = "tabPageExaminations";
        tabPageExaminations.Padding = new Padding(3);
        tabPageExaminations.Size = new Size(792, 420);
        tabPageExaminations.TabIndex = 3;
        tabPageExaminations.Text = "Examinations";
        tabPageExaminations.UseVisualStyleBackColor = true;
        // 
        // dataGridViewExaminations
        // 
        dataGridViewExaminations.AllowUserToAddRows = false;
        dataGridViewExaminations.AllowUserToDeleteRows = false;
        dataGridViewExaminations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewExaminations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewExaminations.Dock = DockStyle.Fill;
        dataGridViewExaminations.Location = new Point(3, 3);
        dataGridViewExaminations.Name = "dataGridViewExaminations";
        dataGridViewExaminations.ReadOnly = true;
        dataGridViewExaminations.RowHeadersWidth = 51;
        dataGridViewExaminations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewExaminations.Size = new Size(786, 414);
        dataGridViewExaminations.TabIndex = 0;
        // 
        // PatientDetailForm
        // 
        ClientSize = new Size(517, 420);
        Controls.Add(tabControlDetails);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PatientDetailForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "PatientDetailForm";
        tabControlDetails.ResumeLayout(false);
        tabPageBasicInfo.ResumeLayout(false);
        tabPageBasicInfo.PerformLayout();
        tabPageDiseaseHistory.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewDiseaseHistories).EndInit();
        tabPageMedications.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewMedications).EndInit();
        tabPageExaminations.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewExaminations).EndInit();
        ResumeLayout(false);
    }

    private void ApplyDynamicLayout()
    {
        this.Text = _readOnly ? "Patient Details" : (_patient == null ? "Add Patient" : "Edit Patient");

        txtFirstName.ReadOnly = _readOnly;
        txtLastName.ReadOnly = _readOnly;
        txtOib.ReadOnly = _readOnly;
        dtpDateOfBirth.Enabled = !_readOnly;
        txtGender.ReadOnly = _readOnly;
        txtResidenceAddress.ReadOnly = _readOnly;
        txtPermanentAddress.ReadOnly = _readOnly;

        btnSave.Visible = !_readOnly;
        btnSave.Location = new Point(264, _readOnly ? 470 : 370);
        btnCancel.Location = new Point(345, _readOnly ? 470 : 370);
        btnCancel.Text = _readOnly ? "Close" : "Cancel";

        if (_readOnly)
        {
            tabControlDetails.Controls.Add(tabPageDiseaseHistory);
            tabControlDetails.Controls.Add(tabPageMedications);
            tabControlDetails.Controls.Add(tabPageExaminations);
            tabControlDetails.Size = new Size(800, 448);
            this.ClientSize = new Size(824, 500);
        }
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

    private void txtOib_TextChanged(object sender, EventArgs e)
    {

    }

    private void txtLastName_TextChanged(object sender, EventArgs e)
    {

    }
}



using DbApp.Data;
using DbApp.Models;

namespace DbApp.Forms.Dialogs;

public partial class ExaminationDetailForm : Form
{
    private Examination? _examination;
    private MedicalContext _context;

    private ComboBox cmbPatient;
    private ComboBox cmbDoctor;
    private ComboBox cmbExaminationType;
    private DateTimePicker dtpScheduledDate;
    private TextBox txtStatus;
    private Button btnSave;
    private Button btnCancel;
    private Label lblPatient;
    private Label lblDoctor;
    private Label lblExaminationType;
    private Label lblScheduledDate;
    private Label lblStatus;

    public ExaminationDetailForm(Examination? examination, MedicalContext context)
    {
        _examination = examination;
        _context = context;
        InitializeComponent();
        this.Text = _examination == null ? "Schedule Examination" : "Edit Examination";
        LoadPatientComboBox();
        LoadDoctorComboBox();
        LoadExaminationTypeComboBox();
        LoadExaminationData();
    }

    private void InitializeComponent()
    {
        cmbPatient = new ComboBox();
        cmbDoctor = new ComboBox();
        cmbExaminationType = new ComboBox();
        dtpScheduledDate = new DateTimePicker();
        txtStatus = new TextBox();
        btnSave = new Button();
        btnCancel = new Button();
        lblPatient = new Label();
        lblDoctor = new Label();
        lblExaminationType = new Label();
        lblScheduledDate = new Label();
        lblStatus = new Label();
        SuspendLayout();
        // 
        // cmbPatient
        // 
        cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPatient.Location = new Point(147, 12);
        cmbPatient.Name = "cmbPatient";
        cmbPatient.Size = new Size(300, 28);
        cmbPatient.TabIndex = 1;
        // 
        // cmbDoctor
        // 
        cmbDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDoctor.Location = new Point(147, 42);
        cmbDoctor.Name = "cmbDoctor";
        cmbDoctor.Size = new Size(300, 28);
        cmbDoctor.TabIndex = 3;
        // 
        // cmbExaminationType
        // 
        cmbExaminationType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbExaminationType.Location = new Point(147, 72);
        cmbExaminationType.Name = "cmbExaminationType";
        cmbExaminationType.Size = new Size(300, 28);
        cmbExaminationType.TabIndex = 5;
        // 
        // dtpScheduledDate
        // 
        dtpScheduledDate.CustomFormat = "yyyy-MM-dd HH:mm";
        dtpScheduledDate.Format = DateTimePickerFormat.Custom;
        dtpScheduledDate.Location = new Point(147, 99);
        dtpScheduledDate.Name = "dtpScheduledDate";
        dtpScheduledDate.ShowUpDown = true;
        dtpScheduledDate.Size = new Size(300, 27);
        dtpScheduledDate.TabIndex = 7;
        dtpScheduledDate.ValueChanged += dtpScheduledDate_ValueChanged;
        // 
        // txtStatus
        // 
        txtStatus.Location = new Point(147, 132);
        txtStatus.Name = "txtStatus";
        txtStatus.Size = new Size(300, 27);
        txtStatus.TabIndex = 9;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(215, 170);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(75, 30);
        btnSave.TabIndex = 10;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += BtnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(296, 170);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(75, 30);
        btnCancel.TabIndex = 11;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += BtnCancel_Click;
        // 
        // lblPatient
        // 
        lblPatient.AutoSize = true;
        lblPatient.Location = new Point(12, 15);
        lblPatient.Name = "lblPatient";
        lblPatient.Size = new Size(57, 20);
        lblPatient.TabIndex = 0;
        lblPatient.Text = "Patient:";
        // 
        // lblDoctor
        // 
        lblDoctor.AutoSize = true;
        lblDoctor.Location = new Point(12, 45);
        lblDoctor.Name = "lblDoctor";
        lblDoctor.Size = new Size(58, 20);
        lblDoctor.TabIndex = 2;
        lblDoctor.Text = "Doctor:";
        // 
        // lblExaminationType
        // 
        lblExaminationType.AutoSize = true;
        lblExaminationType.Location = new Point(12, 75);
        lblExaminationType.Name = "lblExaminationType";
        lblExaminationType.Size = new Size(129, 20);
        lblExaminationType.TabIndex = 4;
        lblExaminationType.Text = "Examination Type:";
        // 
        // lblScheduledDate
        // 
        lblScheduledDate.AutoSize = true;
        lblScheduledDate.Location = new Point(12, 105);
        lblScheduledDate.Name = "lblScheduledDate";
        lblScheduledDate.Size = new Size(117, 20);
        lblScheduledDate.TabIndex = 6;
        lblScheduledDate.Text = "Scheduled Date:";
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(12, 135);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(52, 20);
        lblStatus.TabIndex = 8;
        lblStatus.Text = "Status:";
        // 
        // ExaminationDetailForm
        // 
        ClientSize = new Size(461, 210);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(txtStatus);
        Controls.Add(lblStatus);
        Controls.Add(dtpScheduledDate);
        Controls.Add(lblScheduledDate);
        Controls.Add(cmbExaminationType);
        Controls.Add(lblExaminationType);
        Controls.Add(cmbDoctor);
        Controls.Add(lblDoctor);
        Controls.Add(cmbPatient);
        Controls.Add(lblPatient);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ExaminationDetailForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "ExaminationDetailForm";
        ResumeLayout(false);
        PerformLayout();
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

    private void LoadDoctorComboBox()
    {
        var doctors = _context.Doctors.ToList();
        cmbDoctor.Items.Clear();
        foreach (var doctor in doctors)
        {
            cmbDoctor.Items.Add(new { Id = doctor.Id, Display = $"Dr. {doctor.FirstName} {doctor.LastName} - {doctor.Specialization}" });
        }
        cmbDoctor.DisplayMember = "Display";
        cmbDoctor.ValueMember = "Id";
    }

    private void LoadExaminationTypeComboBox()
    {
        cmbExaminationType.Items.Clear();
        var types = Enum.GetNames(typeof(ExaminationType));
        foreach (var type in types)
        {
            cmbExaminationType.Items.Add(type);
        }
    }

    private void LoadExaminationData()
    {
        if (_examination != null)
        {
            cmbExaminationType.SelectedItem = _examination.ExaminationType;
            dtpScheduledDate.Value = _examination.ScheduledDate;
            txtStatus.Text = _examination.Status ?? "";

            // Select patient
            for (int i = 0; i < cmbPatient.Items.Count; i++)
            {
                var item = cmbPatient.Items[i];
                var idProperty = item.GetType().GetProperty("Id");
                if (idProperty != null && (long)idProperty.GetValue(item)! == _examination.PatientId)
                {
                    cmbPatient.SelectedIndex = i;
                    break;
                }
            }

            // Select doctor
            for (int i = 0; i < cmbDoctor.Items.Count; i++)
            {
                var item = cmbDoctor.Items[i];
                var idProperty = item.GetType().GetProperty("Id");
                if (idProperty != null && (long)idProperty.GetValue(item)! == _examination.DoctorId)
                {
                    cmbDoctor.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;

        try
        {
            if (_examination == null)
            {
                _examination = new Examination();
            }

            var selectedPatient = cmbPatient.SelectedItem;
            var patientIdProperty = selectedPatient?.GetType().GetProperty("Id");
            _examination.PatientId = patientIdProperty != null ? (long)patientIdProperty.GetValue(selectedPatient)! : 0;

            var selectedDoctor = cmbDoctor.SelectedItem;
            var doctorIdProperty = selectedDoctor?.GetType().GetProperty("Id");
            _examination.DoctorId = doctorIdProperty != null ? (long)doctorIdProperty.GetValue(selectedDoctor)! : 0;

            _examination.ExaminationType = cmbExaminationType.SelectedItem?.ToString() ?? "";
            _examination.ScheduledDate = dtpScheduledDate.Value;
            _examination.Status = string.IsNullOrWhiteSpace(txtStatus.Text) ? null : txtStatus.Text.Trim();

            if (_examination.Id == 0)
            {
                _context.Examinations.Add(_examination);
            }
            else
            {
                _context.Examinations.Update(_examination);
            }

            _context.SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving examination: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool ValidateInput()
    {
        if (cmbPatient.SelectedItem == null)
        {
            MessageBox.Show("Please select a patient.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (cmbDoctor.SelectedItem == null)
        {
            MessageBox.Show("Please select a doctor.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (cmbExaminationType.SelectedItem == null)
        {
            MessageBox.Show("Please select an examination type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void dtpScheduledDate_ValueChanged(object sender, EventArgs e)
    {

    }
}



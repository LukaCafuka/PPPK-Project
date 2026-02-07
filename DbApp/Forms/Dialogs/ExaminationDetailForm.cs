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
        LoadPatientComboBox();
        LoadDoctorComboBox();
        LoadExaminationTypeComboBox();
        LoadExaminationData();
    }

    private void InitializeComponent()
    {
        this.cmbPatient = new ComboBox();
        this.cmbDoctor = new ComboBox();
        this.cmbExaminationType = new ComboBox();
        this.dtpScheduledDate = new DateTimePicker();
        this.txtStatus = new TextBox();
        this.btnSave = new Button();
        this.btnCancel = new Button();
        this.lblPatient = new Label();
        this.lblDoctor = new Label();
        this.lblExaminationType = new Label();
        this.lblScheduledDate = new Label();
        this.lblStatus = new Label();
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
        // lblDoctor
        // 
        this.lblDoctor.AutoSize = true;
        this.lblDoctor.Location = new Point(12, 45);
        this.lblDoctor.Name = "lblDoctor";
        this.lblDoctor.Size = new Size(45, 15);
        this.lblDoctor.TabIndex = 2;
        this.lblDoctor.Text = "Doctor:";
        // 
        // cmbDoctor
        // 
        this.cmbDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbDoctor.Location = new Point(120, 42);
        this.cmbDoctor.Name = "cmbDoctor";
        this.cmbDoctor.Size = new Size(300, 23);
        this.cmbDoctor.TabIndex = 3;
        // 
        // lblExaminationType
        // 
        this.lblExaminationType.AutoSize = true;
        this.lblExaminationType.Location = new Point(12, 75);
        this.lblExaminationType.Name = "lblExaminationType";
        this.lblExaminationType.Size = new Size(100, 15);
        this.lblExaminationType.TabIndex = 4;
        this.lblExaminationType.Text = "Examination Type:";
        // 
        // cmbExaminationType
        // 
        this.cmbExaminationType.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbExaminationType.Location = new Point(120, 72);
        this.cmbExaminationType.Name = "cmbExaminationType";
        this.cmbExaminationType.Size = new Size(300, 23);
        this.cmbExaminationType.TabIndex = 5;
        // 
        // lblScheduledDate
        // 
        this.lblScheduledDate.AutoSize = true;
        this.lblScheduledDate.Location = new Point(12, 105);
        this.lblScheduledDate.Name = "lblScheduledDate";
        this.lblScheduledDate.Size = new Size(90, 15);
        this.lblScheduledDate.TabIndex = 6;
        this.lblScheduledDate.Text = "Scheduled Date:";
        // 
        // dtpScheduledDate
        // 
        this.dtpScheduledDate.Location = new Point(120, 102);
        this.dtpScheduledDate.Name = "dtpScheduledDate";
        this.dtpScheduledDate.Size = new Size(300, 23);
        this.dtpScheduledDate.TabIndex = 7;
        this.dtpScheduledDate.Format = DateTimePickerFormat.Custom;
        this.dtpScheduledDate.CustomFormat = "yyyy-MM-dd HH:mm";
        this.dtpScheduledDate.ShowUpDown = true;
        // 
        // lblStatus
        // 
        this.lblStatus.AutoSize = true;
        this.lblStatus.Location = new Point(12, 135);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new Size(42, 15);
        this.lblStatus.TabIndex = 8;
        this.lblStatus.Text = "Status:";
        // 
        // txtStatus
        // 
        this.txtStatus.Location = new Point(120, 132);
        this.txtStatus.Name = "txtStatus";
        this.txtStatus.Size = new Size(300, 23);
        this.txtStatus.TabIndex = 9;
        // 
        // btnSave
        // 
        this.btnSave.Location = new Point(264, 170);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new Size(75, 30);
        this.btnSave.TabIndex = 10;
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += BtnSave_Click;
        // 
        // btnCancel
        // 
        this.btnCancel.Location = new Point(345, 170);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new Size(75, 30);
        this.btnCancel.TabIndex = 11;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += BtnCancel_Click;
        // 
        // ExaminationDetailForm
        // 
        this.ClientSize = new Size(450, 210);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.txtStatus);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.dtpScheduledDate);
        this.Controls.Add(this.lblScheduledDate);
        this.Controls.Add(this.cmbExaminationType);
        this.Controls.Add(this.lblExaminationType);
        this.Controls.Add(this.cmbDoctor);
        this.Controls.Add(this.lblDoctor);
        this.Controls.Add(this.cmbPatient);
        this.Controls.Add(this.lblPatient);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "ExaminationDetailForm";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = _examination == null ? "Schedule Examination" : "Edit Examination";
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
}



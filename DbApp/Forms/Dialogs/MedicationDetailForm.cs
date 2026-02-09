using DbApp.Data;
using DbApp.Models;

namespace DbApp.Forms.Dialogs;

public partial class MedicationDetailForm : Form
{
    private Medication? _medication;
    private MedicalContext _context;

    private ComboBox cmbPatient;
    private TextBox txtMedicationName;
    private NumericUpDown numDose;
    private TextBox txtDoseUnit;
    private TextBox txtFrequency;
    private TextBox txtCondition;
    private DateTimePicker dtpPrescribedDate;
    private Button btnSave;
    private Button btnCancel;
    private Label lblPatient;
    private Label lblMedicationName;
    private Label lblDose;
    private Label lblDoseUnit;
    private Label lblFrequency;
    private Label lblCondition;
    private Label lblPrescribedDate;

    public MedicationDetailForm(Medication? medication, MedicalContext context)
    {
        _medication = medication;
        _context = context;
        InitializeComponent();
        this.Text = _medication == null ? "Add Medication" : "Edit Medication";
        LoadPatientComboBox();
        LoadMedicationData();
    }

    private void InitializeComponent()
    {
        cmbPatient = new ComboBox();
        txtMedicationName = new TextBox();
        numDose = new NumericUpDown();
        txtDoseUnit = new TextBox();
        txtFrequency = new TextBox();
        txtCondition = new TextBox();
        dtpPrescribedDate = new DateTimePicker();
        btnSave = new Button();
        btnCancel = new Button();
        lblPatient = new Label();
        lblMedicationName = new Label();
        lblDose = new Label();
        lblDoseUnit = new Label();
        lblFrequency = new Label();
        lblCondition = new Label();
        lblPrescribedDate = new Label();
        ((System.ComponentModel.ISupportInitialize)numDose).BeginInit();
        SuspendLayout();
        // 
        // cmbPatient
        // 
        cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPatient.Location = new Point(149, 12);
        cmbPatient.Name = "cmbPatient";
        cmbPatient.Size = new Size(300, 28);
        cmbPatient.TabIndex = 1;
        // 
        // txtMedicationName
        // 
        txtMedicationName.Location = new Point(149, 42);
        txtMedicationName.Name = "txtMedicationName";
        txtMedicationName.Size = new Size(300, 27);
        txtMedicationName.TabIndex = 3;
        // 
        // numDose
        // 
        numDose.DecimalPlaces = 2;
        numDose.Location = new Point(149, 72);
        numDose.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        numDose.Name = "numDose";
        numDose.Size = new Size(150, 27);
        numDose.TabIndex = 5;
        // 
        // txtDoseUnit
        // 
        txtDoseUnit.Location = new Point(378, 72);
        txtDoseUnit.Name = "txtDoseUnit";
        txtDoseUnit.Size = new Size(71, 27);
        txtDoseUnit.TabIndex = 7;
        // 
        // txtFrequency
        // 
        txtFrequency.Location = new Point(149, 102);
        txtFrequency.Name = "txtFrequency";
        txtFrequency.Size = new Size(300, 27);
        txtFrequency.TabIndex = 9;
        // 
        // txtCondition
        // 
        txtCondition.Location = new Point(149, 132);
        txtCondition.Multiline = true;
        txtCondition.Name = "txtCondition";
        txtCondition.Size = new Size(300, 60);
        txtCondition.TabIndex = 11;
        // 
        // dtpPrescribedDate
        // 
        dtpPrescribedDate.Location = new Point(149, 200);
        dtpPrescribedDate.Name = "dtpPrescribedDate";
        dtpPrescribedDate.Size = new Size(300, 27);
        dtpPrescribedDate.TabIndex = 13;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(289, 244);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(75, 30);
        btnSave.TabIndex = 14;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += BtnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(370, 244);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(75, 30);
        btnCancel.TabIndex = 15;
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
        // lblMedicationName
        // 
        lblMedicationName.AutoSize = true;
        lblMedicationName.Location = new Point(12, 45);
        lblMedicationName.Name = "lblMedicationName";
        lblMedicationName.Size = new Size(131, 20);
        lblMedicationName.TabIndex = 2;
        lblMedicationName.Text = "Medication Name:";
        // 
        // lblDose
        // 
        lblDose.AutoSize = true;
        lblDose.Location = new Point(12, 75);
        lblDose.Name = "lblDose";
        lblDose.Size = new Size(46, 20);
        lblDose.TabIndex = 4;
        lblDose.Text = "Dose:";
        // 
        // lblDoseUnit
        // 
        lblDoseUnit.AutoSize = true;
        lblDoseUnit.Location = new Point(295, 74);
        lblDoseUnit.Name = "lblDoseUnit";
        lblDoseUnit.Size = new Size(77, 20);
        lblDoseUnit.TabIndex = 6;
        lblDoseUnit.Text = "Dose Unit:";
        // 
        // lblFrequency
        // 
        lblFrequency.AutoSize = true;
        lblFrequency.Location = new Point(12, 105);
        lblFrequency.Name = "lblFrequency";
        lblFrequency.Size = new Size(79, 20);
        lblFrequency.TabIndex = 8;
        lblFrequency.Text = "Frequency:";
        // 
        // lblCondition
        // 
        lblCondition.AutoSize = true;
        lblCondition.Location = new Point(12, 135);
        lblCondition.Name = "lblCondition";
        lblCondition.Size = new Size(77, 20);
        lblCondition.TabIndex = 10;
        lblCondition.Text = "Condition:";
        // 
        // lblPrescribedDate
        // 
        lblPrescribedDate.AutoSize = true;
        lblPrescribedDate.Location = new Point(12, 205);
        lblPrescribedDate.Name = "lblPrescribedDate";
        lblPrescribedDate.Size = new Size(117, 20);
        lblPrescribedDate.TabIndex = 12;
        lblPrescribedDate.Text = "Prescribed Date:";
        // 
        // MedicationDetailForm
        // 
        ClientSize = new Size(464, 293);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(dtpPrescribedDate);
        Controls.Add(lblPrescribedDate);
        Controls.Add(txtCondition);
        Controls.Add(lblCondition);
        Controls.Add(txtFrequency);
        Controls.Add(lblFrequency);
        Controls.Add(txtDoseUnit);
        Controls.Add(lblDoseUnit);
        Controls.Add(numDose);
        Controls.Add(lblDose);
        Controls.Add(txtMedicationName);
        Controls.Add(lblMedicationName);
        Controls.Add(cmbPatient);
        Controls.Add(lblPatient);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "MedicationDetailForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "MedicationDetailForm";
        ((System.ComponentModel.ISupportInitialize)numDose).EndInit();
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

    private void LoadMedicationData()
    {
        if (_medication != null)
        {
            txtMedicationName.Text = _medication.MedicationName;
            numDose.Value = _medication.Dose;
            txtDoseUnit.Text = _medication.DoseUnit;
            txtFrequency.Text = _medication.Frequency;
            txtCondition.Text = _medication.Condition ?? "";
            dtpPrescribedDate.Value = _medication.PrescribedDate;

            // Select patient
            for (int i = 0; i < cmbPatient.Items.Count; i++)
            {
                var item = cmbPatient.Items[i];
                var idProperty = item.GetType().GetProperty("Id");
                if (idProperty != null && (long)idProperty.GetValue(item)! == _medication.PatientId)
                {
                    cmbPatient.SelectedIndex = i;
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
            if (_medication == null)
            {
                _medication = new Medication();
            }

            var selectedPatient = cmbPatient.SelectedItem;
            var idProperty = selectedPatient?.GetType().GetProperty("Id");
            _medication.PatientId = idProperty != null ? (long)idProperty.GetValue(selectedPatient)! : 0;

            _medication.MedicationName = txtMedicationName.Text.Trim();
            _medication.Dose = numDose.Value;
            _medication.DoseUnit = txtDoseUnit.Text.Trim();
            _medication.Frequency = txtFrequency.Text.Trim();
            _medication.Condition = string.IsNullOrWhiteSpace(txtCondition.Text) ? null : txtCondition.Text.Trim();
            _medication.PrescribedDate = dtpPrescribedDate.Value;

            if (_medication.Id == 0)
            {
                _context.Medications.Add(_medication);
            }
            else
            {
                _context.Medications.Update(_medication);
            }

            _context.SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving medication: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool ValidateInput()
    {
        if (cmbPatient.SelectedItem == null)
        {
            MessageBox.Show("Please select a patient.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtMedicationName.Text))
        {
            MessageBox.Show("Medication name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtDoseUnit.Text))
        {
            MessageBox.Show("Dose unit is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtFrequency.Text))
        {
            MessageBox.Show("Frequency is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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



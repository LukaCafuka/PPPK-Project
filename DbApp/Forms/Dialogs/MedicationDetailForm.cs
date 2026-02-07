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
        LoadPatientComboBox();
        LoadMedicationData();
    }

    private void InitializeComponent()
    {
        this.cmbPatient = new ComboBox();
        this.txtMedicationName = new TextBox();
        this.numDose = new NumericUpDown();
        this.txtDoseUnit = new TextBox();
        this.txtFrequency = new TextBox();
        this.txtCondition = new TextBox();
        this.dtpPrescribedDate = new DateTimePicker();
        this.btnSave = new Button();
        this.btnCancel = new Button();
        this.lblPatient = new Label();
        this.lblMedicationName = new Label();
        this.lblDose = new Label();
        this.lblDoseUnit = new Label();
        this.lblFrequency = new Label();
        this.lblCondition = new Label();
        this.lblPrescribedDate = new Label();
        ((System.ComponentModel.ISupportInitialize)(this.numDose)).BeginInit();
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
        // lblMedicationName
        // 
        this.lblMedicationName.AutoSize = true;
        this.lblMedicationName.Location = new Point(12, 45);
        this.lblMedicationName.Name = "lblMedicationName";
        this.lblMedicationName.Size = new Size(100, 15);
        this.lblMedicationName.TabIndex = 2;
        this.lblMedicationName.Text = "Medication Name:";
        // 
        // txtMedicationName
        // 
        this.txtMedicationName.Location = new Point(120, 42);
        this.txtMedicationName.Name = "txtMedicationName";
        this.txtMedicationName.Size = new Size(300, 23);
        this.txtMedicationName.TabIndex = 3;
        // 
        // lblDose
        // 
        this.lblDose.AutoSize = true;
        this.lblDose.Location = new Point(12, 75);
        this.lblDose.Name = "lblDose";
        this.lblDose.Size = new Size(36, 15);
        this.lblDose.TabIndex = 4;
        this.lblDose.Text = "Dose:";
        // 
        // numDose
        // 
        this.numDose.DecimalPlaces = 2;
        this.numDose.Location = new Point(120, 72);
        this.numDose.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        this.numDose.Name = "numDose";
        this.numDose.Size = new Size(150, 23);
        this.numDose.TabIndex = 5;
        // 
        // lblDoseUnit
        // 
        this.lblDoseUnit.AutoSize = true;
        this.lblDoseUnit.Location = new Point(276, 75);
        this.lblDoseUnit.Name = "lblDoseUnit";
        this.lblDoseUnit.Size = new Size(61, 15);
        this.lblDoseUnit.TabIndex = 6;
        this.lblDoseUnit.Text = "Dose Unit:";
        // 
        // txtDoseUnit
        // 
        this.txtDoseUnit.Location = new Point(343, 72);
        this.txtDoseUnit.Name = "txtDoseUnit";
        this.txtDoseUnit.Size = new Size(77, 23);
        this.txtDoseUnit.TabIndex = 7;
        // 
        // lblFrequency
        // 
        this.lblFrequency.AutoSize = true;
        this.lblFrequency.Location = new Point(12, 105);
        this.lblFrequency.Name = "lblFrequency";
        this.lblFrequency.Size = new Size(64, 15);
        this.lblFrequency.TabIndex = 8;
        this.lblFrequency.Text = "Frequency:";
        // 
        // txtFrequency
        // 
        this.txtFrequency.Location = new Point(120, 102);
        this.txtFrequency.Name = "txtFrequency";
        this.txtFrequency.Size = new Size(300, 23);
        this.txtFrequency.TabIndex = 9;
        // 
        // lblCondition
        // 
        this.lblCondition.AutoSize = true;
        this.lblCondition.Location = new Point(12, 135);
        this.lblCondition.Name = "lblCondition";
        this.lblCondition.Size = new Size(61, 15);
        this.lblCondition.TabIndex = 10;
        this.lblCondition.Text = "Condition:";
        // 
        // txtCondition
        // 
        this.txtCondition.Location = new Point(120, 132);
        this.txtCondition.Multiline = true;
        this.txtCondition.Name = "txtCondition";
        this.txtCondition.Size = new Size(300, 60);
        this.txtCondition.TabIndex = 11;
        // 
        // lblPrescribedDate
        // 
        this.lblPrescribedDate.AutoSize = true;
        this.lblPrescribedDate.Location = new Point(12, 205);
        this.lblPrescribedDate.Name = "lblPrescribedDate";
        this.lblPrescribedDate.Size = new Size(91, 15);
        this.lblPrescribedDate.TabIndex = 12;
        this.lblPrescribedDate.Text = "Prescribed Date:";
        // 
        // dtpPrescribedDate
        // 
        this.dtpPrescribedDate.Location = new Point(120, 202);
        this.dtpPrescribedDate.Name = "dtpPrescribedDate";
        this.dtpPrescribedDate.Size = new Size(300, 23);
        this.dtpPrescribedDate.TabIndex = 13;
        // 
        // btnSave
        // 
        this.btnSave.Location = new Point(264, 280);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new Size(75, 30);
        this.btnSave.TabIndex = 14;
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += BtnSave_Click;
        // 
        // btnCancel
        // 
        this.btnCancel.Location = new Point(345, 280);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new Size(75, 30);
        this.btnCancel.TabIndex = 15;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += BtnCancel_Click;
        // 
        // MedicationDetailForm
        // 
        this.ClientSize = new Size(450, 320);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.dtpPrescribedDate);
        this.Controls.Add(this.lblPrescribedDate);
        this.Controls.Add(this.txtCondition);
        this.Controls.Add(this.lblCondition);
        this.Controls.Add(this.txtFrequency);
        this.Controls.Add(this.lblFrequency);
        this.Controls.Add(this.txtDoseUnit);
        this.Controls.Add(this.lblDoseUnit);
        this.Controls.Add(this.numDose);
        this.Controls.Add(this.lblDose);
        this.Controls.Add(this.txtMedicationName);
        this.Controls.Add(this.lblMedicationName);
        this.Controls.Add(this.cmbPatient);
        this.Controls.Add(this.lblPatient);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "MedicationDetailForm";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = _medication == null ? "Add Medication" : "Edit Medication";
        ((System.ComponentModel.ISupportInitialize)(this.numDose)).EndInit();
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



namespace DbApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new TabControl();
            this.tabPagePatients = new TabPage();
            this.patientListForm1 = new Forms.PatientListForm();
            this.tabPageDiseaseHistory = new TabPage();
            this.diseaseHistoryForm1 = new Forms.DiseaseHistoryForm();
            this.tabPageMedications = new TabPage();
            this.medicationForm1 = new Forms.MedicationForm();
            this.tabPageExaminations = new TabPage();
            this.examinationForm1 = new Forms.ExaminationForm();
            this.tabPageDoctors = new TabPage();
            this.doctorListForm1 = new Forms.DoctorListForm();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPagePatients);
            this.tabControl1.Controls.Add(this.tabPageDiseaseHistory);
            this.tabControl1.Controls.Add(this.tabPageMedications);
            this.tabControl1.Controls.Add(this.tabPageExaminations);
            this.tabControl1.Controls.Add(this.tabPageDoctors);
            this.tabControl1.Dock = DockStyle.Fill;
            this.tabControl1.Location = new Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(1200, 700);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPagePatients
            // 
            this.tabPagePatients.Controls.Add(this.patientListForm1);
            this.tabPagePatients.Location = new Point(4, 24);
            this.tabPagePatients.Name = "tabPagePatients";
            this.tabPagePatients.Padding = new Padding(3);
            this.tabPagePatients.Size = new Size(1192, 672);
            this.tabPagePatients.TabIndex = 0;
            this.tabPagePatients.Text = "Patients";
            this.tabPagePatients.UseVisualStyleBackColor = true;
            // 
            // patientListForm1
            // 
            this.patientListForm1.Dock = DockStyle.Fill;
            this.patientListForm1.Location = new Point(3, 3);
            this.patientListForm1.Name = "patientListForm1";
            this.patientListForm1.Size = new Size(1186, 666);
            this.patientListForm1.TabIndex = 0;
            // 
            // tabPageDiseaseHistory
            // 
            this.tabPageDiseaseHistory.Controls.Add(this.diseaseHistoryForm1);
            this.tabPageDiseaseHistory.Location = new Point(4, 24);
            this.tabPageDiseaseHistory.Name = "tabPageDiseaseHistory";
            this.tabPageDiseaseHistory.Padding = new Padding(3);
            this.tabPageDiseaseHistory.Size = new Size(1192, 672);
            this.tabPageDiseaseHistory.TabIndex = 1;
            this.tabPageDiseaseHistory.Text = "Disease History";
            this.tabPageDiseaseHistory.UseVisualStyleBackColor = true;
            // 
            // diseaseHistoryForm1
            // 
            this.diseaseHistoryForm1.Dock = DockStyle.Fill;
            this.diseaseHistoryForm1.Location = new Point(3, 3);
            this.diseaseHistoryForm1.Name = "diseaseHistoryForm1";
            this.diseaseHistoryForm1.Size = new Size(1186, 666);
            this.diseaseHistoryForm1.TabIndex = 0;
            // 
            // tabPageMedications
            // 
            this.tabPageMedications.Controls.Add(this.medicationForm1);
            this.tabPageMedications.Location = new Point(4, 24);
            this.tabPageMedications.Name = "tabPageMedications";
            this.tabPageMedications.Padding = new Padding(3);
            this.tabPageMedications.Size = new Size(1192, 672);
            this.tabPageMedications.TabIndex = 2;
            this.tabPageMedications.Text = "Medications";
            this.tabPageMedications.UseVisualStyleBackColor = true;
            // 
            // medicationForm1
            // 
            this.medicationForm1.Dock = DockStyle.Fill;
            this.medicationForm1.Location = new Point(3, 3);
            this.medicationForm1.Name = "medicationForm1";
            this.medicationForm1.Size = new Size(1186, 666);
            this.medicationForm1.TabIndex = 0;
            // 
            // tabPageExaminations
            // 
            this.tabPageExaminations.Controls.Add(this.examinationForm1);
            this.tabPageExaminations.Location = new Point(4, 24);
            this.tabPageExaminations.Name = "tabPageExaminations";
            this.tabPageExaminations.Padding = new Padding(3);
            this.tabPageExaminations.Size = new Size(1192, 672);
            this.tabPageExaminations.TabIndex = 3;
            this.tabPageExaminations.Text = "Examinations";
            this.tabPageExaminations.UseVisualStyleBackColor = true;
            // 
            // examinationForm1
            // 
            this.examinationForm1.Dock = DockStyle.Fill;
            this.examinationForm1.Location = new Point(3, 3);
            this.examinationForm1.Name = "examinationForm1";
            this.examinationForm1.Size = new Size(1186, 666);
            this.examinationForm1.TabIndex = 0;
            // 
            // tabPageDoctors
            // 
            this.tabPageDoctors.Controls.Add(this.doctorListForm1);
            this.tabPageDoctors.Location = new Point(4, 24);
            this.tabPageDoctors.Name = "tabPageDoctors";
            this.tabPageDoctors.Padding = new Padding(3);
            this.tabPageDoctors.Size = new Size(1192, 672);
            this.tabPageDoctors.TabIndex = 4;
            this.tabPageDoctors.Text = "Doctors";
            this.tabPageDoctors.UseVisualStyleBackColor = true;
            // 
            // doctorListForm1
            // 
            this.doctorListForm1.Dock = DockStyle.Fill;
            this.doctorListForm1.Location = new Point(3, 3);
            this.doctorListForm1.Name = "doctorListForm1";
            this.doctorListForm1.Size = new Size(1186, 666);
            this.doctorListForm1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 700);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Medical System Management";
            this.WindowState = FormWindowState.Maximized;
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPagePatients;
        private TabPage tabPageDiseaseHistory;
        private TabPage tabPageMedications;
        private TabPage tabPageExaminations;
        private TabPage tabPageDoctors;
        private Forms.PatientListForm patientListForm1;
        private Forms.DiseaseHistoryForm diseaseHistoryForm1;
        private Forms.MedicationForm medicationForm1;
        private Forms.ExaminationForm examinationForm1;
        private Forms.DoctorListForm doctorListForm1;
    }
}

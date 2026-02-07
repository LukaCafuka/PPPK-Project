using DbApp.Data;
using DbApp.Models;

namespace DbApp.Forms;

public partial class DoctorListForm : UserControl
{
    private DataGridView dataGridViewDoctors;
    private Label lblInfo;

    public DoctorListForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.dataGridViewDoctors = new DataGridView();
        this.lblInfo = new Label();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDoctors)).BeginInit();
        this.SuspendLayout();
        // 
        // lblInfo
        // 
        this.lblInfo.AutoSize = true;
        this.lblInfo.Location = new Point(12, 15);
        this.lblInfo.Name = "lblInfo";
        this.lblInfo.Size = new Size(300, 15);
        this.lblInfo.TabIndex = 0;
        this.lblInfo.Text = "Doctors are defined during application initialization (read-only).";
        // 
        // dataGridViewDoctors
        // 
        this.dataGridViewDoctors.AllowUserToAddRows = false;
        this.dataGridViewDoctors.AllowUserToDeleteRows = false;
        this.dataGridViewDoctors.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewDoctors.Location = new Point(12, 50);
        this.dataGridViewDoctors.Name = "dataGridViewDoctors";
        this.dataGridViewDoctors.ReadOnly = true;
        this.dataGridViewDoctors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewDoctors.Size = new Size(1176, 604);
        this.dataGridViewDoctors.TabIndex = 1;
        this.dataGridViewDoctors.Dock = DockStyle.Fill;
        this.dataGridViewDoctors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // 
        // DoctorListForm
        // 
        this.Controls.Add(this.dataGridViewDoctors);
        this.Controls.Add(this.lblInfo);
        this.Name = "DoctorListForm";
        this.Size = new Size(1200, 700);
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDoctors)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    public void LoadDoctors(MedicalContext context)
    {
        try
        {
            var doctors = context.Doctors.ToList();
            dataGridViewDoctors.DataSource = doctors.Select(d => new
            {
                d.Id,
                d.FirstName,
                d.LastName,
                d.Specialization
            }).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading doctors: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}



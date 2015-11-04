namespace TFSTestMigrationTool
{
    public partial class formMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMain));
            this.btnSelectSourceTfsProject = new System.Windows.Forms.Button();
            this.btnCopyTestItems = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblSourceTfsProject = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.treeTestPlans = new System.Windows.Forms.TreeView();
            this.imageListTfsIcons = new System.Windows.Forms.ImageList(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblDestinationTfsProject = new System.Windows.Forms.Label();
            this.btnSelectDestinationTfsProject = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.myStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectSourceTfsProject
            // 
            this.btnSelectSourceTfsProject.Location = new System.Drawing.Point(6, 19);
            this.btnSelectSourceTfsProject.Name = "btnSelectSourceTfsProject";
            this.btnSelectSourceTfsProject.Size = new System.Drawing.Size(172, 23);
            this.btnSelectSourceTfsProject.TabIndex = 1;
            this.btnSelectSourceTfsProject.Text = "Select Source TFS Project";
            this.btnSelectSourceTfsProject.UseVisualStyleBackColor = true;
            this.btnSelectSourceTfsProject.Click += new System.EventHandler(this.btnSelectSourceTfsProject_Click);
            // 
            // btnCopyTestItems
            // 
            this.btnCopyTestItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopyTestItems.Location = new System.Drawing.Point(6, 19);
            this.btnCopyTestItems.Name = "btnCopyTestItems";
            this.btnCopyTestItems.Size = new System.Drawing.Size(506, 40);
            this.btnCopyTestItems.TabIndex = 3;
            this.btnCopyTestItems.Text = "Copy Selected Plans, Suites, and Test Cases";
            this.btnCopyTestItems.UseVisualStyleBackColor = true;
            this.btnCopyTestItems.Click += new System.EventHandler(this.btnCopyTestItems_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblSourceTfsProject);
            this.groupBox2.Controls.Add(this.btnSelectSourceTfsProject);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(522, 49);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Source TFS Project";
            // 
            // lblSourceTfsProject
            // 
            this.lblSourceTfsProject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSourceTfsProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSourceTfsProject.Location = new System.Drawing.Point(184, 19);
            this.lblSourceTfsProject.Name = "lblSourceTfsProject";
            this.lblSourceTfsProject.Size = new System.Drawing.Size(331, 23);
            this.lblSourceTfsProject.TabIndex = 8;
            this.lblSourceTfsProject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.treeTestPlans);
            this.groupBox1.Location = new System.Drawing.Point(12, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(522, 261);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test Plans, Suites, and Test Cases";
            // 
            // treeTestPlans
            // 
            this.treeTestPlans.CheckBoxes = true;
            this.treeTestPlans.ImageIndex = 0;
            this.treeTestPlans.ImageList = this.imageListTfsIcons;
            this.treeTestPlans.Location = new System.Drawing.Point(6, 19);
            this.treeTestPlans.Name = "treeTestPlans";
            this.treeTestPlans.SelectedImageIndex = 0;
            this.treeTestPlans.Size = new System.Drawing.Size(509, 236);
            this.treeTestPlans.TabIndex = 0;
            // 
            // imageListTfsIcons
            // 
            this.imageListTfsIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTfsIcons.ImageStream")));
            this.imageListTfsIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTfsIcons.Images.SetKeyName(0, "TfsProjectIcon.png");
            this.imageListTfsIcons.Images.SetKeyName(1, "TfsProjectPlanIcon.png");
            this.imageListTfsIcons.Images.SetKeyName(2, "TfsTestSuiteIcon.png");
            this.imageListTfsIcons.Images.SetKeyName(3, "TfsTestCaseIcon.png");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblDestinationTfsProject);
            this.groupBox3.Controls.Add(this.btnSelectDestinationTfsProject);
            this.groupBox3.Location = new System.Drawing.Point(12, 334);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(522, 49);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Destination TFS Project";
            // 
            // lblDestinationTfsProject
            // 
            this.lblDestinationTfsProject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDestinationTfsProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDestinationTfsProject.Location = new System.Drawing.Point(184, 19);
            this.lblDestinationTfsProject.Name = "lblDestinationTfsProject";
            this.lblDestinationTfsProject.Size = new System.Drawing.Size(331, 23);
            this.lblDestinationTfsProject.TabIndex = 8;
            this.lblDestinationTfsProject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSelectDestinationTfsProject
            // 
            this.btnSelectDestinationTfsProject.Location = new System.Drawing.Point(6, 19);
            this.btnSelectDestinationTfsProject.Name = "btnSelectDestinationTfsProject";
            this.btnSelectDestinationTfsProject.Size = new System.Drawing.Size(172, 23);
            this.btnSelectDestinationTfsProject.TabIndex = 0;
            this.btnSelectDestinationTfsProject.Text = "Select Destination TFS Project";
            this.btnSelectDestinationTfsProject.UseVisualStyleBackColor = true;
            this.btnSelectDestinationTfsProject.Click += new System.EventHandler(this.btnSelectDestinationTfsProject_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.myStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 462);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(542, 25);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // myStatus
            // 
            this.myStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.myStatus.Name = "myStatus";
            this.myStatus.Size = new System.Drawing.Size(40, 20);
            this.myStatus.Text = "        ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnCopyTestItems);
            this.groupBox4.Location = new System.Drawing.Point(12, 389);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(518, 69);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Migration";
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 487);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TFS/MTM Test Plan Migration Tool";
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelectSourceTfsProject;
        private System.Windows.Forms.Button btnCopyTestItems;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblSourceTfsProject;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ImageList imageListTfsIcons;
        private System.Windows.Forms.TreeView treeTestPlans;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblDestinationTfsProject;
        private System.Windows.Forms.Button btnSelectDestinationTfsProject;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel myStatus;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}


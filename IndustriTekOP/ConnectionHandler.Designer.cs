namespace IndustriTekOP
{
    partial class ConnectionHandler
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionHandler));
            this.Preloader = new System.Windows.Forms.PictureBox();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.SubStatusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Preloader)).BeginInit();
            this.SuspendLayout();
            // 
            // Preloader
            // 
            this.Preloader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            resources.ApplyResources(this.Preloader, "Preloader");
            this.Preloader.Name = "Preloader";
            this.Preloader.TabStop = false;
            this.Preloader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Preloader_MouseDown);
            // 
            // StatusLabel
            // 
            resources.ApplyResources(this.StatusLabel, "StatusLabel");
            this.StatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(243)))));
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.StatusLabel_MouseDown);
            // 
            // SubStatusLabel
            // 
            resources.ApplyResources(this.SubStatusLabel, "SubStatusLabel");
            this.SubStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(116)))), ((int)(((byte)(119)))));
            this.SubStatusLabel.Name = "SubStatusLabel";
            this.SubStatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SubStatusLabel_MouseDown);
            // 
            // ConnectionHandler
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.Controls.Add(this.SubStatusLabel);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.Preloader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ConnectionHandler";
            this.Load += new System.EventHandler(this.ConnectionHandler_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ConnectionHandler_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.Preloader)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Preloader;
        public System.Windows.Forms.Label StatusLabel;
        public System.Windows.Forms.Label SubStatusLabel;
    }
}


namespace TestPlaylistCreation
{
    partial class TestWindow
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
            this.CreatePlaylist_button = new System.Windows.Forms.Button();
            this.CreatePlaylistDirectory_tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CreatePlaylistName_tb = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CreatePlaylist_button
            // 
            this.CreatePlaylist_button.Location = new System.Drawing.Point(255, 64);
            this.CreatePlaylist_button.Name = "CreatePlaylist_button";
            this.CreatePlaylist_button.Size = new System.Drawing.Size(139, 23);
            this.CreatePlaylist_button.TabIndex = 0;
            this.CreatePlaylist_button.Text = "Create Playlist";
            this.CreatePlaylist_button.UseVisualStyleBackColor = true;
            this.CreatePlaylist_button.Click += new System.EventHandler(this.CreatePlaylist_button_Click);
            // 
            // CreatePlaylistDirectory_tb
            // 
            this.CreatePlaylistDirectory_tb.Location = new System.Drawing.Point(108, 12);
            this.CreatePlaylistDirectory_tb.Name = "CreatePlaylistDirectory_tb";
            this.CreatePlaylistDirectory_tb.Size = new System.Drawing.Size(286, 20);
            this.CreatePlaylistDirectory_tb.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Directory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Name";
            // 
            // CreatePlaylistName_tb
            // 
            this.CreatePlaylistName_tb.Location = new System.Drawing.Point(108, 38);
            this.CreatePlaylistName_tb.Name = "CreatePlaylistName_tb";
            this.CreatePlaylistName_tb.Size = new System.Drawing.Size(286, 20);
            this.CreatePlaylistName_tb.TabIndex = 3;
            // 
            // TestWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 315);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CreatePlaylistName_tb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CreatePlaylistDirectory_tb);
            this.Controls.Add(this.CreatePlaylist_button);
            this.Name = "TestWindow";
            this.Text = "TestWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CreatePlaylist_button;
        private System.Windows.Forms.TextBox CreatePlaylistDirectory_tb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox CreatePlaylistName_tb;
    }
}
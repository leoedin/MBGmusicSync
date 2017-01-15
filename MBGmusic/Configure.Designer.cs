namespace MusicBeePlugin
{
    partial class Configure
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
            this.loginButton = new System.Windows.Forms.Button();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.emailLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.rememberCheckbox = new System.Windows.Forms.CheckBox();
            this.warnSaveLabel = new System.Windows.Forms.Label();
            this.syncNowButton = new System.Windows.Forms.Button();
            this.loginStatusLabel = new System.Windows.Forms.Label();
            this.syncStatusLabel = new System.Windows.Forms.Label();
            this.localPlaylistBox = new System.Windows.Forms.CheckedListBox();
            this.googleMusicPlaylistBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.allLocalPlayCheckbox = new System.Windows.Forms.CheckBox();
            this.allRemotePlayCheckbox = new System.Windows.Forms.CheckBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.toGMusicRadiobutton = new System.Windows.Forms.RadioButton();
            this.fromGMusicRadioButton = new System.Windows.Forms.RadioButton();
            this.fetchPlaylistStatusLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(754, 50);
            this.loginButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(150, 44);
            this.loginButton.TabIndex = 0;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(24, 50);
            this.emailTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(316, 31);
            this.emailTextBox.TabIndex = 1;
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point(18, 17);
            this.emailLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(65, 25);
            this.emailLabel.TabIndex = 2;
            this.emailLabel.Text = "Email";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(384, 50);
            this.passwordTextBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(316, 31);
            this.passwordTextBox.TabIndex = 3;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(378, 17);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(106, 25);
            this.passwordLabel.TabIndex = 4;
            this.passwordLabel.Text = "Password";
            // 
            // rememberCheckbox
            // 
            this.rememberCheckbox.AutoSize = true;
            this.rememberCheckbox.Location = new System.Drawing.Point(754, 15);
            this.rememberCheckbox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rememberCheckbox.Name = "rememberCheckbox";
            this.rememberCheckbox.Size = new System.Drawing.Size(207, 29);
            this.rememberCheckbox.TabIndex = 5;
            this.rememberCheckbox.Text = "Remember Login";
            this.rememberCheckbox.UseVisualStyleBackColor = true;
            // 
            // warnSaveLabel
            // 
            this.warnSaveLabel.AutoSize = true;
            this.warnSaveLabel.Location = new System.Drawing.Point(1000, 17);
            this.warnSaveLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.warnSaveLabel.Name = "warnSaveLabel";
            this.warnSaveLabel.Size = new System.Drawing.Size(132, 75);
            this.warnSaveLabel.TabIndex = 6;
            this.warnSaveLabel.Text = "Stores login \r\ndetails in \r\nplain text!";
            // 
            // syncNowButton
            // 
            this.syncNowButton.Location = new System.Drawing.Point(312, 1008);
            this.syncNowButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.syncNowButton.Name = "syncNowButton";
            this.syncNowButton.Size = new System.Drawing.Size(250, 44);
            this.syncNowButton.TabIndex = 8;
            this.syncNowButton.Text = "Synchronise Selected";
            this.syncNowButton.UseVisualStyleBackColor = true;
            this.syncNowButton.Click += new System.EventHandler(this.syncNowButton_Click);
            // 
            // loginStatusLabel
            // 
            this.loginStatusLabel.AutoSize = true;
            this.loginStatusLabel.Location = new System.Drawing.Point(30, 115);
            this.loginStatusLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.loginStatusLabel.Name = "loginStatusLabel";
            this.loginStatusLabel.Size = new System.Drawing.Size(139, 25);
            this.loginStatusLabel.TabIndex = 9;
            this.loginStatusLabel.Text = "Not logged in";
            // 
            // syncStatusLabel
            // 
            this.syncStatusLabel.AutoSize = true;
            this.syncStatusLabel.Location = new System.Drawing.Point(608, 1017);
            this.syncStatusLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.syncStatusLabel.Name = "syncStatusLabel";
            this.syncStatusLabel.Size = new System.Drawing.Size(137, 25);
            this.syncStatusLabel.TabIndex = 10;
            this.syncStatusLabel.Text = "Click to Sync";
            // 
            // localPlaylistBox
            // 
            this.localPlaylistBox.CheckOnClick = true;
            this.localPlaylistBox.FormattingEnabled = true;
            this.localPlaylistBox.Location = new System.Drawing.Point(12, 88);
            this.localPlaylistBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.localPlaylistBox.Name = "localPlaylistBox";
            this.localPlaylistBox.Size = new System.Drawing.Size(526, 680);
            this.localPlaylistBox.TabIndex = 11;
            this.localPlaylistBox.SelectedIndexChanged += new System.EventHandler(this.localPlaylistBox_SelectedIndexChanged);
            // 
            // googleMusicPlaylistBox
            // 
            this.googleMusicPlaylistBox.CheckOnClick = true;
            this.googleMusicPlaylistBox.FormattingEnabled = true;
            this.googleMusicPlaylistBox.Location = new System.Drawing.Point(564, 88);
            this.googleMusicPlaylistBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.googleMusicPlaylistBox.Name = "googleMusicPlaylistBox";
            this.googleMusicPlaylistBox.Size = new System.Drawing.Size(526, 680);
            this.googleMusicPlaylistBox.TabIndex = 12;
            this.googleMusicPlaylistBox.SelectedIndexChanged += new System.EventHandler(this.googleMusicPlaylistBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 25);
            this.label1.TabIndex = 13;
            this.label1.Text = "Local playlists";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(558, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(228, 25);
            this.label2.TabIndex = 14;
            this.label2.Text = "Google Music playlists";
            // 
            // allLocalPlayCheckbox
            // 
            this.allLocalPlayCheckbox.AutoSize = true;
            this.allLocalPlayCheckbox.Location = new System.Drawing.Point(398, 44);
            this.allLocalPlayCheckbox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.allLocalPlayCheckbox.Name = "allLocalPlayCheckbox";
            this.allLocalPlayCheckbox.Size = new System.Drawing.Size(134, 29);
            this.allLocalPlayCheckbox.TabIndex = 15;
            this.allLocalPlayCheckbox.Text = "Select All";
            this.allLocalPlayCheckbox.UseVisualStyleBackColor = true;
            this.allLocalPlayCheckbox.CheckedChanged += new System.EventHandler(this.allLocalPlayCheckbox_CheckedChanged);
            // 
            // allRemotePlayCheckbox
            // 
            this.allRemotePlayCheckbox.AutoSize = true;
            this.allRemotePlayCheckbox.Location = new System.Drawing.Point(954, 44);
            this.allRemotePlayCheckbox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.allRemotePlayCheckbox.Name = "allRemotePlayCheckbox";
            this.allRemotePlayCheckbox.Size = new System.Drawing.Size(134, 29);
            this.allRemotePlayCheckbox.TabIndex = 16;
            this.allRemotePlayCheckbox.Text = "Select All";
            this.allRemotePlayCheckbox.UseVisualStyleBackColor = true;
            this.allRemotePlayCheckbox.CheckedChanged += new System.EventHandler(this.allRemotePlayCheckbox_CheckedChanged);
            // 
            // closeButton
            // 
            this.closeButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.closeButton.Location = new System.Drawing.Point(980, 1058);
            this.closeButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(150, 44);
            this.closeButton.TabIndex = 17;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // toGMusicRadiobutton
            // 
            this.toGMusicRadiobutton.AutoSize = true;
            this.toGMusicRadiobutton.Checked = true;
            this.toGMusicRadiobutton.Location = new System.Drawing.Point(24, 1013);
            this.toGMusicRadiobutton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.toGMusicRadiobutton.Name = "toGMusicRadiobutton";
            this.toGMusicRadiobutton.Size = new System.Drawing.Size(231, 29);
            this.toGMusicRadiobutton.TabIndex = 18;
            this.toGMusicRadiobutton.TabStop = true;
            this.toGMusicRadiobutton.Text = "-> To Google Music";
            this.toGMusicRadiobutton.UseVisualStyleBackColor = true;
            this.toGMusicRadiobutton.CheckedChanged += new System.EventHandler(this.toGMusicRadiobutton_CheckedChanged);
            // 
            // fromGMusicRadioButton
            // 
            this.fromGMusicRadioButton.AutoSize = true;
            this.fromGMusicRadioButton.Location = new System.Drawing.Point(24, 1058);
            this.fromGMusicRadioButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.fromGMusicRadioButton.Name = "fromGMusicRadioButton";
            this.fromGMusicRadioButton.Size = new System.Drawing.Size(255, 29);
            this.fromGMusicRadioButton.TabIndex = 19;
            this.fromGMusicRadioButton.Text = "<- From Google Music";
            this.fromGMusicRadioButton.UseVisualStyleBackColor = true;
            this.fromGMusicRadioButton.CheckedChanged += new System.EventHandler(this.fromGMusicRadioButton_CheckedChanged);
            // 
            // fetchPlaylistStatusLabel
            // 
            this.fetchPlaylistStatusLabel.AutoSize = true;
            this.fetchPlaylistStatusLabel.Location = new System.Drawing.Point(840, 115);
            this.fetchPlaylistStatusLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.fetchPlaylistStatusLabel.Name = "fetchPlaylistStatusLabel";
            this.fetchPlaylistStatusLabel.Size = new System.Drawing.Size(286, 25);
            this.fetchPlaylistStatusLabel.TabIndex = 20;
            this.fetchPlaylistStatusLabel.Text = "Login to see remote playlists";
            this.fetchPlaylistStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.allLocalPlayCheckbox);
            this.groupBox1.Controls.Add(this.localPlaylistBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.allRemotePlayCheckbox);
            this.groupBox1.Controls.Add(this.googleMusicPlaylistBox);
            this.groupBox1.Location = new System.Drawing.Point(24, 163);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(1106, 812);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set Up Sync";
            // 
            // Configure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 1125);
            this.Controls.Add(this.fetchPlaylistStatusLabel);
            this.Controls.Add(this.syncStatusLabel);
            this.Controls.Add(this.fromGMusicRadioButton);
            this.Controls.Add(this.toGMusicRadiobutton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.loginStatusLabel);
            this.Controls.Add(this.syncNowButton);
            this.Controls.Add(this.warnSaveLabel);
            this.Controls.Add(this.rememberCheckbox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.emailTextBox);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "Configure";
            this.Text = "Google Music Sync";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Configure_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.CheckBox rememberCheckbox;
        private System.Windows.Forms.Label warnSaveLabel;
        private System.Windows.Forms.Button syncNowButton;
        private System.Windows.Forms.Label loginStatusLabel;
        private System.Windows.Forms.Label syncStatusLabel;
        private System.Windows.Forms.CheckedListBox localPlaylistBox;
        private System.Windows.Forms.CheckedListBox googleMusicPlaylistBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox allLocalPlayCheckbox;
        private System.Windows.Forms.CheckBox allRemotePlayCheckbox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.RadioButton toGMusicRadiobutton;
        private System.Windows.Forms.RadioButton fromGMusicRadioButton;
        private System.Windows.Forms.Label fetchPlaylistStatusLabel;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
﻿
namespace ValheimPlusManager
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.installClientButton = new System.Windows.Forms.Button();
            this.clientInstalledLabel = new System.Windows.Forms.Label();
            this.serverInstalledLabel = new System.Windows.Forms.Label();
            this.installServerButton = new System.Windows.Forms.Button();
            this.manageServerButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.manageClientButton = new System.Windows.Forms.Button();
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            this.checkServerUpdatesIconButton = new FontAwesome.Sharp.IconButton();
            this.installServerUpdateIconButton = new FontAwesome.Sharp.IconButton();
            this.errorLabel = new System.Windows.Forms.Label();
            this.clientPanel = new System.Windows.Forms.Panel();
            this.serverPanel = new System.Windows.Forms.Panel();
            this.clientPanel.SuspendLayout();
            this.serverPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(-1, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(404, 43);
            this.label1.TabIndex = 0;
            this.label1.Text = "ValheimPlus Manager";
            // 
            // installClientButton
            // 
            this.installClientButton.Location = new System.Drawing.Point(5, 68);
            this.installClientButton.Name = "installClientButton";
            this.installClientButton.Size = new System.Drawing.Size(185, 32);
            this.installClientButton.TabIndex = 1;
            this.installClientButton.Text = "Install ValheimPlus on client";
            this.installClientButton.UseVisualStyleBackColor = true;
            // 
            // clientInstalledLabel
            // 
            this.clientInstalledLabel.AutoSize = true;
            this.clientInstalledLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.clientInstalledLabel.Location = new System.Drawing.Point(4, 32);
            this.clientInstalledLabel.Name = "clientInstalledLabel";
            this.clientInstalledLabel.Size = new System.Drawing.Size(167, 15);
            this.clientInstalledLabel.TabIndex = 2;
            this.clientInstalledLabel.Text = "ValheimPlus installed on client";
            // 
            // serverInstalledLabel
            // 
            this.serverInstalledLabel.AutoSize = true;
            this.serverInstalledLabel.Location = new System.Drawing.Point(7, 41);
            this.serverInstalledLabel.Name = "serverInstalledLabel";
            this.serverInstalledLabel.Size = new System.Drawing.Size(169, 15);
            this.serverInstalledLabel.TabIndex = 3;
            this.serverInstalledLabel.Text = "ValheimPlus installed on server";
            // 
            // installServerButton
            // 
            this.installServerButton.Location = new System.Drawing.Point(8, 75);
            this.installServerButton.Name = "installServerButton";
            this.installServerButton.Size = new System.Drawing.Size(185, 32);
            this.installServerButton.TabIndex = 4;
            this.installServerButton.Text = "Install ValheimPlus on server";
            this.installServerButton.UseVisualStyleBackColor = true;
            this.installServerButton.Click += new System.EventHandler(this.installServerButton_Click);
            // 
            // manageServerButton
            // 
            this.manageServerButton.Location = new System.Drawing.Point(8, 113);
            this.manageServerButton.Name = "manageServerButton";
            this.manageServerButton.Size = new System.Drawing.Size(185, 32);
            this.manageServerButton.TabIndex = 5;
            this.manageServerButton.Text = "Manage ValheimPlus settings";
            this.manageServerButton.UseVisualStyleBackColor = true;
            this.manageServerButton.Click += new System.EventHandler(this.manageServerButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 21);
            this.label2.TabIndex = 6;
            this.label2.Text = "Client";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 21);
            this.label3.TabIndex = 7;
            this.label3.Text = "Server";
            // 
            // manageClientButton
            // 
            this.manageClientButton.Location = new System.Drawing.Point(5, 106);
            this.manageClientButton.Name = "manageClientButton";
            this.manageClientButton.Size = new System.Drawing.Size(185, 32);
            this.manageClientButton.TabIndex = 8;
            this.manageClientButton.Text = "Manage ValheimPlus settings";
            this.manageClientButton.UseVisualStyleBackColor = true;
            this.manageClientButton.Click += new System.EventHandler(this.manageClientButton_Click);
            // 
            // iconButton1
            // 
            this.iconButton1.IconChar = FontAwesome.Sharp.IconChar.Download;
            this.iconButton1.IconColor = System.Drawing.Color.Black;
            this.iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton1.IconSize = 24;
            this.iconButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.iconButton1.Location = new System.Drawing.Point(196, 68);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Size = new System.Drawing.Size(185, 32);
            this.iconButton1.TabIndex = 9;
            this.iconButton1.Text = "Check for updates";
            this.iconButton1.UseVisualStyleBackColor = true;
            this.iconButton1.Click += new System.EventHandler(this.iconButton1_Click);
            // 
            // checkServerUpdatesIconButton
            // 
            this.checkServerUpdatesIconButton.IconChar = FontAwesome.Sharp.IconChar.Download;
            this.checkServerUpdatesIconButton.IconColor = System.Drawing.Color.Black;
            this.checkServerUpdatesIconButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.checkServerUpdatesIconButton.IconSize = 24;
            this.checkServerUpdatesIconButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.checkServerUpdatesIconButton.Location = new System.Drawing.Point(199, 75);
            this.checkServerUpdatesIconButton.Name = "checkServerUpdatesIconButton";
            this.checkServerUpdatesIconButton.Size = new System.Drawing.Size(185, 32);
            this.checkServerUpdatesIconButton.TabIndex = 9;
            this.checkServerUpdatesIconButton.Text = "Check for updates";
            this.checkServerUpdatesIconButton.UseVisualStyleBackColor = true;
            this.checkServerUpdatesIconButton.Click += new System.EventHandler(this.checkServerUpdatesIconButton_Click_1);
            // 
            // installServerUpdateIconButton
            // 
            this.installServerUpdateIconButton.IconChar = FontAwesome.Sharp.IconChar.Cogs;
            this.installServerUpdateIconButton.IconColor = System.Drawing.Color.Black;
            this.installServerUpdateIconButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.installServerUpdateIconButton.IconSize = 24;
            this.installServerUpdateIconButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.installServerUpdateIconButton.Location = new System.Drawing.Point(199, 113);
            this.installServerUpdateIconButton.Name = "installServerUpdateIconButton";
            this.installServerUpdateIconButton.Size = new System.Drawing.Size(185, 32);
            this.installServerUpdateIconButton.TabIndex = 10;
            this.installServerUpdateIconButton.Text = "Install update";
            this.installServerUpdateIconButton.UseVisualStyleBackColor = true;
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.errorLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.errorLabel.ForeColor = System.Drawing.Color.Red;
            this.errorLabel.Location = new System.Drawing.Point(0, 373);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(0, 20);
            this.errorLabel.TabIndex = 11;
            this.errorLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // clientPanel
            // 
            this.clientPanel.Controls.Add(this.manageClientButton);
            this.clientPanel.Controls.Add(this.installClientButton);
            this.clientPanel.Controls.Add(this.clientInstalledLabel);
            this.clientPanel.Controls.Add(this.label2);
            this.clientPanel.Controls.Add(this.iconButton1);
            this.clientPanel.Location = new System.Drawing.Point(-1, 48);
            this.clientPanel.Name = "clientPanel";
            this.clientPanel.Size = new System.Drawing.Size(395, 155);
            this.clientPanel.TabIndex = 12;
            // 
            // serverPanel
            // 
            this.serverPanel.Controls.Add(this.installServerUpdateIconButton);
            this.serverPanel.Controls.Add(this.checkServerUpdatesIconButton);
            this.serverPanel.Controls.Add(this.label3);
            this.serverPanel.Controls.Add(this.manageServerButton);
            this.serverPanel.Controls.Add(this.installServerButton);
            this.serverPanel.Controls.Add(this.serverInstalledLabel);
            this.serverPanel.Location = new System.Drawing.Point(-1, 211);
            this.serverPanel.Name = "serverPanel";
            this.serverPanel.Size = new System.Drawing.Size(395, 155);
            this.serverPanel.TabIndex = 13;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(392, 393);
            this.Controls.Add(this.serverPanel);
            this.Controls.Add(this.clientPanel);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ValheimPlus Manager";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.clientPanel.ResumeLayout(false);
            this.clientPanel.PerformLayout();
            this.serverPanel.ResumeLayout(false);
            this.serverPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button installClientButton;
        private System.Windows.Forms.Label clientInstalledLabel;
        private System.Windows.Forms.Label serverInstalledLabel;
        private System.Windows.Forms.Button installServerButton;
        private System.Windows.Forms.Button manageServerButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button manageClientButton;
        private FontAwesome.Sharp.IconButton iconButton1;
        private FontAwesome.Sharp.IconButton checkServerUpdatesIconButton;
        private FontAwesome.Sharp.IconButton installServerUpdateIconButton;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Panel clientPanel;
        private System.Windows.Forms.Panel serverPanel;
    }
}


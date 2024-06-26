﻿namespace chat_client.View.Chat
{
    partial class Chat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            this.chatConnectionListBox = new System.Windows.Forms.ListBox();
            this.chatMessageListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.emojiListButton = new System.Windows.Forms.PictureBox();
            this.sendButton = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.loggedUsername = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emojiListButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sendButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // chatConnectionListBox
            // 
            this.chatConnectionListBox.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.chatConnectionListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatConnectionListBox.FormattingEnabled = true;
            this.chatConnectionListBox.ItemHeight = 16;
            this.chatConnectionListBox.Location = new System.Drawing.Point(28, 169);
            this.chatConnectionListBox.Name = "chatConnectionListBox";
            this.chatConnectionListBox.Size = new System.Drawing.Size(428, 84);
            this.chatConnectionListBox.TabIndex = 3;
            // 
            // chatMessageListBox
            // 
            this.chatMessageListBox.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.chatMessageListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatMessageListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatMessageListBox.FormattingEnabled = true;
            this.chatMessageListBox.ItemHeight = 16;
            this.chatMessageListBox.Location = new System.Drawing.Point(28, 264);
            this.chatMessageListBox.Name = "chatMessageListBox";
            this.chatMessageListBox.Size = new System.Drawing.Size(428, 224);
            this.chatMessageListBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F);
            this.label1.Location = new System.Drawing.Point(138, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(206, 39);
            this.label1.TabIndex = 6;
            this.label1.Text = "Chat Instatly\r\n";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label2.Location = new System.Drawing.Point(159, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 25);
            this.label2.TabIndex = 8;
            this.label2.Text = "Connect Globally\r\n";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(28, 481);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(428, 39);
            this.textBox2.TabIndex = 9;
            // 
            // messageTextBox
            // 
            this.messageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageTextBox.Location = new System.Drawing.Point(28, 495);
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(359, 22);
            this.messageTextBox.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(187, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(90, 91);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // emojiListButton
            // 
            this.emojiListButton.Image = ((System.Drawing.Image)(resources.GetObject("emojiListButton.Image")));
            this.emojiListButton.Location = new System.Drawing.Point(391, 493);
            this.emojiListButton.Name = "emojiListButton";
            this.emojiListButton.Size = new System.Drawing.Size(28, 26);
            this.emojiListButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.emojiListButton.TabIndex = 11;
            this.emojiListButton.TabStop = false;
            this.emojiListButton.Click += new System.EventHandler(this.EmojiListButton_Click);
            // 
            // sendButton
            // 
            this.sendButton.Image = ((System.Drawing.Image)(resources.GetObject("sendButton.Image")));
            this.sendButton.Location = new System.Drawing.Point(425, 493);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(28, 27);
            this.sendButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.sendButton.TabIndex = 12;
            this.sendButton.TabStop = false;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(437, 12);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(35, 34);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 13;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Click += new System.EventHandler(this.HandleLoginAccount);
            // 
            // loggedUsername
            // 
            this.loggedUsername.AutoSize = true;
            this.loggedUsername.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F);
            this.loggedUsername.Location = new System.Drawing.Point(25, 31);
            this.loggedUsername.Name = "loggedUsername";
            this.loggedUsername.Size = new System.Drawing.Size(72, 23);
            this.loggedUsername.TabIndex = 14;
            this.loggedUsername.Text = "Maxim";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10F);
            this.label3.Location = new System.Drawing.Point(24, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "Logged in:";
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.loggedUsername);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.emojiListButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.chatMessageListBox);
            this.Controls.Add(this.chatConnectionListBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Chat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HandleFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emojiListButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sendButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private System.Windows.Forms.PictureBox pictureBox1;
        //private System.Windows.Forms.PictureBox exitPictureBox;
        //private System.Windows.Forms.PictureBox sendMessagePictureBox;
        private System.Windows.Forms.ListBox chatConnectionListBox;
        private System.Windows.Forms.ListBox chatMessageListBox;
        private System.Windows.Forms.Label label1;
        //private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox emojiListButton;
        private System.Windows.Forms.PictureBox sendButton;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label loggedUsername;
        private System.Windows.Forms.Label label3;
    }
}
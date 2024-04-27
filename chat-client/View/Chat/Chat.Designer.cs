namespace chat_client.View.Chat
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
            this.sendButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.emojiButton = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
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
            this.messageTextBox.Location = new System.Drawing.Point(28, 489);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(359, 30);
            this.messageTextBox.TabIndex = 5;
            // 
            // sendButton
            // 
            this.sendButton.BackgroundImage = global::chat_client.Properties.Resources.send;
            this.sendButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.sendButton.Location = new System.Drawing.Point(420, 488);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(34, 32);
            this.sendButton.TabIndex = 17;
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.BackgroundImage = global::chat_client.Properties.Resources.logout;
            this.exitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.exitButton.Location = new System.Drawing.Point(450, 4);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(32, 32);
            this.exitButton.TabIndex = 16;
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.handleLoginAccount);
            // 
            // emojiButton
            // 
            this.emojiButton.BackgroundImage = global::chat_client.Properties.Resources.emoji;
            this.emojiButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.emojiButton.Location = new System.Drawing.Point(388, 488);
            this.emojiButton.Name = "emojiButton";
            this.emojiButton.Size = new System.Drawing.Size(32, 32);
            this.emojiButton.TabIndex = 15;
            this.emojiButton.UseVisualStyleBackColor = true;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::chat_client.Properties.Resources.chat_app;
            this.pictureBox3.Location = new System.Drawing.Point(117, -23);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(257, 162);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 12;
            this.pictureBox3.TabStop = false;
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.emojiButton);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.chatMessageListBox);
            this.Controls.Add(this.chatConnectionListBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Chat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.handleFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
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
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button emojiButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button sendButton;
    }
}
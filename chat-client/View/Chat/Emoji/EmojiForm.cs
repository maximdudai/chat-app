using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat_client.View.Chat.Emoji
{
    public partial class EmojiForm : Form
    {
        public event Action<string> EmojiSelected;
        public EmojiForm()
        {
            InitializeComponent();
            InitializeEmojiButtons();
        }
        private void InitializeEmojiButtons()
        {
            var emojis = typeof(EmojiList).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            int x = 10; // Initial X position of the first button
            int y = 10; // Initial Y position of the first button
            const int spacing = 50; // Space between buttons

            foreach (var emoji in emojis)
            {
                string emojiChar = emoji.GetValue(null).ToString();
                Button emojiButton = new Button
                {
                    Text = emojiChar,
                    Location = new Point(x, y),
                    Size = new Size(40, 40)
                };
                // attach the event handler to the button and get it to chat form
                emojiButton.Click += EmojiButton_Click;
                this.Controls.Add(emojiButton);

                x += spacing;
                // If the button is out of the form size, move to the next line
                if (x > this.ClientSize.Width - 40)
                {
                    x = 10;
                    y += spacing;
                }
            }
        }

        private void EmojiButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null && EmojiSelected != null)
            {
                // Trigger the event, sending the emoji text
                EmojiSelected(button.Text);
            }
            this.Close(); // Close the form after selecting an emoji
        }
    }
}

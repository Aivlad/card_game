
namespace FoolGame
{
    partial class TableShell
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.User1 = new System.Windows.Forms.RichTextBox();
            this.User2 = new System.Windows.Forms.RichTextBox();
            this.Table = new System.Windows.Forms.RichTextBox();
            this.Deck = new System.Windows.Forms.RichTextBox();
            this.Droping = new System.Windows.Forms.RichTextBox();
            this.Logs = new System.Windows.Forms.RichTextBox();
            this.StrokeIndex = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.Hint = new System.Windows.Forms.ToolTip(this.components);
            this.Trump = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // User1
            // 
            this.User1.Location = new System.Drawing.Point(12, 12);
            this.User1.Name = "User1";
            this.User1.Size = new System.Drawing.Size(606, 250);
            this.User1.TabIndex = 1;
            this.User1.Text = "";
            // 
            // User2
            // 
            this.User2.Location = new System.Drawing.Point(12, 579);
            this.User2.Name = "User2";
            this.User2.Size = new System.Drawing.Size(606, 250);
            this.User2.TabIndex = 2;
            this.User2.Text = "";
            // 
            // Table
            // 
            this.Table.Location = new System.Drawing.Point(12, 268);
            this.Table.Name = "Table";
            this.Table.Size = new System.Drawing.Size(606, 305);
            this.Table.TabIndex = 3;
            this.Table.Text = "";
            // 
            // Deck
            // 
            this.Deck.Location = new System.Drawing.Point(709, 12);
            this.Deck.Name = "Deck";
            this.Deck.Size = new System.Drawing.Size(405, 531);
            this.Deck.TabIndex = 4;
            this.Deck.Text = "";
            // 
            // Droping
            // 
            this.Droping.Location = new System.Drawing.Point(1120, 12);
            this.Droping.Name = "Droping";
            this.Droping.Size = new System.Drawing.Size(405, 561);
            this.Droping.TabIndex = 5;
            this.Droping.Text = "";
            // 
            // Logs
            // 
            this.Logs.Location = new System.Drawing.Point(709, 579);
            this.Logs.Name = "Logs";
            this.Logs.Size = new System.Drawing.Size(816, 250);
            this.Logs.TabIndex = 6;
            this.Logs.Text = "";
            // 
            // StrokeIndex
            // 
            this.StrokeIndex.Location = new System.Drawing.Point(624, 268);
            this.StrokeIndex.Name = "StrokeIndex";
            this.StrokeIndex.Size = new System.Drawing.Size(79, 20);
            this.StrokeIndex.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(624, 294);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Ход";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Trump
            // 
            this.Trump.Location = new System.Drawing.Point(709, 545);
            this.Trump.Name = "Trump";
            this.Trump.Size = new System.Drawing.Size(405, 28);
            this.Trump.TabIndex = 9;
            this.Trump.Text = "";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(624, 323);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(79, 40);
            this.button2.TabIndex = 10;
            this.button2.Text = "Забрать/пас";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(624, 466);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(79, 40);
            this.button3.TabIndex = 11;
            this.button3.Text = "Тест: сброс до 4";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // TableShell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(1537, 841);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Trump);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.StrokeIndex);
            this.Controls.Add(this.Logs);
            this.Controls.Add(this.Droping);
            this.Controls.Add(this.Deck);
            this.Controls.Add(this.Table);
            this.Controls.Add(this.User2);
            this.Controls.Add(this.User1);
            this.Name = "TableShell";
            this.Text = "Fool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox User1;
        private System.Windows.Forms.RichTextBox User2;
        private System.Windows.Forms.RichTextBox Table;
        private System.Windows.Forms.RichTextBox Deck;
        private System.Windows.Forms.RichTextBox Droping;
        private System.Windows.Forms.RichTextBox Logs;
        private System.Windows.Forms.TextBox StrokeIndex;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip Hint;
        private System.Windows.Forms.RichTextBox Trump;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}


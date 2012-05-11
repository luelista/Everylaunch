namespace Everylaunch {
  partial class Form1 {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.TextBox1 = new System.Windows.Forms.TextBox();
      this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
      this.Label1 = new System.Windows.Forms.Label();
      this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.ListView1 = new System.Windows.Forms.ListView();
      this.ImageList1 = new System.Windows.Forms.ImageList(this.components);
      this.SuspendLayout();
      // 
      // TextBox1
      // 
      this.TextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.TextBox1.Location = new System.Drawing.Point(84, 5);
      this.TextBox1.Name = "TextBox1";
      this.TextBox1.Size = new System.Drawing.Size(357, 20);
      this.TextBox1.TabIndex = 3;
      this.TextBox1.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
      this.TextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
      this.TextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
      // 
      // ColumnHeader2
      // 
      this.ColumnHeader2.Width = 212;
      // 
      // Label1
      // 
      this.Label1.BackColor = System.Drawing.Color.DodgerBlue;
      this.Label1.Location = new System.Drawing.Point(-5, 0);
      this.Label1.Name = "Label1";
      this.Label1.Size = new System.Drawing.Size(464, 32);
      this.Label1.TabIndex = 5;
      // 
      // ColumnHeader1
      // 
      this.ColumnHeader1.Width = 224;
      // 
      // ListView1
      // 
      this.ListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ListView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.ListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1,
            this.ColumnHeader2});
      this.ListView1.FullRowSelect = true;
      this.ListView1.HideSelection = false;
      this.ListView1.Location = new System.Drawing.Point(5, 9);
      this.ListView1.MultiSelect = false;
      this.ListView1.Name = "ListView1";
      this.ListView1.Size = new System.Drawing.Size(440, 771);
      this.ListView1.SmallImageList = this.ImageList1;
      this.ListView1.TabIndex = 4;
      this.ListView1.UseCompatibleStateImageBehavior = false;
      this.ListView1.View = System.Windows.Forms.View.Details;
      // 
      // ImageList1
      // 
      this.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.ImageList1.ImageSize = new System.Drawing.Size(16, 16);
      this.ImageList1.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(452, 241);
      this.Controls.Add(this.TextBox1);
      this.Controls.Add(this.Label1);
      this.Controls.Add(this.ListView1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "Form1";
      this.ShowIcon = false;
      this.Text = "Everylaunch";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.TextBox TextBox1;
    internal System.Windows.Forms.ColumnHeader ColumnHeader2;
    internal System.Windows.Forms.Label Label1;
    internal System.Windows.Forms.ColumnHeader ColumnHeader1;
    internal System.Windows.Forms.ListView ListView1;
    internal System.Windows.Forms.ImageList ImageList1;
  }
}


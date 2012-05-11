using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Everylaunch {
  public partial class Form1 : Form {

    EveryThingIPCWindow api = new EveryThingIPCWindow();
     
    Font headFont, smallFont;

    Stack<String> icoLoadStack = new Stack<String>();
    bool icoLoading = false;

    public Form1() {
      InitializeComponent();

      headFont = new Font(ListView1.Font, FontStyle.Bold);
      smallFont = new Font(ListView1.Font.FontFamily, 6, FontStyle.Regular, GraphicsUnit.Point);

      Hotkeys.RegisterHotKey(this, Keys.Control | Keys.G, 0x01);
    }

    private void TextBox1_TextChanged(object sender, EventArgs e) {
      string catSearch, catName;
      ListView1.Items.Clear();

      if ((ImageList1.Images.Count > 500)) {
        ImageList1.Images.Clear();
      }

      string searchKeyword = TextBox1.Text;
      searchMe(searchKeyword, "*.lnk \"\\desktop\"", "Desktop");
      searchMe(searchKeyword, "*.lnk \"\\start menu\"", "Start Menu");
      searchMe(searchKeyword, "*.exe", "Executables");

      if (ListView1.Items.Count > 1) {
        ListView1.Items[1].Selected = true;
      }

    }

    private void Form1_Load(object sender, EventArgs e) {
    }


    // CF Note: The WndProc is not present in the Compact Framework (as of vers. 3.5)! please derive from the MessageWindow class in order to handle WM_HOTKEY
    protected override void WndProc(ref Message m) {
      base.WndProc(ref m);

      if (m.Msg == Hotkeys.WM_HOTKEY) {
        this.Show();
        this.Activate();
        TextBox1.Text = "";
        ListView1.Items.Clear();
        TextBox1.Focus();
      }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
      if (e.CloseReason == CloseReason.UserClosing) {
        e.Cancel = true;
        this.Hide();
      } else {
        Hotkeys.UnregisterHotKey(this, 0x01);
      }
    }

    void searchMe(string searchKeyword, string catSearch, string catName) {
      var res = api.GetResults((catSearch + (" " + searchKeyword)), 10);
      if ((res.Length <= 1)) return;

      ListView1.Items.Add(catName).Font = headFont;

      foreach (var d in res) {
        var lvi = ListView1.Items.Add(d.name);
        icoLoadStack.Push(d.filespec);
        lvi.Tag = d;
        lvi.ImageKey = d.filespec;
        var lvsvi = lvi.SubItems.Add(d.filespec);
      }
      if (!icoLoading) {
        Thread t = new Thread(this.work);
        t.Start();
      }
    }

    void work() {
      icoLoading = true;
      try {
        while ((icoLoadStack.Count > 0)) {
          string filespec = icoLoadStack.Pop();
          if (!String.IsNullOrEmpty(filespec) && !ImageList1.Images.ContainsKey(filespec)) {
            Icon ico = ShellIcon.GetSmallIcon(filespec);
            if (ico != null) {
              this.Invoke((ThreadStart)delegate {
                ImageList1.Images.Add(filespec, ico);
              });
            }
          }
        }
      } catch   {
      } finally {
        icoLoading = false;
      }
     }

    private void TextBox1_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
        case Keys.Down:
          while (selectedIndex < ListView1.Items.Count - 1) {
            selectedIndex++;
            if (ListView1.SelectedItems[0].Tag != null) break;
          }
          e.Handled = true;
          break;

        case Keys.Up:
          while (selectedIndex > 1) {
            selectedIndex--;
            if (ListView1.SelectedItems[0].Tag != null) break;
          }
          e.Handled = true;
          break;

        case Keys.Return:
          e.Handled = true;
          e.SuppressKeyPress = true;

          if (selectedIndex == -1) return;
          this.Hide();

          string fileSpec = ((ResultObj)ListView1.SelectedItems[0].Tag).filespec;
          Process.Start(fileSpec);

          break;

        case Keys.Escape:
          this.Hide();
          break;

      }
    }

    int selectedIndex {
      get {
        if (ListView1.SelectedIndices.Count == 0) return -1;
        return ListView1.SelectedIndices[0];
      }
      set {
        ListView1.Items[value].Selected = true;
      }
    }

    private void TextBox1_KeyPress(object sender, KeyPressEventArgs e) {
      //to get rid of stupid beep sound
      if (e.KeyChar == (char)13) {
        e.Handled = true;
      }
    }

  }
}

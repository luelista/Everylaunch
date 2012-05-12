using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Everylaunch {
  public partial class Form1 : Form {
    EveryThingIPCWindow api = new EveryThingIPCWindow();
    
    Font headFont, smallFont;

    Stack<String> icoLoadStack = new Stack<String>();
    bool icoLoading = false;

    List<String> lastUsed = new List<String>();

    public Form1() {
      InitializeComponent();

      headFont = new Font(ListView1.Font, FontStyle.Bold);
      smallFont = new Font(ListView1.Font.FontFamily, 6, FontStyle.Regular, GraphicsUnit.Point);

      Hotkeys.RegisterHotKey(this, Keys.Enter, 0x01, true);
      Hotkeys.RegisterHotKey(this, Keys.Space, 0x02, true);
    }

    private void TextBox1_TextChanged(object sender, EventArgs e) {
      timer1.Stop();
      timer1.Start();
    }

    void searchMe() {
      string catSearch, catName;
      ListView1.Items.Clear();

      string searchKeyword = TextBox1.Text;

      if (String.IsNullOrEmpty(searchKeyword)) {
        searchLastused("", "", "Last used applications", 20);
      } else {
        searchCat(searchKeyword, "*.lnk \"\\desktop\"", "Desktop", 10);
        searchCat(searchKeyword, "*.lnk \"\\start menu\"", "Start Menu", 10);
        searchCat(searchKeyword, "*.exe", "Executables", 25);
      }

      if (ListView1.Items.Count > 1) {
        ListView1.Items[1].Selected = true;
      }
    }

    private void Form1_Load(object sender, EventArgs e) {
      Top = 0;
      Left = Screen.PrimaryScreen.WorkingArea.Right - Width - 50;
    }


    // CF Note: The WndProc is not present in the Compact Framework (as of vers. 3.5)! please derive from the MessageWindow class in order to handle WM_HOTKEY
    protected override void WndProc(ref Message m) {
      base.WndProc(ref m);

      if (m.Msg == Hotkeys.WM_HOTKEY || m.LParam == (IntPtr)0x01) {
        
      }
      if (m.Msg == Hotkeys.WM_HOTKEY || m.LParam == (IntPtr)0x02) {
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

    void searchCat(string searchKeyword, string catSearch, string catName, int maxlen) {
      var res = api.GetResults((catSearch + (" " + searchKeyword)), maxlen);
      if ((res.Length < 1)) return;

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

    void searchLastused(string searchKeyword, string catSearch, string catName, int maxlen) {
      if ((lastUsed.Count < 1)) return;

      ListView1.Items.Add(catName).Font = headFont;

      for (int i = lastUsed.Count - 1; i >= 0; i--) {
        var d = lastUsed[i];
        var lvi = ListView1.Items.Add(Path.GetFileName(d));
        icoLoadStack.Push(d);
        lvi.Tag = new ResultObj() {filespec=d, name=Path.GetFileName(d) };
        lvi.ImageKey = d;
        var lvsvi = lvi.SubItems.Add(d);
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
          addToMru(fileSpec);

          break;

        case Keys.Escape:
          this.Hide();
          break;

      }
    }

    void addToMru(string filespec) {
      if (lastUsed.Contains(filespec)) lastUsed.Remove(filespec);
      lastUsed.Add(filespec);
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
      if (e.KeyChar == (char)13 || e.KeyChar == (char)27) {
        e.Handled = true;
      }
    }
    
    private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e) {
      if (selectedIndex == -1) return;
      this.Hide();

      string fileSpec = ((ResultObj)ListView1.SelectedItems[0].Tag).filespec;
      Process.Start("explorer.exe", "/e,/select,\"" + fileSpec + "\"");
      addToMru(fileSpec);

    }

    private void ListView1_MouseClick(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        ListViewItem it = ListView1.GetItemAt(e.X, e.Y);
        if (it == null) return;
        this.Hide();

        string fileSpec = ((ResultObj)it.Tag).filespec;
        Process.Start(fileSpec);
        addToMru(fileSpec);

      }
    }

    private void timer1_Tick(object sender, EventArgs e) {
      timer1.Stop();
      searchMe();
    }

    private void Form1_VisibleChanged(object sender, EventArgs e) {
      if ((ImageList1.Images.Count > 250)) {
        ImageList1.Images.Clear();
      }
    }

  }
}

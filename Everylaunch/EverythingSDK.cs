
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;


namespace Everylaunch {

  public struct ResultObj {
    public string name;
    public string filespec;
  }

  public class EveryThingIPCWindow : NativeWindow, IDisposable {


    private const int MY_REPLY_ID = 0;
    private ListView withEventsField_targetListview;
    public ListView targetListview {
      get { return withEventsField_targetListview; }
      set {
        if (withEventsField_targetListview != null) {
          withEventsField_targetListview.RetrieveVirtualItem -= targetListview_RetrieveVirtualItem;
          withEventsField_targetListview.VirtualItemsSelectionRangeChanged -= targetListview_VirtualItemsSelectionRangeChanged;
        }
        withEventsField_targetListview = value;
        if (withEventsField_targetListview != null) {
          withEventsField_targetListview.RetrieveVirtualItem += targetListview_RetrieveVirtualItem;
          withEventsField_targetListview.VirtualItemsSelectionRangeChanged += targetListview_VirtualItemsSelectionRangeChanged;
        }
      }
    }
    string keyword;

    int viewPortStart;
    protected override void WndProc(ref System.Windows.Forms.Message m) {
      // use the default id (0) for the nId parameter.
      if (Everything.IsQueryReply(m.Msg, m.WParam, m.LParam, MY_REPLY_ID)) {
        int i = 0;
        const int bufsize = 260;
        StringBuilder buf = new StringBuilder(bufsize);

        targetListview.VirtualListSize = Everything.GetTotResults();
        targetListview.Refresh();
        // IPC Query reply.
        // clear the old list of results            
        //listBox1.Items.Clear()
        // set the window title
        //Text = (textBox1.Text + (" - " _
        //            + (Everything_GetTotResults + " Results")))
        // loop through the results, adding each result to the listbox.
        i = 0;
        //Do While (i < Everything_GetNumResults)
        //  ' get the result's full path and file name.
        //  Everything_GetResultFullPathName(i, buf, bufsize)
        //  ' add it to the list box                
        //  listBox1.Items.Insert(i, buf)
        //  i = (i + 1)
        //Loop
        //vScrollBar1.Minimum = 0
        //vScrollBar1.Maximum = (Everything_GetTotResults - 1)
        //vScrollBar1.SmallChange = 1
        //vScrollBar1.LargeChange = (listBox1.ClientRectangle.Height / listBox1.ItemHeight)
      }
      base.WndProc(ref m);
    }

    private void targetListview_RetrieveVirtualItem(object sender, System.Windows.Forms.RetrieveVirtualItemEventArgs e) {
      if (e.ItemIndex < viewPortStart | e.ItemIndex > viewPortStart + 140) {
        viewPortStart = Math.Max(0, e.ItemIndex - 50);
        doSearch(true);
      }

      var fullPath = Everything.GetResultFullPathName(e.ItemIndex - viewPortStart);
      var path = Everything.GetResultPath(e.ItemIndex - viewPortStart);
      var fileName = Everything.GetResultFileName(e.ItemIndex - viewPortStart);

      e.Item = new ListViewItem(fileName);
      //e.Item.SubItems.Add("")
      //e.Item.SubItems.Add("")
      //e.Item.SubItems.Add(path)
      //If Everything.IsFolderResult(e.ItemIndex) Then
      //  e.Item.ImageIndex = 0
      //Else
      //  e.Item.ImageIndex = 1
      //End If
    }

    public ResultObj[] GetResults(string searchKeyword, int maxCount) {
      // set the search
      Everything.SetSearch(searchKeyword);
      // set the reply window (this window) [REQUIRED if not waiting for results in Everything_Query].
      Everything.SetReplyWindow(Handle);
      // set the reply id.
      Everything.SetReplyID(MY_REPLY_ID);

      // set up the visible result window.
      Everything.SetOffset(0);
      Everything.SetMax(maxCount);

      // execute the query
      Everything.Query(true);

      ResultObj[] res = new ResultObj[Everything.GetNumFileResults()];
      int j = 0;
      for (int i = 0; i <= Everything.GetNumResults() - 1; i++) {
        if ((Everything.IsFileResult(i))) {
          res[j].filespec = Everything.GetResultFullPathName(i);
          res[j].name = System.IO.Path.GetFileNameWithoutExtension(res[j].filespec);
          j++;
        }
      }

      return res;
    }


    private void doSearch(bool wait) {
      // set the search
      Everything.SetSearch(keyword);
      // set the reply window (this window) [REQUIRED if not waiting for results in Everything_Query].
      Everything.SetReplyWindow(Handle);
      // set the reply id.
      Everything.SetReplyID(MY_REPLY_ID);

      // set up the visible result window.
      Everything.SetOffset(viewPortStart);
      Everything.SetMax(150);

      // execute the query
      Everything.Query(wait);
    }

    public void StartSearch(string searchKeyword, ListView target) {
      targetListview = target;
      target.Items.Clear();
      target.VirtualMode = true;
      target.VirtualListSize = 0;
      viewPortStart = 0;
      keyword = searchKeyword;
      doSearch(false);
    }
    public EveryThingIPCWindow() {
      CreateParams cp = new CreateParams();
      cp.Caption = "mw Everything IPC Window- side4";
      this.CreateHandle(cp);
    }

    // To detect redundant calls
    private bool disposedValue = false;

    // IDisposable
    protected virtual void Dispose(bool disposing) {
      if (!this.disposedValue) {
        if (disposing) {
          //  free other state (managed objects).
        }
        Everything.SetReplyWindow(IntPtr.Zero);
        Everything.Reset();

        targetListview.VirtualListSize = 0;
        targetListview.VirtualMode = false;
        targetListview = null;
        this.DestroyHandle();
      }
      this.disposedValue = true;
    }

    #region " IDisposable Support "
    // This code added by Visual Basic to correctly implement the disposable pattern.
    public void Dispose() {
      // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion


    private void targetListview_VirtualItemsSelectionRangeChanged(object sender, System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventArgs e) {
    }
  }

  public abstract class Everything {
    [DllImport("Everything.dll", EntryPoint = "Everything_SetSearchW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]


    public static extern int SetSearch(string lpSearchString);
    [DllImport("Everything.dll", EntryPoint = "Everything_SetMatchPath", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern void SetMatchPath(bool bEnable);
    [DllImport("Everything.dll", EntryPoint = "Everything_SetMatchCase", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern void SetMatchCase(bool bEnable);
    [DllImport("Everything.dll", EntryPoint = "Everything_SetMatchWholeWord", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern void SetMatchWholeWord(bool bEnable);
    [DllImport("Everything.dll", EntryPoint = "Everything_SetRegex", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern void SetRegex(bool bEnable);
    [DllImport("Everything.dll", EntryPoint = "Everything_SetMax", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern void SetMax(int dwMax);
    [DllImport("Everything.dll", EntryPoint = "Everything_SetOffset", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern void SetOffset(int dwOffset);
    [DllImport("Everything.dll", EntryPoint = "_Everything_SetReplyWindow@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern void SetReplyWindow(IntPtr hWnd);
    [DllImport("Everything.dll", EntryPoint = "_Everything_SetReplyID@4", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern void SetReplyID(int nId);
    [DllImport("Everything.dll", EntryPoint = "Everything_GetMatchPath", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern bool GetMatchPath();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetMatchCase", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern bool GetMatchCase();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetMatchWholeWord", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern bool GetMatchWholeWord();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetRegex", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern bool GetRegex();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetMax", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern UInt32 GetMax();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetOffset", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern UInt32 GetOffset();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetSearchW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]

    public static extern string GetSearch();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetLastError", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern int GetLastError();
    [DllImport("Everything.dll", EntryPoint = "_Everything_GetReplyWindow@0", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern IntPtr GetReplyWindow();
    [DllImport("Everything.dll", EntryPoint = "_Everything_GetReplyID@0", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern int GetReplyID();
    [DllImport("Everything.dll", EntryPoint = "Everything_QueryW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]

    public static extern bool Query(bool bWait);
    [DllImport("Everything.dll", EntryPoint = "Everything_IsQueryReply", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern bool IsQueryReply(int message, IntPtr wParam, IntPtr lParam, uint nId);
    [DllImport("Everything.dll", EntryPoint = "Everything_SortResultsByPath", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern void SortResultsByPath();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetNumFileResults", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]


    public static extern int GetNumFileResults();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetNumFolderResults", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern int GetNumFolderResults();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetNumResults", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern int GetNumResults();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetTotFileResults", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern int GetTotFileResults();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetTotFolderResults", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern int GetTotFolderResults();
    [DllImport("Everything.dll", EntryPoint = "Everything_GetTotResults", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern int GetTotResults();
    [DllImport("Everything.dll", EntryPoint = "Everything_IsVolumeResult", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern bool IsVolumeResult(int nIndex);
    [DllImport("Everything.dll", EntryPoint = "Everything_IsFolderResult", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern bool IsFolderResult(int nIndex);
    [DllImport("Everything.dll", EntryPoint = "Everything_IsFileResult", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

    public static extern bool IsFileResult(int nIndex);
    [DllImport("Everything.dll", EntryPoint = "Everything_GetResultFullPathNameW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]

    private static extern void GetResultFullPathName(int nIndex, StringBuilder lpString, int nMaxCount);
    [DllImport("Everything.dll", EntryPoint = "Everything_GetResultFileNameW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    private static extern IntPtr GetResultFileNameW(int nIndex);
    [DllImport("Everything.dll", EntryPoint = "Everything_GetResultPathW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    private static extern IntPtr GetResultPathW(int nIndex);

    public static string GetResultFullPathName(int nIndex) {
      const int bufsize = 260;
      StringBuilder buf = new StringBuilder(bufsize);
      Everything.GetResultFullPathName(nIndex, buf, bufsize);
      return buf.ToString();
    }

    public static string GetResultFileName(int nIndex) {
      IntPtr ptr = Everything.GetResultFileNameW(nIndex);
      return Marshal.PtrToStringUni(ptr);
    }

    public static string GetResultPath(int nIndex) {
      IntPtr ptr = Everything.GetResultPathW(nIndex);
      return Marshal.PtrToStringUni(ptr);
    }
    [DllImport("Everything.dll", EntryPoint = "Everything_Reset", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern void Reset();

  }
}

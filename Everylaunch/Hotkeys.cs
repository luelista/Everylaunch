using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Everylaunch {
  class Hotkeys {
    #region fields
    public static int MOD_ALT = 0x1;
    public static int MOD_CONTROL = 0x2;
    public static int MOD_SHIFT = 0x4;
    public static int MOD_WIN = 0x8;
    public static int WM_HOTKEY = 0x312;
    #endregion

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public static void RegisterHotKey(Form f, Keys key, int keyId, bool winkey) {
      int modifiers = 0;

      if ((key & Keys.Alt) == Keys.Alt)
        modifiers = modifiers | Hotkeys.MOD_ALT;

      if ((key & Keys.Control) == Keys.Control)
        modifiers = modifiers | Hotkeys.MOD_CONTROL;

      if ((key & Keys.Shift) == Keys.Shift)
        modifiers = modifiers | Hotkeys.MOD_SHIFT;

      if (winkey)
        modifiers = modifiers | Hotkeys.MOD_WIN;

      Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
      RegisterHotKey((IntPtr)f.Handle, keyId, (uint)modifiers, (uint)k);
    }

    private delegate void Func();

    public static void UnregisterHotKey(Form f, int keyId) {
      try {
        UnregisterHotKey(f.Handle, keyId); // modify this if you want more than one hotkey
      } catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
    }
    
  }
}

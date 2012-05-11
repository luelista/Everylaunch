using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Everylaunch {
  static class Program {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      
      //to avoid form being shown initially
      //http://www.daveamenta.com/2009-09/c-dont-display-the-startup-form/
      new Form1(); 
      
      Application.Run();
    }
  }
}

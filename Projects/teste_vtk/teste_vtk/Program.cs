using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//VTK
using Kitware.VTK;

namespace teste_vtk
{
    
    static class Program
    {
        static ApplicationContext MainContext = new ApplicationContext();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
          

           Application.EnableVisualStyles();
           Application.SetCompatibleTextRenderingDefault(false);
           Application.Run(new Form_simulacao());

        }

        public static void SetMainForm(Form MainForm)
        {
            MainContext.MainForm = MainForm;
        }

        public static void ShowMainForm()
        {
            MainContext.MainForm.Show();
        }



    }
}

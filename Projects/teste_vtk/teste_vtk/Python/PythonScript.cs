using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using NumpyDotNet;
using clr_mtrand;
using System.Diagnostics;
using System.IO;


namespace teste_vtk
{
    class PythonScript
    {
        public void ExecuteCommand()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = "../../Python";//System.IO.Directory.GetFiles("../../Python").ToString();//Directory.GetCurrentDirectory();
            process.StartInfo.Arguments = "/C py gera_aviao.py";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

           
        }
    }
}

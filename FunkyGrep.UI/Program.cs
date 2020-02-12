#region License
// Copyright (c) 2012 Raif Atef Wasef
// This source file is licensed under the  MIT license.
// 
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom 
// the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY 
// KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
// OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mono.Options;

namespace FunkyGrep.UI
{
    static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var optionSet = new OptionSet();
                List<string> unprocessed = optionSet.Parse(args);

                string initialFolder = unprocessed.FirstOrDefault();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(initialFolder));
            }
            catch (OptionException ex)
            {
                // ReSharper disable LocalizableElement
                MessageBox.Show(
                    "Invalid arguments passed via command line. Details: " + ex,
                    "Fatal error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                // ReSharper restore LocalizableElement
            }
            catch (Exception ex)
            {
                // ReSharper disable LocalizableElement
                MessageBox.Show(
                    "An unhandled error occurred. Details: " + ex,
                    "Fatal error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                // ReSharper restore LocalizableElement
            }
        }
    }
}

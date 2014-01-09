using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace VDFBuild
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            
            if (args.Length > 0) {
                List<string> args2 = new List<string>(args);
                args2.RemoveAt(0);
                String sFullArgs = String.Join(" ", args2.ToArray());
                System.Console.WriteLine(String.Join(" ", args)); //Displaying the command called
                Process process = new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = args[0],
                        Arguments = sFullArgs
                    }
                };
                try
                {
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                    System.Console.WriteLine("Check the VDF.sublime-build for the correct path to DFComp.exe");
                }

                string filepat = @"([^""]*)\.src";

                Regex rsrcfile = new Regex(filepat, RegexOptions.IgnoreCase);
                Match m = rsrcfile.Match(sFullArgs);

                //Use Regex to find the error file name
                if (m.Success)
                {
                    String sFileName = m.Groups[1].Captures[0].ToString() + ".err";
                    if (File.Exists(sFileName))
                    {
                        StreamReader sr = new StreamReader(sFileName);

                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string linepat = @"(ERROR:.*) ON LINE: ([^\s]*).*FILE: (.*)";

                            Regex rline = new Regex(linepat, RegexOptions.IgnoreCase);
                            // Match the regular expression pattern against a text string.
                            Match linematches = rline.Match(line);
                            if (linematches.Success)
                            {
                                System.Console.WriteLine(string.Format("{0} {1} {2}", linematches.Groups[3].Captures[0].ToString(), linematches.Groups[2].Captures[0].ToString(), linematches.Groups[1].Captures[0].ToString()));
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Build compiled successfully without error");
                    }
                }

                
            }
        }
    }
}
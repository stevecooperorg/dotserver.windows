namespace dotserver.windows
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class DotCompiler
    {
        public DotCompiler(string compilerPath)
        {
            this.CompilerPath = compilerPath;
        }

        public string CompilerPath
        {
            get; set;
        }

        public void Compile(string path)
        {
            // "C:\Users\steve.cooper\Desktop\work\imports\ImportDependencies.dot" -Tgif -o "C:\Users\steve.cooper\Desktop\work\imports\ImportDependencies.gif"
            var gifPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".gif");
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = string.Format("{0} -Tgif -o {1}", path, gifPath),
                FileName = this.CompilerPath
            };
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = processInfo;

            process.Start();
            process.WaitForExit();
            Console.WriteLine("finished " + process.ExitCode);
        }
    }
}
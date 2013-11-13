using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotserver.windows
{
    class Program
    {

        static int Main(string[] args)
        {
            var options = new Options();
            if (!TryParseOptions(args, out options))
            {
                Console.WriteLine("Expected usage: dotserver.windows <directoryToMonitor>");
                return -1;
            }
                        
            // now set up a watcher;
            Console.WriteLine("Watching " + options.Directory + " for " + options.Filter);

            var watcherFactory = new FileSystemObservableFactory();
            var allFileChanges = watcherFactory.SequenceOfFileSystemEvents(options, throttleSpeed: 200);

            allFileChanges.Subscribe(s =>
            {
                var file = Path.GetFileName(s);
                Console.WriteLine("{0}: {1}", DateTime.Now.ToShortTimeString(), file);
                new DotCompiler(options.DotCompilerPath).Compile(s);
            });

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();          

            return 0;
        }



        static bool TryParseOptions(string[] args, out Options options)
        {
            if (args.Length != 1)
            {
                options = null;
                return false;
            }

            var directory = args[0];

            if (!Directory.Exists(directory))
            {
                options = null;
                return false;
            }

            options = new Options 
            {
                Directory = directory,
                Extension = "dot",
                DotCompilerPath = @"C:\Program Files (x86)\Graphviz2.34\bin\dot.exe"
            };
            return true;
        }
    }
    
    internal class DotCompiler
    {
        public string CompilerPath { get; set; }

        public DotCompiler(string compilerPath)
        {
            this.CompilerPath = compilerPath;
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

    internal class FileSystemObservableFactory
    {
        public IObservable<string> SequenceOfFileSystemEvents(Options options, long throttleSpeed)
        {
            var watcher = new FileSystemWatcher(options.Directory);
            watcher.Filter = options.Filter;
            watcher.NotifyFilter =
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.FileName |
                NotifyFilters.DirectoryName;

            var createdFiles = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                h => watcher.Created += h,
                h => watcher.Created -= h)
                .Select(x => x.EventArgs.FullPath);

            var changedFiles = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                h => watcher.Changed += h,
                h => watcher.Changed -= h)
                .Select(x => x.EventArgs.FullPath);

            var deletedFiles = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                h => watcher.Deleted += h,
                h => watcher.Deleted -= h)
                .Select(x => x.EventArgs.FullPath);

            var renamedFiles = Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
                h => watcher.Renamed += h,
                h => watcher.Renamed -= h)
                .Select(x => x.EventArgs.FullPath);

            var allFileChanges = createdFiles
                .Merge(changedFiles)
                .Merge(deletedFiles)
                .Merge(renamedFiles)
                .Throttle(TimeSpan.FromMilliseconds(throttleSpeed));

            watcher.EnableRaisingEvents = true;

            return allFileChanges;
        }
    }

    internal class Options
    {
        public string Directory { get; set; }
        public string Extension { get; set; }
        public string DotCompilerPath { get; set; }
        public string Filter
        {
            get
            {
                return "*." + this.Extension;
            }
        }
    }
}

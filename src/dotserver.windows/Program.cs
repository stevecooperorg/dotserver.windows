namespace dotserver.windows
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static int Main(string[] args)
        {
            var optionParser = new OptionParser();
            var options = new Options();

            if (!optionParser.TryParseOptions(args, out options))
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
    }
}
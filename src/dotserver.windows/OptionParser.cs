namespace dotserver.windows
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class OptionParser
    {
        public bool TryParseOptions(string[] args, out Options options)
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
                Extension = ConfigurationManager.AppSettings["dotserver.dotextension"],
                DotCompilerPath = ConfigurationManager.AppSettings["dotserver.dotcompilerpath"]
            };
            return true;
        }
    }
}
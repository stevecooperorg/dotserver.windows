namespace dotserver.windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Options
    {
        public string Directory
        {
            get; set;
        }

        public string DotCompilerPath
        {
            get; set;
        }

        public string Extension
        {
            get; set;
        }

        public string Filter
        {
            get
            {
                return "*." + this.Extension;
            }
        }
    }
}
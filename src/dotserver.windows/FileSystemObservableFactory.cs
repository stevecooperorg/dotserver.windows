namespace dotserver.windows
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
}
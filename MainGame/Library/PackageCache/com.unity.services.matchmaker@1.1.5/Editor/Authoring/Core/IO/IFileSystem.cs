using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Services.Matchmaker.Authoring.Core.IO
{
    interface IFileSystem
    {
        public string GetFileName(string path);
        public List<string> GetFiles(string path, string searchPattern, bool recursive);
        public Task<string> ReadAllText(string path, CancellationToken token);
        public Task WriteAllText(string path, string contents, CancellationToken token);
        public void Delete(string path);
    }
}

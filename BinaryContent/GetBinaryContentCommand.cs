using System.IO;
using System.Linq;
using System.Management.Automation;

namespace io.Fallon.BinaryWriter
{
    [Cmdlet(VerbsCommon.Get, "BinaryContent")]
    public class GetBinaryContentCommand: PSCmdlet
    {

        [Parameter(Mandatory = true)]
        public string FilePath;
        public int BufferSize = 512;

        private Stream _inputStream;
        protected override void BeginProcessing()
        {
            string fullFilePath = GetUnresolvedProviderPathFromPSPath(FilePath);
            var shouldCreate = (File.Exists(fullFilePath)) ? FileMode.Open : FileMode.OpenOrCreate;
            _inputStream = new FileStream(
                FilePath,
                mode: FileMode.Open,
                access: FileAccess.Read,
                share: FileShare.None,
                bufferSize: BufferSize);
        }

        protected override void ProcessRecord()
        {
            var readBytes = new byte[BufferSize];
            // then copy the remainder of the segments
            int byteCount = _inputStream.Read(readBytes, 0, BufferSize);

            do
            {
                WriteObject(byteCount == BufferSize ? readBytes : readBytes.Take(byteCount).ToArray());
                byteCount = _inputStream.Read(readBytes, 0, BufferSize);
            } while (byteCount > 0);
        }

        protected override void StopProcessing()
        {
            _inputStream.Close();
        }

        protected override void EndProcessing()
        {
            _inputStream.Close();
        }
    }
}
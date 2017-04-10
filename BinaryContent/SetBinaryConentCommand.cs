using System.IO;
using System.Management.Automation;

namespace BinaryWriter
{
    [Cmdlet(VerbsCommon.Set, "BinaryContent")]
    public class SetBinaryConentCommand: PSCmdlet
    {
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ParameterSetName = nameof(InputBytes))]
        public byte[] InputBytes;
        [Parameter(Mandatory = true,
            ParameterSetName = nameof(InputBytes))]
        public string FilePath;
        [Parameter(ParameterSetName = nameof(InputBytes))]
        public int BufferSize = 512;

        private Stream _outputStream;
        protected override void BeginProcessing()
        {
            string fullFilePath = GetUnresolvedProviderPathFromPSPath(FilePath);
            var shouldCreate = (File.Exists(fullFilePath)) ? FileMode.Open : FileMode.OpenOrCreate;
            _outputStream = new FileStream(
                FilePath,
                mode: FileMode.Open,
                access: FileAccess.Write,
                share: FileShare.None,
                bufferSize: BufferSize);
        }

        protected override void ProcessRecord()
        {
            int curByte = 0;
            int preBytes = InputBytes.Length - curByte;
            // copy any segments that are irregularly sized
            _outputStream.Write(InputBytes, curByte, preBytes);
            // then copy the remainder of the segments
            for (curByte = preBytes; 
                curByte < InputBytes.Length; 
                curByte+=BufferSize)
            {
            _outputStream.Write(InputBytes, curByte, BufferSize);
            }
        }

        protected override void StopProcessing()
        {
            _outputStream.Close();
        }

        protected override void EndProcessing()
        {
            _outputStream.Close();
        }
    }
}

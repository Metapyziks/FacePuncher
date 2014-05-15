using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FacePuncher.Network
{
    public abstract class Connection : IDisposable
    {
        private readonly TcpClient _socket;
        protected readonly NetworkStream _stream;
        public Connection(TcpClient socket)
        {
            _socket = socket;
            _stream = socket.GetStream();
        }

        public async void Run()
        {
            //try
            //{
                var stream = _socket.GetStream();
                while (true)
                {
                    await ReadPacket();
                }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Error while handling message:{0}{1}{0}Disconnecting.", Environment.NewLine, ex);
            //    _socket.Close();
            //    Console.ReadLine();
            //    throw;
            //}
        }

        private async Task ReadPacket()
        {
            var packetId = await _stream.ReadByteAsync();
            if (packetId == 0)
            {
                // unsolicited packet
                await HandlePushedPacket();
            }
            else
            {
                // answer packet
                await HandleAnswer(packetId);
            }
        }

        protected abstract Task HandlePushedPacket();

        // just a tiny bit ugly
        private readonly TaskCompletionSource<TaskCompletionSource<bool>>[] _answerHandles = new TaskCompletionSource<TaskCompletionSource<bool>>[byte.MaxValue];
        private async Task HandleAnswer(byte packetId)
        {
            var answerProcessingHandle = new TaskCompletionSource<bool>();
            _answerHandles[packetId].SetResult(answerProcessingHandle);
            await answerProcessingHandle.Task;
        }
        protected async Task<TaskCompletionSource<bool>> WaitForAnswer()
        {
            for (int i = 0; i < _answerHandles.Length; i++)
            {
                if (_answerHandles[i] == null)
                {
                    _answerHandles[i] = new TaskCompletionSource<TaskCompletionSource<bool>>();
                    return await _answerHandles[i].Task;
                }
            }
            // No free slots
            await Task.WhenAny(from ah in _answerHandles select ah.Task);
            return await WaitForAnswer();
        }

        public void Dispose()
        {
            _socket.Close();
        }
    }
}
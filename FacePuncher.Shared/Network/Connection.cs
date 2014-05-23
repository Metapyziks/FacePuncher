/* Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
using System.Net.Sockets;
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
                    await Task.Yield();
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
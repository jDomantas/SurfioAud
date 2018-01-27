using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfioAud
{
    class Microphone
    {
        private readonly WaveIn _input;
        private readonly float[] _buffer;
        private int _readPosition;
        private int _writePosition;
        private bool _startedWriting;

        public Microphone()
        {
            _input = new WaveIn
            {
                WaveFormat = new WaveFormat(44100, 1)
            };
            _input.DataAvailable += DataAvailable;
            _input.StartRecording();

            _buffer = new float[44100];
            _startedWriting = false;
        }

        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            if (!_startedWriting)
            {
                _writePosition = 4410;
                _startedWriting = true;
            }

            if (e.BytesRecorded % 2 != 0)
                throw new Exception("got odd number of bytes");

            for (int i = 0; i < e.BytesRecorded / 2; i++)
            {
                int low = e.Buffer[i * 2];
                int high = e.Buffer[i * 2 + 1];
                int val = low | (high << 8);
                _buffer[_writePosition++] = val / 32768f;
                _writePosition %= _buffer.Length;
            }
        }

        public float GetTickValue()
        {
            if (!_startedWriting) return 0;

            float total = 0;
            
            for (int i = 0; i < 735; i++)
            {
                total += Math.Abs(_buffer[_readPosition++]);
                _readPosition %= _buffer.Length;
            }

            return total;
        }
    }
}

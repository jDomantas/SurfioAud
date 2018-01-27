using NAudio.Wave;
using System;

namespace SurfioAud
{
    class Microphone
    {
        private readonly float[] _buffer;
        private int _readPosition;
        private int _writePosition;
        private bool _startedWriting;

        public Microphone()
        {
            var input = new WaveIn
            {
                WaveFormat = new WaveFormat(44100, 1)
            };
            input.DataAvailable += DataAvailable;
            input.StartRecording();

            _buffer = new float[44100];
            _startedWriting = false;
        }

        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            if (!_startedWriting)
            {
                _writePosition = 2205;
                _startedWriting = true;
            }

            if (e.BytesRecorded % 2 != 0)
                throw new Exception("got odd number of bytes");

            for (int i = 0; i < e.BytesRecorded / 2; i++)
            {
                int low = e.Buffer[i * 2];
                int high = e.Buffer[i * 2 + 1];
                int val = low | (high << 8);
                _buffer[_writePosition++] = (short) val / 32768f;
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

            return total / 50;
        }
    }
}

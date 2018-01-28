using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NAudio.Wave;
using System;

namespace SurfioAud
{
    class Microphone
    {
        private static Microphone _aHorribleHack;

        private readonly float[] _buffer;
        private int _readPosition;
        private int _writePosition;
        private bool _startedWriting;
        private readonly float[] _infoRead;
        private int _infoNextPos;

        public Microphone()
        {
            _aHorribleHack = this;

            var input = new WaveIn
            {
                WaveFormat = new WaveFormat(44100, 1)
            };
            input.DataAvailable += DataAvailable;
            input.StartRecording();

            _buffer = new float[44100];
            _startedWriting = false;
            _infoRead = new float[60 * 5];
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

            total /= 25;
            _infoRead[_infoNextPos++] = total;
            _infoNextPos %= _infoRead.Length;
            return total;
        }

        public static void DrawDebugInfo(SpriteBatch sb)
        {
            if (_aHorribleHack == null)
            {
                return;
            }

            for (int i = 0; i < _aHorribleHack._infoRead.Length; i++)
            {
                float value = _aHorribleHack._infoRead[(_aHorribleHack._infoNextPos + i) % _aHorribleHack._infoRead.Length];
                sb.Draw(Resources.Pixel, new Rectangle(i * 2, 0, 2, (int)(value * 100)), Color.White);
            }
        }
    }
}

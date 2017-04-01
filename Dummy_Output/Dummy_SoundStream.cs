/* 
   The MIT License (MIT)
Copyright (c) 2013 Philipp Schröck <philsch@hotmail.de>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions 
of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */
using PluginSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dummy_Output
{
    public class Dummy_SoundStream : ISoundStream
    {
        StreamStatus m_status = StreamStatus.Created;

        public StreamStatus Status
        {
            get { return m_status; }
        }

        public bool IsSupport(string path)
        {
            Console.WriteLine("[Dummy] Stream is supported: ");
            return true;
        }

        public bool Create(ISoundSystem AudioSystem)
        {
            Console.WriteLine("[Dummy] Stream is created: ");
            return true;
        }

        public void Destroy()
        {
            Console.WriteLine("[Dummy] Stream is destroyed: ");
        }

        public void Set3DMinMaxDistance(float min, float max)
        {
            Console.WriteLine(string.Format("[Dummy] Set3DMinMaxDistance: {0} {1} ",min,max));
        }

        public bool Set3DSettings(Vector3 pos, Vector3 vel)
        {
            return true;
        }

        public int GetPosition()
        {
            return 0;
        }

        public bool SetPosition(uint pos)
        {
            return true;
        }

        public int GetLenght()
        {
            return 100;
        }

        public bool SetChannelMix(float fleft, float fright, float rleft, float rright, float sleft, float sright, float center, float bass)
        {
            Console.WriteLine("[Dummy] Stream SetChannelMix ");
            return true;
        }

        public void SetVolume(float vol)
        {
            Console.WriteLine("[Dummy] Stream set volume ");
        }

        public bool LoadStream(string FileName, bool b3D = false)
        {
            Console.WriteLine("[Dummy] Stream Loaded " + FileName);
            m_status = StreamStatus.Loaded;
            return true;
        }

        public bool LoadStream(System.IO.Stream stream, bool b3D = false)
        {
            return true;
        }

        public bool CloseStream()
        {
            Console.WriteLine("[Dummy] Stream closed ");
            m_status = StreamStatus.Closed;
            return true;
        }

        public bool PlayStream(Vector3 Position, float minDistance = 1.0f, float maxDistance = 100.0f)
        {
            Console.WriteLine("[Dummy] Stream played ");
            m_status = StreamStatus.Playing;
            return true;
        }

        public bool PauseStream()
        {
            m_status = m_status == StreamStatus.Paused ? StreamStatus.Playing : StreamStatus.Playing;
            Console.WriteLine("[Dummy] Stream Paused ");
            return true;
        }

        public bool StopStream()
        {
            Console.WriteLine("[Dummy] Stream stopped ");
            m_status = StreamStatus.Stopped;
            return true;
        }

        public IntPtr GetTag(string key)
        {
            return IntPtr.Zero;
        }
    }
}

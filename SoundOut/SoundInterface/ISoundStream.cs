/*The MIT License (MIT)
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
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.*/
using System;

namespace PluginSystem
{
    public enum StreamStatus
    {
        Created,
        Loaded,
        Closed,
        Playing,
        Paused,
        Stopped
    }
    public class Vector3
    {
        public float x,y,z;

        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public  Vector3(float xyz) { x = xyz; y = xyz; z = xyz; }
    }
    public interface ISoundStream
    {
        StreamStatus Status { get; }

        bool IsSupport(string path);

        bool Create(ISoundSystem AudioSystem);
		void Destroy();

		void Set3DMinMaxDistance(float min, float max);
        bool Set3DSettings(Vector3 pos, Vector3 vel);

		int	 GetPosition();
		bool SetPosition(uint pos);
		int  GetLenght();

		bool SetChannelMix(float fleft, float fright, float rleft, float rright,
			float sleft, float sright, float center, float bass);

		void SetVolume(float vol);

		bool LoadStream(string FileName, bool b3D = false);
        bool LoadStream(System.IO.Stream stream, bool b3D = false);

		bool CloseStream();
		bool PlayStream(Vector3 Position, float minDistance = 1.0f,
			float maxDistance = 100.0f);
		bool PauseStream();
		bool StopStream();

        IntPtr GetTag(string key);
        
    }
}

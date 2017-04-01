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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSystem;

namespace Dummy_Output
{
    
    public class Output : IOutPlugin
    {
        private Dummy_SoundSystem m_System = new Dummy_SoundSystem();

        public PluginType Type
        {
            get { return PluginType.SoundOutPlugin; }
        }

        public Vector3 ListenerPosition
        {
            get;
            set;
        }
        public ISoundSystem System
        {
            get { return m_System; }
        }

        public IPluginHost Host
        {
            get;
            set;
        }

        public string Name
        {
            get { return "Dummy Output"; }
        }

        public string Autor
        {
            get { return "Philipp Schröck"; }
        }

        public string Version
        {
            get { return "0.0"; }
        }

        public bool Create(SoundSystemConfig config)
        {
            return true;
        }

        public void Destroy()
        {
       
        }

        public void Update(float updatetime)
        {
            m_System.Update(updatetime);
        }

        public ISoundStream CreateStream(string path, bool b3D)
        {
            Dummy_SoundStream stream = new Dummy_SoundStream();
            stream.IsSupport(path);
            stream.Create(m_System);
            stream.LoadStream(path, b3D);

            return stream;
        }

        public ISoundStream CreateStream(System.IO.Stream stream, bool b3D)
        {
            Dummy_SoundStream _stream = new Dummy_SoundStream();

            _stream.Create(m_System);
            _stream.LoadStream(stream, b3D);

            return _stream;
        }

        
    }
}

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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PluginSystem;
using PluginSystem.VisualRenderer;
using System;
using System.Windows;

namespace Classic_Visual
{
    public class Visual : IVisualRenderer
    {
        private string m_name;
        private string m_autor;
        private string m_version;
        private PluginSystem.IPluginHost m_host;

        protected System.Windows.Size m_size;
        protected Texture2D m_blank;
        protected SpriteBatch m_batch;

        protected float[] ChannelOneData, ChannelTwoData;
        protected ISoundSystem m_soundSystem;

        public Visual(string name, string autor, string version)
        {
            m_name = name;
            m_autor = autor;
            m_version = version;
        }

        public PluginSystem.PluginType Type
        {
            get { return PluginSystem.PluginType.VisualRendererPlugin; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public string Autor
        {
            get { return m_autor; }
        }

        public string Version
        {
            get { return m_version; }
        }

        public PluginSystem.IPluginHost Host
        {
            get { return m_host; }
            set { m_host = value; }
        }

        public virtual void Start(Size size, ISoundSystem system)
        {
            m_size = size;
            m_soundSystem = system;
        }

        public virtual void Stop()
        {
        }

        public virtual void LoadContent(GraphicsDevice device)
        {
            m_blank = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
            m_blank.SetData(new[] { Color.White });

            m_batch = new SpriteBatch(device);
        }

        public virtual void Update(GraphicsDevice device)
        {
            if (m_soundSystem != null)
            {
                ChannelOneData = m_soundSystem.getWaveData((int)m_size.Width, 0);
                ChannelTwoData = m_soundSystem.getWaveData((int)m_size.Width, 1);
            }
            else
            {
                ChannelOneData = new float[(int)m_size.Width];
                ChannelTwoData = new float[(int)m_size.Width];
            }
        }

        public virtual void Draw(GraphicsDevice device)
        {
        }

        public virtual void SizeChanged(Size size)
        {
            m_size = size;
        }
        protected void DrawLine(float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            m_batch.Draw(m_blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }
    }
}

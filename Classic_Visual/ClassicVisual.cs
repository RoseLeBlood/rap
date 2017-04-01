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


using Classic_Visual.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Classic_Visual
{
    public class ClassicVisual : Visual
    {
        public ClassicVisual()
            : base("Glow bulb", "Philipp Schröck", "1.1")
        {
        }

        public override void Draw(GraphicsDevice device)
        {
            device.Clear(Color.Black);

            m_batch.Begin();

            for (int x = 0; x < Math.Min(ChannelOneData.Length, ChannelTwoData.Length) - 1; x++)
            {
                m_batch.DrawLine(new Vector2(x + 1, (ChannelOneData[x] + 1) / 2.0f * (float)m_size.Height),
                                 new Vector2(x + 2, (ChannelOneData[x + 1] + 1) / 2.0f * (float)m_size.Height),
                                 new Color(1.0f, 0.0f, 0.0f),
                                 2.0f);


                m_batch.DrawLine(new Vector2(x + 1, ((-ChannelTwoData[x]) + 1) / 2.0f * (float)m_size.Height),
                                 new Vector2(x + 2, ((-ChannelTwoData[x + 1]) + 1) / 2.0f * (float)m_size.Height),
                                 new Color(1.0f, 1.0f, 0.0f),
                                 2.0f);
            }
            m_batch.End();
        }
    }
}

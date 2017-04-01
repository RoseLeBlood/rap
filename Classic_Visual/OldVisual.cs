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

namespace Classic_Visual
{
    public class OldVisual : Visual
    {

        public OldVisual()
            : base("Old Waveform", "Philipp Schröck", "1.1")
        {
            
        }
        public override void LoadContent(GraphicsDevice device)
        {
            base.LoadContent(device);

            
        }
        public override void Update(GraphicsDevice device)
        {
            base.Update(device);

        }
        public override void Draw(GraphicsDevice device)
        {
            device.Clear(Color.Black);

            Render(ChannelOneData, +40, 1, Color.Blue);
            Render(ChannelTwoData, -40, 2, Color.Red);
            

        }
        private void Render( float[] channel, int b,int c, Color color)
        {
            m_batch.Begin();

            for (int x = 1; x < channel.Length - 1; x+= 2)
            {
                    m_batch.DrawLine(new Vector2(x+c, ((channel[x] + 1) / 2.0f  * (float)m_size.Height) + b),
                                     new Vector2(x+c, ((-channel[x] + 1) / 2.0f * (float)m_size.Height) + b),
                                     color,
                                     1.0f);
            }
            m_batch.End();
        }
    }
}

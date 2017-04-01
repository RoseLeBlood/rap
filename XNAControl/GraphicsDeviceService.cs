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
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControl
{
    class GraphicsDeviceService : IGraphicsDeviceService
    {
        private static readonly GraphicsDeviceService singletonInstance = new GraphicsDeviceService();
        private static int referenceCount;

        private GraphicsDevice graphicsDevice;
        private PresentationParameters parameters;


        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }

        private GraphicsDeviceService() 
        { 
        }
        private void CreateDevice(IntPtr windowHandle, int width, int height)
        {
            parameters = new PresentationParameters();

            parameters.BackBufferWidth = Math.Max(width, 1);
            parameters.BackBufferHeight = Math.Max(height, 1);
            parameters.BackBufferFormat = SurfaceFormat.Alpha8;
            parameters.DepthStencilFormat = DepthFormat.Depth24Stencil8;
            parameters.DeviceWindowHandle = windowHandle;
            parameters.MultiSampleCount = 2;
            parameters.PresentationInterval = PresentInterval.Immediate;
            parameters.IsFullScreen = false;

            graphicsDevice = new GraphicsDevice(
                GraphicsAdapter.DefaultAdapter,
                GraphicsProfile.HiDef,
                parameters);

            if (DeviceCreated != null)
                DeviceCreated(this, EventArgs.Empty);
        }
        public static GraphicsDeviceService AddRef(IntPtr windowHandle, int width, int height)
        {
            if (Interlocked.Increment(ref referenceCount) == 1)
            {
                singletonInstance.CreateDevice(windowHandle, width, height);
            }
            return singletonInstance;
        }
        public void Release(bool disposing)
        {
            if (Interlocked.Decrement(ref referenceCount) == 0)
            {
                if (disposing)
                {
                    if (DeviceDisposing != null)
                        DeviceDisposing(this, EventArgs.Empty);
                    graphicsDevice.Dispose();
                }
                graphicsDevice = null;
            }
        }
        public void ResetDevice(int width, int height)
        {
            if (DeviceResetting != null)
                DeviceResetting(this, EventArgs.Empty);

            parameters.BackBufferWidth = Math.Max(parameters.BackBufferWidth, width);
            parameters.BackBufferHeight = Math.Max(parameters.BackBufferHeight, height);

            graphicsDevice.Reset(parameters);

            if (DeviceReset != null)
                DeviceReset(this, EventArgs.Empty);
        }
    }
}

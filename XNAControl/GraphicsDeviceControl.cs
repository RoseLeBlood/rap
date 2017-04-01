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
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace XNAControl
{
    public class GraphicsDeviceControl : HwndHost
    {
        private const string m_windowClass = "{19E000B0-B76A-4008-8044-82D90E027F58}";
        private IntPtr m_hWnd;
        private GraphicsDeviceService m_graphicsService;

        public GraphicsDevice Device
        {
            get { return m_graphicsService.GraphicsDevice; }
        }

        private bool m_applicationHasFocus = false;
        private bool m_mouseInWindow = false;
        private MouseState m_mouseState = new MouseState();
        private bool m_isMouseCaptured = false;
        private int m_capturedMouseX;
        private int m_capturedMouseY;
        private int m_capturedMouseClientX;
        private int m_capturedMouseClientY;

        public event EventHandler<GraphicsDeviceEventArgs> LoadContent;
        public event EventHandler<GraphicsDeviceEventArgs> RenderXna;
        public event EventHandler<MouseEventArgs> LeftButtonDown;
        public event EventHandler<MouseEventArgs> LeftButtonUp;
        public event EventHandler<MouseEventArgs> LeftButtonDblClick;
        public event EventHandler<MouseEventArgs> RightButtonDown;
        public event EventHandler<MouseEventArgs> RightButtonUp;
        public event EventHandler<MouseEventArgs> RightButtonDblClick;
        public event EventHandler<MouseEventArgs> MiddleButtonDown;
        public event EventHandler<MouseEventArgs> MiddleButtonUp;
        public event EventHandler<MouseEventArgs> MiddleButtonDblClick;
        public event EventHandler<MouseEventArgs> X1ButtonDown;
        public event EventHandler<MouseEventArgs> X1ButtonUp;
        public event EventHandler<MouseEventArgs> X1ButtonDblClick;
        public event EventHandler<MouseEventArgs> X2ButtonDown;
        public event EventHandler<MouseEventArgs> X2ButtonUp;
        public event EventHandler<MouseEventArgs> X2ButtonDblClick;
        public event EventHandler<MouseEventArgs> XNAMouseMove;
        public event EventHandler<MouseEventArgs> XNAMouseEnter;
        public event EventHandler<MouseEventArgs> XNAMouseLeave;

        public GraphicsDeviceControl()
        {
            Loaded += new System.Windows.RoutedEventHandler(XnaWindowHost_Loaded);
            SizeChanged += new SizeChangedEventHandler(XnaWindowHost_SizeChanged);
            Application.Current.Activated += new EventHandler(Current_Activated);
            Application.Current.Deactivated += new EventHandler(Current_Deactivated);

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }
        public new void CaptureMouse()
        {
            if (m_isMouseCaptured)
                return;

            NativeMethods.ShowCursor(false);
            m_isMouseCaptured = true;

            NativeMethods.POINT p = new NativeMethods.POINT();
            NativeMethods.GetCursorPos(ref p);
            m_capturedMouseX = p.X;
            m_capturedMouseY = p.Y;

            NativeMethods.ScreenToClient(m_hWnd, ref p);
            m_capturedMouseClientX = p.X;
            m_capturedMouseClientY = p.Y;
        }

        public new void ReleaseMouseCapture()
        {
            if (!m_isMouseCaptured)
                return;

            NativeMethods.ShowCursor(true);
            m_isMouseCaptured = false;
        }
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (m_isMouseCaptured &&
                (int)m_mouseState.Position.X != m_capturedMouseX &&
                (int)m_mouseState.Position.Y != m_capturedMouseY)
            {
                NativeMethods.SetCursorPos(m_capturedMouseX, m_capturedMouseY);

                m_mouseState.Position = m_mouseState.PreviousPosition = new Point(m_capturedMouseClientX, m_capturedMouseClientY);
            }

            if (m_graphicsService == null)
                return;

            int width = (int)ActualWidth;
            int height = (int)ActualHeight;

            if (width < 1 || height < 1)
                return;

            Viewport viewport = new Viewport(0, 0, width, height);
            m_graphicsService.GraphicsDevice.Viewport = viewport;

            if (RenderXna != null)
                RenderXna(this, new GraphicsDeviceEventArgs(m_graphicsService.GraphicsDevice));

            m_graphicsService.GraphicsDevice.Present(viewport.Bounds, null, m_hWnd);
        }

        void XnaWindowHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_graphicsService == null)
            {
                m_graphicsService = GraphicsDeviceService.AddRef(m_hWnd, (int)ActualWidth, (int)ActualHeight);

                if (LoadContent != null)
                    LoadContent(this, new GraphicsDeviceEventArgs(m_graphicsService.GraphicsDevice));
            }
        }

        void XnaWindowHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_graphicsService != null)
                m_graphicsService.ResetDevice((int)ActualWidth, (int)ActualHeight);
        }

        void Current_Activated(object sender, EventArgs e)
        {
            m_applicationHasFocus = true;
        }

        void Current_Deactivated(object sender, EventArgs e)
        {
            m_applicationHasFocus = false;
            ResetMouseState();

            if (m_mouseInWindow)
            {
                m_mouseInWindow = false;
                if (XNAMouseLeave != null)
                    XNAMouseLeave(this, new MouseEventArgs(m_mouseState));
            }

            ReleaseMouseCapture();
        }

        private void ResetMouseState()
        {
            bool fireL = m_mouseState.LeftButton == MouseButtonState.Pressed;
            bool fireM = m_mouseState.MiddleButton == MouseButtonState.Pressed;
            bool fireR = m_mouseState.RightButton == MouseButtonState.Pressed;
            bool fireX1 = m_mouseState.X1Button == MouseButtonState.Pressed;
            bool fireX2 = m_mouseState.X2Button == MouseButtonState.Pressed;

            m_mouseState.LeftButton = MouseButtonState.Released;
            m_mouseState.MiddleButton = MouseButtonState.Released;
            m_mouseState.RightButton = MouseButtonState.Released;
            m_mouseState.X1Button = MouseButtonState.Released;
            m_mouseState.X2Button = MouseButtonState.Released;

            MouseEventArgs args = new MouseEventArgs(m_mouseState);
            if (fireL && LeftButtonUp != null)
                LeftButtonUp(this, args);
            if (fireM && MiddleButtonUp != null)
                MiddleButtonUp(this, args);
            if (fireR && RightButtonUp != null)
                RightButtonUp(this, args);
            if (fireX1 && X1ButtonUp != null)
                X1ButtonUp(this, args);
            if (fireX2 && X2ButtonUp != null)
                X2ButtonUp(this, args);

            m_mouseInWindow = false;
        }
        protected override void Dispose(bool disposing)
        {
            if (m_graphicsService != null)
            {
                m_graphicsService.Release(disposing);
                m_graphicsService = null;
            }
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            base.Dispose(disposing);
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            m_hWnd = CreateHostWindow(hwndParent.Handle);
            return new HandleRef(this, m_hWnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd.Handle);
            m_hWnd = IntPtr.Zero;
        }
        private IntPtr CreateHostWindow(IntPtr hWndParent)
        {
            RegisterWindowClass();

            return NativeMethods.CreateWindowEx(0, m_windowClass, "",
               NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE,
               0, 0, (int)Width, (int)Height, hWndParent, IntPtr.Zero, IntPtr.Zero, 0);
        }
        private void RegisterWindowClass()
        {
            NativeMethods.WNDCLASSEX wndClass = new NativeMethods.WNDCLASSEX();
            wndClass.cbSize = (uint)Marshal.SizeOf(wndClass);
            wndClass.hInstance = NativeMethods.GetModuleHandle(null);
            wndClass.lpfnWndProc = NativeMethods.DefaultWindowProc;
            wndClass.lpszClassName = m_windowClass;
            wndClass.hCursor = NativeMethods.LoadCursor(IntPtr.Zero, NativeMethods.IDC_ARROW);

            NativeMethods.RegisterClassEx(ref wndClass);
        }
        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_LBUTTONDOWN:
                    m_mouseState.LeftButton = MouseButtonState.Pressed;
                    if (LeftButtonDown != null)
                        LeftButtonDown(this, new MouseEventArgs(m_mouseState));
                    break;
                case NativeMethods.WM_LBUTTONUP:
                    m_mouseState.LeftButton = MouseButtonState.Released;
                    if (LeftButtonUp != null)
                        LeftButtonUp(this, new MouseEventArgs(m_mouseState));
                    break;
                case NativeMethods.WM_LBUTTONDBLCLK:
                    if (LeftButtonDblClick != null)
                        LeftButtonDblClick(this, new MouseEventArgs(m_mouseState, MouseButton.Left));
                    break;
                case NativeMethods.WM_RBUTTONDOWN:
                    m_mouseState.RightButton = MouseButtonState.Pressed;
                    if (RightButtonDown != null)
                        RightButtonDown(this, new MouseEventArgs(m_mouseState));
                    break;
                case NativeMethods.WM_RBUTTONUP:
                    m_mouseState.RightButton = MouseButtonState.Released;
                    if (RightButtonUp != null)
                        RightButtonUp(this, new MouseEventArgs(m_mouseState));
                    break;
                case NativeMethods.WM_RBUTTONDBLCLK:
                    if (RightButtonDblClick != null)
                        RightButtonDblClick(this, new MouseEventArgs(m_mouseState, MouseButton.Right));
                    break;
                case NativeMethods.WM_MBUTTONDOWN:
                    m_mouseState.MiddleButton = MouseButtonState.Pressed;
                    if (MiddleButtonDown != null)
                        MiddleButtonDown(this, new MouseEventArgs(m_mouseState));
                    break;
                case NativeMethods.WM_MBUTTONUP:
                    m_mouseState.MiddleButton = MouseButtonState.Released;
                    if (MiddleButtonUp != null)
                        MiddleButtonUp(this, new MouseEventArgs(m_mouseState));
                    break;
                case NativeMethods.WM_MBUTTONDBLCLK:
                    if (MiddleButtonDblClick != null)
                        MiddleButtonDblClick(this, new MouseEventArgs(m_mouseState, MouseButton.Middle));
                    break;
                case NativeMethods.WM_XBUTTONDOWN:
                    if (((int)wParam & NativeMethods.MK_XBUTTON1) != 0)
                    {
                        m_mouseState.X1Button = MouseButtonState.Pressed;
                        if (X1ButtonDown != null)
                            X1ButtonDown(this, new MouseEventArgs(m_mouseState));
                    }
                    else if (((int)wParam & NativeMethods.MK_XBUTTON2) != 0)
                    {
                        m_mouseState.X2Button = MouseButtonState.Pressed;
                        if (X2ButtonDown != null)
                            X2ButtonDown(this, new MouseEventArgs(m_mouseState));
                    }
                    break;
                case NativeMethods.WM_XBUTTONUP:
                    if (((int)wParam & NativeMethods.MK_XBUTTON1) != 0)
                    {
                        m_mouseState.X1Button = MouseButtonState.Released;
                        if (X1ButtonUp != null)
                            X1ButtonUp(this, new MouseEventArgs(m_mouseState));
                    }
                    else if (((int)wParam & NativeMethods.MK_XBUTTON2) != 0)
                    {
                        m_mouseState.X2Button = MouseButtonState.Released;
                        if (X2ButtonUp != null)
                            X2ButtonUp(this, new MouseEventArgs(m_mouseState));
                    }
                    break;
                case NativeMethods.WM_XBUTTONDBLCLK:
                    if (((int)wParam & NativeMethods.MK_XBUTTON1) != 0)
                    {
                        if (X1ButtonDblClick != null)
                            X1ButtonDblClick(this, new MouseEventArgs(m_mouseState, MouseButton.XButton1));
                    }
                    else if (((int)wParam & NativeMethods.MK_XBUTTON2) != 0)
                    {
                        if (X2ButtonDblClick != null)
                            X2ButtonDblClick(this, new MouseEventArgs(m_mouseState, MouseButton.XButton2));
                    }
                    break;
                case NativeMethods.WM_MOUSEMOVE:
                    if (!m_applicationHasFocus)
                        break;

                    m_mouseState.PreviousPosition = m_mouseState.Position;
                    m_mouseState.Position = new Point(
                        NativeMethods.GetXLParam((int)lParam),
                        NativeMethods.GetYLParam((int)lParam));

                    if (!m_mouseInWindow)
                    {
                        m_mouseInWindow = true;
                        m_mouseState.PreviousPosition = m_mouseState.Position;

                        if (XNAMouseEnter != null)
                            XNAMouseEnter(this, new MouseEventArgs(m_mouseState));

                        NativeMethods.TRACKMOUSEEVENT tme = new NativeMethods.TRACKMOUSEEVENT();
                        tme.cbSize = Marshal.SizeOf(typeof(NativeMethods.TRACKMOUSEEVENT));
                        tme.dwFlags = NativeMethods.TME_LEAVE;
                        tme.hWnd = hwnd;
                        NativeMethods.TrackMouseEvent(ref tme);
                    }
                    if (m_mouseState.Position != m_mouseState.PreviousPosition)
                    {
                        if (XNAMouseMove != null)
                            XNAMouseMove(this, new MouseEventArgs(m_mouseState));
                    }

                    break;
                case NativeMethods.WM_MOUSELEAVE:
                    if (m_isMouseCaptured)
                        break;

                    ResetMouseState();

                    if (XNAMouseLeave != null)
                        XNAMouseLeave(this, new MouseEventArgs(m_mouseState));
                    break;
                default:
                    break;
            }

            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }
    }
}

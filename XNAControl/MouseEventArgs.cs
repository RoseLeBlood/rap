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
using System.Windows.Input;
using System.Windows;

namespace XNAControl
{
    public class MouseEventArgs : EventArgs
    {
        MouseButtonState m_leftButton;
        MouseButtonState m_rightButton;
        MouseButtonState m_middleButton;
        MouseButtonState m_x1Button;
        MouseButtonState m_x2Button;
        MouseButton? m_doubleClicked;
        int m_wheelDelta;
        int m_horizontalWheelDelta;
        Point m_position;
        Point m_prevPosition;

        public MouseButtonState LeftButton 
        {
            get { return m_leftButton; } 
        }
        public MouseButtonState RightButton 
        {
            get { return m_rightButton; } 
        }
        public MouseButtonState MiddleButton 
        {
            get { return m_middleButton; } 
        }
        public MouseButtonState X1Button 
        {
            get { return m_x1Button; }  
        }
        public MouseButtonState X2Button 
        {
            get { return m_x2Button; } 
        }
        public MouseButton? DoubleClickButton 
        {
            get { return m_doubleClicked; } 
        }
        public int WheelDelta 
        {
            get { return m_wheelDelta; }
        }
        public int HorizontalWheelDelta 
        {
            get { return m_horizontalWheelDelta; }
        }

        public Point Position 
        {
            get { return m_position; } 
        }
        public Point PreviousPosition 
        {
            get { return m_prevPosition; }
        }

        public MouseEventArgs(MouseState state)
        {
            m_leftButton = state.LeftButton;
            m_rightButton = state.RightButton;
            m_middleButton = state.MiddleButton;
            m_x1Button = state.X1Button;
            m_x2Button = state.X2Button;
            m_position = state.Position;
            m_prevPosition = state.PreviousPosition;
        }
        public MouseEventArgs(MouseState state, int mouseWheelDelta, int mouseHWheelDelta)
            : this(state)
        {
            m_wheelDelta = mouseWheelDelta;
            m_horizontalWheelDelta = mouseHWheelDelta;
        }

        public MouseEventArgs(MouseState state, MouseButton doubleClickButton)
            : this(state)
        {
            m_doubleClicked = doubleClickButton;
        }
    }
}

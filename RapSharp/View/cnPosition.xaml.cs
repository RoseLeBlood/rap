/*abgewandelte MIT-Licens 
NON-COMMERCIAL LICENSE
Ramona Audio Player Copyright (c)  2013 Philipp Schröck <philsch@hotmail.de>

1.0  Hiermit wird unentgeltlich, jeder Person, die eine Kopie der Software und der zugehörigen 
Dokumentationen (die \"Software\") erhält, die Erlaubnis erteilt, sie uneingeschränkt zu benutzen, 
inklusive und ohne Ausnahme,  dem Recht, sie zu verwenden, kopieren, ändern und/oder verbreiten und Personen, 
die diese Software erhalten, diese Rechte zu geben, unter den folgenden Bedingungen:

Der obige Urheberrechtsvermerk, dieser Erlaubnisvermerk und der Quellcode sind in allen Kopien oder Teilkopien 
der Software beizulegen.

2.0 Die Kommerzielle Nutzung dieser Software oder Teile aus der Software ist ausdrücklich verboten.

3.0 DIE SOFTWARE WIRD OHNE JEDE AUSDRÜCKLICHE ODER IMPLIZIERTE GARANTIE BEREITGESTELLT, EINSCHLIESSLICH DER 
GARANTIE ZUR BENUTZUNG FÜR DEN VORGESEHENEN ODER EINEM BESTIMMTEN ZWECK SOWIE  
JEGLICHER RECHTSVERLETZUNG, JEDOCH NICHT DARAUF BESCHRÄNKT. IN KEINEM FALL SIND DIE AUTOREN ODER 
COPYRIGHTINHABER FÜR JEGLICHEN SCHADEN ODER SONSTIGE ANSPRÜCHE HAFTBAR ZU MACHEN, OB INFOLGE DER 
ERFÜLLUNG EINES VERTRAGES, EINES DELIKTES ODER ANDERS IM ZUSAMMENHANG MIT DER SOFTWARE ODER SONSTIGER 
VERWENDUNG DER SOFTWARE ENTSTANDEN.*/

using PluginSystem;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RapSharp
{
    /// <summary>
    /// Interaktionslogik für cnPosition.xaml
    /// </summary>
    public partial class cnPosition : UserControl
    {
        private Point clickPosition;
        private Vector3 m_listener = new Vector3(0);

        public event EventHandler<Vector3> ListenerPositionChanged;

        public bool IsMoving 
        { 
            get; 
            internal set; 
        }
        public Vector3 Listener
        {
            get { return m_listener; }
            set { m_listener = value; }
        }

        public cnPosition()
        {
            InitializeComponent();
            ResetPosition();
        }

        public void listener_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMoving = true;
            clickPosition = e.GetPosition(gridMain);

            var transform = listener.RenderTransform as TranslateTransform;

            if (transform != null)
            {
                clickPosition.X -= transform.X;
                clickPosition.Y -= transform.Y;
            }

            listener.CaptureMouse();
        }

        public void listener_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsMoving = false;
            listener.ReleaseMouseCapture();
        }

        private void listener_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMoving && listener != null)
            {
                SetPosition(e.GetPosition(gridMain), null, listener.RenderTransform as TranslateTransform);
            }
        }
        private void listener_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ResetPosition();
        }
        private void ResetPosition()
        {
            if (listener != null)
            {
                TranslateTransform transform = listener.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    listener.RenderTransform = transform;
                }
                transform.X = 0;
                transform.Y = 31.25f;

                Listener.x = -(float)transform.X * 0.08f; 
                Listener.z = -(float)transform.Y * 0.08f; // by Lefthand +Y | Righthand +Y

                lblInfos.Content = string.Format("Listener Position: {0} {1}", Listener.x.ToString("0.0"),
                    Listener.z.ToString("0.0"));

                if (ListenerPositionChanged != null)
                    ListenerPositionChanged(this, m_listener);
            }
        }
        private void SetPosition(Point currentPosition, Point? newPosition, TranslateTransform transform)
        {
            Point rel = (newPosition.HasValue ? newPosition.Value : new Point(currentPosition.X - clickPosition.X,
                                                                    currentPosition.Y - clickPosition.Y));


            if (transform == null)
            {
                transform = new TranslateTransform();
                listener.RenderTransform = transform;
            }

            if (currentPosition.X < (gridMain.ActualWidth - listener.Width / 2) &&
                currentPosition.X >= listener.Width / 2)
            {
                transform.X = rel.X;
            }
            if (currentPosition.Y < (gridMain.ActualHeight - listener.Height / 2) &&
                currentPosition.Y >= listener.Height / 2)
            {
                transform.Y = rel.Y;
            }

            // Transfomation auf RightHand Vector3
            Listener.x = -(float)transform.X * 0.08f; // by Lefthand -X | Righthand +X
            Listener.z = -(float)transform.Y * 0.08f; // by Lefthand +Y | Righthand +Y

            lblInfos.Content = string.Format("Listener Position: {0} {1}", Listener.x.ToString("0.0"),
                Listener.z.ToString("0.0"));

            if (ListenerPositionChanged != null)
                ListenerPositionChanged(this, m_listener);
        }

        private void listener_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEnabled)
            {
                this.Opacity = 1.0;
            }
            else
                this.Opacity = 0.5;
        }
    }
}

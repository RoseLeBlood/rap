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
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace RapSharp.logic
{
    public class Plalistitem : ListBoxItem
    {
        private static Brush GeradeBrush = new BrushConverter().ConvertFromString("#FF9D9D9D") as Brush;

        private PluginSystem.ISoundStream m_stream;
        private PluginSystem.IOutPlugin m_system;
        private PluginSystem.StreamStatus m_status;
        
        private DispatcherTimer m_timer = new DispatcherTimer();

        private string m_path;
        private string oldTitle, oldArtist, oldAlbum;
        private bool m_mustupdate = true;
        private bool m_isUngerade;


        public string Status 
        {
            get 
            {
                m_status = m_stream.Status;
                return Enum.GetName(typeof(PluginSystem.StreamStatus), m_status);
            }
        }
        public double TimeLenght
        {
            get
            {
                return m_stream.GetLenght();
            }
        }
        public double TimePosition
        {
            get { return m_stream.GetPosition(); }
            set
            {
                m_stream.SetPosition((uint)value);
            }
        }
        public bool IsWhite
        {
            get { return m_isUngerade; }
            set 
            { 
                m_isUngerade = value;
                this.Foreground = value ? GeradeBrush : Brushes.Gray;
            }
        }
        public double TimeLeft
        {
            get { return TimeLenght - TimeLeft; }
        }
        public Plalistitem()
        {
            
            this.Foreground = Brushes.Gray;
        }
        public Plalistitem(string anzeigeName)
        {
            this.Foreground = Brushes.Gray;
            m_mustupdate = anzeigeName == string.Empty;
            this.Content = anzeigeName;
        }

       
        public bool FirstLoad(string path, PluginSystem.IOutPlugin system)
        {
            m_path = path;
            if (!UpdateSystem(system))
            {
               MessageBox.Show(string.Format("Stream: {0} wird nicht unterstüzt", path),
                    "Ramona Audio Player", MessageBoxButton.OK);

                return false;
            }

            if (m_mustupdate)
            {
                if (path.Contains("http://"))
                {
                    this.Content = string.Format("Network: {0}", path);
                    oldTitle = string.Format("Network: {0}", path);
                }
                else
                {
                    IntPtr iartist = m_stream.GetTag("ARTIST");
                    IntPtr ialbum = m_stream.GetTag("ALBUM");
                    IntPtr ititle = m_stream.GetTag("TITLE");

                    string title = ititle != IntPtr.Zero ? Marshal.PtrToStringAnsi(ititle).Trim() : "Unknown title";
                    string artist = iartist != IntPtr.Zero ? Marshal.PtrToStringAnsi(iartist).Trim() : "Unknown artist";
                    string album = ialbum != IntPtr.Zero ? Marshal.PtrToStringAnsi(ialbum).Trim() : "Unknown album";

                    title = title != "" ? title : "Unknown title";
                    artist = artist != "" ? artist : "Unknown artist";
                    album = album != "" ? album : "Unknown album";

                    if (title == "Unknown title" && artist == "Unknown artist" && album == "Unknown album")
                    {
                        this.Content = Path.GetFileName(path);
                    }
                    else
                    {
                        oldAlbum = album != "" ? album : "Unknown album";
                        oldTitle = title != "" ? title : "Unknown title";
                        oldArtist = artist != "" ? artist : "Unknown artist";

                        this.Content = string.Format("{0} {1} {2}",
                            oldTitle,
                            oldArtist,
                            oldAlbum);
                    }
                }
            }

           return m_stream.CloseStream();

        }
        public bool PlayItem()
        {
            try
            {
                if (m_stream != null && m_stream.LoadStream(m_path, true) && m_stream.PlayStream(new PluginSystem.Vector3(0.0f, 0.0f, 1.0f)))
                {
                    this.Foreground = Brushes.DarkOrange;


                    m_timer.Tick += m_timer_Tick;
                    m_timer.Interval = new TimeSpan(0, 0, 5);
                    m_timer.Start();
                    return true;
                }
                
            }
            catch
            {
                this.Foreground = IsWhite ? GeradeBrush : Brushes.Gray;
                
            }
            return false;
        }

        

        void m_timer_Tick(object sender, EventArgs e)
        {
            IntPtr iartist = m_stream.GetTag("ARTIST");
            IntPtr ititle = m_stream.GetTag("TITLE");
            IntPtr ialbum = m_stream.GetTag("ALBUM");

            string title = ititle != IntPtr.Zero ? Marshal.PtrToStringAnsi(ititle).Trim() : "";
            string artist = iartist != IntPtr.Zero ? Marshal.PtrToStringAnsi(iartist).Trim() : "";
            string album = ialbum != IntPtr.Zero ? Marshal.PtrToStringAnsi(ialbum).Trim() : "";


            if (title != "" || artist != "")
            {
                this.Content = string.Format("{0} {1} {2}",
                    title != "" ? title : oldTitle,
                    artist != "" ? artist : oldArtist,
                    album != "" ? album : oldAlbum);

                oldTitle = title;
                oldArtist = artist;
                oldAlbum = album;
            }
        }
        public bool StopItem()
        {
            if (m_stream.StopStream())
            {
                this.Foreground = IsWhite ? GeradeBrush : Brushes.Gray;
                m_timer.Stop();

                return m_stream.CloseStream();
            }
            return false;
        }
        public bool PauseItem()
        {
            return m_stream.PauseStream();
        }
        public bool UpdateSystem(PluginSystem.IOutPlugin system)
        {
            m_system = system;
            m_stream = system.CreateStream(m_path, true);

            

            return (m_stream != null) ;
        }


        public bool IsPlaying 
        {
            get { return m_stream.Status == PluginSystem.StreamStatus.Playing; }
        }

        public bool IsPaused
        {
            get { return m_stream.Status == PluginSystem.StreamStatus.Paused; }
        }
        public override string ToString()
        {
            return string.Format("{0}|_|{1}", this.Content.ToString(), m_path);
        }
    }
}

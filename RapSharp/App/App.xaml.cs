/*Ramona Audio Player Copyright (c)  2013 Philipp Schröck <philsch@hotmail.de>


1.0  Hiermit wird unentgeltlich, jeder Person, die eine Kopie der Software und der zugehörigen 
Dokumentationen (die \"Software\") erhält, die Erlaubnis erteilt, sie uneingeschränkt zu benutzen, 
inklusive und ohne Ausnahme,  dem Recht, sie zu verwenden, kopieren, ändern und/oder verbreiten und Personen, 
die diese Software erhalten, diese Rechte zu geben, unter den folgenden Bedingungen:

Der obige Urheberrechtsvermerk, dieser Erlaubnisvermerk und der Quellcode sind in allen Kopien oder Teilkopien 
der Software beizulegen.

2.0 Die Kommerzielle und/oder Gewerbliche Nutzung dieser Software und/oder Teile ist ausdrücklich verboten.

3.0 DIE SOFTWARE WIRD OHNE JEDE AUSDRÜCKLICHE ODER IMPLIZIERTE GARANTIE BEREITGESTELLT, EINSCHLIESSLICH DER 
GARANTIE ZUR BENUTZUNG FÜR DEN VORGESEHENEN ODER EINEM BESTIMMTEN ZWECK SOWIE  
JEGLICHER RECHTSVERLETZUNG, JEDOCH NICHT DARAUF BESCHRÄNKT. IN KEINEM FALL SIND DIE AUTOREN ODER 
COPYRIGHTINHABER FÜR JEGLICHEN SCHADEN ODER SONSTIGE ANSPRÜCHE HAFTBAR ZU MACHEN, OB INFOLGE DER 
ERFÜLLUNG EINES VERTRAGES, EINES DELIKTES ODER ANDERS IM ZUSAMMENHANG MIT DER SOFTWARE ODER SONSTIGER 
VERWENDUNG DER SOFTWARE ENTSTANDEN.*/

using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RapSharp
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : System.Windows.Application, ISingleInstanceApp
    {
        private const string Unique = "My_Unique_Application_String";

        private static Process m_currentProcessstatic;
        private static MainWindow m_window;

        [STAThread]
        public static void Main()
        {
            //http://blogs.microsoft.co.il/blogs/arik/archive/2010/05/28/wpf-single-instance-application.aspx

            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                m_currentProcessstatic = Process.GetCurrentProcess();
                var application = new App();

                application.InitializeComponent();
                m_window = new MainWindow();
                application.Run(m_window);

                SingleInstance<App>.Cleanup();
            }
        }


        public static string GetAppBaseGetDirectory()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly‌().Location);
            return path;
        }
        public static string GetPluginPath()
        {
            string path= (GetAppBaseGetDirectory() +  "/Plugin/");
            return path;
        }
        public static string GetConfigPath()
        {
            return Path.Combine(GetAppBaseGetDirectory(), "Config/config.xml");
        }
        public static string GetAppDataPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RamonaAudioPlayer");
        }
        public static string GetLastPlaylistPath()
        {
            return GetAppDataPath() + "RapLastPlaylist.xmu";
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            logic.DialogsOpen.AddToPlaylist(args.ToArray(),
                m_window.lstPlaylist, m_window.m_current.Plugin, false);

            return true;
        }
    }
}

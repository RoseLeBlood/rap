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
using PluginSystem.VisualRenderer;
using RapSharp.logic;
using RapSharp.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace RapSharp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Felder

        private PluginSystem.PluginHost m_host;
        internal OutputMenuItem m_current = null;
        private logic.Plalistitem m_curPlaying = null;
        private DispatcherTimer m_updateTimer;
        private DispatcherTimer m_updateSystemTimer = null;
        private bool m_sliderValueChanged = false;

        private string[] m_args;

        private bool m_repeatTitle = false;
        private bool m_repeatAll = false;
        private int m_ticks = 0;

        #endregion

        

        public MainWindow()
        {
            InitializeComponent();
            m_host = new PluginSystem.PluginHost();
            m_host.onRegister += AddOutputToMenu;
            m_args = Environment.GetCommandLineArgs();
        }

        #region publicMethoden
        public void AddOutputToMenu(object sender, PluginSystem.IPlugin plugin)
        {
            if (plugin.Type == PluginType.SoundOutPlugin)
            {
                OutputMenuItem item = new OutputMenuItem(plugin as IOutPlugin);

                item.Click += (itemoutput_click);

                if (m_current == null && item.Name.Contains("Dummy") == false)
                    m_current = item;

                menuOutput.Items.Add(item);
            }
            else if (plugin.Type == PluginType.VisualRendererPlugin)
            {
                visual.RegisterVisualPlugin(plugin as IVisualRenderer);
            }
        }
        #endregion

        #region private Methoden
        private void frmMain_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                m_host.LoadPlugins(App.GetPluginPath());
                m_current.IsChecked = true;
                if (m_current.Plugin.Create(PluginSystem.SoundSystemConfig.FromXML(App.GetConfigPath())))
                {
                    this.visual.MenuItem = m_current;

                    m_updateTimer = new DispatcherTimer();
                    m_updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 900);
                    m_updateTimer.Tick += dispatcherTimer_Tick;

                    m_updateSystemTimer = new DispatcherTimer();
                    m_updateSystemTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
                    m_updateSystemTimer.Tick += m_updateSystemTimer_Tick;
                    m_updateSystemTimer.Start();

                    if (File.Exists(App.GetLastPlaylistPath()))
                    {
                        Playlist list = DialogsOpen.OpenPlaylist(App.GetLastPlaylistPath());
                        DialogsOpen.AddToPlaylist(list, lstPlaylist, m_current.Plugin);
                    }

                    DialogsOpen.AddToPlaylist(m_args, lstPlaylist, m_current.Plugin, false);
                }
                else
                {
                    frmMessageBox.ShowDialog("Audio system can not create", "Ramona Audio Player");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                frmMessageBox.ShowDialog(ex.ToString(), "Ramona Audio Player");
                this.Close();
            }
        }

        private void m_updateSystemTimer_Tick(object sender, EventArgs e)
        {
            if (m_curPlaying != null && m_curPlaying.Status == "Playing")
                m_current.Plugin.ListenerPosition = listenerPosition.Listener;

            m_current.Plugin.Update(m_updateSystemTimer.Interval.Milliseconds);
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
             
            if (m_curPlaying != null )
            {
                sitemStatus.Content = m_curPlaying.Status;

                // Text Right to left reihenfolge erst 9,8,7...0
                sitemTime.Content = string.Format("{1} - {0}",
                    MSToFormatedString(m_curPlaying.TimePosition),
                    MSToFormatedString(m_curPlaying.TimeLenght));



                sliderUpdate.Value = m_curPlaying.TimePosition; 
                try
                {
                    if ((sliderUpdate.Value == m_curPlaying.TimeLenght || sliderUpdate.Value == 0) &&
                        m_curPlaying.TimeLenght > 0)
                    {
                        if (m_ticks == 2)
                        {
                            if (m_repeatTitle)
                            {
                                m_curPlaying.StopItem();
                                m_curPlaying.PlayItem();
                            }
                            else if (lstPlaylist.Items.IndexOf(m_curPlaying) + 1 == lstPlaylist.Items.Count)
                            {
                                if (m_repeatAll)
                                {
                                    lstPlaylist.SelectedItem = lstPlaylist.Items.GetItemAt(0);
                                    cmdPlay_Click(sender, null);
                                }
                                else
                                {
                                    cmdStop_Click(sender, null);
                                }
                            }
                            else
                                cmdNext_Click(sender, null);
                        }
                        else
                        {
                            m_ticks++;
                        }
                    }
                }
                catch(Exception ex)
                {
                    frmMessageBox.ShowDialog(ex.ToString(), "Ramona Audio Player");
                }
            }
            else
            {
                sitemStatus.Content = "Stopped";
            }
            
        }
        private string MSToFormatedString(double ms)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(ms);
            return string.Format("{0}:{1}:{2}", span.Hours.ToString("00"), span.Minutes.ToString("00"), span.Seconds.ToString("00"));
        }
        private void frmMain_Closed(object sender, EventArgs e)
        {
            if (m_current != null)
                m_current.Plugin.Destroy();
        }
        private void itemoutput_click(object sender, RoutedEventArgs e)
        {
            OutputMenuItem clicked = ((OutputMenuItem)sender);
            
            if (clicked == m_current)
            {
                clicked.IsChecked = true;
            }
            else
            {
                if(m_curPlaying != null)
                    this.cmdStop_Click(sender, e);

                m_current.Plugin.Destroy();
                m_current.IsChecked = false;
                m_current = clicked;

                m_current.Plugin.Create(PluginSystem.SoundSystemConfig.FromXML(App.GetConfigPath()));
                m_current.IsChecked = true;
                this.visual.MenuItem = m_current;

                foreach (Plalistitem item in lstPlaylist.Items)
                {
                    item.UpdateSystem(m_current.Plugin);
                }

            }
        }

        private void lstPlaylist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            cmdStop_Click(sender, null);
            cmdPlay_Click(sender, null);
        }

        private void lstPlaylist_Drop(object sender, System.Windows.DragEventArgs e)
        {
            List<string> list = new List<string>(e.Data.GetData(System.Windows.DataFormats.FileDrop, true) as string[]);
            list.Sort();

            foreach (var item in list)
            {
                if (System.IO.Path.GetExtension(item) == ".xmu")
                {
                   Playlist plist = DialogsOpen.OpenPlaylist(item);
                   DialogsOpen.AddToPlaylist(plist, lstPlaylist, m_current.Plugin, false);
                }
                else
                    OpenFile(item);
            }
        }
        private void cmdPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_curPlaying != null && m_curPlaying.Status == "Playing")
                    m_curPlaying.StopItem();

                if ((lstPlaylist.SelectedItem as logic.Plalistitem) != null)
                {
                    if ((lstPlaylist.SelectedItem as logic.Plalistitem).PlayItem())
                    {
                        m_curPlaying = lstPlaylist.SelectedItem as logic.Plalistitem;
                        sliderUpdate.Maximum = m_curPlaying.TimeLenght;
                        sliderUpdate.Value = 1;

                        cmdPause.IsEnabled = true;
                        cmdStop.IsEnabled = true;
                        cmdNext.IsEnabled = true;
                        cmdBack.IsEnabled = true;

                        cmdPlay.IsEnabled = false;
                        m_updateTimer.Start();
                        listenerPosition.IsEnabled = true;
                    }
                }
            }
            catch(Exception)
            {
            }
        }

        private void cmdPause_Click(object sender, RoutedEventArgs e)
        {
            m_curPlaying.PauseItem();
        }

        private void cmdStop_Click(object sender, RoutedEventArgs e)
        {
            if (m_curPlaying == null)
                return;

            m_ticks = 0;
            m_updateTimer.Stop();
            m_curPlaying.StopItem() ;
            
            m_curPlaying = null;
               
            cmdPause.IsEnabled = false;
            cmdStop.IsEnabled = false;
            cmdNext.IsEnabled = false;
           
            cmdBack.IsEnabled = false;
            cmdPlay.IsEnabled = true;

            listenerPosition.IsEnabled = false;
        }

        private void cmdNext_Click(object sender, RoutedEventArgs e)
        {
            int indexCur = lstPlaylist.Items.IndexOf(m_curPlaying);

            if (indexCur + 1 < lstPlaylist.Items.Count && m_curPlaying.StopItem())
            {
                m_updateTimer.Stop();

                m_curPlaying = lstPlaylist.Items.GetItemAt(indexCur + 1) as Plalistitem;
                m_curPlaying.PlayItem();

                sliderUpdate.Maximum = m_curPlaying.TimeLenght;
                sliderUpdate.Value = 1;

                m_ticks = 0;

                m_updateTimer.Start();
            }
        }

        private void cmdBack_Click(object sender, RoutedEventArgs e)
        {
            int indexCur = lstPlaylist.Items.IndexOf(m_curPlaying);

            if (indexCur - 1 != -1 && m_curPlaying.StopItem())
            {
                m_updateTimer.Stop();

                m_curPlaying = lstPlaylist.Items.GetItemAt(indexCur - 1) as Plalistitem;
                m_curPlaying.PlayItem();

                m_current.Plugin.ListenerPosition = new Vector3(0);
                m_current.Plugin.Update(1);

                sliderUpdate.Maximum = m_curPlaying.TimeLenght;
                sliderUpdate.Value = 1;
                m_ticks = 0;

                m_updateTimer.Start();
            }
        }

        

        private void menuOpenFile_Click(object sender, RoutedEventArgs e)
        {
            string[] files =  DialogsOpen.OpenFile();

            if (files != null)
            {
                menuClearPlaylist_Click(sender, e);
                DialogsOpen.AddToPlaylist(files, this.lstPlaylist, m_current.Plugin, false);
            }
        }

        private void menuOpenStream_Click(object sender, RoutedEventArgs e)
        {
            frmOpenStream sopenStream = new frmOpenStream();
            Nullable<bool> result = sopenStream.ShowDialog();

            if (result.Value == true)
            {
                OpenFile(sopenStream.Address);
            }
        }

        private void menuOpenDir_Click(object sender, RoutedEventArgs e)
        {
           string[] files =  DialogsOpen.DialogOpenDir();
           if (files != null)
           {
               menuClearPlaylist_Click(sender, e);
               DialogsOpen.AddToPlaylist(files, this.lstPlaylist, m_current.Plugin, false);
           }
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }

        private void menuInfo_Click(object sender, RoutedEventArgs e)
        {
            frmInfo info = new frmInfo();
     
            info.ShowDialog();
        }

       
        private void OpenFile(string file, string anzeig = "")
        {
            Plalistitem play_item = (anzeig == string.Empty ? new Plalistitem() : new Plalistitem(anzeig));

            if (play_item.FirstLoad(file, m_current.Plugin))
            {
                play_item.IsWhite = lstPlaylist.Items.Count % 2 == 1;
                lstPlaylist.Items.Add(play_item);
            }
        }

        private void menuClearPlaylist_Click(object sender, RoutedEventArgs e)
        {
            lstPlaylist.Items.Clear();

            sitemStatus.Content = "Closed";
            sitemTime.Content = "00:00:00 - 00:00:00";

            listenerPosition.IsEnabled = false;
            
        }

        private void menuLoadPlaylis_Click(object sender, RoutedEventArgs e)
        {
            Playlist list = DialogsOpen.OpenPlaylist();
            DialogsOpen.AddToPlaylist(list, lstPlaylist, m_current.Plugin);
        }

        private void meneSavePlaylist_Click(object sender, RoutedEventArgs e)
        {
            DialogsOpen.SavePlaylist(lstPlaylist.Items);
        }

        private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lstPlaylist.Items.Count > 0)
            {
                Directory.CreateDirectory(App.GetAppDataPath());
                try
                {
                    DialogsOpen.SavePlaylist(App.GetLastPlaylistPath(), lstPlaylist.Items);
                }
                catch (Exception)
                {

                }
            }
        }

        private void sliderUpdate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_sliderValueChanged == true)
            {
               // System.Windows.MessageBox.Show("Value changed to " + sliderUpdate.Value.ToString());
                m_sliderValueChanged = false;
                e.Handled = true;

               if(m_curPlaying !=null)
                   m_curPlaying.TimePosition = sliderUpdate.Value;

              
            }
        }
        private void sliderUpdate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_sliderValueChanged = true;
        }
        private void cmd3DView_Click(object sender, RoutedEventArgs e)
        {
            listenerPosition.Visibility = System.Windows.Visibility.Visible;
            visual.Visibility = System.Windows.Visibility.Hidden;

            menu3DPosition.IsChecked = true;
            menuVisual.IsChecked = false;
        }

        private void menuVisual_Click(object sender, RoutedEventArgs e)
        {
            listenerPosition.Visibility = System.Windows.Visibility.Hidden;
            visual.Visibility = System.Windows.Visibility.Visible;

            menu3DPosition.IsChecked = false;
            menuVisual.IsChecked = true;
        }
        #endregion

        private void menuRepeatAll_Click(object sender, RoutedEventArgs e)
        {
            m_repeatAll = menuRepeatAll.IsChecked;
            if (menuRepeatTitle.IsChecked && m_repeatAll)
                menuRepeatTitle.IsChecked = m_repeatTitle = false;

        }

        private void menuRepeatTitle_Click(object sender, RoutedEventArgs e)
        {
            m_repeatTitle = menuRepeatTitle.IsChecked;
            if (menuRepeatAll.IsChecked && m_repeatTitle)
                menuRepeatAll.IsChecked = m_repeatAll = false;
        }

        private void menuConfig_Click(object sender, RoutedEventArgs e)
        {
            frmConfig config = new frmConfig();
            config.Config = PluginSystem.SoundSystemConfig.FromXML(App.GetConfigPath());
            Nullable<bool> ret = config.ShowDialog();

            if (ret == true)
            {
                cmdStop_Click(sender, e);

                PluginSystem.SoundSystemConfig.ToXML(config.Config, App.GetConfigPath());
                m_current.Plugin.Destroy();
                if (m_current.Plugin.Create(config.Config) == false)
                {
                    frmMessageBox.ShowDialog("Sound System kann nicht erstellt werden. Bitte versuchen Sie eine" +
                        " andere Configuration.", "Ramona Audio Player");

                }
            }
        }
    }
}

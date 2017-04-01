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
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;

namespace RapSharp.logic
{
    public class DialogsOpen
    {
        internal static string[] OpenFile()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Title = "Ramona Audio Player Open File";
                dialog.Filter = "Audio file|*.mp3;*.flac;*.ogg;*.mp2;*.wma;*.wav";
                dialog.Filter += "|All files|*.*";
                dialog.Multiselect = true;
                dialog.CheckPathExists = true;

                Nullable<bool> result = dialog.ShowDialog();

                if (result.Value)
                {
                    return dialog.FileNames;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }
        
        internal static void SavePlaylist(ItemCollection items)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = " Save Playlist";
            dialog.Filter = "XML Media Urls|*.xmu";
            dialog.AddExtension = true;

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                SavePlaylist(dialog.FileName, items);
            }
        }
        internal static void SavePlaylist(string path, ItemCollection items)
        {

            using (Playlist playlist = new Playlist())
            {

                foreach (var item in items)
                {
                    string[] rep = item.ToString().Split(new string[] { "|_|" },
                        StringSplitOptions.RemoveEmptyEntries);
                    playlist.Add(new Item(rep[1], rep[0]));

                }
                Playlist.WriteXML(playlist, path);
            }
        }
        internal static Playlist OpenPlaylist()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Title = "Ramona Audio Player Open Playlist";
                dialog.Filter = "XML Media Urls|*.xmu";

                Nullable<bool> result = dialog.ShowDialog();

                if (result == true)
                {
                    return Playlist.ReadXML(dialog.OpenFile());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }
        internal static Playlist OpenPlaylist(string file)
        {
            Playlist list = null;
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                list = OpenPlaylist(stream);
            }
            return list;
        }
        internal static Playlist OpenPlaylist(Stream file)
        {
            return Playlist.ReadXML(file);
        }

        internal static string[] DialogOpenDir()
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.ShowNewFolderButton = false;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = dialog.SelectedPath;
                    return System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);
                }
                
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }
        internal static void AddToPlaylist(Playlist playlist, System.Windows.Controls.ListBox lstPlaylist,
            IOutPlugin plugin, bool clearPlaylist = true)
        {
            if (playlist == null)
                return;

            try
            {
                if(clearPlaylist)
                    lstPlaylist.Items.Clear();

                foreach (var item in playlist)
                {
                    if (IsFileSupport(item.Path))
                    {
                        Plalistitem play_item = new Plalistitem(item.Tag);

                        
                        if (play_item.FirstLoad(item.Path, plugin))
                        {
                            play_item.IsWhite = lstPlaylist.Items.Count % 2 == 1;
                            lstPlaylist.Items.Add(play_item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        internal static Plalistitem AddToPlaylist(string file, System.Windows.Controls.ListBox lstPlaylist,
            IOutPlugin plugin, bool clearPlaylist)
        {
            List<Plalistitem> items= AddToPlaylist(new string[] { file }, lstPlaylist, plugin, clearPlaylist);

            if (items != null && items.Count > 0)
            {
                return items[0];
            }
            return null;
        }
        internal static List<Plalistitem> AddToPlaylist(string[] files, System.Windows.Controls.ListBox lstPlaylist,
            IOutPlugin plugin, bool clearPlaylist)
        {
            if (files == null)
                return null;

            List<Plalistitem> list = new List<Plalistitem>();
            try
            {
                if(clearPlaylist)
                    lstPlaylist.Items.Clear();

                foreach (var item in files)
                {
                    if (IsFileSupport(item))
                    {
                        Plalistitem play_item = new Plalistitem();

                        if (play_item.FirstLoad(item, plugin))
                        {
                            play_item.IsWhite = lstPlaylist.Items.Count % 2 == 1;
                            lstPlaylist.Items.Add(play_item);

                            list.Add(play_item);
                        }
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        private static bool IsFileSupport(string file)
        {
            if (file.Contains("http://"))
                return true;

            switch (System.IO.Path.GetExtension(file))
            {
                case ".mp3":
                case ".mp2":
                case ".ogg":
                case ".flac":
                case ".wav":
                    return true;
                default:
                    return false;
            }
        }


        
    }
}

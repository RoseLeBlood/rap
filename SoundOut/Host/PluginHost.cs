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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PluginSystem
{
    public class PluginHost : IPluginHost
    {
        public event EventHandler<IPlugin> onRegister;

        private SortedList<string, IPlugin> m_plugins = new SortedList<string, IPlugin>();

        public SortedList<string, IPlugin> Plugins
        {
            get { return m_plugins; }
        }

        public void Register(IPlugin plugin)
        {
            string name = plugin.Name;

            if (Find(name) == null)
            {
                plugin.Host = this;
                m_plugins.Add(name, plugin);

                OnRegister(plugin);
            }
        }
        public IPlugin Find(string name)
        {
            IPlugin _out = null;

            foreach (var item in m_plugins)
            {
                if (item.Key == name)
                {
                    _out = item.Value;
                    break;
                }
            }
            return _out;
        }

        public void LoadPlugins(String pPluginPath)
        {
            String[] files = Directory.GetFiles(pPluginPath);

            foreach (String file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                if (fileInfo.Extension.Equals(".dll"))
                {
                    Dictionary<string, IPlugin> dictionary = GetModul(file, typeof(IPlugin));

                    foreach (var a in dictionary)
                    {
                        Register(a.Value);
                    }
                }
            }
        }
        protected virtual void OnRegister(IPlugin plugin)
        {
            if (onRegister != null)
            {
                onRegister(this, plugin);
            }
        }
        private Dictionary<string, IPlugin> GetModul(string pFileName, Type pTypeInterface)
        {
            //Hier speichern wir unsere Plugins
            Dictionary<string, IPlugin> interfaceinstances = new Dictionary<string, IPlugin>();

            Assembly assembly;
            try
            {
                //Loads an assembly given its file name or path.
                assembly = Assembly.LoadFrom(pFileName);


                // http://msdn.microsoft.com/de-de/library/t0cs7xez.aspx
                // Assembly Eigenschaften checken
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsPublic) // Ruft einen Wert ab, der angibt, ob der Type als öffentlich deklariert ist. 
                    {
                        if (!type.IsAbstract)  //nur Assemblys verwenden die nicht Abstrakt sind
                        {
                            // Sucht die Schnittstelle mit dem angegebenen Namen. 
                            Type typeInterface = type.GetInterface(pTypeInterface.ToString(), true);

                            //Make sure the interface we want to use actually exists
                            if (typeInterface != null)
                            {
                                try
                                {

                                    object activedInstance = Activator.CreateInstance(type);
                                    if (activedInstance != null)
                                    {
                                        interfaceinstances.Add(type.Name, activedInstance as IPlugin);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    System.Diagnostics.Debug.WriteLine(exception);
                                }
                            }

                            typeInterface = null;
                        }
                    }
                }
                assembly = null;
            }
            catch(Exception e)
            {
                //MessageBox.Show
                MessageBox.Show(e.ToString());
            }
            return interfaceinstances;
        }
    }
}

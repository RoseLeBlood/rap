/*NON-COMMERCIAL LICENSE
------------------------------------
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginSystem;

namespace FMOD_Output
{
    public class Output : IOutPlugin
    {
        private FMOD_SoundSystem system;
        private IPluginHost host;

        public PluginType Type
        {
            get { return PluginType.SoundOutPlugin; }
        }

        public Vector3 ListenerPosition
        {
            get
            {
                return system.ListenerPosition;
            }
            set
            {
                system.ListenerPosition = value;
            }
        }

        public IPluginHost Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
            }
        }
        public ISoundSystem System
        {
            get { return system; }
        }

        public string Name
        {
            get { return "FMOD Sound System Copyright Firelight Technologies Pty Ltd"; }
        }

        public string Autor
        {
            get { return "Philipp Schroeck"; }
        }

        public string Version
        {
            get { return "0.5"; }
        }


        public bool Create(SoundSystemConfig config)
        {
            system = new FMOD_SoundSystem();
            return system.Create(config);
        }

        public void Destroy()
        {
            system.Destroy();
        }

        public void Update(float updatetime)
        {
            system.Update(updatetime);
        }

        public ISoundStream CreateStream(string path, bool b3D)
        {
            FMOD_SoundStream stream = new FMOD_SoundStream();

            if (stream.IsSupport(path))
            {
                stream.Create(system);
                stream.LoadStream(path, b3D);
                return stream;
            }
            return null;
        }

        public ISoundStream CreateStream(System.IO.Stream stream, bool b3D)
        {
            FMOD_SoundStream _stream = new FMOD_SoundStream();
            _stream.Create(system);
            _stream.LoadStream(stream, b3D);
            return _stream;
        }


        
    }
}

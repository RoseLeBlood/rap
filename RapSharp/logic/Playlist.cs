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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RapSharp.logic
{
    [Serializable]
    public struct Item
    {
        public string Path;
        public string Tag;

        public Item(string path, string tag)
        {
            Path = path;
            Tag = tag;
        }
    }

    [Serializable]
    public class Playlist : List<Item>, IDisposable
    {
        public Playlist()
            : base()
        {
        }

        public static Playlist ReadXML(Stream filename)
        {
            XmlDocument xmlDoc = new XmlDocument();

            using (Stream reader = filename)
            {
                XmlSerializer ser = new XmlSerializer(typeof(Playlist));
                return ser.Deserialize(reader) as Playlist;
            }
        }
        public static void WriteXML(Playlist o, string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();

            using (FileStream writer = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Playlist));
                ser.Serialize(writer, o); 
            }
        }

        public void Dispose()
        {
            this.Clear();
        }
    }
}

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
using System.Windows.Controls;
using System.Windows.Media;

namespace RapSharp.logic
{
    public class OutputMenuItem : MenuItem 
    {
        private IOutPlugin m_plugin = null;

        public IOutPlugin Plugin 
        {
            get { return m_plugin; } 
        }

        public OutputMenuItem(PluginSystem.IOutPlugin plugin)
            : base()
        {
            m_plugin = plugin;

            this.Header = plugin.Name;
            this.Foreground = Brushes.Black;
            this.IsEnabled = true;
            this.Header = plugin.Name;
            this.IsCheckable = true;
            this.IsChecked = false;
            
            this.Name = "menu_" + plugin.Name.Replace(" ", "");
        }
        
    }
}

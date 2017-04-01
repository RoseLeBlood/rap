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

namespace RapSharp.View
{
    /// <summary>
    /// Interaktionslogik für frmConfig.xaml
    /// </summary>
    public partial class frmConfig : Window
    {
        private SoundSystemConfig m_config;

        public SoundSystemConfig Config
        {
            get { return m_config; }
            set { m_config = value; ParseConfig(); }
        }
        public frmConfig()
        {
            InitializeComponent();
        }

        private void gridMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
                
            }
        }

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            string channel = cobChannel.Text;
            string SampleRate = (cobSamplerate.Text);
            string Bit = (cobBit.Text);
            string soundcard = cobOutput.Text;

            m_config.Channel = GetChannel(channel);
            m_config.Format = GetFormat(Bit, SampleRate);
            m_config.Output = GetAusgabe(soundcard);

            this.Close();
        }

        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
        

        private void cobChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void cobSamplerate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        
        private void cobBit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void cobOutput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void cobSoundcard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private RASOUNDAUSGABE GetAusgabe(string ausgabe)
        {
            switch (ausgabe)
            {
                case "ASIO": return RASOUNDAUSGABE.ASIO;
                default: return RASOUNDAUSGABE.WASAPI;
            }
        }
        private RACHANNELMODE GetChannel(string channel)
        {
            /*<Label Content="Mono" />
                <Label Content="Stereo" />
                <Label Content="Quad" />
                <Label Content="5.1" />
                <Label Content="7.1" />*/
            switch (channel)
            {
                case "Mono": return RACHANNELMODE.Mono;
                case "Stereo": return RACHANNELMODE.Stereo;
                case "Quad": return RACHANNELMODE.Quad;
                case "5.1": return RACHANNELMODE.FiveDotOne;
                case "7.1": return RACHANNELMODE.SevenDotOne;
                default: return RACHANNELMODE.Stereo;

            }
        }
        private RASOUNDFORMAT GetFormat(string bit, string samplerate)
        {
            switch (bit)
            {
                case "16":
                    switch (samplerate)
                    {
                        case "44100": return RASOUNDFORMAT.PCM16_44100;
                        case "48000": return RASOUNDFORMAT.PCM16_48000;
                        case "96000": return RASOUNDFORMAT.PCM16_96000;
                        default: return RASOUNDFORMAT.PCM16_48000;
                    }
                case "24":
                    switch (samplerate)
                    {
                        case "44100": return RASOUNDFORMAT.PCM24_44100;
                        case "48000": return RASOUNDFORMAT.PCM24_48000;
                        case "96000": return RASOUNDFORMAT.PCM24_96000;
                        case "192000": return RASOUNDFORMAT.PCM24_192000;
                        default: return RASOUNDFORMAT.PCM24_96000;
                    }
                case "32":
                    switch (samplerate)
                    {
                        case "44100": return RASOUNDFORMAT.PCM32_44100;
                        case "48000": return RASOUNDFORMAT.PCM32_48000;
                        case "96000": return RASOUNDFORMAT.PCM32_96000;
                        case "192000": return RASOUNDFORMAT.PCM32_192000;
                        default: return RASOUNDFORMAT.PCM32_96000;
                    }
                case "Float":
                    switch (samplerate)
                    {
                        case "44100": return RASOUNDFORMAT.PCMFLOAT_44100;
                        case "48000": return RASOUNDFORMAT.PCMFLOAT_48000;
                        case "96000": return RASOUNDFORMAT.PCMFLOAT_96000;
                        case "192000": return RASOUNDFORMAT.PCMFLOAT_192000;
                        default: return RASOUNDFORMAT.PCMFLOAT_96000;
                    }
                default:
                    return RASOUNDFORMAT.PCM24_96000;
            }
        }
        private void ParseConfig()
        {
            switch (m_config.Channel)
            {
                case RACHANNELMODE.Mono:
                    cobChannel.SelectedIndex = 0;
                    break;
                case RACHANNELMODE.Stereo:
                    cobChannel.SelectedIndex = 1;
                    break;
                case RACHANNELMODE.Quad:
                    cobChannel.SelectedIndex = 2;
                    break;
                case RACHANNELMODE.FiveDotOne:
                    cobChannel.SelectedIndex = 3;
                    break;
                case RACHANNELMODE.SevenDotOne:
                    cobChannel.SelectedIndex = 4;
                    break;
                default:
                    cobChannel.SelectedIndex = 1;
                    break;
            }
            switch (m_config.Output)
            {
                case RASOUNDAUSGABE.WASAPI:
                    cobOutput.SelectedIndex = 0;
                    break;
                default:
                    cobOutput.SelectedIndex = 1;
                    break;
            }
            switch (m_config.Format)
            {
                case RASOUNDFORMAT.PCM16_44100:
                    cobSamplerate.SelectedIndex = 0;
                    cobBit.SelectedIndex = 0;
                    break;
                case RASOUNDFORMAT.PCM16_48000:
                    cobSamplerate.SelectedIndex = 1;
                    cobBit.SelectedIndex = 0;
                    break;
                case RASOUNDFORMAT.PCM16_96000:
                    cobSamplerate.SelectedIndex = 2;
                    cobBit.SelectedIndex = 0;
                    break;
                case RASOUNDFORMAT.PCM24_44100:
                    cobSamplerate.SelectedIndex = 0;
                    cobBit.SelectedIndex = 1;
                    break;
                case RASOUNDFORMAT.PCM24_48000:
                    cobSamplerate.SelectedIndex = 1;
                    cobBit.SelectedIndex = 1;
                    break;
                case RASOUNDFORMAT.PCM24_96000:
                    cobSamplerate.SelectedIndex = 2;
                    cobBit.SelectedIndex = 1;
                    break;
                case RASOUNDFORMAT.PCM24_192000:
                    cobSamplerate.SelectedIndex = 3;
                    cobBit.SelectedIndex = 1;
                    break;
                case RASOUNDFORMAT.PCM32_44100:
                    cobSamplerate.SelectedIndex = 0;
                    cobBit.SelectedIndex = 2;
                    break;
                case RASOUNDFORMAT.PCM32_48000:
                    cobSamplerate.SelectedIndex = 1;
                    cobBit.SelectedIndex = 2;
                    break;
                case RASOUNDFORMAT.PCM32_96000:
                    cobSamplerate.SelectedIndex = 2;
                    cobBit.SelectedIndex = 2;
                    break;
                case RASOUNDFORMAT.PCM32_192000:
                    cobSamplerate.SelectedIndex = 3;
                    cobBit.SelectedIndex = 2;
                    break;
                case RASOUNDFORMAT.PCMFLOAT_44100:
                    cobSamplerate.SelectedIndex = 0;
                    cobBit.SelectedIndex = 3;
                    break;
                case RASOUNDFORMAT.PCMFLOAT_48000:
                    cobSamplerate.SelectedIndex = 1;
                    cobBit.SelectedIndex = 3;
                    break;
                case RASOUNDFORMAT.PCMFLOAT_96000:
                    cobSamplerate.SelectedIndex = 2;
                    cobBit.SelectedIndex = 3;
                    break;
                case RASOUNDFORMAT.PCMFLOAT_192000:
                    cobSamplerate.SelectedIndex = 3;
                    cobBit.SelectedIndex = 3;
                    break;
            }
            cobSoundcard.SelectedIndex = 0;
        }
    }
}

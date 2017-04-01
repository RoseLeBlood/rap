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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PluginSystem
{
    [Serializable]
    public enum RACHANNELMODE
    {
        Mono,
        Stereo,
        Quad,
        Surround,
        FiveDotOne,
        SevenDotOne,
    }
    [Serializable]
    public enum RASOUNDFORMAT
    {
        PCM16_44100,
        PCM16_48000,
        PCM16_96000,

        PCM24_44100,
        PCM24_48000,
        PCM24_96000,
        PCM24_192000,

        PCM32_44100,
        PCM32_48000,
        PCM32_96000,
        PCM32_192000,

        PCMFLOAT_44100,
        PCMFLOAT_48000,
        PCMFLOAT_96000,
        PCMFLOAT_192000
    }
    [Serializable]
    public enum RASOUNDAUSGABE
    {
        WINMM,
        WASAPI,
        DSOUND,
        ASIO
    };

    public class SoundSystemConfig
    {
        public RACHANNELMODE Channel { get; set; }
        public RASOUNDFORMAT Format { get; set; }
        public RASOUNDAUSGABE Output { get; set; }
        public int SoundCard { get; set; }

        public SoundSystemConfig()
        {
            Channel = RACHANNELMODE.Stereo;
            Format = RASOUNDFORMAT.PCM16_96000;
            Output = RASOUNDAUSGABE.WASAPI;
            SoundCard = 0;
        }

        public SoundSystemConfig(RACHANNELMODE channel, RASOUNDFORMAT format, RASOUNDAUSGABE output)
        {
            Channel = channel;
            Format = format;
            Output = output;
            SoundCard = 0;
        }
        public static void ToXML(object value, string path)
        {
            
            try
            {
                File.Delete(path);

                XmlSerializer x = new XmlSerializer(value.GetType());
                using (FileStream ms = new FileStream(path, FileMode.CreateNew))
                {
                     x.Serialize(ms, value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static SoundSystemConfig FromXML(string path)
        {
            try
            {
                XmlSerializer x = new XmlSerializer(typeof(SoundSystemConfig));

                using (FileStream ms = new FileStream(path, FileMode.Open))
                {
                    return (SoundSystemConfig)x.Deserialize(ms);
                }
            }
            catch
            {
                SoundSystemConfig config =  new SoundSystemConfig();
                ToXML(config, path);
                return config;
            }
        }
        /*public RACHANNELMODE Channel { get; set; }
        public RASOUNDFORMAT Format { get; set; }
        public RASOUNDAUSGABE Output { get; set; }
        public int SoundCard { get; set; }*/


        public override string ToString()
        {
            try
            {
                XmlSerializer x = new XmlSerializer(this.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    x.Serialize(ms, this);
                    ms.Position = 0;
                    using (StreamReader sr = new StreamReader(ms, Encoding.Default))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}

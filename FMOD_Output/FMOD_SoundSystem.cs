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
using System.Threading;

namespace FMOD_Output
{
    public class FMOD_SoundSystem : ISoundSystem
    {
        private FMOD.System m_system = new FMOD.System();
        private Vector3 m_position = new Vector3(0);

        FMOD.VECTOR forward = new FMOD.VECTOR();
        FMOD.VECTOR up = new FMOD.VECTOR();
        FMOD.VECTOR pos = new FMOD.VECTOR();
        FMOD.VECTOR vel = new FMOD.VECTOR();
        FMOD.VECTOR lastPos = new FMOD.VECTOR();

        public Vector3 ListenerPosition
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }
        internal FMOD.System System
        {
            get { return m_system; }
        }
        public bool Create(SoundSystemConfig config)
        {
            uint version = 0;
			int samplerate = 44100;
            FMOD.SOUND_FORMAT format = FMOD.SOUND_FORMAT.PCM16;

            FMOD.RESULT result = FMOD.Factory.System_Create(ref m_system);
			if(ERRCHECK(result, "Create") == false) return false;

            result = m_system.getVersion(ref version);
            if (!ERRCHECK(result)) return false;

            if (version < FMOD.VERSION.number)
            {
                
                Console.WriteLine("INCORRECT DLL VERSION!!", "FMOD ERROR");
                return false;
            }

            result = m_system.setDriver(config.SoundCard);
            ERRCHECK(result);

            switch (config.Output)
            {
                case RASOUNDAUSGABE.WINMM:
                    m_system.setOutput(FMOD.OUTPUTTYPE.WINMM);
                    break;
                case RASOUNDAUSGABE.WASAPI:
                    m_system.setOutput(FMOD.OUTPUTTYPE.WASAPI);
                    break;
                case RASOUNDAUSGABE.DSOUND:
                    m_system.setOutput(FMOD.OUTPUTTYPE.DSOUND);
                    break;
                case RASOUNDAUSGABE.ASIO:
                    m_system.setOutput(FMOD.OUTPUTTYPE.ASIO);
                    break;
            };
            switch (config.Format)
            {
                case RASOUNDFORMAT.PCM16_44100:
                    samplerate = 44100;
                    format = FMOD.SOUND_FORMAT.PCM16;
                    break;
                case RASOUNDFORMAT.PCM16_48000:
                    samplerate = 48000;
                    format = FMOD.SOUND_FORMAT.PCM16;
                    break;
                case RASOUNDFORMAT.PCM16_96000:
                    samplerate = 96000;
                    format = FMOD.SOUND_FORMAT.PCM16;
                    break;
                case RASOUNDFORMAT.PCM24_44100:
                    samplerate = 44100;
                    format = FMOD.SOUND_FORMAT.PCM24;
                    break;
                case RASOUNDFORMAT.PCM24_48000:
                    samplerate = 48000;
                    format = FMOD.SOUND_FORMAT.PCM24;
                    break;
                case RASOUNDFORMAT.PCM24_96000:
                    samplerate = 96000;
                    format = FMOD.SOUND_FORMAT.PCM24;
                    break;
                case RASOUNDFORMAT.PCM24_192000:
                    samplerate = 192000;
                    format = FMOD.SOUND_FORMAT.PCM24;
                    break;
                case RASOUNDFORMAT.PCMFLOAT_44100:
                    samplerate = 44100;
                    format = FMOD.SOUND_FORMAT.PCMFLOAT;
                    break;
                case RASOUNDFORMAT.PCMFLOAT_48000:
                    samplerate = 48000;
                    format = FMOD.SOUND_FORMAT.PCMFLOAT;
                    break;
                case RASOUNDFORMAT.PCMFLOAT_96000:
                    samplerate = 96000;
                    format = FMOD.SOUND_FORMAT.PCMFLOAT;
                    break;
                case RASOUNDFORMAT.PCMFLOAT_192000:
                    samplerate = 192000;
                    format = FMOD.SOUND_FORMAT.PCMFLOAT;
                    break;
                case RASOUNDFORMAT.PCM32_44100:
                    samplerate = 44100;
                    format = FMOD.SOUND_FORMAT.PCM32;
                    break;
                case RASOUNDFORMAT.PCM32_48000:
                    samplerate = 48000;
                    format = FMOD.SOUND_FORMAT.PCM32;
                    break;
                case RASOUNDFORMAT.PCM32_96000:
                    samplerate = 96000;
                    format = FMOD.SOUND_FORMAT.PCM32;
                    break;
                case RASOUNDFORMAT.PCM32_192000:
                    samplerate = 192000;
                    format = FMOD.SOUND_FORMAT.PCM32;
                    break;
            }
            m_system.setSoftwareFormat(samplerate, format, 0, 0, FMOD.DSP_RESAMPLER.LINEAR);

            switch (config.Channel)
            {
                case RACHANNELMODE.Mono:
                    result = m_system.setSpeakerMode(FMOD.SPEAKERMODE.MONO);
                    break;
                case RACHANNELMODE.Stereo:
                    result = m_system.setSpeakerMode(FMOD.SPEAKERMODE.STEREO);
                    break;
                case RACHANNELMODE.Quad:
                    result = m_system.setSpeakerMode(FMOD.SPEAKERMODE.QUAD);
                    break;
                case RACHANNELMODE.FiveDotOne:
                    result = m_system.setSpeakerMode(FMOD.SPEAKERMODE._5POINT1);
                    break;
                case RACHANNELMODE.SevenDotOne:
                    result = m_system.setSpeakerMode(FMOD.SPEAKERMODE._7POINT1);
                    break;
                case RACHANNELMODE.Surround:
                    result = m_system.setSpeakerMode(FMOD.SPEAKERMODE.SURROUND);
                    break;
            };
            result = m_system.init(32,  FMOD.INITFLAGS._3D_RIGHTHANDED | FMOD.INITFLAGS.DTS_NEURALSURROUND, IntPtr.Zero);
            if (!ERRCHECK(result)) return false;

            m_system.set3DSettings(1.0f, 1.0f, 1.0f);

            Console.WriteLine("[FMOD] SoundSystem created");

            m_system.get3DListenerAttributes(0, ref pos, ref vel, ref forward, ref up);

            return true;
        }

        public bool Destroy()
        {
            m_system.close();
            m_system.release();

            Console.WriteLine("[FMOD] SoundSystem destroyed");
            return true;
        }

        public bool Update(float updatetime)
        {
            pos.x = m_position.x;
            pos.y = m_position.y;
            pos.z = m_position.z;

            vel.x = (pos.x - lastPos.x) * (1 / updatetime);
            vel.y = (pos.y - lastPos.y) * (1 / updatetime);
            vel.z = (pos.z - lastPos.z) * (1 / updatetime);

            //Update listener attributes
            m_system.set3DListenerAttributes(0, ref pos, ref vel, ref forward, ref up);

            m_position.x = pos.x;
            m_position.y = pos.y;
            m_position.z = pos.z;

            lastPos = pos;

            m_system.update();
            return true;
        }
        
        public bool ERRCHECK(FMOD.RESULT result, string Func = "Create")
        {
            if (result != FMOD.RESULT.OK)
			{
                string s = string.Format("FMOD ({0}) {1} [{2}]", result, FMOD.Error.String(result), Func);

                Console.WriteLine(s);
				return false;
			}
			return true;
        }


        public float[] getWaveData(int size, int channel)
        {
            float[] data = new float[size];

            m_system.getWaveData(data, size, channel);
            return data;
        }


        public float[] getSpectrum(int size, int channel, DSP_FFT fft)
        {
            float[] data = new float[size];

            switch(fft)
            {
                case DSP_FFT.RECT:
                    m_system.getSpectrum(data, size, channel, FMOD.DSP_FFT_WINDOW.RECT);
                    break;
                case DSP_FFT.BLACKMAN:
                    m_system.getSpectrum(data, size, channel, FMOD.DSP_FFT_WINDOW.BLACKMAN);
                    break;
                case DSP_FFT.BLACKMANHARRIS:
                    m_system.getSpectrum(data, size, channel, FMOD.DSP_FFT_WINDOW.BLACKMANHARRIS);
                    break;
                case DSP_FFT.HAMMING:
                    m_system.getSpectrum(data, size, channel, FMOD.DSP_FFT_WINDOW.HAMMING);
                    break;
                case DSP_FFT.HANNING:
                    m_system.getSpectrum(data, size, channel, FMOD.DSP_FFT_WINDOW.HANNING);
                    break;
                case DSP_FFT.TRIANGLE:
                    m_system.getSpectrum(data, size, channel, FMOD.DSP_FFT_WINDOW.TRIANGLE);
                    break;
            }
            return data;
        }
       
    }
}

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
using FMOD;
using System.IO;
using System.Runtime.InteropServices;

namespace FMOD_Output
{
    public class FMOD_SoundStream : ISoundStream
    {
        Sound m_sound;
        Channel m_channel;
        FMOD_SoundSystem m_system;
        StreamStatus m_status;
        bool is3d = false;

        public StreamStatus Status
        {
            get { return m_status; }
        }

        public bool Create(ISoundSystem AudioSystem)
        {
            if (AudioSystem is FMOD_SoundSystem)
            {
                m_system = AudioSystem as FMOD_SoundSystem;
                m_status = StreamStatus.Created;
                m_sound = null;
                m_channel = null;

                Console.WriteLine("[FMOD] SoundStream created");
            }
            else
                throw new NotSupportedException("Create SoundSystem");



            return true;
        }

        public void Destroy()
        {
            StopStream();
            m_sound.release();

            Console.WriteLine("[FMOD] SoundStream destroyed");
        }

        public void Set3DMinMaxDistance(float min, float max)
        {
            m_sound.set3DMinMaxDistance(min, max);
        }

        public bool Set3DSettings(Vector3 pos, Vector3 vel)
        {
            if (m_channel != null)
            {
                FMOD.VECTOR _pos;
                FMOD.VECTOR _vel;


                _pos.x = pos.x;
                _pos.y = pos.y;
                _pos.z = pos.z;

                _vel.x = vel.x;
                _vel.y = vel.y;
                _vel.z = vel.z;

                FMOD.RESULT result = m_channel.set3DAttributes(ref _pos, ref _pos);
                if (!m_system.ERRCHECK(result, "SetVelocity::set3DAttributes")) return false;

                return true;
            }
            return false;
        }
        public int GetPosition()
        {
            uint currtime = 0;
			if(m_channel != null)
			{
				m_channel.getPosition(ref currtime, TIMEUNIT.MS);
			}
			return (int)currtime;
        }

        public bool SetPosition(uint pos)
        {
            if (m_channel == null) return false;

            FMOD.RESULT result = m_channel.setPosition(pos, TIMEUNIT.MS);
            if (!m_system.ERRCHECK(result, "SetPosition")) return false;

            return true;
        }

        public int GetLenght()
        {
            uint length = 0;
			if(m_channel != null)
			{
				m_sound.getLength(ref length, TIMEUNIT.MS);
			}
			return (int)length;
        }

        public bool SetChannelMix(float fleft, float fright, float rleft, float rright, float sleft, float sright, float center, float bass)
        {
            if (m_channel == null) return false;

            FMOD.RESULT result = m_channel.setSpeakerMix(fleft, fright, center, bass, rleft, rright, sleft, sright);
            if (!m_system.ERRCHECK(result, "SetChannelMix")) return false;

            return true;
        }

        public void SetVolume(float vol)
        {
            if (m_channel == null) return;

            m_channel.setVolume(vol);
        }

        public bool LoadStream(string FileName, bool b3D = false)
        {
            FMOD.RESULT result;
			FMOD.System sys = m_system.System;

            is3d = b3D;

			if(sys != null)
			{
                result = sys.createStream(FileName, (b3D) ? FMOD.MODE._3D : FMOD.MODE._2D, ref m_sound);
                if (!m_system.ERRCHECK(result, "LoadStream")) return false;

                m_status = StreamStatus.Loaded;

                Console.WriteLine("[FMOD] SoundStream file: "+ FileName +" file loaded");
			}
			else
			{
				return false;
			}
            return true;
        }

        public bool LoadStream(System.IO.Stream stream, bool b3D = false)
        {
            throw new NotImplementedException();
        }

        public bool CloseStream()
        {
            if (m_sound != null)
            {
                FMOD.RESULT result = m_sound.release();
                if (!m_system.ERRCHECK(result, "CloseStream")) return false;

                m_status = StreamStatus.Closed;
                m_channel = null;

                Console.WriteLine("[FMOD] SoundStream closed");
                return true;
            }
            return false;
        }

        public bool PlayStream(Vector3 Position, float minDistance = -100.0f, float maxDistance = 100.0f)
        {
            if (m_system.System == null) { throw new Exception("Sound System nicht vorhanden");  }

            if (m_channel != null)
            {
                m_channel.stop();
            }
            FMOD.RESULT result = m_system.System.playSound(CHANNELINDEX.REUSE, m_sound, true, ref m_channel);
            if (!m_system.ERRCHECK(result, "playSound")) return false;

            if (is3d)
            {
                FMOD.VECTOR position, velocity;
                position.x = Position.x;
                position.y = Position.y;
                position.z = Position.z;

                velocity.x = 0;
                velocity.y = 0;
                velocity.z = 0;

                m_channel.set3DAttributes(ref position, ref velocity);

                Set3DMinMaxDistance(-100.0f, 100.0f);
            }
            result = m_channel.setPaused(false);
            if (!m_system.ERRCHECK(result, "setPaused")) return false;

            m_status = StreamStatus.Playing;

            Console.WriteLine("[FMOD] SoundStream played");

            return true;
        }

        public bool PauseStream()
        {
            bool m_paused = false;

			if(m_channel != null )
			{
				FMOD.RESULT result = m_channel.getPaused(ref m_paused);
				if(!m_system.ERRCHECK(result, "getPaused")) return false;

				result = m_channel.setPaused(!m_paused);
                if (!m_system.ERRCHECK(result, "setPaused")) return false;

                m_status = (m_paused ? StreamStatus.Paused : StreamStatus.Playing);

                Console.WriteLine("[FMOD] SoundStream " + Enum.GetName(m_status.GetType(), m_status).ToLower());
			}
			return true;
        }

        public bool StopStream()
        {
            if (m_channel != null)
            {
                m_channel.stop();

                m_status = StreamStatus.Stopped;

                Console.WriteLine("[FMOD] SoundStream stopped");
            }
            return true;
        }

        public bool IsSupport(string path)
        {
            if (path.Contains("http://") || path.Contains("https://"))
                return true;

            switch (Path.GetExtension(path))
            {
                case ".mp3":
                case ".ogg":
                case ".flac":
                case ".wma":
                case ".mp2":
                case ".wav":
                case ".mod":
                    Console.WriteLine("[FMOD] File support");
                    return true;
                default:
                    Console.WriteLine("[FMOD] File not support");
                    return false;
            }
        }


        public IntPtr GetTag(string key)
        {
            if (m_sound == null)
                return IntPtr.Zero;

            FMOD.TAG tag = new TAG();

            if (m_sound.getTag(key, -1, ref tag) == FMOD.RESULT.OK)
            {
                return tag.data;
            }

            return IntPtr.Zero;
        }
    }
}

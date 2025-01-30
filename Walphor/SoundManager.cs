using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Walphor
{
    internal class SoundManager
    {

        private IWavePlayer wavePlayer;
        private AudioFileReader mainMusicReader;
        private LoopStream mainMusicLoop;
        private float masterVolume = 0.5f;
        private IWavePlayer walkingSoundPlayer;
        private AudioFileReader walkingSoundReader;
        private LoopStream walkingSoundLoop;
        private IWavePlayer clickSoundPlayer;
        private AudioFileReader clickSoundReader;
        private IWavePlayer dialogueSoundPlayer;
        private AudioFileReader dialogueSoundReader;

        public string CurrentWalkingSoundPath { get; private set; }
        public SoundManager()
        {
            wavePlayer = new WaveOutEvent();
        }

        public void PlayMainMusic(string musicFilePath)
        {
            if (mainMusicReader != null)
            {
                mainMusicReader.Dispose();
            }

            mainMusicReader = new AudioFileReader(musicFilePath)
            {
                Volume = masterVolume
            };
            mainMusicLoop = new LoopStream(mainMusicReader);

            wavePlayer.Init(mainMusicLoop);
            wavePlayer.Play();
        }
        public void StopMainMusic()
        {
            if (mainMusicReader != null)
            {
                wavePlayer.Stop();
                mainMusicReader.Dispose();
                mainMusicReader = null;
                mainMusicLoop = null;
            }
        }
        public void SetMasterVolume(float volume)
        {
            masterVolume = volume;
            if (mainMusicReader != null)
            {
                mainMusicReader.Volume = masterVolume;
            }
            if (walkingSoundReader != null)
            {
                walkingSoundReader.Volume = masterVolume;
            }
        }
        public float GetMasterVolume()
        {
            return masterVolume;
        }

        public void LoadMasterVolume(float volume)
        {
            SetMasterVolume(volume);
        }
        public void PlayWalkingSound(string soundFilePath)
        {
            StopWalkingSound();
            walkingSoundReader = new AudioFileReader(soundFilePath)
            {
                Volume = masterVolume
            };
            walkingSoundLoop = new LoopStream(walkingSoundReader);

            walkingSoundPlayer = new WaveOutEvent();
            walkingSoundPlayer.Init(walkingSoundLoop);
            walkingSoundPlayer.Play();

            CurrentWalkingSoundPath = soundFilePath;
        }
        public void StopWalkingSound()
        {
            if (walkingSoundPlayer != null)
            {
                walkingSoundPlayer.Stop();
                walkingSoundPlayer.Dispose();
                walkingSoundPlayer = null;
            }
            CurrentWalkingSoundPath = null;
        }
        public void PlayClickSound(string soundFilePath)
        {
            StopClickSound();
            clickSoundReader = new AudioFileReader(soundFilePath)
            {
                Volume = masterVolume
            };

            clickSoundPlayer = new WaveOutEvent();
            clickSoundPlayer.Init(clickSoundReader);
            clickSoundPlayer.Play();
        }
        public void StopClickSound()
        {
            if (clickSoundPlayer != null)
            {
                clickSoundPlayer.Stop();
                clickSoundPlayer.Dispose();
                clickSoundPlayer = null;
            }
            if (clickSoundReader != null)
            {
                clickSoundReader.Dispose();
                clickSoundReader = null;
            }
        }
        public void PlayDialogueSound(string soundFilePath)
        {
            StopDialogueSound();
            dialogueSoundReader = new AudioFileReader(soundFilePath)
            {
                Volume = masterVolume
            };

            dialogueSoundPlayer = new WaveOutEvent();
            dialogueSoundPlayer.Init(dialogueSoundReader);
            dialogueSoundPlayer.Play();
        }
        public void StopDialogueSound()
        {
            if (dialogueSoundPlayer != null)
            {
                dialogueSoundPlayer.Stop();
                dialogueSoundPlayer.Dispose();
                dialogueSoundPlayer = null;
            }
            if (dialogueSoundReader != null)
            {
                dialogueSoundReader.Dispose();
                dialogueSoundReader = null;
            }
        }
        public void Dispose()
        {
            StopMainMusic();
            StopWalkingSound();
            StopClickSound();
            StopDialogueSound();
            wavePlayer.Dispose();
        }
    }

    public class LoopStream : WaveStream
    {
        private readonly WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat => sourceStream.WaveFormat;

        public override long Length => sourceStream.Length;

        public override long Position
        {
            get => sourceStream.Position;
            set => sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        break;
                    }

                    // Looping
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }
    }
}

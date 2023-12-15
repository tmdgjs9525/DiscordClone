using DiscordClone.Core.Models;
using DiscordClone.Core.Util;
using Microsoft.AspNet.SignalR.Client;
using MySqlConnector;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordClone.MasterChannel.Util
{
    public class SignalR
    {
        public IHubProxy _hubProxy { get; private set; }
        private IWaveIn waveIn;

        private SignalR()
        {
        }
        private static SignalR instance;
        public static SignalR Instance()
        {
            if (instance == null)
            {
                instance = new SignalR();
            }
            return instance;
        }


        public void StartSignalR()
        {
            var connection = new HubConnection(@"http://125.184.186.132:8080");
            _hubProxy = connection.CreateHubProxy("MyHub");

            connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.WriteLine($"Error connecting to SignalR: {task.Exception}");
                }
                else
                {
                    Debug.WriteLine("Connected to SignalR");
                    User user = User.Instance();
                    // Send the message to the server
                    _hubProxy.Invoke("MatchGuidWithUserName", user.userName,user.guid);
                    
                }
            });
            _hubProxy.On<byte[]>("ReceiveAudio", async buffer =>
            {
                Debug.WriteLine(buffer.Length);
                playAudioAsync(buffer);
            });
            _hubProxy.On<string>("ReceiveMessage",  message =>
            {
                Debug.WriteLine(message);
            });

            
        }

        public void SendMessageToFriend(Guid friendGuid,string messsage)
        {
            _hubProxy.Invoke("SendMessage", friendGuid, messsage);
        }
        private void StartAudioCapture()
        {
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(16000,1);
            waveIn.DataAvailable += async (sender, e) =>
            {
                // Send audio data to the server
                byte[] audioBuffer = new byte[e.BytesRecorded];
                Buffer.BlockCopy(e.Buffer, 0, audioBuffer, 0, e.BytesRecorded);
                await Task.Run(() => _hubProxy.Invoke("SendAudio", audioBuffer));
            };

            waveIn.StartRecording();
        }

        private async Task playAudioAsync(byte[] buffer)
        {
            try
            {
                if (buffer != null)
                {
                    // 처리 로직...
                    WaveFormat waveFormat = new WaveFormat(16000, 16, 1);
                    RawSourceWaveStream waveStream = new RawSourceWaveStream(buffer, 0, buffer.Length, waveFormat);
                    await Task.Run(() =>
                    {
                        using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                        {
                            waveOut.DesiredLatency = 180;
                            waveOut.Init(waveStream);
                            waveOut.Play();
                            while (waveOut.PlaybackState == PlaybackState.Playing)
                            {
                                Thread.Sleep(200);
                            }
                        }
                    });
                }
            }
            catch
            {

            }
        }
    }
}

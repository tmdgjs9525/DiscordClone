using DiscordClone.Core.Models;
using DiscordClone.Core.Util;
using DiscordClone.MasterChannel.Models;
using Microsoft.AspNet.SignalR.Client;
using MySqlConnector;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordClone.MasterChannel.Util
{
    public class SignalR
    {
        public IHubProxy _hubProxy { get; private set; }
        private IWaveIn waveIn;
        public delegate void receiveMesssageDelegate(Message message);
        receiveMesssageDelegate receiveMesssage;

        public delegate void receiveJoinChatRoomDelegate(Friend friend);
        receiveJoinChatRoomDelegate receiveJoinChatRoom;
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
        public void setRecieveMessageDelegate(receiveMesssageDelegate method)
        {
            receiveMesssage = method;
        }
        public void setRecevieJoinChatRoom(receiveJoinChatRoomDelegate method)
        {
            receiveJoinChatRoom = method;
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
                    Debug.WriteLine($"aaaaaaaaaaaaaaaaaaa");
                }
                else
                {
                    Debug.WriteLine("Connected to SignalR");

                }
            });
            _hubProxy.On<byte[]>("ReceiveAudio", buffer =>
            {
                Debug.WriteLine(buffer.Length);
                playAudioAsync(buffer);
            });
            _hubProxy.On<string>("test", message =>
            {
                Debug.WriteLine(message);
            });

            _hubProxy.On<Message>("ReceiveMessage",  message =>
            {
                receiveMesssage(message);
                Debug.WriteLine(message);
            });
            _hubProxy.On<Friend>("joinDirectChatRoom", friend =>
            {
                receiveJoinChatRoom(friend);
            });

        }
        public void joinDirectChatRoom(Friend currentUser,Guid friendGuid)
        {
            _hubProxy.Invoke("joinDirectChatRoom", currentUser , friendGuid);
        }
        public void ChatRoomOnLive(Guid userGuid,Guid friendGuid)
        {
            _hubProxy.Invoke("chatRoomOnLive", userGuid, friendGuid);
        }
        public async Task<bool> readChatRoomOnLive(Guid userGuid, Guid friendGuid)
        {
            return await _hubProxy.Invoke<bool>("isChatRoomOnLive", userGuid, friendGuid);
        }
        public void MatchGuidWithUserName(Guid userGuid)
        {
            _hubProxy.Invoke("MatchGuidWithUserName", userGuid);

        }
        public void sendMessage(Guid friendUserGuid, Message message)
        {
            _hubProxy.Invoke<List<Message>>("sendMessage", friendUserGuid, message);
        }
        public async Task<List<Message>> getDirectMessages(Guid userGuid,Guid friendUserGuid)
        {
            List<Message> messages = null;
            try
            {
                messages = await _hubProxy.Invoke<List<Message>>("getDirectMessages", userGuid, friendUserGuid);
                Debug.WriteLine(messages);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }
            return messages;
        }

        public async Task<User> loginAsync(string userId, string password)
        {
            User user = null;
            try
            {
                user = await _hubProxy.Invoke<User>("login", userId, password);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }
            return user;
        }
        public void SendMessageToFriend(Guid friendGuid,string messsage)
        {
            _hubProxy.Invoke("SendMessage", friendGuid, messsage);
        }
        public void StartAudioCapture(Guid receiverGuid)
        {
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(16000,1);
            waveIn.DataAvailable += async (sender, e) =>
            {
                // Send audio data to the server
                byte[] audioBuffer = new byte[e.BytesRecorded];
                Buffer.BlockCopy(e.Buffer, 0, audioBuffer, 0, e.BytesRecorded);
                await Task.Run(() => _hubProxy.Invoke("SendAudio", audioBuffer,receiverGuid));
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

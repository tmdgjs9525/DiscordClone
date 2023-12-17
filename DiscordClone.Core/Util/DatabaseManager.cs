using DiscordClone.Core.Models;
using DiscordClone.MasterChannel.Models;
using Microsoft.VisualBasic.ApplicationServices;
using MySqlConnector;
using NAudio.Wave.Asio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Message = DiscordClone.Core.Models.Message;
using User = DiscordClone.Core.Models.User;

namespace DiscordClone.Core.Util
{
    public class DatabaseManager
    {
        string connectionString = "server=125.184.186.132;user=user;database=discord;port=3306;password=qwer1;";

        private DatabaseManager()
        {
        }
        private static DatabaseManager instance;
        public static DatabaseManager Instance()
        {
            if (instance == null)
            {
                instance = new DatabaseManager();
            }
            return instance;
        }

        public List<Message> GetDirectMessages(User user,Guid friendUserGuid)
        {
            Guid chatRoomGuid;
            List<Message> messages = new List<Message>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                Guid chatRoomID = Guid.NewGuid();
                //채팅방 생성
                string findDirectChatnRoomGuidQuery = "SELECT F.chatRoomId FROM friendships " +
                       $"F WHERE (F.UserID1 = '{user.guid}' " +
                       $"AND F.UserID2 = '{friendUserGuid}')";
                using (MySqlCommand cmd = new MySqlCommand(findDirectChatnRoomGuidQuery, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        chatRoomGuid = (Guid)reader["chatRoomId"];
                    }
                }

                //채팅방 guid로 메세지 받아오는 부분
                string getMessagesQuery = $"SELECT M.SenderID, U.Username AS SenderName, M.MessageContent, M.Timestamp " +
                    $"FROM messages M " +
                    $"INNER JOIN users U ON M.SenderID = U.Guid " +
                    $"WHERE M.chatroom = '{chatRoomGuid}' " +
                    $"ORDER BY M.Timestamp ASC;";
                Guid beforeSenderGuid = new Guid();
                using (MySqlCommand cmd = new MySqlCommand(getMessagesQuery, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Guid senderGuid = (Guid)reader["SenderID"];
                            string dbMessage = reader.GetString("MessageContent");
                            string dbUserName = reader.GetString("SenderName");
                            DateTime Time = reader.GetDateTime(reader.GetOrdinal("Timestamp"));
                            if (!senderGuid.Equals(beforeSenderGuid))
                            {
                                messages.Add(new Message(Time, dbUserName, senderGuid));

                            }
                            messages.Last().messages.Add(dbMessage);
                            beforeSenderGuid = senderGuid;
                        }
                    }
                }
                 
            }
            return messages;
        }
        public void AddFriendAndCreateChatRoom(Guid userGuid,Guid friendUserGuid, string chatRoomName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                Guid chatRoomID = Guid.NewGuid();
                //채팅방 생성
                string createChatRoomQuery = $"INSERT INTO chatroomdirect (RoomID,User1,User2) VALUES " +
                    $"(\"{chatRoomID}\",\"{userGuid}\",\"{friendUserGuid}\")";
                using (MySqlCommand cmd = new MySqlCommand(createChatRoomQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                //친구추가
                string addFriendships = $"INSERT INTO friendships (UserID1,UserID2,ChatRoomID) VALUES " +
                    $"(\"{userGuid}\",\"{friendUserGuid}\",\"{chatRoomID}\")";
                using (MySqlCommand cmd = new MySqlCommand(addFriendships, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                string addFriendshipsOther = $"INSERT INTO friendships (UserID1,UserID2,ChatRoomID) VALUES " +
                    $"(\"{friendUserGuid}\",\"{userGuid}\",\"{chatRoomID}\")";
                using (MySqlCommand cmd = new MySqlCommand(addFriendshipsOther, connection))
                {
                    cmd.ExecuteNonQuery();
                }

            }
        }
        public Friend SearchFriend(Guid userGuid,string userId)
        {
            Friend? friend = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = $"Select * FROM users where (userid = \"{userId}\")";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string dbUserId = reader.GetString("UserID");
                            string dbUserName = reader.GetString("UserName");
                            Guid guid = (Guid)reader["Guid"];
                            if (!guid.Equals(userGuid))
                            {
                                friend = new Friend(dbUserId, dbUserName, guid, FriendState.ONLINE, false);
                            }
                        }
                    }
                }
            }
            return friend ;
        }
        private Guid findFriendChatRoomID(Guid currentUserGuid, Guid friendUserGuid)
        {
            Guid chatroomID;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string findChatRoomQuery = $"SELECT chatRoomId FROM friendships " +
                    $"WHERE (UserID1 = \'{currentUserGuid}\' AND UserID2 = \'{friendUserGuid}\')";
                using (MySqlCommand cmd = new MySqlCommand(findChatRoomQuery, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        chatroomID = (Guid)reader["chatRoomId"];
                    }
                }
            }
            return chatroomID;
        }
        public void AddMessage(Guid userGuid,Guid friendUserGuid, string message)
        {
            Guid chatRoomId = findFriendChatRoomID (userGuid, friendUserGuid);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string createChatRoomQuery = $"INSERT INTO messages (chatroom,SenderID,MessageContent) VALUES " +
                    $"(\"{chatRoomId}\",\"{userGuid}\",\"{message}\")";
                using (MySqlCommand cmd = new MySqlCommand(createChatRoomQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            }

        public bool accountUserNameDuplicationCheck(string userId)
        {

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = $"Select userid FROM users";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 여기서 데이터를 읽고 활용
                            string dbUserId = reader.GetString("UserID");
                            if (userId.Equals(dbUserId))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool createAccount(string userID,string username, string password, Guid guid)
        {
            bool isDuplication = accountUserNameDuplicationCheck(username);

            if (isDuplication == true)
            {
                return false;
            }
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();


                string query = $"INSERT INTO users (userID,Username,Password,Guid) values(\"{userID}\"\"{username}\",\"{password}\",\"{guid.ToString()}\")";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            return true;

        }

        public List<Friend> loadFriends(Guid userGuid)
        {
            List<Friend> friends = new List<Friend>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT U.Guid, U.UserID, U.username FROM Friendships F " +
                    "JOIN Users U ON F.UserID2 = U.Guid " +
                    $"WHERE F.UserID1 = \"{userGuid}\"";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 여기서 데이터를 읽고 활용
                            string dbUserId = reader.GetString("UserID");
                            string dbUserName = reader.GetString("UserName");
                            Guid guid = (Guid)reader["Guid"];
                            friends.Add(new Friend(dbUserId,dbUserName, guid, FriendState.ONLINE,true));
                        }
                    }

                }
                connection.Close();
            }
            return friends;
        }
        public bool login(ref User user,string userId, string password)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();


                // 데이터 조회
                string query = "SELECT * FROM users;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 여기서 데이터를 읽고 활용
                            string dbUserId = reader.GetString("UserID");
                            string dbUserName = reader.GetString("UserName");
                            string dbPassword = reader.GetString("Password");
                            Guid guid = (Guid)reader["Guid"];
                            if (userId.Equals(dbUserName) && password.Equals(dbPassword))
                            {
                                user = new User(dbUserId,dbUserName, password, guid);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}

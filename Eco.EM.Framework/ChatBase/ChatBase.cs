using System;
using Eco.Core.Plugins.Interfaces;
using Eco.EM.Framework.Logging;
using Eco.EM.Framework.Utils;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat;
using Eco.Gameplay.Systems.Messaging.Mail;
using Eco.Gameplay.Systems.Messaging.Notifications;
using Eco.Shared.IoC;
using Eco.Shared.Localization;
using Eco.Shared.Services;
using Eco.Shared.Utils;

namespace Eco.EM.Framework.ChatBase
{
    public class ChatBase : IModKitPlugin
    {
        public enum RecipientType
        {
            Server,
            Player
        }
        public enum MessageType
        {
            Temporary,
            Permanent,
            InfoPanel,
            CustomInfo,
            OkBox,
            GlobalAnnoucement
        }
        public enum PanelType
        {
            InfoPanel,
            CustomPanel
        }

        /// <summary>
        /// Announcements Can only be made to the server and can show as either a Popup Box or an Info Panel
        /// you can specify if you would like to use a popup box or the Info Panel
        /// To Specify if its a popup or Info panel you just need to enter 2 strings at the start or just one
        /// a single string will make it a popup, 2 strings will make it an info panel, 
        /// The announcement doesn't support use of the Custom Info Panel though
        /// Remember Info Panels can be customized
        /// Announcements will also show up in the chat In the General Section
        /// An announcement will only show for online users, for offline users as well use Global Announcement
        /// </summary>
        public struct Announcement
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;

            public string Title;
            public string Content;
            public NotificationStyle NotificationStyle;
            public NotificationCategory NotificationCategory;
            public MessageType MessageType;
            public string Instance;
            public bool SendToAll;
            public bool SendToChat;

            public Announcement(string content, bool sendToAll = false, bool sendToChat = false)
            {
                Title = string.Empty;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = MessageType.OkBox;
                SendToAll = sendToAll;
                SendToChat = sendToChat;
                Instance = null;
            }

            public Announcement(string title, string content, string instance, bool sendToAll = false, bool sendToChat = false)
            {
                Title = title;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = MessageType.InfoPanel;
                SendToAll = sendToAll;
                SendToChat = sendToChat;
                Instance = instance;
            }

            public Announcement(string title, string content, string instance, MessageType messageType, bool sendToAll = false, bool sendToChat = false)
            {
                Title = title;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = messageType;
                SendToAll = sendToAll;
                SendToChat = sendToChat;
                Instance = instance;
            }
        }

        /// <summary>
        /// Chat is used to Send Only to the Server Chat
        /// It can be sent to a player or the entire server
        /// You can also specify which channel it is posted too, Default is General
        /// MessageType = Temp Or Permanent Message - Default is Temporary
        /// NotificationCategory = Which Channel to post the message too IE: General, Trades, Notifications etc - Default Is General
        /// </summary>
        public struct Chat
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public bool SendingUser;


            public RecipientType RecipientType; //Server Or User
            public MessageType MessageType; //Temp Or Not 

            public string Content;
            public NotificationStyle NotificationStyle;
            public NotificationCategory NotificationCategory;

            public Chat(string content, User user, bool sendingUser = false)
            {
                Content = content;
                Player = user.Player ?? null;
                User = user ?? null;
                RecipientType = sendingUser ? RecipientType.Server : RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = MessageType.Temporary;
                SendingUser = sendingUser;
            }

            public Chat(string content, User user, MessageType messageType, bool sendingUser = false)
            {
                Content = content;
                Player = user.Player ?? null;
                User = user ?? null;
                RecipientType = sendingUser ? RecipientType.Server : RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = messageType;
                SendingUser = sendingUser;
            }

            public Chat(string content, User user, NotificationCategory defaultChatTags, bool sendingUser = false)
            {
                Content = content;
                Player = user.Player ?? null;
                User = user ?? null;
                RecipientType = sendingUser ? RecipientType.Server : RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = defaultChatTags;
                MessageType = MessageType.Temporary;
                SendingUser = sendingUser;
            }

            public Chat(string content, User user, MessageType messageType, NotificationCategory defaultChatTags, bool sendingUser = false)
            {
                Content = content;
                Player = user.Player ?? null;
                User = user ?? null;
                RecipientType = sendingUser ? RecipientType.Server : RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = defaultChatTags;
                MessageType = messageType;
                SendingUser = sendingUser;
            }

            public Chat(string content, Player player)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = MessageType.Temporary;
                SendingUser = false;
            }

            public Chat(string content, Player player, MessageType messageType)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = messageType;
                SendingUser = false;
            }

            public Chat(string content, Player player, NotificationCategory defaultChatTags)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = defaultChatTags;
                MessageType = MessageType.Temporary;
                SendingUser = false;
            }

            public Chat(string content, Player player, MessageType messageType, NotificationCategory defaultChatTags)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = defaultChatTags;
                MessageType = messageType;
                SendingUser = false;
            }
        }

        /// <summary>
        /// Error is the same as the Error box shown to players, this can be sent to all players or a single player
        /// this only accepts content input
        /// </summary>
        public struct Error
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;

            public string Content;
            public NotificationStyle NotificationStyle;

            public bool SendToChat;

            public Error(string content, bool sendToChat)
            {
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Error;
                SendToChat = sendToChat;
            }

            public Error(string content, User user, bool sendToChat)
            {
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Error;
                SendToChat = sendToChat;
            }

            public Error(string content, Player player, bool sendToChat)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Error;
                SendToChat = sendToChat;
            }
        }

        /// <summary>
        /// Info Sends a little info panel down the bottom similar to the Error and Warning just in a light style box with light text
        /// Info boxes can be sent to a single player or all players on the server
        /// </summary>
        public struct Info
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;

            public string Content;
            public NotificationStyle NotificationStyle;

            public bool SendToChat;

            public Info(string content, bool sendToChat)
            {
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Info;
                SendToChat = sendToChat;
            }

            public Info(string content, User user, bool sendToChat)
            {
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Info;
                SendToChat = sendToChat;
            }

            public Info(string content, Player player, bool sendToChat)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Info;
                SendToChat = sendToChat;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public struct InfoBox
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;

            public string Content;
            public NotificationStyle NotificationStyle;

            public bool SendToChat;

            public InfoBox(string content, bool sendToChat)
            {
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.InfoBox;
                SendToChat = sendToChat;
            }

            public InfoBox(string content, User user, bool sendToChat)
            {
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.InfoBox;
                SendToChat = sendToChat;
            }

            public InfoBox(string content, Player player, bool sendToChat)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.InfoBox;
                SendToChat = sendToChat;
            }
        }

        /// <summary>
        /// While it may be a little on the confusing side, The InfoPane Struct allows for using the InfoPanel or the CustomPanel
        /// This is used for displaying information in a popup info box that can be resized, contain large messages or just for a nicer feel when sending 
        /// large amounts of info to players
        /// This will also post the message to the Notifications Channel as a permanent Message so users can look back on them, this can be sent to all online players 
        /// or just a single player
        /// you may also set it too temp if you like
        /// NotificationCategory = Which Channel to post the message too IE: General, Trades, Notifications etc - Default Is General
        /// MessageType is a bool - False = Send Info Panel, True = Send Custom Info Panel - Default is False
        /// TempMessage is a bool - False = Sends a Permanent Message to the Notifications part in the chat, True = Send a Temporary Notification to the chat
        /// Some Players May miss the info panel so as a secondary measure we added the notifications addition just incase people miss it and can go back and 
        /// have a look
        /// </summary>
        public struct InfoPane
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;
            public PanelType PanelType;
            public bool TempMessage;
            public bool SendToChat;

            public string Title;
            public string Content;
            public string Instance;
            public NotificationStyle ChatCategory;
            public NotificationCategory NotificationCategory;

            public InfoPane(string title, string content, string instance, bool sendToChat)
            {
                Title = title;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                ChatCategory = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                PanelType = PanelType.InfoPanel;
                TempMessage = false;
                SendToChat = sendToChat;
                Instance = instance;
            }
            public InfoPane(string title, string content, string instance, PanelType usePanelType, bool sendToChat)
            {
                Title = title;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                ChatCategory = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                PanelType = usePanelType;
                TempMessage = false;
                SendToChat = sendToChat;
                Instance = instance;
            }
            public InfoPane(string title, string content, string instance, PanelType usePanelType, bool tempMessage, bool sendToChat)
            {
                Title = title;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                ChatCategory = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                PanelType = usePanelType;
                TempMessage = tempMessage;
                SendToChat = sendToChat;
                Instance = instance;
            }
            public InfoPane(string title, string content, string instance, User user, bool sendToChat)
            {
                Title = title;
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                ChatCategory = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                PanelType = PanelType.InfoPanel;
                TempMessage = false;
                SendToChat = sendToChat;
                Instance = instance;
            }
            public InfoPane(string title, string content, string instance, User user, PanelType usePanelType, bool sendToChat)
            {
                Title = title;
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                ChatCategory = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                PanelType = usePanelType;
                TempMessage = false;
                SendToChat = sendToChat;
                Instance = instance;
            }
            public InfoPane(string title, string content, string instance, User user, PanelType usePanelType, bool tempMessage, bool sendToChat)
            {
                Title = title;
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                ChatCategory = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                PanelType = usePanelType;
                TempMessage = tempMessage;
                SendToChat = sendToChat;
                Instance = instance;
            }
            public InfoPane(string title, string content, string instance, Player player, bool sendToChat)
            {
                Title = title;
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                ChatCategory = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                PanelType = PanelType.InfoPanel;
                TempMessage = false;
                SendToChat = sendToChat;
                Instance = instance;
            }
            public InfoPane(string title, string content, string instance, Player player, PanelType usePanelType, bool sendToChat)
            {
                Title = title;
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                ChatCategory = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                PanelType = usePanelType;
                TempMessage = false;
                SendToChat = sendToChat;
                Instance = instance;
            }
            public InfoPane(string title, string content, string instance, Player player, PanelType usePanelType, bool tempMessage, bool sendToChat)
            {
                Title = title;
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                ChatCategory = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                PanelType = usePanelType;
                TempMessage = tempMessage;
                SendToChat = sendToChat;
                Instance = instance;
            }
        }

        /// <summary>
        /// Mail sends a new "Mail Message" to a selected users Mailbox ( The notifications bar on the right)
        /// You can specify a tag for this message but im not sure what the tags actually do.. Default is Notification
        /// These can be sent to the entire server (even offline players) or a single player
        /// </summary>
        public struct Mail
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;

            public string Content;
            public string Tag;

            public Mail(string content)
            {
                Content = content;
                Player = null;
                User = null;
                Tag = "Notifications";
                RecipientType = RecipientType.Server;
            }
            public Mail(string content, string tag)
            {
                Content = content;
                Player = null;
                User = null;
                Tag = tag;
                RecipientType = RecipientType.Server;
            }
            public Mail(string content, User user)
            {
                Content = content;
                Player = user.Player;
                User = user;
                Tag = "Notifications";
                RecipientType = RecipientType.Player;
            }
            public Mail(string content, string tag, User user)
            {
                Content = content;
                Player = user.Player;
                User = user;
                Tag = tag;
                RecipientType = RecipientType.Player;
            }
            public Mail(string content, Player player)
            {
                Content = content;
                Player = player;
                User = player.User;
                Tag = "Notifications";
                RecipientType = RecipientType.Player;
            }
            public Mail(string content, string tag, Player player)
            {
                Content = content;
                Player = player;
                User = player.User;
                Tag = tag;
                RecipientType = RecipientType.Player;
            }
        }

        /// <summary>
        /// ChatBase.Message is an older function but it is the most flexible,
        /// this may require some advanced knowledge to use
        /// </summary>
        public struct Message
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;
            public MessageType MessageType;

            public string Title;
            public string Content;
            public NotificationStyle NotificationStyle;
            public NotificationCategory NotificationCategory;

            public Message(string content)
            {
                Title = string.Empty;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = MessageType.Temporary;
            }
            public Message(string content, MessageType messageType)
            {
                Title = string.Empty;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = messageType;
            }
            public Message(string content, NotificationCategory defaultChatTags)
            {
                Title = string.Empty;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = defaultChatTags;
                MessageType = MessageType.Temporary;
            }
            public Message(string content, NotificationStyle messageCategory, NotificationCategory defaultChatTags)
            {
                Title = string.Empty;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = messageCategory;
                NotificationCategory = defaultChatTags;
                MessageType = MessageType.Temporary;
            }
            public Message(string content, NotificationStyle messageCategory, NotificationCategory defaultChatTags, MessageType messageType)
            {
                Title = string.Empty;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = messageCategory;
                NotificationCategory = defaultChatTags;
                MessageType = messageType;
            }
            public Message(string title, string content)
            {
                Title = title;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = string.IsNullOrWhiteSpace(title) ? MessageType.OkBox : MessageType.InfoPanel;
            }
            public Message(string title, string content, MessageType messageType)
            {
                Title = title;
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = messageType;
            }
            public Message(string content, User user)
            {
                Title = string.Empty;
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = MessageType.Temporary;
            }
            public Message(string content, User user, MessageType messageType)
            {
                Title = string.Empty;
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = messageType;
            }
            public Message(string content, User user, NotificationCategory defaultChatTags)
            {
                Title = string.Empty;
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = defaultChatTags;
                MessageType = MessageType.Temporary;
            }
            public Message(string content, User user, NotificationStyle messageCategory, NotificationCategory defaultChatTags)
            {
                Title = string.Empty;
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = messageCategory;
                NotificationCategory = defaultChatTags;
                MessageType = MessageType.Temporary;
            }
            public Message(string content, User user, NotificationStyle messageCategory, NotificationCategory defaultChatTags, MessageType messageType)
            {
                Title = string.Empty;
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = messageCategory;
                NotificationCategory = defaultChatTags;
                MessageType = messageType;
            }
            public Message(string title, string content, User user)
            {
                Title = title;
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = string.IsNullOrWhiteSpace(title) ? MessageType.OkBox : MessageType.InfoPanel;
            }
            public Message(string content, Player player)
            {
                Title = string.Empty;
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = MessageType.Temporary;
            }
            public Message(string content, Player player, MessageType messageType)
            {
                Title = string.Empty;
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = messageType;
            }
            public Message(string content, Player player, NotificationCategory defaultChatTags)
            {
                Title = string.Empty;
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = defaultChatTags;
                MessageType = MessageType.Temporary;
            }
            public Message(string content, Player player, NotificationStyle messageCategory, NotificationCategory defaultChatTags)
            {
                Title = string.Empty;
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = messageCategory;
                NotificationCategory = defaultChatTags;
                MessageType = MessageType.Temporary;
            }
            public Message(string content, Player player, NotificationStyle messageCategory, NotificationCategory defaultChatTags, MessageType messageType)
            {
                Title = string.Empty;
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = messageCategory;
                NotificationCategory = defaultChatTags;
                MessageType = messageType;
            }
            public Message(string title, string content, Player player)
            {
                Title = title;
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Chat;
                NotificationCategory = NotificationCategory.Notifications;
                MessageType = string.IsNullOrWhiteSpace(title) ? MessageType.OkBox : MessageType.InfoPanel;
            }

        }

        /// <summary>
        /// OkBox is a Popup Ok Box
        /// this can be used for when you want to make sure the user got the message
        /// </summary>
        public struct OkBox
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;

            public string Content;

            public OkBox(string content)
            {
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
            }

            public OkBox(string content, User user)
            {
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
            }

            public OkBox(string content, Player player)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
            }
        }

        /// <summary>
        /// Warning Sends a little Warning Box to the player, For Completion's sake we have also made it so this can be sent to all players online as well
        /// this only accepts a message and a user
        /// </summary>
        public struct Warning
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;

            public string Content;
            public NotificationStyle NotificationStyle;

            public bool SendToChat;

            public Warning(string content, bool sendToChat)
            {
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Warning;
                SendToChat = sendToChat;
            }

            public Warning(string content, User user, bool sendToChat)
            {
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Warning;
                SendToChat = sendToChat;
            }

            public Warning(string content, Player player, bool sendToChat)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Warning;
                SendToChat = sendToChat;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public struct Whisper
        {
            public Player Player;   // Required for RecipientType.Player;
            public User User;       // Required for RecipientType.User;
            public RecipientType RecipientType;

            public string Content;
            public NotificationStyle NotificationStyle;

            public Whisper(string content)
            {
                Content = content;
                Player = null;
                User = null;
                RecipientType = RecipientType.Server;
                NotificationStyle = NotificationStyle.Info;
            }

            public Whisper(string content, User user)
            {
                Content = content;
                Player = user.Player;
                User = user;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Info;
            }

            public Whisper(string content, Player player)
            {
                Content = content;
                Player = player;
                User = player.User;
                RecipientType = RecipientType.Player;
                NotificationStyle = NotificationStyle.Info;
            }
        }

        #region Announcement
        public static bool Send(Announcement Message)
        {
            return Message.RecipientType switch
            {
                RecipientType.Server => SendToServer(Message),
                RecipientType.Player => false,
                _ => false,
            };
        }
        internal static bool SendToServer(Announcement Message)
        {
            try
            {
                switch (Message.MessageType)
                {
                    case MessageType.InfoPanel:
                        foreach (var user in PlayerUtils.OnlineUsers)
                        {
                            user.Player.OpenInfoPanel(Message.Title, Message.Content, Message.NotificationStyle.ToString());
                        }
                        break;
                    case MessageType.CustomInfo:
                        foreach (var user in PlayerUtils.OnlineUsers)
                        {
                            user.Player.OpenCustomPanel(Message.Title, Message.Content, Message.NotificationStyle.ToString());
                        }
                        break;
                    case MessageType.OkBox:
                        foreach (var user in PlayerUtils.OnlineUsers)
                        {
                            user.Player.OkBoxLoc($"{Message.Content}");
                        }
                        break;
                }
                if (Message.SendToAll)
                    NotificationManager.GlobalNotification(Localizer.DoStr($"{Message.Content}"), Message.NotificationCategory, true);
                if (Message.SendToChat)
                    NotificationManager.ServerMessageToAll(Localizer.DoStr(Message.Content));
                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region Chat
        public static bool Send(Chat Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.Player == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(Chat Message)
        {
            switch (Message.MessageType)
            {
                case MessageType.Temporary:
                    NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User, Message.NotificationCategory, Message.NotificationStyle, true);
                    break;
                case MessageType.Permanent:
                    NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User, Message.NotificationCategory, Message.NotificationStyle, false);
                    break;
            }

            return true;
        }
        internal static bool SendToServer(Chat Message)
        {
            var chatReceiver = ChatParsingUtils.ResolveReceiver(Message.Content, out string message);
            if (chatReceiver.Failed)
            {
                LoggingUtils.Debug(chatReceiver.Message);
                return false;
            }
            try
            {
                switch (Message.MessageType)
                {
                    case MessageType.Temporary:
                        NotificationManager.ServerMessageToAll(Localizer.DoStr(Message.Content), Message.NotificationCategory, Message.NotificationStyle, forceTemporary: true);
                        break;
                    case MessageType.Permanent:
                        ServiceHolder<IChatManager>.Obj.Send(Message.User, chatReceiver.Val, Localizer.DoStr(message));
                        break;
                    case MessageType.InfoPanel:
                        throw new NotImplementedException("Server Message to all using this system does not support Info Panel");
                    case MessageType.CustomInfo:
                        throw new NotImplementedException("Server Message to all using this system does not support Custom Info Panel");
                    case MessageType.OkBox:
                        throw new NotImplementedException("Server Message to all using this system does not support Ok Box");
                    case MessageType.GlobalAnnoucement:
                        throw new NotImplementedException("Server Message to all using this system does not support Global Announcement");
                    default:
                        throw new NotImplementedException("You need to provide a MessageType to use this method");
                }
                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region Error
        public static bool Send(Error Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.Player == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(Error Message)
        {
            Message.Player.Msg(Localizer.DoStr(Shared.Utils.Text.Error(Message.Content)), Message.NotificationStyle);
            if (Message.SendToChat)
                NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User);
            return true;
        }
        internal static bool SendToServer(Error Message)
        {
            try
            {
                foreach (var user in PlayerUtils.OnlineUsers)
                {
                    user.Player.Msg(Localizer.DoStr(Shared.Utils.Text.Error(Message.Content)), Message.NotificationStyle);
                    if (Message.SendToChat)
                        NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User);
                }
                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region Info
        public static bool Send(Info Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.Player == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(Info Message)
        {
            Message.Player.Msg(Localizer.DoStr(Shared.Utils.Text.Info(Message.Content)), Message.NotificationStyle);
            if (Message.SendToChat)
                NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User);
            return true;
        }
        internal static bool SendToServer(Info Message)
        {
            try
            {
                foreach (var user in PlayerUtils.OnlineUsers)
                {
                    user.Player.Msg(Localizer.DoStr(Shared.Utils.Text.Info(Message.Content)), Message.NotificationStyle);
                    if (Message.SendToChat)
                        NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User);
                }
                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region InfoBox
        public static bool Send(InfoBox Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.Player == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(InfoBox Message)
        {
            Message.Player.Msg(Localizer.DoStr(Shared.Utils.Text.InfoLight(Message.Content)), Message.NotificationStyle);
            if (Message.SendToChat)
                NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User);
            return true;
        }
        internal static bool SendToServer(InfoBox Message)
        {
            try
            {
                foreach (var user in PlayerUtils.OnlineUsers)
                {
                    user.Player.Msg(Localizer.DoStr(Shared.Utils.Text.InfoLight(Message.Content)), Message.NotificationStyle);
                    if (Message.SendToChat)
                        NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User);
                }
                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region InfoPane
        public static bool Send(InfoPane Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.Player == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(InfoPane Message)
        {
            switch (Message.PanelType)
            {
                case PanelType.CustomPanel:
                    Message.Player.OpenCustomPanel(Message.Title, Message.Content, Message.Instance);
                    break;
                case PanelType.InfoPanel:
                    Message.Player.OpenInfoPanel(Message.Title, Message.Content, Message.Instance);
                    break;
            }
            if (Message.SendToChat)
            {
                if (Message.TempMessage)
                    NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User, Message.NotificationCategory, Message.ChatCategory, true);
                else
                    NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User, Message.NotificationCategory, Message.ChatCategory, false);
            }
            return true;
        }
        internal static bool SendToServer(InfoPane Message)
        {
            try
            {
                switch (Message.PanelType)
                {
                    case PanelType.CustomPanel:
                        foreach (var user in PlayerUtils.OnlineUsers)
                        {
                            user.Player.OpenCustomPanel(Message.Title, Message.Content, Message.Instance);
                        }
                        break;
                    case PanelType.InfoPanel:
                        foreach (var user in PlayerUtils.OnlineUsers)
                        {
                            user.Player.OpenInfoPanel(Message.Title, Message.Content, Message.Instance);
                        }
                        break;
                }
                if (Message.TempMessage)
                    NotificationManager.ServerMessageToAll(Localizer.DoStr(Message.Content), NotificationCategory.Notifications, Message.ChatCategory, forceTemporary: true);
                else
                    NotificationManager.ServerMessageToAll(Localizer.DoStr(Message.Content));

                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region Mail
        public static bool Send(Mail Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.User == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(Mail Message)
        {
            var mailMessage = new MailMessage(Message.Content, Message.Tag);

            Message.User.Mailbox.Add(mailMessage, !Message.User.IsOnline);
            return true;
        }
        internal static bool SendToServer(Mail Message)
        {
            try
            {
                var mailMessage = new MailMessage(Message.Content, Message.Tag);
                foreach (var user in PlayerUtils.Users)
                {
                    user.Mailbox.Add(mailMessage, !user.IsOnline);
                }
                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region Message
        public static bool Send(Message Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.Player == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(Message Message)
        {
            switch (Message.MessageType)
            {
                case MessageType.InfoPanel:
                    Message.Player.OpenInfoPanel(Message.Title, Message.Content, Message.NotificationStyle.ToString());
                    break;
                case MessageType.CustomInfo:
                    Message.Player.OpenCustomPanel(Message.Title, Message.Content, Message.NotificationStyle.ToString());
                    break;
                case MessageType.OkBox:
                    Message.Player.OkBoxLoc($"{Message.Content}");
                    break;
                case MessageType.Temporary:
                    NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User, Message.NotificationCategory, Message.NotificationStyle, true);
                    break;
                case MessageType.Permanent:
                    NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User, Message.NotificationCategory, Message.NotificationStyle, false);
                    break;
            }

            return true;
        }
        internal static bool SendToServer(Message Message)
        {
            try
            {
                switch (Message.MessageType)
                {
                    case MessageType.InfoPanel:
                        foreach (var user in PlayerUtils.OnlineUsers)
                        {
                            user.Player.OpenInfoPanel(Message.Title, Message.Content, Message.NotificationStyle.ToString());
                        }
                        break;
                    case MessageType.OkBox:
                        foreach (var user in PlayerUtils.OnlineUsers)
                        {
                            user.Player.OkBoxLoc($"{Message.Content}");
                        }
                        break;
                    case MessageType.Temporary:
                        NotificationManager.ServerMessageToAll(Localizer.DoStr(Message.Content), forceTemporary: true);
                        break;
                    case MessageType.Permanent:
                        NotificationManager.ServerMessageToAll(Localizer.DoStr(Message.Content));
                        break;
                    case MessageType.GlobalAnnoucement:
                        NotificationManager.GlobalNotification(Localizer.DoStr($"{Message.Content}"), Message.NotificationCategory, true);
                        break;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region OkBox
        public static bool Send(OkBox Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.Player == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(OkBox Message)
        {
            Message.Player.OkBoxLoc($"{Message.Content}");
            return true;
        }
        internal static bool SendToServer(OkBox Message)
        {
            try
            {
                foreach (var user in PlayerUtils.OnlineUsers)
                {
                    user.Player.OkBoxLoc($"{Message.Content}");
                }
                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region Warning
        public static bool Send(Warning Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.Player == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(Warning Message)
        {
            Message.Player.Msg(Localizer.DoStr(Shared.Utils.Text.Warning(Message.Content)), Message.NotificationStyle);
            if (Message.SendToChat)
                NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User);
            return true;
        }
        internal static bool SendToServer(Warning Message)
        {
            try
            {
                foreach (var user in PlayerUtils.OnlineUsers)
                {
                    user.Player.Msg(Localizer.DoStr(Shared.Utils.Text.Warning(Message.Content)), Message.NotificationStyle);
                    if (Message.SendToChat)
                        NotificationManager.ServerMessageToPlayer(Localizer.DoStr(Message.Content), Message.User);
                }
                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion
        #region Whisper
        public static bool Send(Whisper Message)
        {
            switch (Message.RecipientType)
            {
                case RecipientType.Player:
                    if (Message.Player == null)
                        return false;
                    return SendToPlayer(Message);
                case RecipientType.Server:
                    return SendToServer(Message);
            }

            return false;
        }
        internal static bool SendToPlayer(Whisper Message)
        {
            Message.Player.Msg(Localizer.DoStr(Shared.Utils.Text.WhisperLight(Message.Content)), Message.NotificationStyle);
            return true;
        }
        internal static bool SendToServer(Whisper Message)
        {
            try
            {
                foreach (var user in PlayerUtils.OnlineUsers)
                {
                    user.Player.Msg(Localizer.DoStr(Shared.Utils.Text.WhisperLight(Message.Content)), Message.NotificationStyle);
                }
                return true;
            }
            catch (Exception e)
            {
                Log.WriteErrorLineLocStr($"There was an error, Error was: {e}");
                return false;
            }
        }
        #endregion

        public string GetStatus() => "Active";

        public override string ToString() => Localizer.DoStr("EM - Chatbase");

        public string GetCategory() => "Elixr Mods";
    }
}

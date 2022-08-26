using Eco.Gameplay.Players;
using Eco.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using static Eco.EM.Framework.ChatBase.ChatBase;

namespace Eco.EM.Framework.ChatBase
{
    /// <summary>
    /// ChatBaseExtended Just Provides some default methods for the chat base itself, so instead of adding in: 
    /// ChatBase.Send(new ChatBase.MessageType());
    /// You will be able to just use a simple method the same way, this is basically just in case you would like to keep your code shortened and simple
    /// Basic usage will be like:
    /// ChatBaseExtended.CBChat(string, user);
    /// Alternatively you could set the Using as a static using
    /// using static Eco.EM.Framework.ChatBase.ChatBaseExtended;
    /// and that will allow you to just use the methods defined here.
    /// to avoid Conflicts all Method start with CB
    /// </summary>
    public class ChatBaseExtended
    {
        #region Chatbase Announcement
        public static bool CBAnnouncement(string content, bool sendToAll = false, bool sendToChat = false) => Send(new Announcement(content, sendToAll, sendToChat));
        public static bool CBAnnouncement(string title, string content, string instance, bool sendToAll = false, bool sendToChat = false) => Send(new Announcement(title, content, instance, sendToAll, sendToChat));
        public static bool CBAnnouncement(string title, string content, string instance, MessageType messageType, bool sendToAll = false, bool sendToChat = false) => Send(new Announcement(title, content, instance, messageType, sendToAll, sendToChat));
        #endregion
        #region Chatbase Chat
        // Used for sending to the server only
        public static bool CBChat(string Content, User user, bool forServer) => Send(new Chat(Content, user, forServer));
        public static bool CBChat(string content, User user, MessageType messageType, bool forServer) => Send(new Chat(content, user, messageType, forServer));
        public static bool CBChat(string content, User user, NotificationCategory defaultChatTags, bool forServer) => Send(new Chat(content, user, defaultChatTags, forServer));
        public static bool CBChat(string content, User user, MessageType messageType, NotificationCategory defaultChatTags, bool forServer) => Send(new Chat(content, user, messageType, defaultChatTags, forServer));
        // Used for sending to users only
        public static bool CBChat(string Content, User user) => Send(new Chat(Content, user));
        public static bool CBChat(string content, User user, MessageType messageType) => Send(new Chat(content, user, messageType));
        public static bool CBChat(string content, User user, NotificationCategory defaultChatTags) => Send(new Chat(content, user, defaultChatTags));
        public static bool CBChat(string content, User user, MessageType messageType, NotificationCategory defaultChatTags) => Send(new Chat(content, user, messageType, defaultChatTags));
        // Used for sending to players only
        public static bool CBChat(string Content, Player player) => Send(new Chat(Content, player));
        public static bool CBChat(string content, Player player, MessageType messageType) => Send(new Chat(content, player, messageType));
        public static bool CBChat(string content, Player player, NotificationCategory defaultChatTags) => Send(new Chat(content, player, defaultChatTags));
        public static bool CBChat(string content, Player player, MessageType messageType, NotificationCategory defaultChatTags) => Send(new Chat(content, player, messageType, defaultChatTags));
        #endregion
        #region Chatbase Error
        public static bool CBError(string content, bool sendToChat = false) => Send(new Error(content, sendToChat));
        public static bool CBError(string content, User user, bool sendToChat = false) => Send(new Error(content, user, sendToChat));
        public static bool CBError(string content, Player player, bool sendToChat = false) => Send(new Error(content, player, sendToChat));
        #endregion
        #region Chatbase Info
        public static bool CBInfo(string content, bool sendToChat = false) => Send(new Info(content, sendToChat));
        public static bool CBInfo(string content, User user, bool sendToChat = false) => Send(new Info(content, user, sendToChat));
        public static bool CBInfo(string content, Player player, bool sendToChat = false) => Send(new Info(content, player, sendToChat));
        #endregion
        #region Chatbase Info box
        public static bool CBInfoBox(string content, bool sendToChat = false) => Send(new InfoBox(content, sendToChat));
        public static bool CBInfoBox(string content, User user, bool sendToChat = false) => Send(new InfoBox(content, user, sendToChat));
        public static bool CBInfoBox(string content, Player player, bool sendToChat = false) => Send(new InfoBox(content, player, sendToChat));
        #endregion
        #region Chatbase Info pannel
        public static bool CBInfoPane(string title, string content, string instance, bool sendToChat = false) => Send(new InfoPane(title, content, instance, sendToChat));
        public static bool CBInfoPane(string title, string content, string instance, PanelType usePanelType, bool sendToChat = false) => Send(new InfoPane(title, content, instance, usePanelType, sendToChat));
        public static bool CBInfoPane(string title, string content, string instance, PanelType usePanelType, bool tempMessage, bool sendToChat = false) => Send(new InfoPane(title, content, instance, usePanelType, tempMessage, sendToChat));

        public static bool CBInfoPane(string title, string content, string instance, User user, bool sendToChat = false) => Send(new InfoPane(title, content, instance, user, sendToChat));
        public static bool CBInfoPane(string title, string content, string instance, User user, PanelType usePanelType, bool sendToChat = false) => Send(new InfoPane(title, content, instance, user, usePanelType, sendToChat));
        public static bool CBInfoPane(string title, string content, string instance, User user, PanelType usePanelType, bool tempMessage, bool sendToChat = false) => Send(new InfoPane(title, content, instance, user, usePanelType, tempMessage, sendToChat));

        public static bool CBInfoPane(string title, string content, string instance, Player player, bool sendToChat = false) => Send(new InfoPane(title, content, instance, player, sendToChat));
        public static bool CBInfoPane(string title, string content, string instance, Player player, PanelType usePanelType, bool sendToChat = false) => Send(new InfoPane(title, content, instance, player, usePanelType, sendToChat));
        public static bool CBInfoPane(string title, string content, string instance, Player player, PanelType usePanelType, bool tempMessage, bool sendToChat = false) => Send(new InfoPane(title, content, instance, player, usePanelType, tempMessage, sendToChat));
        #endregion
        #region Chatbase Mail
        public static bool CBMail(string content) => Send(new Mail(content));
        public static bool CBMail(string content, string tag) => Send(new Mail(content, tag));
        public static bool CBMail(string content, User user) => Send(new Mail(content, user));
        public static bool CBMail(string content, string tag, User user) => Send(new Mail(content, tag, user));
        public static bool CBMail(string content, Player player) => Send(new Mail(content, player));
        public static bool CBMail(string content, string tag, Player player) => Send(new Mail(content, tag, player));
        #endregion
        #region Chatbase Message
        public static bool CBMessage(string content) => Send(new Message(content));
        public static bool CBMessage(string content, MessageType messageType) => Send(new Message(content, messageType));
        public static bool CBMessage(string content, NotificationCategory defaultChatTags) => Send(new Message(content, defaultChatTags));
        public static bool CBMessage(string content, NotificationStyle messageCategory, NotificationCategory defaultChatTags) => Send(new Message(content, messageCategory, defaultChatTags));
        public static bool CBMessage(string content, NotificationStyle messageCategory, NotificationCategory defaultChatTags, MessageType messageType) => Send(new Message(content, messageCategory, defaultChatTags, messageType));
        public static bool CBMessage(string title, string content) => Send(new Message(title, content));
        public static bool CBMessage(string title, string content, MessageType messageType) => Send(new Message(title, content, messageType));

        public static bool CBMessage(string content, User user) => Send(new Message(content, user));
        public static bool CBMessage(string content, User user, MessageType messageType) => Send(new Message(content, user, messageType));
        public static bool CBMessage(string content, User user, NotificationCategory defaultChatTags) => Send(new Message(content, user, defaultChatTags));
        public static bool CBMessage(string content, User user, NotificationStyle messageCategory, NotificationCategory defaultChatTags) => Send(new Message(content, user, messageCategory, defaultChatTags));
        public static bool CBMessage(string content, User user, NotificationStyle messageCategory, NotificationCategory defaultChatTags, MessageType messageType) => Send(new Message(content, user, messageCategory, defaultChatTags, messageType));
        public static bool CBMessage(string title, string content, User user) => Send(new Message(title, content, user));

        public static bool CBMessage(string content, Player player) => Send(new Message(content, player));
        public static bool CBMessage(string content, Player player, MessageType messageType) => Send(new Message(content, player, messageType));
        public static bool CBMessage(string content, Player player, NotificationCategory defaultChatTags) => Send(new Message(content, player, defaultChatTags));
        public static bool CBMessage(string content, Player player, NotificationStyle messageCategory, NotificationCategory defaultChatTags) => Send(new Message(content, player, messageCategory, defaultChatTags));
        public static bool CBMessage(string content, Player player, NotificationStyle messageCategory, NotificationCategory defaultChatTags, MessageType messageType) => Send(new Message(content, player, messageCategory, defaultChatTags, messageType));
        public static bool CBMessage(string title, string content, Player player) => Send(new Message(title, content, player));
        #endregion
        #region Chatbase Ok Box
        public static bool CBOkBox(string content) => Send(new OkBox(content));
        public static bool CBOkBox(string content, User user) => Send(new OkBox(content, user));
        public static bool CBOkBox(string content, Player player) => Send(new OkBox(content, player));
        #endregion
        #region Chatbase Warning
        public static bool CBWarning(string content, bool sendToChat = false) => Send(new Warning(content, sendToChat));
        public static bool CBWarning(string content, User user, bool sendToChat = false) => Send(new Warning(content, user, sendToChat));
        public static bool CBWarning(string content, Player player, bool sendToChat = false) => Send(new Warning(content, player, sendToChat));
        #endregion
        #region Chatbase Whisper
        public static bool CBWhisper(string content) => Send(new Whisper(content));
        public static bool CBWhisper(string content, User user) => Send(new Whisper(content, user));
        public static bool CBWhisper(string content, Player player) => Send(new Whisper(content, player));
        #endregion
    }
}
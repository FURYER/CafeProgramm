using ProtoBuf;
using System;
using System.Collections.Generic;

namespace NewApp
{
    [ProtoContract]
    public class UserInfo
    {
        [ProtoMember(1)]
        public string login;
        [ProtoMember(2)]
        public string[] fullname;
        [ProtoMember(3)]
        public string permission;
        [ProtoMember(4)]
        public string persona;
        [ProtoMember(5)]
        public DateTime registerDate;
        [ProtoMember(6)]
        public int id;
        [ProtoMember(7)]
        public byte[] img;
        [ProtoMember(8)]
        public List<Notes> notes = new List<Notes>();
        [ProtoMember(9)]
        public List<Favourites> favourites = new List<Favourites>();
        [ProtoMember(10)]
        public List<Chat> chats = new List<Chat>();

        [ProtoContract]
        public class Notes
        {
            [ProtoMember(1)]
            public int id;
            [ProtoMember(2)]
            public string text;
            [ProtoMember(3)]
            public List<byte[]> imgs = new List<byte[]>();
            [ProtoMember(4)]
            public int likes;
            [ProtoMember(5)]
            public List<Comments> comments = new List<Comments>();
            [ProtoMember(6)]
            public DateTime createDate;
            [ProtoMember(7)]
            public bool like;

            [ProtoContract]
            public class Comments
            {
                [ProtoMember(1)]
                public int id;
                [ProtoMember(2)]
                public string text;
                [ProtoMember(3)]
                public string fullname;
                [ProtoMember(4)]
                public int likes;
                [ProtoMember(5)]
                public byte[] profilePicture;
            }
        }
        
        [ProtoContract]
        public class Favourites
        {
            [ProtoMember(1)]
            public int id;
            [ProtoMember(2)]
            public string[] fullname;
            [ProtoMember(3)]
            public byte[] img;
            [ProtoMember(4)]
            public string login;
            [ProtoMember(5)]
            public bool status;
            [ProtoMember(6)]
            public string persona;
        }

        [ProtoContract]
        public class Chat
        {
            [ProtoMember(1)]
            public int id;
            [ProtoMember(2)]
            public List<ChatMessages> chatMessages = new List<ChatMessages>();
            [ProtoMember(3)]
            public int idUser;
            [ProtoMember(4)]
            public byte[] imgUser;
            [ProtoMember(5)]
            public string[] nameUser;
            [ProtoMember(6)]
            public string loginUser;
            [ProtoMember(7)]
            public bool check;

            [ProtoContract]
            public class ChatMessages
            {
                [ProtoMember(1)]
                public int id;
                [ProtoMember(2)]
                public string text;
                [ProtoMember(3)]
                public int idAuthor;
            }
        }
    }
}
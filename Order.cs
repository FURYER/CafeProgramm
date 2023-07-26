using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessByJob
{
    [ProtoContract]
    public class Order
    {
        [ProtoMember(1)]
        public int id { get; set; }
        [ProtoMember(2)]
        public string[] fullname { get; set; }
        public string Fullname
        {
            get { return fullname[0] + " " + fullname[1] + " " + fullname[2]; }
        }
        [ProtoMember(3)]
        public int userId { get; set; }
        [ProtoMember(4)]
        public string status { get; set; }
        [ProtoMember(5)]
        public DateTime date {  get; set; }
        public string Date
        {
            get { return date.ToString("d MMMM yyyy HH:mm"); }
        }
        [ProtoMember(6)]
        public List<Product> products = new List<Product>();
        [ProtoMember(7)]
        public double totalPrice { get; set; } = 0;
        public string TotalPrice
        {
            get { return string.Format("{0:0.00}", totalPrice) + " руб."; }
        }
        [ProtoMember(8)]
        public string requisites { get; set; }

        [ProtoContract]
        public class Product
        {
            [ProtoMember(1)]
            public int id;
            [ProtoMember(2)]
            public string name;
            [ProtoMember(3)]
            public int count;
            [ProtoMember(4)]
            public double price;
            [ProtoMember(5)]
            public string type;
            [ProtoMember(6)]
            public string articul;
            [ProtoMember(7)]
            public int buyCount;
        }
    }
}

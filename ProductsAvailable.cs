using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BusinessByJob
{
    [ProtoContract]
    public class ProductsAvailable
    {
        [ProtoMember(1)]
        public List<NomenclatureType> types = new List<NomenclatureType>();
        [ProtoMember(2)]
        public List<Nomenclature> nomenclatures = new List<Nomenclature>();
        [ProtoMember(3)]
        public List<Product> products = new List<Product>();

        [ProtoContract]
        public class NomenclatureType
        {
            [ProtoMember(1)]
            public int id { get; set; }
            [ProtoMember(2)]
            public string name { get; set; }
        }

        [ProtoContract]
        public class Nomenclature
        {
            [ProtoMember(1)]
            public int id;
            [ProtoMember(2)]
            public string name { get; set; }
            [ProtoMember(3)]
            public string articul { get; set; }
            [ProtoMember(4)]
            public string type { get; set; }
            [ProtoMember(5)]
            public double price { get; set; }
            public string Price
            {
                get
                {
                    return string.Format("{0:0.00}", price) + " руб.";
                }
            }
            [ProtoMember(6)]
            public byte[] img { get; set; }
        }

        [ProtoContract]
        public class Product
        {
            [ProtoMember(1)]
            public int id;
            [ProtoMember(2)]
            public string name { get; set; }
            [ProtoMember(3)]
            public int count { get; set; }
            [ProtoMember(4)]
            public double price { get; set; }
            public string Price
            {
                get
                {
                    return string.Format("{0:0.00}", price) + " руб.";
                }
            }
            [ProtoMember(5)]
            public string type;
            [ProtoMember(6)]
            public string articul;
            public Slider slider { get; set; }
            [ProtoMember(7)]
            public byte[] img { get; set; }
        }
    }
}

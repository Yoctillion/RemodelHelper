﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace RemodelHelper.Models
{
    [DataContract]
    public class Item
    {
        [DataMember]
        public int SlotId { get; set; }

        [DataMember]
        public int ShipId { get; set; }

        [DataMember]
        public int NewId { get; set; }

        [DataMember]
        public Week Week { get; set; }
    }

    public static class ItemExtensions
    {
        public static SlotItemInfo GetBaseSlotInfo(this Item item)
            => KanColleClient.Current.Master.SlotItems[item.SlotId];

        public static ShipInfo GetAssistantInfo(this Item item)
            => KanColleClient.Current.Master.Ships[item.ShipId];

        public static SlotItemInfo GetUpgradeSlotInfo(this Item item)
            => KanColleClient.Current.Master.SlotItems[item.NewId];
    }
}

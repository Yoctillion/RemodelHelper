using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using RemodelHelper.ViewModels;

namespace RemodelHelper.Models
{
    [DataContract]
    public class RemodelData : NotificationObject
    {
        private static Master Master => KanColleClient.Current.Master;

        private string DataPath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "grabacr.net",
            "KanColleViewer",
            "RemodelData.json");

        public static RemodelData Current { get; } = new RemodelData();


        private readonly DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(RemodelData));

        [DataMember]
        public string Version { get; set; }

        private Item[] _items;
        [DataMember]
        public Item[] Items
        {
            get { return this._items; }
            set
            {
                if (this._items != value)
                {
                    this._items = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private RemodelData()
        {
            this.Load();
            if (Items == null)
            {
                Items = new Item[0];
                Version = "0.0.0";
            }
        }

        public void Save()
        {
            using (var stream = Stream.Synchronized(new FileStream(this.DataPath, FileMode.Create, FileAccess.Write)))
            {
                this._serializer.WriteObject(stream, this);
            }
        }

        public void Load()
        {
            if (!File.Exists(this.DataPath)) return;
            using (var stream = Stream.Synchronized(new FileStream(this.DataPath, FileMode.Open, FileAccess.Read)))
            {
                var obj = this._serializer.ReadObject(stream) as RemodelData;
                if (obj?.Version != null) this.Version = obj.Version;
                if (obj?.Items != null) this.Items = obj.Items;
            }
        }

        internal async Task<ItemViewModel[]> GetRemodelInfo(DayOfWeek day)
        {
            var available = this.Items.Where(item => item.Week.HasFlag(day.Convert()))
                .GroupBy(item => item.SlotId).ToArray();

            return await Task.Run(() =>
            {
                // 等待Master初始化完成
                WaitForMaster();

                return available.Select(slotGroup => new ItemViewModel
                {
                    Slot = new SlotViewModel
                    {
                        Name = Master.SlotItems[slotGroup.Key]?.Name ?? "未知",
                        Type = Master.SlotItems[slotGroup.Key]?.IconType ?? SlotItemIconType.Unknown,
                    },

                    NewSlot = slotGroup.GroupBy(slot => slot.NewId).Select(newSlots => new NewSlotViewModel
                    {
                        Name = Master.SlotItems[newSlots.Key]?.Name ?? "更新不可",
                        Type = Master.SlotItems[newSlots.Key]?.IconType ?? SlotItemIconType.Unknown,

                        Ships = newSlots.Select(item => new ShipViewModel { Name = Master.Ships[item.ShipId]?.Name ?? "" })
                                .ToArray(),
                    }).ToArray(),

                }).ToArray();
            });
        }

        public async void UpdateFromInternet()
        {
            const string url = @"https://raw.githubusercontent.com/Yoctillion/RemodelHelper/master/Data/RemodelData.json";
            try
            {
                await Task.Run(() =>
                {
                    WaitForMaster();
                    using (var client = new WebClient())
                    using (var stream = client.OpenRead(url))
                        if (stream != null)
                        {
                            var check = _serializer.ReadObject(stream) as RemodelData;

                            if (check == null) return;

                            if (VersionComparer.Compare(this.Version, check.Version) < 0)
                            {
                                this.Version = check.Version;
                                this.Items = check.Items;
                                this.Save();
                            }
                        }
                }
                );
            }
            catch { }
        }

        private static void WaitForMaster()
        {
            while (Master == null) Thread.Sleep(100);
        }

        private static class VersionComparer
        {
            public static int Compare(string x, string y)
            {
                if (string.IsNullOrEmpty(y)) return 1;
                if (string.IsNullOrEmpty(x)) return -1;

                var vx = x.Split('.');
                var vy = y.Split('.');

                for (var i = 0; i < 3; i++)
                {
                    var vxi = int.Parse(vx[i]);
                    var vyi = int.Parse(vy[i]);
                    if (vxi < vyi) return -1;
                    if (vxi > vyi) return 1;
                }
                return 0;
            }
        }
    }
}

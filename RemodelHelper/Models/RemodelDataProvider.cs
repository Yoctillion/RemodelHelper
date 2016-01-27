using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Mvvm;

namespace RemodelHelper.Models
{
    public class RemodelDataProvider
    {
        public static RemodelDataProvider Current { get; } = new RemodelDataProvider();

        private RemodelData _rawData;

        private RemodelData RawData
        {
            get { return this._rawData; }
            set
            {
                if (this._rawData != value)
                {
                    this._rawData = value;
                    this.ParseData();
                }
            }
        }


        public string Version => this.RawData?.Version;

        public IdentifiableTable<BaseSlotInfo> Items { get; internal set; } = new IdentifiableTable<BaseSlotInfo>();


        private string DataPath { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "grabacr.net",
                "KanColleViewer",
                "RemodelData.json");


        private RemodelDataProvider()
        {
            this.Load();
            if (this.RawData == null)
                this.RawData = new RemodelData { Version = "000000", Items = new Item[0], NewSlots = new UpdateData[0], };

            KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), this.ParseData, false);
        }


        public event Action DataChanged;

        #region analyze data

        private readonly object _updateLock = new object();

        private void ParseData()
        {
            if (!KanColleClient.Current.IsStarted || this.RawData == null) return; // 初始化未完成

            lock (this._updateLock)
            {
                this.IsReady = false;

                foreach (var item in this.RawData.Items.Where(item => item.GetBaseSlotInfo() != null))
                {
                    if (!this.Items.ContainsKey(item.SlotId))
                        this.Items.Add(new BaseSlotInfo(item));
                    else
                        this.Items[item.SlotId].AddUpgradeSlot(item);
                }

                foreach (var item in this.Items.Values)
                    foreach (var newSlot in item.UpgradeSlots.Values)
                        newSlot.Level =
                            this.RawData.NewSlots.FirstOrDefault(s => s.Id == item.Id && s.NewId == newSlot.Id)?.Lv ?? 0;

                this.IsReady = true;
            }

            DataChanged?.Invoke();
        }

        public bool IsReady { get; private set; } = false;

        #endregion


        #region S/L & update

        private readonly DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(RemodelData));

        public void Save()
        {
            using (var stream = Stream.Synchronized(new FileStream(this.DataPath, FileMode.Create, FileAccess.Write)))
                this._serializer.WriteObject(stream, this.RawData);
        }

        public void Load()
        {
            if (!File.Exists(this.DataPath)) return;
            using (var stream = Stream.Synchronized(new FileStream(this.DataPath, FileMode.Open, FileAccess.Read)))
            {
                var obj = this._serializer.ReadObject(stream) as RemodelData;
                if (obj != null) this.RawData = obj;
            }
        }

        public void UpdateFromInternet()
        {
            var url = new Uri("https://raw.githubusercontent.com/Yoctillion/RemodelHelper/master/Data/RemodelData.json");

            using (var client = new WebClient())
            {
                client.OpenReadCompleted += (s, e) =>
                {
                    if (e.Error == null)
                        using (var stream = e.Result)
                        {
                            var check = this._serializer.ReadObject(stream) as RemodelData;

                            if (string.CompareOrdinal(this.RawData.Version, check?.Version) < 0)
                            {
                                this.RawData = check;
                                this.Save();
                            }
                        }
                };
                client.OpenReadAsync(url);
            }
        }

        #endregion
    }
}

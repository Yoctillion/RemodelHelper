using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Grabacr07.KanColleWrapper;
using Livet;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;

namespace RemodelHelper.Models
{
    public class RemodelDataProvider : NotificationObject, IDisposableHolder
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

        public IdentifiableTable<ItemInfo> Items { get; internal set; } = new IdentifiableTable<ItemInfo>();


        private string DataPath { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "grabacr.net",
                "KanColleViewer",
                "RemodelData.json");

        private readonly DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(RemodelData));


        private RemodelDataProvider()
        {
            this.Load();
            if (this.RawData == null)
                this.RawData = new RemodelData { Version = "000000", Items = new Item[0], NewSlots = new UpdateData[0], };

            KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), this.ParseData, false).AddTo(this);
        }

        #region analyze data

        private void ParseData()
        {
            if (!KanColleClient.Current.IsStarted || this.RawData == null) return; // 初始化未完成

            this.IsUpdateFinished = false;

            foreach (var item in this.RawData.Items.Where(item => item.GetSlotInfo() != null))
            {
                if (!this.Items.ContainsKey(item.SlotId))
                    this.Items.Add(new ItemInfo(item));
                else
                    this.Items[item.SlotId].AddNewItem(item);
            }

            foreach (var item in this.Items.Values)
                foreach (var newSlot in item.NewSlots.Values)
                    newSlot.Level = this.RawData.NewSlots.FirstOrDefault(s => s.Id == item.Id && s.NewId == newSlot.Id)?.Lv ?? 0;

            this.IsUpdateFinished = true;

            DataChanged?.Invoke();
        }

        public bool IsUpdateFinished { get; set; }

        public event Action DataChanged;

        #endregion


        #region S/L & update

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
            var url = new Uri(@"https://raw.githubusercontent.com/Yoctillion/RemodelHelper/master/Data/RemodelData.json");

            var client = new WebClient();
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

        #endregion


        private readonly LivetCompositeDisposable _compositeDisposable = new LivetCompositeDisposable();

        public void Dispose()
        {
            this._compositeDisposable.Dispose();
        }

        public ICollection<IDisposable> CompositeDisposable => this._compositeDisposable;
    }
}

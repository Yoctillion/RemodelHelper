using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Mvvm;
using Notifier = Grabacr07.KanColleWrapper.Notifier;

namespace RemodelHelper.Models
{
    public class RemodelDataProvider : Notifier
    {
        public static RemodelDataProvider Current { get; } = new RemodelDataProvider();

        #region RawData

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

        #endregion

        #region Items

        public string Version => this.RawData?.Version;

        private IdentifiableTable<BaseSlotInfo> _items = new IdentifiableTable<BaseSlotInfo>();

        public IdentifiableTable<BaseSlotInfo> Items
        {
            get { return this._items; }
            private set
            {
                if (this._items != value)
                {
                    this._items = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region IsReady

        private bool _isReady;

        public bool IsReady
        {
            get { return this._isReady; }
            private set
            {
                if (this._isReady != value)
                {
                    this._isReady = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region IsUpdating

        private bool _isUpdating;

        public bool IsUpdating
        {
            get { return this._isUpdating; }
            private set
            {
                if (this._isUpdating != value)
                {
                    this._isUpdating = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion


        private string DataPath { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "grabacr.net",
                "KanColleViewer",
                "RemodelData.json");


        private RemodelDataProvider()
        {
            this.Load();
            if (this.RawData == null)
                this.RawData = new RemodelData { Version = "000000", Items = new Item[0], NewSlots = new UpgradeData[0], };

            KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), this.ParseData, false);
        }


        #region analyze data

        private readonly object _updateLock = new object();

        private void ParseData()
        {
            if (!KanColleClient.Current.IsStarted || this.RawData == null) return; // 初始化未完成

            lock (this._updateLock)
            {
                this.IsReady = false;

                var newItems = new IdentifiableTable<BaseSlotInfo>();

                foreach (var item in this.RawData.Items.Where(item => item.GetBaseSlotInfo() != null))
                {
                    if (!newItems.ContainsKey(item.SlotId))
                        newItems.Add(new BaseSlotInfo(item));
                    else
                        newItems[item.SlotId].AddUpgradeSlot(item);
                }

                foreach (var item in newItems.Values)
                    foreach (var newSlot in item.UpgradeSlots.Values)
                        newSlot.Level =
                            this.RawData.NewSlots.FirstOrDefault(s => s.Id == item.Id && s.NewId == newSlot.Id)?.Lv ?? 0;

                this.Items = newItems;

                this.IsReady = true;
            }
        }

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


        private readonly object _iLock = new object();

        public async void UpdateFromInternet()
        {
            const string url = "https://raw.githubusercontent.com/Yoctillion/RemodelHelper/master/Data/RemodelData.json";

            await Task.Run(() =>
            {
                lock (this._iLock)
                {
                    this.IsUpdating = true;

                    try
                    {
                        using (var client = new WebClient())
                        using (var stream = client.OpenRead(url))
                        {
                            var check = this._serializer.ReadObject(stream) as RemodelData;

                            if (string.CompareOrdinal(this.RawData.Version, check?.Version) < 0)
                            {
                                this.RawData = check;
                                this.Save();
                            }
                        }
                    }
                    catch (WebException)
                    {
                    }

                    this.IsUpdating = false;
                }
            });
        }

        #endregion
    }
}

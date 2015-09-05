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

        public async void UpdateFromInternet()
        {
            const string url = @"https://raw.githubusercontent.com/Yoctillion/RemodelHelper/master/Data/RemodelData.json";
            try
            {
                await Task.Run(() =>
                {
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

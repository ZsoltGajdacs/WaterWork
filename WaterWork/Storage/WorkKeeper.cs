﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UsageWatcher;
using WaterWork.Helpers;
using WaterWork.Models;

namespace WaterWork.Storage
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkKeeper : INotifyPropertyChanged, IDisposable
    {
        [NonSerialized]
        private IWatcher watcher;

        public Dictionary<DateTime, WorkDay> WorkDays { get; private set; }
        public Dictionary<int, int> DaysWorkedInMonth { get; private set; }
        public List<DateTime> LeaveDays { get; set; }
        public List<DateTime> SickDays { get; set; }
        public WorkSettings Settings { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public IWatcher GetWatcher()
        {
            return watcher;
        }

        internal void InitWatcher()
        {
            watcher = new Watcher("WaterWork", Settings.WatcherResolution,
                Settings.WatcherSavePreference, Settings.WatcherDataPrecision);
        }

        #region Event handler
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Singleton
        public static WorkKeeper Instance { get { return lazy.Value; } }

        private WorkKeeper()
        {
            WorkDays = new Dictionary<DateTime, WorkDay>();
            DaysWorkedInMonth = new Dictionary<int, int>();
            LeaveDays = new List<DateTime>();
            SickDays = new List<DateTime>();

            Settings = new WorkSettings
            {
                WatcherResolution = Resolution.TwoMinutes,
                WatcherSavePreference = SavePreference.KeepDataForToday,
                WatcherDataPrecision = DataPrecision.HighPrecision
            };
        }

        private static readonly Lazy<WorkKeeper> lazy = new Lazy<WorkKeeper>(() =>
        {
            string path = FilesLocation.GetSaveDirPath() + FilesLocation.GetWaterWorkFileName();
            WorkKeeper workKeeper = Serializer.JsonObjectDeserialize<WorkKeeper>(path);

            return workKeeper ?? new WorkKeeper();
        });

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //  dispose managed state (managed objects).
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
                watcher.Dispose();
                disposedValue = true;
            }
        }

        //  override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UsageKeeper()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            //GC.SuppressFinalize(this);
        }
        #endregion
    }
}

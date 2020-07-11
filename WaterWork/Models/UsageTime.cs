﻿using Newtonsoft.Json;
using System;

namespace WaterWork.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class UsageTime
    {
        private TimeSpan _usage;

        public DateTime Day { get; set; }
        public TimeSpan Usage
        {
            get => _usage;
            set
            {
                if (value != _usage)
                {
                    _usage = value;
                    LastRefreshDate = DateTime.Now;
                }
            }
        }
        public DateTime LastRefreshDate { get; private set; }

        public UsageTime(DateTime day, TimeSpan usage)
        {
            Day = day;
            Usage = usage;
        }
    }
}

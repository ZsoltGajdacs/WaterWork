﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WaterWork.Model
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkDay : INotifyPropertyChanged
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int LunchBreakDuration { get; set; }
        public float WaterConsumptionCount { get; set; }
        public float AmountOfLitreInOneUnit { get; set; }
        public Boolean IsLunchTimeWorkTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public WorkDay()
        {
            AmountOfLitreInOneUnit = 1;
        }

        internal void IncreaseWaterConsumption()
        {
            ++WaterConsumptionCount;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

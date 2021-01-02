﻿using System;
using System.Globalization;
using System.Windows;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Services;

namespace WorkTimeStat.Windows
{
    public partial class StatisticsWindow : Window
    {
        internal StatisticsWindow(double dailyWorkHours)
        {
            InitializeComponent();
            mainGrid.DataContext = this;

            StatisticsDto dto = CreateStatDto(dailyWorkHours);
            AssignDataToWindowControls(ref dto);
        }

        private static StatisticsDto CreateStatDto(double dailyWorkHours)
        {
            StatisticsDto dto = new StatisticsDto
            {
                dailyWorkHours = dailyWorkHours
            };

            // Monthly
            int thisMonth = DateTime.Now.Month;
            dto.mWorkedHours = StatisticsService.CalcMonthlyWorkedHours(thisMonth);
            dto.mFullHours = StatisticsService.CalcMonthlyTotalHours(thisMonth);
            dto.mCalcHours = StatisticsService.GetUsageForMonth(thisMonth);
            dto.mLeftHours = AddPlusIfNeeded(StatisticsService.CalcMonthlyHoursDifference(thisMonth));

            // Last month
            int lastMonth = thisMonth == 1 ? 1 : thisMonth - 1;
            dto.pmWorkedHours = StatisticsService.CalcMonthlyWorkedHours(lastMonth);
            dto.pmFullHours = StatisticsService.CalcMonthlyTotalHours(lastMonth);
            dto.pmCalcHours = StatisticsService.GetUsageForMonth(lastMonth);
            dto.pmLeftHours = AddPlusIfNeeded(StatisticsService.CalcMonthlyHoursDifference(lastMonth));

            // Daily
            WorkDay today = WorkDayService.GetCurrentDay();
            dto.dWorkedHours = StatisticsService.CalcDailyWorkedHours(today);
            dto.dFullHours = StatisticsService.CalcFullHoursForDay(today);
            dto.dCalcHours = StatisticsService.GetUsageForToday();
            dto.dLeftHours = AddPlusIfNeeded(StatisticsService.CalcDailyHoursDifference(today));

            // Yesterday
            WorkDay yesterWorkday = WorkDayService.GetYesterWorkDay();
            dto.ywdWorkedHours = StatisticsService.CalcDailyWorkedHours(yesterWorkday);
            dto.ywdFullHours = StatisticsService.CalcFullHoursForDay(yesterWorkday);
            dto.ywdCalcHours = StatisticsService.GetUsageForDay(yesterWorkday);
            dto.ywdLeftHours = AddPlusIfNeeded(StatisticsService.CalcDailyHoursDifference(yesterWorkday));

            return dto;
        }

        private void AssignDataToWindowControls(ref StatisticsDto dto)
        {
            yesterworkdayWorkedHours.Content = NumberFormatter.FormatNum(dto.ywdWorkedHours);
            yesterworkdayFullHours.Content = NumberFormatter.FormatNum(dto.ywdFullHours);
            yesterworkdayCalcHours.Content = NumberFormatter.FormatNum(dto.ywdCalcHours);
            yesterworkdayLeftHours.Content = dto.ywdLeftHours;

            todayWorkedHours.Content = NumberFormatter.FormatNum(dto.dWorkedHours);
            todayFullHours.Content = NumberFormatter.FormatNum(dto.dFullHours);
            todayCalcHours.Content = NumberFormatter.FormatNum(dto.dCalcHours);
            todayLeftHours.Content = dto.dLeftHours;

            monthlyWorkedHours.Content = NumberFormatter.FormatNum(dto.mWorkedHours);
            monthlyFullHours.Content = NumberFormatter.FormatNum(dto.mFullHours);
            monthlyCalcHours.Content = NumberFormatter.FormatNum(dto.mCalcHours);
            monthlyLeftHours.Content = dto.mLeftHours;

            prevMonthlyWorkedHours.Content = NumberFormatter.FormatNum(dto.pmWorkedHours);
            prevMonthlyFullHours.Content = NumberFormatter.FormatNum(dto.pmFullHours);
            prevMonthlyCalcHours.Content = NumberFormatter.FormatNum(dto.pmCalcHours);
            prevMonthlyLeftHours.Content = dto.pmLeftHours;
        }

        private static string AddPlusIfNeeded(double num)
        {
            num = Math.Round(num, 2, MidpointRounding.ToEven);
            return num > 0 ? "+" + num : num.ToString(CultureInfo.InvariantCulture);
        }

        private class StatisticsDto
        {
            public double dailyWorkHours;

            // Monthly
            public double mWorkedHours, mFullHours, mCalcHours;
            public string mLeftHours;

            // Previous Month
            public double pmWorkedHours, pmFullHours, pmCalcHours;
            public string pmLeftHours;

            // Daily
            public double dWorkedHours, dFullHours, dCalcHours;
            public string dLeftHours;

            // Yesterday
            public double ywdWorkedHours, ywdFullHours, ywdCalcHours;
            public string ywdLeftHours;
        }
    }
}
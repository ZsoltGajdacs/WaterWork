﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UsageWatcher.Enums;
using WorkTimeStat.Events;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Services;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Controls
{
    public partial class CalendarControl : UserControl
    {
        private readonly WorkKeeper keeper;
        private readonly DateTime currDate;

        private WorkDay chosenDay;
        private int numOfLeavesLeft;
        private bool leaveAutochk;
        private bool sickAutochk;
        private DateTime selectedDate;

        internal event CloseTheBallonEventHandler CloseBallon;

        public CalendarControl()
        {
            InitializeComponent();
            keeper = WorkKeeper.Instance;

            mainGrid.DataContext = this;

            currDate = DateTime.Now.Date;

            SetToday();
            UpdateLeaveDays();
        }

        private void UpdateLeaveDays()
        {
            numOfLeavesLeft = keeper.Settings.YearlyLeaveNumber - keeper.LeaveDays.Count;
            leaveDayNum.Content = numOfLeavesLeft + " / " + keeper.Settings.YearlyLeaveNumber;
        }

        private void SetToday()
        {
            chosenDay = WorkDayService.GetCurrentDay();
            if (chosenDay != null)
            {
                SetLabels(ref chosenDay);
                selectedDate = currDate.Date;
            }
            else
            {
                SetEmptyLabels();
            }

            chosenDateLabel.Content = DateTime.Today.ToLongDateString();
        }

        private void MainCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainCalendar.SelectedDate.HasValue)
            {
                selectedDate = mainCalendar.SelectedDate.Value.Date;
                chosenDateLabel.Content = selectedDate.ToLongDateString();

                CalendarSetLeaveDay(selectedDate);
                CalendarSetSickDay(selectedDate);

                chosenDay = WorkDayService.GetDayAtDate(selectedDate);

                RefreshLabels(ref chosenDay);
            }
        }

        private void RefreshLabels(ref WorkDay day)
        {
            if (day != null)
            {
                SetLabels(ref day);
            }
            else
            {
                SetEmptyLabels();
            }
        }

        private void SetLabels(ref WorkDay workDay)
        {
            startTimeValue.Content = workDay.StartTime;
            endTimeValue.Content = workDay.EndTime;
            lunchBreakTimeValue.Content = workDay.LunchBreakDuration + " perc";
            otherBreakTimeValue.Content = workDay.OtherBreakDuration + " perc";
            overWorkTimeValue.Content = workDay.OverWorkDuration + " perc";
            WorkTypeValue.Content = workDay.WorkDayType.GetDisplayName();
            WorkPlaceValue.Content = workDay.WorkPlaceType.GetDisplayName();
            workedTimeValue.Content = StatisticsService.CalcDailyWorkedHours(workDay);

            double daysUsage = StatisticsService.GetUsageForDay(workDay);
            watchedTimeValue.Content = NumberFormatter.FormatNum(daysUsage);
        }

        private void SetEmptyLabels()
        {
            startTimeValue.Content = NumberFormatter.NO_DATA;
            endTimeValue.Content = NumberFormatter.NO_DATA;
            lunchBreakTimeValue.Content = NumberFormatter.NO_DATA;
            otherBreakTimeValue.Content = NumberFormatter.NO_DATA;
            overWorkTimeValue.Content = NumberFormatter.NO_DATA;
            WorkTypeValue.Content = NumberFormatter.NO_DATA;
            WorkPlaceValue.Content = NumberFormatter.NO_DATA;
            workedTimeValue.Content = NumberFormatter.NO_DATA;
            watchedTimeValue.Content = NumberFormatter.NO_DATA;
        }

        private void CalendarSetLeaveDay(DateTime selectedDate)
        {
            leaveAutochk = true; //To know that this was auto checked and not the user did it

            if (keeper.LeaveDays.Contains(selectedDate))
            {
                leaveDayChkbox.IsChecked = true;
                leaveDayChkbox.IsEnabled = selectedDate >= currDate;
            }
            else if (selectedDate < currDate)
            {
                leaveDayChkbox.IsChecked = false;
                leaveDayChkbox.IsEnabled = false;
            }
            else
            {
                leaveDayChkbox.IsChecked = false;
                leaveDayChkbox.IsEnabled = true;
            }
        }

        private void CalendarSetSickDay(DateTime selectedDate)
        {
            sickAutochk = true; //To know that this was auto checked and not the user did it

            if (keeper.SickDays.Contains(selectedDate))
            {
                sickDayChkbox.IsChecked = true;
                sickDayChkbox.IsEnabled = selectedDate >= currDate;
            }
            else if (selectedDate < currDate)
            {
                sickDayChkbox.IsChecked = false;
                sickDayChkbox.IsEnabled = false;
            }
            else
            {
                sickDayChkbox.IsChecked = false;
                sickDayChkbox.IsEnabled = true;
            }
        }

        #region Click events
        private void LeaveDayChkbox_Click(object sender, RoutedEventArgs e)
        {
            if (numOfLeavesLeft > 0 && !keeper.LeaveDays.Contains(selectedDate))
            {
                keeper.LeaveDays.Add(selectedDate);

                // Ha szabin vagyok nem lehetek betegen!
                sickDayChkbox.IsChecked = false;
                keeper.SickDays.Remove(selectedDate);
            }
            else if (keeper.LeaveDays.Contains(selectedDate) && !leaveAutochk)
            {
                keeper.LeaveDays.Remove(selectedDate);
            }

            UpdateLeaveDays();
        }

        private void SickDayChkbox_Click(object sender, RoutedEventArgs e)
        {
            if (!keeper.SickDays.Contains(selectedDate))
            {
                keeper.SickDays.Add(selectedDate);

                // Ha betegre megyek nem lehetek szabin....
                leaveDayChkbox.IsChecked = false;
                keeper.LeaveDays.Remove(selectedDate);
                UpdateLeaveDays();
            }
            else if (keeper.SickDays.Contains(selectedDate) && !sickAutochk)
            {
                keeper.SickDays.Remove(selectedDate);
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            FillEditDataForChosenDay();
            HideLabels();
            ShowEditControls();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveDataForChosenDay();
            RefreshLabels(ref chosenDay);
            HideEditControls();
            ShowLabels();

            SaveService.SaveData(SaveUsage.No);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            HideEditControls();
            ShowLabels();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseBallon?.Invoke();
        }
        #endregion

        #region Edit methods
        private void FillEditDataForChosenDay()
        {
            WorkStartHour.Value = chosenDay.StartTime.Hours;
            WorkStartMinute.Value = chosenDay.StartTime.Minutes;
            WorkEndHour.Value = chosenDay.EndTime.Hours;
            WorkEndMinute.Value = chosenDay.EndTime.Minutes;
            EditWorkLaunchTime.Value = chosenDay.LunchBreakDuration;
            EditWorkbreakTime.Value = chosenDay.OtherBreakDuration;
            EditNonworkTime.Value = chosenDay.OverWorkDuration;
        }

        private void SaveDataForChosenDay()
        {
            try
            {
                chosenDay.SetStartTime(WorkStartHour.Value, WorkStartMinute.Value);
                chosenDay.SetEndTime(WorkEndHour.Value, WorkEndMinute.Value);
                chosenDay.SetLunchBreakDuration(EditWorkLaunchTime.Value);
                chosenDay.SetOtherBreakDuration(EditWorkbreakTime.Value);
                chosenDay.SetOverWorkDuration(EditNonworkTime.Value);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Valami nincs kitöltve megfelelően", "Hiányos kitöltés",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowLabels()
        {
            startTimeValue.Visibility = Visibility.Visible;
            endTimeValue.Visibility = Visibility.Visible;
            lunchBreakTimeValue.Visibility = Visibility.Visible;
            otherBreakTimeValue.Visibility = Visibility.Visible;
            overWorkTimeValue.Visibility = Visibility.Visible;
        }

        private void HideLabels()
        {
            startTimeValue.Visibility = Visibility.Collapsed;
            endTimeValue.Visibility = Visibility.Collapsed;
            lunchBreakTimeValue.Visibility = Visibility.Collapsed;
            otherBreakTimeValue.Visibility = Visibility.Collapsed;
            overWorkTimeValue.Visibility = Visibility.Collapsed;
        }

        private void ShowEditControls()
        {
            EditBtn.Visibility = Visibility.Collapsed;
            EditPanel.Visibility = Visibility.Visible;
            WorkStartPanel.Visibility = Visibility.Visible;
            WorkEndPanel.Visibility = Visibility.Visible;
            EditWorkLaunchTime.Visibility = Visibility.Visible;
            EditWorkbreakTime.Visibility = Visibility.Visible;
            EditNonworkTime.Visibility = Visibility.Visible;
        }

        private void HideEditControls()
        {
            EditBtn.Visibility = Visibility.Visible;
            EditPanel.Visibility = Visibility.Collapsed;
            WorkStartPanel.Visibility = Visibility.Collapsed;
            WorkEndPanel.Visibility = Visibility.Collapsed;
            EditWorkLaunchTime.Visibility = Visibility.Collapsed;
            EditWorkbreakTime.Visibility = Visibility.Collapsed;
            EditNonworkTime.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Fixes

        /// <summary>
        /// So I know it was me and not the code (code checking generates event too)
        /// </summary>
        private void LeaveDayChkbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            leaveAutochk = false;
        }

        /// <summary>
        /// So I know it was me and not the code (code checking generates event too)
        /// </summary>
        private void SickDayChkbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sickAutochk = false;
        }

        /// <summary>
        /// Enélkül a Calendarra kattintva kétszer kell máshová kattintani, hogy vegye a lapot!
        /// https://stackoverflow.com/questions/5543119/wpf-button-takes-two-clicks-to-fire-click-event#6420914
        /// </summary>
        private void Calendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            Mouse.Capture(null);
        }

        #endregion

    }
}
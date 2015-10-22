﻿namespace Dynamic.Tureng.Translator
{
    #region using

    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Dynamic.Translator.Core.Config;
    using Dynamic.Translator.Core.Extensions;
    using Dynamic.Translator.Core.ViewModel;
    using Dynamic.Translator.Core.ViewModel.Interfaces;

    #endregion

    public partial class GrowlNotifiactions : IGrowlNotifications
    {
        private readonly Notifications buffer = new Notifications();
        public readonly Notifications Notifications;
        private readonly IStartupConfiguration startupConfiguration;
        private int count;
        public bool IsDisposed;
        private int dynamicHeight;

        public GrowlNotifiactions(IStartupConfiguration startupConfiguration, Notifications notifications)
        {
            this.InitializeComponent();
            this.startupConfiguration = startupConfiguration;
            this.Notifications = notifications;
            this.NotificationsControl.DataContext = this.Notifications;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            this.AddNotificationSync(notification);
        }

        public void AddNotificationSync(Notification notification)
        {
            notification.Id = this.count++;
            if (this.Notifications.Count + 1 > this.startupConfiguration.MaxNotifications)
            {
                this.buffer.Add(notification);
            }
            else
            {
                this.Notifications.Add(notification);
            }

            if (this.Notifications.Count > 0 && !this.IsActive)
            {
                this.Show();
            }
        }

        public void RemoveNotification(Notification notification)
        {
            if (this.Notifications.Contains(notification))
            {
                this.Notifications.Remove(notification);
            }

            if (this.buffer.Count > 0)
            {
                this.Notifications.Add(this.buffer[0]);
                this.buffer.RemoveAt(0);
            }

            if (this.Notifications.Count < 1)
            {
                this.Hide();
                this.OnDispose.InvokeSafely(this, new EventArgs());
            }
        }

        public event EventHandler OnDispose;

        public void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.OnDispose.InvokeSafely(this, new EventArgs());

            this.IsDisposed = true;
        }

        private void NotificationWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Math.Abs(e.NewSize.Height) > 0.0)
            {
                return;
            }

            var element = sender as Grid;
            this.RemoveNotification(this.Notifications.First(n => element != null && n.Id == int.Parse(element.Tag.ToString())));
        }

        public int DynamicHeight
        {
            get { return 1; }
            set { this.dynamicHeight = value; }
        }
    }
}
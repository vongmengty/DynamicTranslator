﻿namespace Dynamic.Translator
{
    #region using

    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows;
    using Core.Config;
    using Core.Dependency.Manager;
    using Core.Orchestrators;
    using Orchestrators;
    using Orchestrators.Observers;

    #endregion

    public partial class MainWindow
    {
        private bool isRunning;
        private ITranslatorBootstrapper translator;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.Close();
            GC.Collect();
            GC.SuppressFinalize(this);
            Application.Current.Shutdown();
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (this.isRunning)
            {
                this.BtnSwitch.Content = "Start Translator";
                this.isRunning = false;
                if (this.translator.IsInitialized)
                    this.translator.Dispose();
            }
            else
            {
                if (!this.translator.IsInitialized)
                    this.translator.Initialize();

                this.isRunning = true;
                this.BtnSwitch.Content = "Stop Translator";
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.translator = new TranslatorBootstrapper(this,
                IocManager.Instance.Resolve<GrowlNotifiactions>(),
                IocManager.Instance.Resolve<IStartupConfiguration>());

            var translatorEvents = Observable
                .FromEventPattern<WhenClipboardContainsTextEventArgs>(
                    h => this.translator.WhenClipboardContainsTextEventHandler += h,
                    h => this.translator.WhenClipboardContainsTextEventHandler -= h);

            translatorEvents.Subscribe(new Finder(
                    IocManager.Instance.Resolve<INotifier>(),
                    IocManager.Instance.Resolve<IMeanFinderFactory>()
                    ));
        }
    }
}
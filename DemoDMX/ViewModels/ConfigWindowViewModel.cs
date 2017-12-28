using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Theia;
using Theia.DmxInterfaces;

namespace DemoDMX.ViewModels
{
    public class ConfigWindowViewModel : INotifyPropertyChanged
    {
        public ConfigWindowViewModel()
        {
            ConnectOrDisconnectCommand = new ViewModelCommand(ConnectOrDisconnect_Execute, ConnectOrDisconnect_CanExecute);
            FlushCommand = new ViewModelCommand(Flush_Execute);
            BlackoutCommand = new ViewModelCommand(Blackout_Execute);
            address = 0;
            channelsInAddress = 0;
        }

        private void Blackout_Execute(object o)
        {
            Blackout();
        }

        private void Blackout()
        {
            foreach (var channel in Channels)
            {
                channel.Value = 0;
            }
            
        }

        private void Flush_Execute(object o)
        {
            Controller.Flush();
        }

        private bool isConnected;
        private int address;
        private int channelsInAddress;

        public ViewModelCommand ConnectOrDisconnectCommand { get; }
        public ViewModelCommand FlushCommand { get; }
        public ViewModelCommand BlackoutCommand { get; }

        private void ConnectOrDisconnect_Execute(object o)
        {
            if (!isConnected)
            {
                var iface = new EnttecPropDmx(SelectedPort);
                Controller = new DmxController(iface);
                Controller.Open();
                Controller.Flush();
                Controller.StartAutoFlush(40);
                IsConnected = true;
            }
            else
            {
                Controller.StopAutoFlush();
                Controller.Close();
                IsConnected = false;
            }
        }

        private bool ConnectOrDisconnect_CanExecute(object o)
        {
            return !String.IsNullOrEmpty(SelectedPort);
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                OnPropertyChanged();
            }
        }

        public DmxController Controller { get; set; }

        public string[] PortNames => SerialPort.GetPortNames();

        public string SelectedPort { get; set; }

        public string Address
        {
            get { return address.ToString(); }
            set
            {
                int val;
                if (!int.TryParse(value, out val)) return;
                if (val > 512 || val < 1) return;
                address = val;
                UpdateChannelsList();
            }
        }

        public string ChannelsInAddress
        {
            get { return channelsInAddress.ToString(); }
            set
            {
                int val;
                if (!int.TryParse(value, out val)) return;
                if (address + val > 512 || val < 1) return;
                channelsInAddress = val;
                UpdateChannelsList();
            }
        }

        public ObservableCollection<Channel> Channels { get; } = new ObservableCollection<Channel>();

        private void UpdateChannelsList()
        {
            Channels.Clear();
            for (var i = 1; i <= channelsInAddress; i++)
            {
                Channels.Add(new Channel(Controller, i, address));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

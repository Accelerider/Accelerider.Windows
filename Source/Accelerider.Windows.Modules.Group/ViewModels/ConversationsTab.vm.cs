﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.ViewModels;
using Accelerider.Windows.Modules.Group.MockData;
using Autofac;

namespace Accelerider.Windows.Modules.Group.ViewModels
{
    public class ConversationsTabViewModel : ViewModelBase
    {
        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        private string _draft;
        private ICommand _sendCommand;

        public ConversationsTabViewModel(IContainer container) : base(container)
        {
            SendCommand = new RelayCommand(
                () =>
                {
                    Messages.Add(new Message
                    {
                        Text = Draft,
                        Author = AcceleriderUser.CurrentNetDiskUser.Username,
                        Date = DateTime.Now,
                        HeadUri = new Uri("http://tvax1.sinaimg.cn/crop.0.10.1125.1125.50/90df8663ly8ff2ts90095j20v90vtwhs.jpg")
                    });
                    Draft = string.Empty;
                },
                () => !string.IsNullOrEmpty(Draft));
        }

        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set => SetProperty(ref _messages, value);
        }

        public string Draft
        {
            get => _draft;
            set => SetProperty(ref _draft, value);
        }

        public ICommand SendCommand
        {
            get => _sendCommand;
            set => SetProperty(ref _sendCommand, value);
        }
    }
}

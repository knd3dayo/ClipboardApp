﻿
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;

namespace WpfApp1.Model
{
    public class TagItem : ObservableObject
    {
        public ObjectId? Id { get; set; }

        private string _tag = "";
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
                OnPropertyChanged("Tag");
            }
        }

        private bool _isChecked = false;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
        private bool _isDeleted = false;
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                _isDeleted = value;
                OnPropertyChanged("IsDeleted");
            }
        }
    }

}
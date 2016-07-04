﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jaze.Control
{
    /// <summary>
    /// Interaction logic for ListSearchHistory.xaml
    /// </summary>
    public partial class ListSearchHistory : UserControl
    {
        public ListSearchHistory()
        {
            InitializeComponent();
        }

        public IEnumerable ItemsSource
        {
            get { return listBox.ItemsSource; }
            set { listBox.ItemsSource = value; }
        }
        public event RoutedEventHandler Show
        {
            add
            {
                AddHandler(ListSearchResult.ShowEvent, value);
            }
            remove
            {
                RemoveHandler(ListSearchResult.ShowEvent, value);
            }
        }


        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ListSearchResult.ShowEvent, listBox.SelectedItem));
        }
    }
}

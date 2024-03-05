using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;
using Epsiloner.Wpf.Utils;
using Microsoft.Win32;

namespace Sample_1
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ApplyStyleForSelectedBehavior_OnClick(object sender, RoutedEventArgs e)
        {
            new SampleApplyStyleForSelectedBehavior.SampleApplyStyleForSelectedBehavior().Show();
        }

        private void KeyboardNavigationBehavior_OnClick(object sender, RoutedEventArgs e)
        {
            new SampleKeyboardNavigationBehavior.SampleKeyboardNavigationBehavior().Show();
        }

        private void MultiStyleExtension_OnClick(object sender, RoutedEventArgs e)
        {
            new SampleMultiStyleExtension.SampleMultiStyleExtension().Show();
        }

        private void GridColumnsForItemsBehavior_OnClick(object sender, RoutedEventArgs e)
        {
            new SampleSmartGridBehavior.SampleSmartGridBehavior().Show();
        }

        private SaveFileDialog _saveFileDialog;

        private void XamlUtilsSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (_saveFileDialog == null)
            {
                _saveFileDialog = new SaveFileDialog
                {
                    CreatePrompt = true,
                    CheckPathExists = true
                };
            }


            if (_saveFileDialog.ShowDialog() != true)
                return;

            var p = new Person
            {
                FirstName = "Venia",
                LastName = "HIDDEN",
                Age = 29,
                Contacts =
                {
                    {"e-mail", "sample@mail.com"},
                    {"phone", "123456789"}
                }
            };

            XamlUtils.Save(p, _saveFileDialog.FileName);
        }
    }

    public class Person
    {
        private Dictionary<string, string> _contacts = new Dictionary<string, string>();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Age { get; set; }

        public Dictionary<string, string> Contacts
        {
            get => _contacts;
            set => _contacts = value ?? new Dictionary<string, string>();
        }
        
        public bool ShouldSerializeLastName() {
            return false;
        }

        public bool ShouldSerialize(string element)
        {
            return true;
        }
        
        public bool ShouldSerialize(object element)
        {
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MMIS
{
    /// <summary>
    /// Interaction logic for ucHDRTile.xaml
    /// </summary>
    public partial class ucHDRTile : UserControl
    {
        public ucHDRTile()
        {
            InitializeComponent();
        }

        public string Header { get; set; }

        public string Production { get; set; }

        public string ProductionDifference { get; set; }
    }
}

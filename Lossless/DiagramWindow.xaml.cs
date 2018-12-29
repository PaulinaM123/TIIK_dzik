using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace Lossless
{
    public partial class DiagramWindow : Window
    {
        public DiagramWindow(string TextToAnalyzing, string WindowName = "Graph")
        {
            InitializeComponent();
            this.Title = WindowName;
            var x = Helpers.CountCharacters(TextToAnalyzing);
            Helpers.LoadDataToColumnSeries((DataPointSeries)mcChart.Series[0], x);
        }
    }
}

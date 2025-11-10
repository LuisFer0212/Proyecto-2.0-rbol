using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    public class StatisticsForm : Form
    {
        public StatisticsForm()
        {
            Text = "Estadísticas (vista)";
            Width = 520;
            Height = 340;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 11);
            BackColor = Theme.BgMain;

            var header = new Label
            {
                Text = "📊 Estadísticas (interfaz, sin cálculos)",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.BgMain
            };
            Controls.Add(header);

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Theme.BgMain
            };
            Controls.Add(panel);

            panel.Controls.Add(Metric("Par más lejano:",        "— / —",  20));
            panel.Controls.Add(Metric("Par más cercano:",       "— / —",  80));
            panel.Controls.Add(Metric("Distancia promedio:",    "— km",   140));
        }

        private Control Metric(string label, string value, int top)
        {
            var container = new Panel
            {
                Left = 10,
                Top = top,
                Width = 460,
                Height = 48,
                BackColor = Theme.Card
            };

            var lbl = new Label
            {
                Text = label,
                Left = 14,
                Top = 12,
                Width = 220,
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Card
            };
            var val = new Label
            {
                Text = value,
                Left = 240,
                Top = 12,
                Width = 200,
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Card
            };

            container.Controls.Add(lbl);
            container.Controls.Add(val);
            return container;
        }
    }
}

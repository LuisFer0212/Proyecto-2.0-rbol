// Interfaz.Estadisticas.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Proyecto_2_Arbol
{
    public class StatisticsForm : Form
    {
        private readonly ArbolGenealogico arbol;

        private Label lblParLejano;
        private Label lblParCercano;
        private Label lblDistanciaPromedio;

        public StatisticsForm(ArbolGenealogico arbol)
        {
            this.arbol = arbol ?? throw new ArgumentNullException(nameof(arbol));

            Text = "Estadísticas";
            Width = 520;
            Height = 340;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 11);
            BackColor = Theme.BgMain;

            var header = new Label
            {
                Text = "📊 Estadísticas del Árbol",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
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

            lblParLejano = AddMetric(panel, "Par más lejano:", "— / —", 80);
            lblParCercano = AddMetric(panel, "Par más cercano:", "— / —", 140);
            lblDistanciaPromedio = AddMetric(panel, "Distancia promedio:", "— km", 200);

            CargarEstadisticas();
        }

        private Label AddMetric(Panel panel, string labelText, string valueText, int top)
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
                Text = labelText,
                Left = 14,
                Top = 12,
                Width = 220,
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Card
            };
            var val = new Label
            {
                Text = valueText,
                Left = 240,
                Top = 12,
                Width = 200,
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Card
            };

            container.Controls.Add(lbl);
            container.Controls.Add(val);
            panel.Controls.Add(container);

            return val;
        }

        private void CargarEstadisticas()
        {
            var lista = arbol.ObtenerTodosLosFamiliares();

            if (lista.Length < 2)
            {
                lblParLejano.Text = "— / —";
                lblParCercano.Text = "— / —";
                lblDistanciaPromedio.Text = "— km";
                return;
            }

            // Obtener estadísticas directamente del grafo
            var (c1, c2, l1, l2, promedio) = arbol.ObtenerEstadisticasGrafo();

            // Par más lejano
            lblParLejano.Text = (l1 != null && l2 != null)
                ? $"{l1.Nombre} / {l2.Nombre}"
                : "— / —";

            // Par más cercano
            lblParCercano.Text = (c1 != null && c2 != null)
                ? $"{c1.Nombre} / {c2.Nombre}"
                : "— / —";

            // Promedio de distancias
            lblDistanciaPromedio.Text = $"{promedio:F2} km";
        }

    }
}
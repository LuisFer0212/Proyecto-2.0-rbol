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

            if (lista.Count < 2)
            {
                // Si hay menos de 2 personas, no se pueden calcular pares
                lblParLejano.Text = "— / —";
                lblParCercano.Text = "— / —";
                lblDistanciaPromedio.Text = "— km";
                return;
            }

            double distanciaPromedio;
            var (lejano1, lejano2, _) = arbol.ParMasCercanoYLejano(out distanciaPromedio);

            lblParLejano.Text = lejano1 != null && lejano2 != null 
                ? $"{lejano1.Nombre} / {lejano2.Nombre}" 
                : "— / —";

            // Par más cercano
            double minDist = double.MaxValue;
            Familiar? cercano1 = null, cercano2 = null;

            for (int i = 0; i < lista.Count; i++)
            {
                for (int j = i + 1; j < lista.Count; j++)
                {
                    double d = GeoHelper.DistanciaKm(lista[i].Latitud, lista[i].Longitud,
                                                    lista[j].Latitud, lista[j].Longitud);
                    if (d < minDist)
                    {
                        minDist = d;
                        cercano1 = lista[i];
                        cercano2 = lista[j];
                    }
                }
            }

            lblParCercano.Text = (cercano1 != null && cercano2 != null) 
                ? $"{cercano1.Nombre} / {cercano2.Nombre}" 
                : "— / —";

            lblDistanciaPromedio.Text = $"{distanciaPromedio:F2} km";
        }
    }
}
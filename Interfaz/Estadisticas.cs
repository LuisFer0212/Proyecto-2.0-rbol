// Interfaz.Estadisticas.cs
// Ventana de estadísticas del árbol genealógico (solo cambios estéticos).

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    // Ventana que muestra las estadísticas generales del árbol genealógico.
    public class StatisticsForm : Form
    {
        // Guarda la referencia al árbol para poder calcular las estadísticas.
        private readonly ArbolGenealogico arbol;

        // Etiquetas donde se muestran los resultados calculados.
        private Label lblParLejano;
        private Label lblParCercano;
        private Label lblDistanciaPromedio;

        public StatisticsForm(ArbolGenealogico arbol)
        {
            // Guarda el árbol que llega desde la ventana principal.
            this.arbol = arbol ?? throw new ArgumentNullException(nameof(arbol));

            // Ajustes básicos de la ventana de estadísticas.
            Text = "Estadísticas";
            Width = 520;
            Height = 500; // Ventana un poco más alta para que todo quepa cómodo.
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 11);
            BackColor = Theme.BgMain;

            // Encabezado azul similar al de las otras ventanas.
            var header = new Label
            {
                Text = "📊 Estadísticas del Árbol",
                Dock = DockStyle.Top,
                Height = 70,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Theme.Btn,       // Azul del tema.
                ForeColor = Theme.TextOnPane // Texto claro sobre fondo azul.
            };
            Controls.Add(header);

            // Panel donde se colocan las tarjetas con cada métrica.
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Theme.BgMain
            };
            Controls.Add(panel);

            // Crea las tres métricas que se van a mostrar en pantalla.
            // Se deja espacio vertical suficiente para que no se corten los textos.
            lblParLejano = AddMetric(panel, "Par más lejano:", "— / —", 80);
            lblParCercano = AddMetric(panel, "Par más cercano:", "— / —", 160);
            lblDistanciaPromedio = AddMetric(panel, "Distancia promedio:", "— km", 240);

            // Carga los valores reales a partir del árbol.
            CargarEstadisticas();
        }

        // Crea una fila de estadística (título + valor) dentro del panel.
        private Label AddMetric(Panel panel, string labelText, string valueText, int top)
        {
            // Tarjeta que agrupa el texto de la métrica.
            var container = new Panel
            {
                Left = 10,
                Top = top,
                Width = 460,
                Height = 70,          // Cajita más alta para que no se corten las letras.
                BackColor = Theme.Card
            };

            // Etiqueta con el nombre de la métrica.
            var lbl = new Label
            {
                Text = labelText,
                Left = 14,
                Top = 15,
                Width = 220,
                Height = 40,          // Alto suficiente para texto de una o dos líneas.
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Card
            };

            // Etiqueta donde se muestra el valor calculado.
            var val = new Label
            {
                Text = valueText,
                Left = 240,
                Top = 15,
                Width = 200,
                Height = 40,          // Igual alto para que no se corte el valor.
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Card
            };

            container.Controls.Add(lbl);
            container.Controls.Add(val);
            panel.Controls.Add(container);

            // Devuelve la etiqueta del valor para poder actualizarla luego.
            return val;
        }

        // Calcula y muestra las estadísticas del árbol en las etiquetas.
        private void CargarEstadisticas()
        {
            // Pide todos los familiares al árbol para saber si hay datos suficientes.
            var lista = arbol.ObtenerTodosLosFamiliares();

            // Si hay menos de dos personas no tiene sentido calcular distancias.
            if (lista.Length < 2)
            {
                lblParLejano.Text = "— / —";
                lblParCercano.Text = "— / —";
                lblDistanciaPromedio.Text = "— km";
                return;
            }

            // Obtener estadísticas directamente del grafo de residencias.
            var (c1, c2, l1, l2, promedio) = arbol.ObtenerEstadisticasGrafo();

            // Par más lejano.
            lblParLejano.Text = (l1 != null && l2 != null)
                ? $"{l1.Nombre} / {l2.Nombre}"
                : "— / —";

            // Par más cercano.
            lblParCercano.Text = (c1 != null && c2 != null)
                ? $"{c1.Nombre} / {c2.Nombre}"
                : "— / —";

            // Promedio de distancias entre todos los pares.
            lblDistanciaPromedio.Text = $"{promedio:F2} km";
        }
    }
}

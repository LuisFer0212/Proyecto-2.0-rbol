using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    public class MainForm : Form
    {
        private Panel panelMenu;
        private Label lblTitulo;
        private Button btnFamilia;
        private Button btnMapa;
        private Button btnEstadisticas;
        private Button btnAcerca;
        private Button btnSalir;
        private Panel panelContenido;

        public MainForm()
        {
            // === Ventana principal ===
            Text = "Árbol Genealógico - Proyecto 2";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;

            // === Panel lateral de menú ===
            panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(40, 40, 60)
            };
            Controls.Add(panelMenu);

            // === Título del menú ===
            lblTitulo = new Label
            {
                Text = "Proyecto 2\nÁrbol Genealógico",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 100,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelMenu.Controls.Add(lblTitulo);

            // === Botones del menú ===
            btnFamilia = CrearBotonMenu("👨‍👩‍👧 Gestionar Familiares", 120);
            btnMapa = CrearBotonMenu("🗺️ Ver Mapa", 180);
            btnEstadisticas = CrearBotonMenu("📊 Estadísticas", 240);
            btnAcerca = CrearBotonMenu("ℹ️ Acerca de", 300);
            btnSalir = CrearBotonMenu("🚪 Salir", 360);

            panelMenu.Controls.AddRange(new Control[] { btnFamilia, btnMapa, btnEstadisticas, btnAcerca, btnSalir });

            // === Panel de contenido principal ===
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke
            };
            Controls.Add(panelContenido);

            // Eventos
            btnSalir.Click += (s, e) => Close();
            btnMapa.Click += (s, e) =>
            {
                // Aquí abrimos tu mapa
                MapaForm mapa = new MapaForm();
                mapa.ShowDialog();
            };
            btnFamilia.Click += (s, e) =>
            {
                FamiliaForm ventana = new FamiliaForm();
                ventana.ShowDialog();
            };

        }

        private Button CrearBotonMenu(string texto, int top)
        {
            return new Button
            {
                Text = texto,
                Width = 200,
                Height = 40,
                Top = top,
                Left = 10,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 80)
            };
        }
    }
}

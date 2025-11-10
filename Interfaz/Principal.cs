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
        private Button btnSalir;
        private Panel panelContenido;

        public MainForm()
        {
            // === Ventana principal ===
            Text = "Árbol Genealógico - Proyecto 2";
            Width = 1100;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            BackColor = ColorTranslator.FromHtml("#F4F6FA");
            Font = new Font("Segoe UI", 11, FontStyle.Regular);

            // === Panel lateral ===
            panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = ColorTranslator.FromHtml("#283142")
            };
            Controls.Add(panelMenu);

            // === Título ===
            lblTitulo = new Label
            {
                Text = "🌳 Árbol Genealógico",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 100,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelMenu.Controls.Add(lblTitulo);

            // === Botones menú ===
            btnFamilia = CrearBotonMenu("👨‍👩‍👧 Gestionar Familia");
            btnMapa = CrearBotonMenu("🗺️ Ver Mapa");
            btnEstadisticas = CrearBotonMenu("📊 Estadísticas");
            btnSalir = CrearBotonMenu("🚪 Salir");

            // Posiciones verticales
            int top = 130;
            foreach (var boton in new[] { btnFamilia, btnMapa, btnEstadisticas, btnSalir })
            {
                boton.Top = top;
                top += 60;
                panelMenu.Controls.Add(boton);
            }

            // === Panel contenido (para el árbol genealógico) ===
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorTranslator.FromHtml("#F4F6FA")
            };
            Controls.Add(panelContenido);

            // === Eventos ===
            btnSalir.Click += (s, e) => Close();
            btnFamilia.Click += (s, e) =>
            {
                new FamiliaForm().ShowDialog();
            };
            btnMapa.Click += (s, e) =>
            {
                new MapaForm().ShowDialog();
            };
            btnEstadisticas.Click += (s, e) =>
            {
                MessageBox.Show("Sección de estadísticas (interfaz próximamente).", "Estadísticas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
        }

        private Button CrearBotonMenu(string texto)
        {
            var btn = new Button
            {
                Text = texto,
                Width = 200,
                Height = 45,
                Left = 15,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#4C6EF5"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            // Efecto hover
            btn.MouseEnter += (s, e) => btn.BackColor = ColorTranslator.FromHtml("#3B5BDB");
            btn.MouseLeave += (s, e) => btn.BackColor = ColorTranslator.FromHtml("#4C6EF5");
            return btn;
        }
    }
}

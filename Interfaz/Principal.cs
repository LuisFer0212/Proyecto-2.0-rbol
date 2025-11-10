using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    public class MainForm : Form
    {
        private Panel panelMenu;
        private Label lblTitulo;
        private Button btnFamilia, btnMapa, btnEstadisticas, btnSalir, btnDark;
        private Panel panelContenido;
        private TreeCanvas canvas;

        public MainForm()
        {
            BuildUI();
            ApplyTheme();
        }

        private void BuildUI()
        {
            // === Ventana principal ===
            Text = "Árbol Genealógico - Proyecto 2";
            Width = 1100;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Font = new Font("Segoe UI", 11);
            DoubleBuffered = true;

            // === Panel de contenido (donde va el árbol) ===
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16)
            };

            // Lienzo del árbol (solo interfaz, sin lógica)
            canvas = new TreeCanvas
            {
                Dock = DockStyle.Fill
            };
            panelContenido.Controls.Add(canvas);

            // === Panel lateral (menú) ===
            panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230
            };

            // Título
            lblTitulo = new Label
            {
                Text = "🌳 Árbol Genealógico",
                Dock = DockStyle.Top,
                Height = 100,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelMenu.Controls.Add(lblTitulo);

            // Botones del menú
            btnFamilia = CreateMenuButton("👨‍👩‍👧 Gestionar Familia");
            btnMapa = CreateMenuButton("🗺️ Ver Mapa");
            btnEstadisticas = CreateMenuButton("📊 Estadísticas");
            btnDark = CreateMenuButton("🌙 Modo oscuro");
            btnSalir = CreateMenuButton("🚪 Salir");

            int top = 130;
            foreach (var b in new[] { btnFamilia, btnMapa, btnEstadisticas, btnDark, btnSalir })
            {
                b.Top = top;
                b.Left = 15;
                panelMenu.Controls.Add(b);
                top += 60;
            }

            // === Agregar paneles al formulario ===
            // IMPORTANTE: primero el contenido, luego el menú (para que no lo tape)
            Controls.Add(panelContenido);
            Controls.Add(panelMenu);

            // === Eventos de botones ===
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
                new StatisticsForm().ShowDialog();
            };

            btnDark.Click += (s, e) =>
            {
                Theme.Dark = !Theme.Dark;
                ApplyTheme();
            };
        }

        private Button CreateMenuButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Width = 200,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 11, FontStyle.Regular)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void ApplyTheme()
        {
            // Colores generales
            BackColor = Theme.BgMain;
            panelMenu.BackColor = Theme.BgPane;
            lblTitulo.ForeColor = Theme.TextOnPane;

            // Aplicar tema a todos los botones del menú
            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button b)
                {
                    b.BackColor = Theme.Btn;
                    b.ForeColor = Color.White;

                    // Efecto hover
                    b.MouseEnter -= HoverIn;
                    b.MouseLeave -= HoverOut;
                    b.MouseEnter += HoverIn;
                    b.MouseLeave += HoverOut;
                }
            }

            // Aplicar tema al panel de contenido y al lienzo
            panelContenido.BackColor = Theme.BgMain;
            canvas.BackColor = Theme.Card;
            canvas.ForeColor = Theme.TextPrimary;

            // Forzar repintado
            canvas.Invalidate();
        }

        private void HoverIn(object? sender, EventArgs e)
        {
            if (sender is Button b)
                b.BackColor = Theme.BtnHover;
        }

        private void HoverOut(object? sender, EventArgs e)
        {
            if (sender is Button b)
                b.BackColor = Theme.Btn;
        }
    }
}

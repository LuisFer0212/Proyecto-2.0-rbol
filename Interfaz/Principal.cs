//Interfaz.Principal.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    // Ventana principal de la aplicación.
    // Desde aquí se puede acceder al mapa, a las estadísticas y al árbol genealógico.
    public class MainForm : Form
    {
        // Árbol genealógico principal de la aplicación.
        private readonly ArbolGenealogico arbol;

        // Panel lateral del menú.
        private Panel panelMenu;

        // Etiqueta del título principal.
        private Label lblTitulo;

        // Botones del menú lateral.
        private Button btnMapa;
        private Button btnEstadisticas;
        private Button btnEliminarArbol;
        private Button btnSalir;

        // Panel donde se muestra el contenido central.
        private Panel panelContenido;

        // Lienzo donde se dibuja el árbol genealógico.
        private TreeCanvas canvas;

        // Constructor de la ventana principal.
        // Carga el árbol desde disco, arma la interfaz y aplica los colores del tema visual.
        public MainForm()
        {
            // Crea la instancia del árbol genealógico.
            arbol = new ArbolGenealogico();

            // Intenta cargar el árbol guardado en el archivo si existe.
            arbol.CargarDesdeArchivo();

            // Construye la interfaz con el árbol actual (vacío o cargado).
            BuildUI();
            ApplyTheme();
        }

        // Construye la interfaz gráfica de la ventana principal.
        private void BuildUI()
        {
            // Configuración general de la ventana.
            Text = "Árbol Genealógico - Proyecto 2";
            Width = 1100;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Font = new Font("Segoe UI", 11);
            DoubleBuffered = true;

            // Panel central de contenido.
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16)
            };

            Controls.Add(panelContenido);

            // Lienzo del árbol.
            canvas = new TreeCanvas(arbol)
            {
                Dock = DockStyle.Fill
            };
            panelContenido.Controls.Add(canvas);

            // Panel lateral del menú.
            panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230
            };

            // Título del panel lateral.
            lblTitulo = new Label
            {
                Text = "🌳 Árbol Genealógico",
                Dock = DockStyle.Top,
                Height = 100,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };
            panelMenu.Controls.Add(lblTitulo);

            // Creación de los botones del menú.
            btnMapa = CreateMenuButton("🗺️ Ver Mapa");
            btnEstadisticas = CreateMenuButton("📊 Estadísticas");
            btnEliminarArbol = CreateMenuButton("🧹 Eliminar árbol");
            btnSalir = CreateMenuButton("🚪 Salir");

            // Ubicación vertical de los botones dentro del panel lateral.
            int top = 130;
            foreach (var boton in new[] { btnMapa, btnEstadisticas, btnEliminarArbol, btnSalir })
            {
                boton.Top = top;
                boton.Left = 15;
                panelMenu.Controls.Add(boton);
                top += 55;
            }

            // Eventos de clic para cada botón.
            btnMapa.Click += BtnMapa_Click;
            btnEstadisticas.Click += BtnEstadisticas_Click;
            btnEliminarArbol.Click += BtnEliminarArbol_Click;
            btnSalir.Click += (s, e) => Close();

            Controls.Add(panelMenu);
        }

        // Crea un botón del menú lateral con estilo base.
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

        // Maneja el clic del botón que elimina el árbol genealógico.
        private void BtnEliminarArbol_Click(object? sender, EventArgs e)
        {
            var resultado = MessageBox.Show(
                "¿Seguro que desea eliminar el árbol genealógico actual?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (resultado == DialogResult.Yes)
            {
                arbol.Limpiar();
                canvas.Invalidate();

                MessageBox.Show(
                    "El árbol se eliminó correctamente.",
                    "Información",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        // Aplica los colores definidos en la clase Theme a la ventana principal y a sus controles.
        private void ApplyTheme()
        {
            // Colores de fondo generales.
            BackColor = Theme.BgMain;
            panelMenu.BackColor = Theme.BgPane;
            lblTitulo.ForeColor = Theme.TextOnPane;

            // Estilo para los botones del menú lateral.
            foreach (Control control in panelMenu.Controls)
            {
                if (control is Button boton)
                {
                    boton.BackColor = Theme.Btn;
                    boton.ForeColor = Color.White;

                    // Eventos de resaltado al pasar el puntero.
                    boton.MouseEnter -= HoverIn;
                    boton.MouseLeave -= HoverOut;
                    boton.MouseEnter += HoverIn;
                    boton.MouseLeave += HoverOut;
                }
            }
        }

        // Maneja el clic del botón que abre la ventana del mapa.
        private void BtnMapa_Click(object? sender, EventArgs e)
        {
            using (var mapaForm = new MapaForm(arbol))
            {
                mapaForm.ShowDialog(this);
            }
        }

        // Maneja el clic del botón que abre la ventana de estadísticas.
        private void BtnEstadisticas_Click(object? sender, EventArgs e)
        {
            using (var estadisticasForm = new StatisticsForm(arbol))
            {
                estadisticasForm.ShowDialog(this);
            }
        }

        // Cambia el fondo del botón cuando el puntero entra en el botón.
        private void HoverIn(object? sender, EventArgs e)
        {
            if (sender is Button boton)
            {
                boton.BackColor = Theme.BtnHover;
            }
        }

        // Restaura el fondo del botón cuando el puntero sale del botón.
        private void HoverOut(object? sender, EventArgs e)
        {
            if (sender is Button boton)
            {
                boton.BackColor = Theme.Btn;
            }
        }
    }
}

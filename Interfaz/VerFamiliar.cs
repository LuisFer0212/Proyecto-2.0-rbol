//Interfaz.VerFamiliar.cs
// Archivo: VerFamiliar.cs
// Ventana de solo lectura para ver la información de un familiar.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    // Ventana que muestra los datos de un familiar sin permitir edición.
    public class VerFamiliar : Form
    {
        // Guarda la referencia al familiar que se quiere mostrar.
        private readonly Familiar familiar;

        public VerFamiliar(Familiar familiar)
        {
            // Guarda el familiar que se va a mostrar en la ventana.
            this.familiar = familiar ?? throw new ArgumentNullException(nameof(familiar));

            ConfigurarVentana();
            ConstruirContenido();
        }

        // Configura las propiedades básicas de la ventana.
        private void ConfigurarVentana()
        {
            // Título de la ventana.
            Text = "Información del familiar";

            // Tamaño base de la ventana.
            Width = 580;
            Height = 460;

            // Posición centrada respecto a la ventana que la abre.
            StartPosition = FormStartPosition.CenterParent;

            // Evita que la ventana se maximice o cambie de tamaño.
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Colores y fuente generales.
            BackColor = Theme.BgMain;
            Font = new Font("Segoe UI", 11, FontStyle.Regular);
        }

        // Construye todos los controles que se muestran en la ventana.
        private void ConstruirContenido()
        {
            Controls.Clear();

            // Franja superior con el título.
            var lblTitulo = new Label
            {
                Text = "Información del familiar",
                Dock = DockStyle.Top,
                Height = 70,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Theme.Btn,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblTitulo);

            // Panel principal que contiene la tarjeta y el botón.
            var panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Theme.BgMain,
                AutoScroll = true // Activa scroll por si el contenido crece.
            };
            Controls.Add(panelContenido);
            panelContenido.BringToFront();

            // Panel inferior donde se ubica el botón de cerrar.
            var panelBoton = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Theme.BgMain
            };
            panelContenido.Controls.Add(panelBoton);

            // Botón para cerrar la ventana.
            var btnCerrar = new Button
            {
                Text = "Cerrar",
                Width = 110,
                Height = 40,
                BackColor = Theme.Btn,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Regular)
            };
            btnCerrar.FlatAppearance.BorderSize = 0;

            // Centra el botón dentro del panel inferior.
            btnCerrar.Anchor = AnchorStyles.None;
            panelBoton.Controls.Add(btnCerrar);
            panelBoton.Resize += (s, e) =>
            {
                // Centra el botón cada vez que cambie el tamaño del panel.
                btnCerrar.Left = (panelBoton.Width - btnCerrar.Width) / 2;
                btnCerrar.Top = (panelBoton.Height - btnCerrar.Height) / 2;
            };

            btnCerrar.Click += (s, e) => Close();

            // Tarjeta blanca donde se muestran los datos.
            var panelCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Card,
                Padding = new Padding(16)
            };
            panelContenido.Controls.Add(panelCard);
            panelCard.BringToFront();

            // Tabla para organizar etiquetas y valores.
            var tabla = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 7,
                Dock = DockStyle.Fill,
                BackColor = Theme.Card,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Configura las columnas de la tabla.
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));

            // Configura las filas de la tabla (alto automático).
            for (int i = 0; i < 7; i++)
            {
                tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            panelCard.Controls.Add(tabla);

            // Llena la tabla con la información del familiar.
            AgregarFila(tabla, 0, "Nombre:", familiar.Nombre);
            AgregarFila(tabla, 1, "Cédula:", familiar.Cedula);
            AgregarFila(tabla, 2, "Fecha de nacimiento:", familiar.FechaNacimiento.ToString("dd/MM/yyyy"));
            AgregarFila(tabla, 3, "Edad:", familiar.Edad.ToString(CultureInfo.InvariantCulture));
            AgregarFila(tabla, 4, "Latitud:", familiar.Latitud.ToString("F6", CultureInfo.InvariantCulture));
            AgregarFila(tabla, 5, "Longitud:", familiar.Longitud.ToString("F6", CultureInfo.InvariantCulture));
            AgregarFila(tabla, 6, "Familia política:", familiar.EsFamiliaPolitica ? "Sí" : "No");
        }

        // Agrega una fila con etiqueta y valor a la tabla.
        private void AgregarFila(TableLayoutPanel tabla, int fila, string etiqueta, string valor)
        {
            // Etiqueta del campo.
            var lblEtiqueta = new Label
            {
                Text = etiqueta,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Card,
                Padding = new Padding(0, 4, 8, 4)
            };

            // Valor del campo.
            var lblValor = new Label
            {
                Text = valor,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Card,
                Padding = new Padding(0, 4, 0, 4),
                AutoEllipsis = true
            };

            tabla.Controls.Add(lblEtiqueta, 0, fila);
            tabla.Controls.Add(lblValor, 1, fila);
        }
    }
}

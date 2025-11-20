//Interfaz.GestionarFamilia.cs
// Archivo: GestionarFamilia.cs
// Ventana de Windows Forms para registrar o agregar familiares.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    // Esta ventana se usa para crear el primer integrante
    // o para agregar un hijo o una pareja a alguien ya registrado.
    public class GestionarFamilia : Form
    {
        // Referencias a la parte de lógica.
        private readonly ArbolGenealogico arbol;
        private readonly Familiar? familiarReferencia;
        private readonly bool esMiembroInicial;

        // Controles de datos del familiar.
        private TextBox txtNombre;
        private TextBox txtCedula;
        private TextBox txtLatitud;
        private TextBox txtLongitud;
        private TextBox txtEdad;
        private DateTimePicker fechaNacimiento;
        private PictureBox fotoPreview;

        // Controles para indicar si es hijo o pareja.
        private GroupBox? grpRelacion;
        private RadioButton? rdbHijo;
        private RadioButton? rdbPareja;

        // Ruta de la foto seleccionada.
        private string rutaFoto = string.Empty;

        // Familiar que se genera al guardar.
        public Familiar? FamiliarCreado { get; private set; }

        public GestionarFamilia(ArbolGenealogico arbol, Familiar? familiarReferencia, bool esMiembroInicial)
        {
            this.arbol = arbol ?? throw new ArgumentNullException(nameof(arbol));
            this.familiarReferencia = familiarReferencia;
            this.esMiembroInicial = esMiembroInicial;

            ConfigurarVentana();
            ConstruirControles();
        }

        // Ajusta la apariencia general de la ventana.
        private void ConfigurarVentana()
        {
            Text = "Registrar familiar";
            Width = 640;
            Height = 860;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = ColorTranslator.FromHtml("#F4F6FA");
            Font = new Font("Segoe UI", 11, FontStyle.Regular);
        }

        // Construye y acomoda todos los controles.
        private void ConstruirControles()
        {
            Controls.Clear();

            // Franja azul con el título.
            var lblTitulo = new Label
            {
                Text = "Datos del familiar",
                Dock = DockStyle.Top,
                Height = 70,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#4C6EF5"),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblTitulo);

            // Contenedor principal con desplazamiento vertical.
            var contenedor = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(30, 80, 30, 30) // espacio para despegar del encabezado azul
            };
            Controls.Add(contenedor);

            // Grupo de relación (solo si no es el primer integrante).
            if (!esMiembroInicial && arbol.TieneMiembros && familiarReferencia != null)
            {
                grpRelacion = new GroupBox
                {
                    Text = "Relación con " + familiarReferencia.Nombre,
                    Width = 560,
                    Height = 90,
                    Margin = new Padding(0, 0, 0, 20)
                };

                // Radio para hijo / hija.
                rdbHijo = new RadioButton
                {
                    Text = "Hijo / Hija",
                    AutoSize = true,
                    Location = new Point(20, 35),
                    Checked = true
                };

                // Radio para pareja, colocado a la par.
                rdbPareja = new RadioButton
                {
                    Text = "Pareja",
                    AutoSize = true,
                    Location = new Point(200, 35)
                };

                grpRelacion.Controls.Add(rdbHijo);
                grpRelacion.Controls.Add(rdbPareja);
                contenedor.Controls.Add(grpRelacion);
            }

            // Tabla con etiquetas y campos.
            var tablaDatos = new TableLayoutPanel
            {
                ColumnCount = 2,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Width = 560,
                Padding = new Padding(0),
                Margin = new Padding(0, 0, 0, 20)
            };

            // Columna de etiquetas.
            tablaDatos.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            // Columna de cajas de texto.
            tablaDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int fila = 0;

            // Nombre completo.
            AgregarFilaTexto(tablaDatos, fila++, "Nombre completo:", out txtNombre);

            // Cédula.
            AgregarFilaTexto(tablaDatos, fila++, "Cédula:", out txtCedula);

            // Fecha de nacimiento.
            {
                var lblFecha = CrearEtiqueta("Fecha de nacimiento:");
                fechaNacimiento = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    MaxDate = DateTime.Today,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    Width = 260,
                    Margin = new Padding(0, 5, 0, 5)
                };

                tablaDatos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tablaDatos.Controls.Add(lblFecha, 0, fila);
                tablaDatos.Controls.Add(fechaNacimiento, 1, fila);
                fila++;
            }

            // Edad.
            AgregarFilaTexto(tablaDatos, fila++, "Edad (si falleció, edad al morir):", out txtEdad);

            // Latitud.
            AgregarFilaTexto(tablaDatos, fila++, "Latitud (residencia):", out txtLatitud);

            // Longitud.
            AgregarFilaTexto(tablaDatos, fila++, "Longitud (residencia):", out txtLongitud);

            contenedor.Controls.Add(tablaDatos);

            // Botón para seleccionar foto.
            var btnSeleccionarFoto = new Button
            {
                Text = "Seleccionar foto",
                Width = 560,
                Height = 40,
                BackColor = ColorTranslator.FromHtml("#5C7CFA"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 0, 10)
            };
            btnSeleccionarFoto.FlatAppearance.BorderSize = 0;
            btnSeleccionarFoto.Click += BtnSeleccionarFoto_Click;
            contenedor.Controls.Add(btnSeleccionarFoto);

            // Vista previa de la foto.
            fotoPreview = new PictureBox
            {
                Width = 260,
                Height = 190,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 15)
            };
            contenedor.Controls.Add(fotoPreview);

            // Panel con los botones Guardar y Cancelar.
            var panelBotones = new Panel
            {
                Width = 560,
                Height = 55,
                Margin = new Padding(0, 5, 0, 0)
            };

            var btnGuardar = new Button
            {
                Text = "Guardar",
                Left = 0,
                Top = 5,
                Width = 250,
                Height = 45,
                BackColor = ColorTranslator.FromHtml("#37B24D"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnGuardar.FlatAppearance.BorderSize = 0;

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Left = 300,
                Top = 5,
                Width = 250,
                Height = 45,
                BackColor = ColorTranslator.FromHtml("#ADB5BD"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancelar.FlatAppearance.BorderSize = 0;

            panelBotones.Controls.Add(btnGuardar);
            panelBotones.Controls.Add(btnCancelar);
            contenedor.Controls.Add(panelBotones);

            // Eventos de los botones inferiores.
            btnCancelar.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            btnGuardar.Click += BtnGuardar_Click;
        }

        // Crea una etiqueta simple para la tabla de datos.
        private Label CrearEtiqueta(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 8, 8, 8)
            };
        }

        // Agrega una fila de "Etiqueta + TextBox" a la tabla.
        private void AgregarFilaTexto(TableLayoutPanel tabla, int fila, string textoEtiqueta, out TextBox caja)
        {
            var etiqueta = CrearEtiqueta(textoEtiqueta);
            caja = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Width = 260,
                Margin = new Padding(0, 5, 0, 5)
            };

            tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tabla.Controls.Add(etiqueta, 0, fila);
            tabla.Controls.Add(caja, 1, fila);
        }

        // Permite seleccionar la fotografía y mostrarla en el recuadro.
        private void BtnSeleccionarFoto_Click(object? sender, EventArgs e)
        {
            using (var dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Seleccionar fotografía";
                dialogo.Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp";

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    rutaFoto = dialogo.FileName;
                    try
                    {
                        fotoPreview.Image = Image.FromFile(rutaFoto);
                    }
                    catch
                    {
                        MessageBox.Show(
                            "No fue posible cargar la imagen seleccionada.",
                            "Aviso",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
            }
        }

        // Valida los datos del formulario y registra el familiar en el árbol.
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            // Revisión de campos obligatorios.
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtCedula.Text) ||
                string.IsNullOrWhiteSpace(txtEdad.Text) ||
                string.IsNullOrWhiteSpace(txtLatitud.Text) ||
                string.IsNullOrWhiteSpace(txtLongitud.Text))
            {
                MessageBox.Show(
                    "Por favor complete todos los campos de texto.",
                    "Campos vacíos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Edad en un rango razonable.
            if (!int.TryParse(txtEdad.Text, out int edad) || edad < 0 || edad > 130)
            {
                MessageBox.Show(
                    "Ingrese una edad válida en el rango de 0 a 130 años.",
                    "Edad inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Latitud.
            if (!double.TryParse(txtLatitud.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double latitud) ||
                latitud < -90 || latitud > 90)
            {
                MessageBox.Show(
                    "La latitud debe estar entre -90 y 90 grados.",
                    "Latitud inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Longitud.
            if (!double.TryParse(txtLongitud.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double longitud) ||
                longitud < -180 || longitud > 180)
            {
                MessageBox.Show(
                    "La longitud debe estar entre -180 y 180 grados.",
                    "Longitud inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Foto obligatoria.
            if (string.IsNullOrWhiteSpace(rutaFoto))
            {
                MessageBox.Show(
                    "Seleccione una fotografía para el familiar.",
                    "Foto requerida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Se crea el objeto con la información del familiar.
            var nuevo = new Familiar(
                txtNombre.Text.Trim(),
                txtCedula.Text.Trim(),
                fechaNacimiento.Value.Date,
                edad,
                latitud,
                longitud,
                rutaFoto
            );

            try
            {
                // Si es el primer integrante del árbol.
                if (esMiembroInicial || !arbol.TieneMiembros)
                {
                    arbol.AgregarMiembroInicial(nuevo);
                }
                // Si ya hay integrantes, se decide si es hijo o pareja.
                else if (familiarReferencia != null && rdbHijo != null && rdbPareja != null)
                {
                    if (rdbHijo.Checked)
                    {
                        arbol.AgregarHijo(familiarReferencia, nuevo);
                    }
                    else if (rdbPareja.Checked)
                    {
                        arbol.AgregarPareja(familiarReferencia, nuevo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible registrar el familiar: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            FamiliarCreado = nuevo;

            MessageBox.Show(
                "El familiar se registró correctamente.",
                "Información",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

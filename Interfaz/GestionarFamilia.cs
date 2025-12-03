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

        // Controles de entrada de datos.
        private TextBox txtNombre;
        private TextBox txtCedula;
        private TextBox txtLatitud;
        private TextBox txtLongitud;
        private TextBox txtEdad;
        private CheckBox chkFallecido;
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
            // Guarda las referencias necesarias para saber qué se está creando.
            this.arbol = arbol ?? throw new ArgumentNullException(nameof(arbol));
            this.familiarReferencia = familiarReferencia;
            this.esMiembroInicial = esMiembroInicial;

            ConfigurarVentana();
            ConstruirControles();
        }

        // Ajusta la apariencia general de la ventana.
        private void ConfigurarVentana()
        {
            // Título que se muestra en la barra superior de la ventana.
            Text = "Registrar familiar";

            // Se define el tamaño de la ventana para que todo quepa sin scroll.
            Width = 640;
            Height = 1020; // más alta para que no se corten los botones inferiores

            // Se centra la ventana en la pantalla.
            StartPosition = FormStartPosition.CenterScreen;

            // Se bloquea el redimensionamiento para mantener la estética.
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Color de fondo suave para el formulario.
            BackColor = ColorTranslator.FromHtml("#F4F6FA");

            // Fuente general para los textos del formulario.
            Font = new Font("Segoe UI", 11, FontStyle.Regular);
        }

        // Construye y acomoda todos los controles.
        private void ConstruirControles()
        {
            Controls.Clear();

            // Franja azul con el título principal de la ventana.
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

            // Contenedor principal sin scroll.
            // Aquí aumento el padding superior para bajar todo el contenido.
            var contenedor = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = false, // Se prefiere ver todo de una vez.
                WrapContents = false,
                // Padding: izquierda, arriba, derecha, abajo.
                Padding = new Padding(40, 120, 40, 20)
            };
            Controls.Add(contenedor);

            // Si no es el primer integrante, se muestra el grupo para elegir hijo o pareja.
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

                // Radio para pareja.
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

            // Tabla con las etiquetas y los campos de texto.
            var tablaDatos = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 0,
                Width = 560,
                AutoSize = true
            };

            tablaDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230));
            tablaDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            int fila = 0;

            // Nombre completo del familiar.
            AgregarFilaTexto(tablaDatos, fila++, "Nombre completo:", out txtNombre);

            // Cédula del familiar.
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

            // Edad actual o edad al morir.
            AgregarFilaTexto(tablaDatos, fila++, "Edad (si falleció, edad al morir):", out txtEdad);

            // Indicar si el familiar está fallecido.
            {
                var lblFallecido = CrearEtiqueta("¿Fallecido?");
                chkFallecido = new CheckBox
                {
                    Text = "Sí",
                    AutoSize = true,
                    Anchor = AnchorStyles.Left
                };

                tablaDatos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tablaDatos.Controls.Add(lblFallecido, 0, fila);
                tablaDatos.Controls.Add(chkFallecido, 1, fila);
                fila++;
            }

            // Latitud de la residencia.
            AgregarFilaTexto(tablaDatos, fila++, "Latitud (residencia):", out txtLatitud);

            // Longitud de la residencia.
            AgregarFilaTexto(tablaDatos, fila++, "Longitud (residencia):", out txtLongitud);

            // Se agrega la tabla de datos al contenedor principal.
            contenedor.Controls.Add(tablaDatos);

            // Botón para seleccionar la foto desde el explorador de archivos.
            var btnSeleccionarFoto = new Button
            {
                Text = "Seleccionar foto",
                Width = 200,
                Height = 40,
                Margin = new Padding(0, 20, 0, 5),
                BackColor = ColorTranslator.FromHtml("#4263EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSeleccionarFoto.FlatAppearance.BorderSize = 0;
            btnSeleccionarFoto.Click += BtnSeleccionarFoto_Click;
            contenedor.Controls.Add(btnSeleccionarFoto);

            // Vista previa de la foto seleccionada.
            fotoPreview = new PictureBox
            {
                Width = 200,
                Height = 200,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 20)
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
            btnGuardar.Click += BtnGuardar_Click;

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
            btnCancelar.Click += (s, e) =>
            {
                // Cierra la ventana sin guardar nada.
                DialogResult = DialogResult.Cancel;
                Close();
            };

            panelBotones.Controls.Add(btnGuardar);
            panelBotones.Controls.Add(btnCancelar);
            contenedor.Controls.Add(panelBotones);
        }

        // Crea una etiqueta con el estilo visual del formulario.
        private Label CrearEtiqueta(string texto)
        {
            // Crea una etiqueta alineada a la derecha para los títulos de cada campo.
            return new Label
            {
                Text = texto,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 10, 5)
            };
        }

        // Agrega una fila a la tabla con una etiqueta y un TextBox.
        private void AgregarFilaTexto(TableLayoutPanel tabla, int fila, string etiqueta, out TextBox txtDestino)
        {
            var lbl = CrearEtiqueta(etiqueta);

            txtDestino = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Width = 260,
                Margin = new Padding(0, 5, 0, 5)
            };

            tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tabla.Controls.Add(lbl, 0, fila);
            tabla.Controls.Add(txtDestino, 1, fila);
        }

        // Maneja el botón para seleccionar la foto desde el explorador.
        private void BtnSeleccionarFoto_Click(object? sender, EventArgs e)
        {
            using var dialogo = new OpenFileDialog
            {
                Title = "Seleccionar fotografía",
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp",
                Multiselect = false
            };

            if (dialogo.ShowDialog(this) == DialogResult.OK)
            {
                rutaFoto = dialogo.FileName;

                try
                {
                    // Muestra la imagen seleccionada en el recuadro de vista previa.
                    fotoPreview.Image = Image.FromFile(rutaFoto);
                }
                catch
                {
                    MessageBox.Show(
                        "No se pudo cargar la imagen seleccionada.",
                        "Error al cargar imagen",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        // Valida los datos del formulario y registra el familiar en el árbol.
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            // Revisión de campos obligatorios básicos.
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

            // Conversión y validación de la edad en un rango razonable.
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

            // Decide si el familiar está fallecido.
            bool fallecido = chkFallecido != null && chkFallecido.Checked;

            // Si la persona está viva, se verifica que la edad coincida con la fecha de nacimiento.
            if (!fallecido)
            {
                DateTime fechaNac = fechaNacimiento.Value.Date;
                DateTime hoy = DateTime.Today;

                // Calcula la edad a partir de la fecha de nacimiento y la fecha de hoy.
                int edadCalculada = hoy.Year - fechaNac.Year;
                if (fechaNac > hoy.AddYears(-edadCalculada))
                {
                    edadCalculada--;
                }

                // Si la edad no coincide, se avisa y no se deja continuar.
                if (edad != edadCalculada)
                {
                    MessageBox.Show(
                        $"La edad indicada no coincide con la edad calculada a partir de la fecha de nacimiento.\n\n" +
                        $"Edad según la fecha: {edadCalculada} años.",
                        "Edad inconsistente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }

            // Conversión y validación de la latitud.
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

            // Conversión y validación de la longitud.
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

            // Valida que se haya seleccionado una fotografía.
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
            )
            {
                // Aquí se guarda si la persona está viva o fallecida.
                Fallecido = fallecido
            };

            try
            {
                // Si es el primer integrante del árbol, se agrega como raíz.
                if (esMiembroInicial || !arbol.TieneMiembros)
                {
                    arbol.AgregarMiembroInicial(nuevo);
                }
                // Si ya hay integrantes, se decide si es hijo o pareja del familiar de referencia.
                else if (familiarReferencia != null && rdbHijo != null && rdbPareja != null)
                {
                    if (rdbHijo.Checked)
                    {
                        // Agrega un hijo al familiar de referencia.
                        arbol.AgregarHijo(familiarReferencia, nuevo);
                    }
                    else if (rdbPareja.Checked)
                    {
                        // Agrega una pareja al familiar de referencia.
                        arbol.AgregarPareja(familiarReferencia, nuevo);
                    }
                }

                // Guarda la referencia al familiar que se acaba de crear.
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
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al registrar el familiar:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}

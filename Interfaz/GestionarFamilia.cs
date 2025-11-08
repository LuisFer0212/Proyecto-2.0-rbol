using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    public class FamiliaForm : Form
    {
        private TextBox txtNombre, txtCedula, txtLatitud, txtLongitud, txtEdad;
        private DateTimePicker fechaNacimiento;
        private PictureBox fotoPreview;
        private Button btnSeleccionarFoto, btnGuardar, btnCancelar;
        private string rutaFoto = "";

        public FamiliaForm()
        {
            // === Ventana ===
            Text = "Agregar Familiar";
            Width = 500;
            Height = 650;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Label lblTitulo = new Label()
            {
                Text = "🧬 Ingreso de Datos del Familiar",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(40, 60, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblTitulo);

            // === Contenedor ===
            Panel panel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30)
            };
            Controls.Add(panel);

            int y = 10;
            int espacio = 50;

            // --- Nombre ---
            panel.Controls.Add(new Label() { Text = "Nombre:", Top = y, Left = 10, Width = 150 });
            txtNombre = new TextBox() { Top = y, Left = 160, Width = 250 };
            panel.Controls.Add(txtNombre);
            y += espacio;

            // --- Cédula ---
            panel.Controls.Add(new Label() { Text = "Cédula:", Top = y, Left = 10, Width = 150 });
            txtCedula = new TextBox() { Top = y, Left = 160, Width = 250 };
            panel.Controls.Add(txtCedula);
            y += espacio;

            // --- Fecha de nacimiento ---
            panel.Controls.Add(new Label() { Text = "Fecha de Nacimiento:", Top = y, Left = 10, Width = 150 });
            fechaNacimiento = new DateTimePicker() { Top = y, Left = 160, Width = 250 };
            panel.Controls.Add(fechaNacimiento);
            y += espacio;

            // --- Edad ---
            panel.Controls.Add(new Label() { Text = "Edad (si falleció, edad al morir):", Top = y, Left = 10, Width = 220 });
            txtEdad = new TextBox() { Top = y, Left = 240, Width = 170 };
            panel.Controls.Add(txtEdad);
            y += espacio;

            // --- Coordenadas ---
            panel.Controls.Add(new Label() { Text = "Latitud:", Top = y, Left = 10, Width = 150 });
            txtLatitud = new TextBox() { Top = y, Left = 160, Width = 250 };
            panel.Controls.Add(txtLatitud);
            y += espacio;

            panel.Controls.Add(new Label() { Text = "Longitud:", Top = y, Left = 10, Width = 150 });
            txtLongitud = new TextBox() { Top = y, Left = 160, Width = 250 };
            panel.Controls.Add(txtLongitud);
            y += espacio + 10;

            // --- Foto ---
            btnSeleccionarFoto = new Button()
            {
                Text = "📷 Seleccionar Foto",
                Left = 10,
                Top = y,
                Width = 400,
                Height = 35,
                BackColor = Color.FromArgb(70, 100, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            panel.Controls.Add(btnSeleccionarFoto);
            y += 45;

            fotoPreview = new PictureBox()
            {
                Top = y,
                Left = 10,
                Width = 400,
                Height = 150,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            panel.Controls.Add(fotoPreview);
            y += 180;

            btnSeleccionarFoto.Click += (s, e) =>
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Imágenes|*.jpg;*.png;*.jpeg";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    rutaFoto = dlg.FileName;
                    fotoPreview.Image = Image.FromFile(rutaFoto);
                }
            };

            // --- Botones finales ---
            btnGuardar = new Button()
            {
                Text = "💾 Guardar",
                Left = 60,
                Top = y,
                Width = 140,
                Height = 40,
                BackColor = Color.FromArgb(40, 120, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnCancelar = new Button()
            {
                Text = "❌ Cancelar",
                Left = 240,
                Top = y,
                Width = 140,
                Height = 40,
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            panel.Controls.Add(btnGuardar);
            panel.Controls.Add(btnCancelar);

            btnCancelar.Click += (s, e) => Close();
            btnGuardar.Click += BtnGuardar_Click;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtCedula.Text) ||
                string.IsNullOrWhiteSpace(txtEdad.Text) ||
                string.IsNullOrWhiteSpace(txtLatitud.Text) ||
                string.IsNullOrWhiteSpace(txtLongitud.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtEdad.Text, out int edad) || edad < 0 || edad > 130)
            {
                MessageBox.Show("Ingrese una edad válida (entre 0 y 130 años).", "Edad inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtLatitud.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double latitud) ||
                latitud < -90 || latitud > 90)
            {
                MessageBox.Show("La latitud debe estar entre -90 y 90 grados.", "Latitud inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtLongitud.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double longitud) ||
                longitud < -180 || longitud > 180)
            {
                MessageBox.Show("La longitud debe estar entre -180 y 180 grados.", "Longitud inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (fechaNacimiento.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser en el futuro.", "Fecha inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(rutaFoto))
            {
                MessageBox.Show("Debe seleccionar una foto del familiar.", "Foto faltante", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si todo está correcto
            MessageBox.Show(
                $"✅ Familiar agregado correctamente:\n\n" +
                $"👤 Nombre: {txtNombre.Text}\n" +
                $"🪪 Cédula: {txtCedula.Text}\n" +
                $"🎂 Nacimiento: {fechaNacimiento.Value.ToShortDateString()}\n" +
                $"🎯 Edad: {edad}\n" +
                $"📍 Coordenadas: ({latitud}, {longitud})",
                "Registro exitoso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            Close();
        }
    }
}

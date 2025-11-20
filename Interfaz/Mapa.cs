// Interfaz.Mapa.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Proyecto_2_Arbol
{
    public class MapaForm : Form
    {
        private readonly ArbolGenealogico arbol;

        private GMapControl mapa;
        private GMapOverlay capaMarcadores;
        private GMapOverlay capaLineas;

        private Button btnZoomIn, btnZoomOut;

        public MapaForm(ArbolGenealogico arbol)
        {
            this.arbol = arbol ?? throw new ArgumentNullException(nameof(arbol));

            // === Ventana ===
            Text = "Mapa Genealógico";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F4F6FA");
            Font = new Font("Segoe UI", 11);

            // === Configurar mapa ===
            mapa = new GMapControl
            {
                Dock = DockStyle.Fill,
                MapProvider = GMapProviders.GoogleMap,
                MinZoom = 2,
                MaxZoom = 18,
                Zoom = 7,
                Position = new PointLatLng(9.934739, -84.087502),
                MouseWheelZoomEnabled = true,
                MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter,
                CanDragMap = true,
                DragButton = MouseButtons.Left
            };

            GMaps.Instance.Mode = AccessMode.ServerOnly;

            capaMarcadores = new GMapOverlay("marcadores");
            capaLineas = new GMapOverlay("lineas");

            mapa.Overlays.Add(capaMarcadores);
            mapa.Overlays.Add(capaLineas);

            Controls.Add(mapa);

            // === Botones de zoom ===
            btnZoomIn = CrearBoton("＋", 10, 10);
            btnZoomOut = CrearBoton("－", 10, 60);

            Controls.Add(btnZoomIn);
            Controls.Add(btnZoomOut);

            btnZoomIn.Click += (s, e) => { if (mapa.Zoom < mapa.MaxZoom) mapa.Zoom++; };
            btnZoomOut.Click += (s, e) => { if (mapa.Zoom > mapa.MinZoom) mapa.Zoom--; };

            // === Cargar familiares reales ===
            CargarFamiliaresEnMapa();
        }

        private void CargarFamiliaresEnMapa()
        {
            capaMarcadores.Markers.Clear();

            var lista = arbol.ObtenerTodosLosFamiliares();

            foreach (var f in lista)
            {
                if (f.Latitud == 0 && f.Longitud == 0)
                    continue; // No tiene ubicación

                var punto = new PointLatLng(f.Latitud, f.Longitud);

                // 🔹 Marcador generado en memoria (no necesita PNG)
                int size = 16;
                Bitmap bmp = new Bitmap(size, size);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.FillEllipse(Brushes.Blue, 0, 0, size, size);
                }

                var marcador = new GMarkerGoogle(punto, bmp)
                {
                    ToolTipText = $"{f.Nombre}\nLat: {f.Latitud}\nLon: {f.Longitud}",
                    ToolTipMode = MarkerTooltipMode.OnMouseOver
                };

                capaMarcadores.Markers.Add(marcador);
            }
        }

        private Button CrearBoton(string texto, int left, int top)
        {
            Button btn = new Button()
            {
                Text = texto,
                Left = left,
                Top = top,
                Width = 40,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#4C6EF5"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = ColorTranslator.FromHtml("#3B5BDB");
            btn.MouseLeave += (s, e) => btn.BackColor = ColorTranslator.FromHtml("#4C6EF5");
            return btn;
        }
    }
}

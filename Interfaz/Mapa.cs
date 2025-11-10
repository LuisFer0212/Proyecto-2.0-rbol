using System;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Proyecto_2_Arbol
{
    public class MapaForm : Form
    {
        private GMapControl mapa;
        private GMapOverlay marcadores;
        private Button btnZoomIn, btnZoomOut;

        public MapaForm()
        {
            // === Ventana ===
            Text = "Mapa Genealógico";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorTranslator.FromHtml("#F4F6FA");
            Font = new Font("Segoe UI", 11);

            // === Control de mapa ===
            mapa = new GMapControl
            {
                Dock = DockStyle.Fill,
                MapProvider = GMapProviders.GoogleMap,
                MinZoom = 2,
                MaxZoom = 18,
                Zoom = 7,
                Position = new PointLatLng(9.934739, -84.087502), // San José, CR
                MouseWheelZoomEnabled = true,
                MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter,
                CanDragMap = true,
                DragButton = MouseButtons.Left
            };

            GMaps.Instance.Mode = AccessMode.ServerOnly;
            marcadores = new GMapOverlay("marcadores");
            mapa.Overlays.Add(marcadores);

            Controls.Add(mapa);

            // === Botones de zoom ===
            btnZoomIn = CrearBoton("＋", 10, 10);
            btnZoomOut = CrearBoton("－", 10, 60);

            Controls.Add(btnZoomIn);
            Controls.Add(btnZoomOut);

            btnZoomIn.Click += (s, e) => { if (mapa.Zoom < mapa.MaxZoom) mapa.Zoom++; };
            btnZoomOut.Click += (s, e) => { if (mapa.Zoom > mapa.MinZoom) mapa.Zoom--; };

            // === Marcador de ejemplo ===
            var marcador = new GMarkerGoogle(
                new PointLatLng(9.934739, -84.087502),
                GMarkerGoogleType.red_dot
            )
            {
                ToolTipText = "Familiar: Juan Pérez\nSan José, Costa Rica"
            };
            marcadores.Markers.Add(marcador);
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

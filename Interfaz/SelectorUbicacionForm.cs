using System;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Drawing.Drawing2D;


namespace Proyecto_2_Arbol
{
    public class SelectorUbicacionForm : Form
    {
        private GMapControl mapa;
        private GMapOverlay capa;

        public double LatitudSeleccionada { get; private set; }
        public double LongitudSeleccionada { get; private set; }

        public bool UbicacionSeleccionada { get; private set; } = false;

        public SelectorUbicacionForm()
        {
            Text = "Seleccione su ubicación";
            Width = 800;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;

            mapa = new GMapControl
            {
                Dock = DockStyle.Fill,
                MapProvider = GMapProviders.GoogleMap,
                MinZoom = 2,
                MaxZoom = 18,
                Zoom = 7,
                Position = new PointLatLng(9.934739, -84.087502), // Costa Rica
                CanDragMap = true
            };

            capa = new GMapOverlay("seleccion");
            mapa.Overlays.Add(capa);

            mapa.MouseClick += Mapa_MouseClick;

            Controls.Add(mapa);

            GMaps.Instance.Mode = AccessMode.ServerAndCache;
        }
        private Bitmap CrearPuntoRojo(int size)
        {
            Bitmap bmp = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                int d = size - 4;
                g.FillEllipse(Brushes.Red, 2, 2, d, d);
                g.DrawEllipse(Pens.Black, 2, 2, d, d);
            }

            return bmp;
        }


        private void Mapa_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var punto = mapa.FromLocalToLatLng(e.X, e.Y);

                LatitudSeleccionada = punto.Lat;
                LongitudSeleccionada = punto.Lng;

                // Limpiar marcadores previos
                capa.Markers.Clear();

                // Crear un puntito rojo reutilizando FotoMarker
                Bitmap icono = CrearPuntoRojo(32);
                var marker = new FotoMarker(punto, icono, 32);

                capa.Markers.Add(marker);

                UbicacionSeleccionada = true;

                // Si quieres que se cierre inmediatamente después del clic:
                DialogResult = DialogResult.OK;
                Close();

                // Si prefieres que el usuario vea el punto y tenga que cerrar manualmente,
                // comenta las dos líneas anteriores.
            }
        }


    }
}

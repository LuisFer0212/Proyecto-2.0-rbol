// Interfaz.Mapa.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Drawing.Drawing2D;
using System.Drawing;

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
            capaLineas.Routes.Clear();

            var lista = arbol.ObtenerTodosLosFamiliares();

            foreach (var f in lista)
            {
                var punto = new PointLatLng(f.Latitud, f.Longitud);

                Bitmap icono = CrearFotoCircular(f.RutaFoto, 60);

                var marker = new GMarkerGoogle(punto, icono)
                {
                    ToolTipMode = MarkerTooltipMode.OnMouseOver,
                    ToolTipText = f.Nombre,
                    Tag = f
                };

                capaMarcadores.Markers.Add(marker);
            }

            // Registrar un único evento general
            mapa.OnMarkerClick -= Mapa_OnMarkerClick;
            mapa.OnMarkerClick += Mapa_OnMarkerClick;
        }

        private Bitmap CrearFotoCircular(string ruta, int size)
        {
            Bitmap bmp = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, size, size);
                    g.SetClip(path);

                    try
                    {
                        using (Image img = Image.FromFile(ruta))
                        {
                            g.DrawImage(img, 0, 0, size, size);
                        }
                    }
                    catch
                    {
                        g.Clear(Color.Gray);
                    }

                    g.ResetClip();

                    g.DrawEllipse(new Pen(Color.Black, 2), 0, 0, size - 1, size - 1);
                }
            }

            return bmp;
        }

        private void Mapa_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag is not Familiar seleccionado)
                return;

            capaLineas.Routes.Clear();

            var lista = arbol.ObtenerTodosLosFamiliares();
            string msg = $"Distancias desde {seleccionado.Nombre}:\n\n";

            foreach (var otro in lista)
            {
                if (otro == seleccionado) continue;

                double dist = GeoHelper.DistanciaKm(
                    seleccionado.Latitud, seleccionado.Longitud,
                    otro.Latitud, otro.Longitud
                );

                msg += $"{otro.Nombre}: {dist:F2} km\n";

                // Dibujar líneas
                var route = new GMapRoute(
                    new[]
                    {
                        new PointLatLng(seleccionado.Latitud, seleccionado.Longitud),
                        new PointLatLng(otro.Latitud, otro.Longitud)
                    },
                    $"ruta-{seleccionado.Nombre}-{otro.Nombre}"
                );

                route.Stroke = new Pen(Color.Red, 2);
                capaLineas.Routes.Add(route);
            }

            mapa.Zoom++;
            mapa.Zoom--;

            MessageBox.Show(msg, "Distancias", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

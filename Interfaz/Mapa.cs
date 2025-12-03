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

namespace Proyecto_2_Arbol
{
    public class MapaForm : Form
    {
        private readonly ArbolGenealogico arbol;

        private GMapControl mapa;
        private GMapOverlay capaMarcadores;
        private GMapOverlay capaGrafoCompleto;
        private GMapOverlay capaRutas;

        private Button btnZoomIn, btnZoomOut;

        public MapaForm(ArbolGenealogico arbol)
        {
            this.arbol = arbol ?? throw new ArgumentNullException(nameof(arbol));

            WindowState = FormWindowState.Maximized;

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
                DragButton = MouseButtons.Left,

                BackColor = Color.Black,        // fondo del control
                EmptyTileColor = Color.Black,   // tiles vacíos negros
                ShowTileGridLines = false
            };

            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            mapa.FillEmptyTiles = true;

            
            // === Crear capas ===
            capaMarcadores = new GMapOverlay("marcadores");     // Fotos
            capaGrafoCompleto = new GMapOverlay("grafo");       // Todas las conexiones
            capaRutas = new GMapOverlay("rutas");               // Dijkstra

            mapa.Overlays.Add(capaGrafoCompleto);
            mapa.Overlays.Add(capaMarcadores);
            mapa.Overlays.Add(capaRutas);

            Controls.Add(mapa);

            // === Botones de zoom ===
            btnZoomIn = CrearBoton("＋", 10, 10);
            btnZoomOut = CrearBoton("－", 10, 60);

            Controls.Add(btnZoomIn);
            Controls.Add(btnZoomOut);

            btnZoomIn.Click += (s, e) => { if (mapa.Zoom < mapa.MaxZoom) mapa.Zoom++; };
            btnZoomOut.Click += (s, e) => { if (mapa.Zoom > mapa.MinZoom) mapa.Zoom--; };

            // === Cargar familiares y conexiones ===
            CargarFamiliaresEnMapa();
        }

        private void CargarFamiliaresEnMapa()
        {
            capaMarcadores.Markers.Clear();
            capaGrafoCompleto.Routes.Clear();
            // NO borrar capaRutas: solo se borra al hacer clic

            var lista = arbol.ObtenerTodosLosFamiliares();

            // === MARCADORES (fotos) ===
            foreach (var f in lista)
            {
                var punto = new PointLatLng(f.Latitud, f.Longitud);

                Bitmap icono = CrearFotoCircular(f.RutaFoto, 60);

                var marker = new FotoMarker(punto, icono, 60)
                {
                    ToolTipMode = MarkerTooltipMode.OnMouseOver,
                    ToolTipText = f.Nombre,
                    Tag = f
                };

                capaMarcadores.Markers.Add(marker);
            }

            // === DIBUJAR TODAS LAS CONEXIONES DEL GRAFO ===
            for (int i = 0; i < lista.Length; i++)
            {
                for (int j = i + 1; j < lista.Length; j++)
                {
                    var f1 = lista[i];
                    var f2 = lista[j];

                    var p1 = new PointLatLng(f1.Latitud, f1.Longitud);
                    var p2 = new PointLatLng(f2.Latitud, f2.Longitud);

                    var route = new GMapRoute(new[] { p1, p2 }, $"edge-{i}-{j}");
                    route.Stroke = new Pen(Color.DarkGray, 2);

                    capaGrafoCompleto.Routes.Add(route);

                    double dist = GeoHelper.DistanciaKm(f1.Latitud, f1.Longitud, f2.Latitud, f2.Longitud);

                    // Punto medio de la línea
                    double midLat = (f1.Latitud + f2.Latitud) / 2.0;
                    double midLng = (f1.Longitud + f2.Longitud) / 2.0;

                    // Crear el marcador de texto (sin fondo, rojo)
                    var label = new GMapTextMarker(
                        new PointLatLng(midLat, midLng),
                        $"{dist:F1} km"
                    );

                    // Agregarlo a la capa del grafo
                    capaGrafoCompleto.Markers.Add(label);
                }

            }
            // Registrar evento de clic en marcador
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

        // ==========================
        // EVENTO DE CLIC EN UN FAMILIAR
        // ==========================
        private void Mapa_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag is not Familiar seleccionado)
                return;

            // Borrar solo las rutas Dijkstra
            capaRutas.Routes.Clear();

            var lista = arbol.ObtenerTodosLosFamiliares();
            string msg = $"Distancias desde {seleccionado.Nombre}:\n\n";

            // Construir grafo real desde el árbol
            var grafo = new GrafoResidencias();
            grafo.ConstruirGrafoDesdeArbol(arbol);

            foreach (var otro in lista)
            {
                if (otro == seleccionado) continue;

                double dist = grafo.DistanciaMinima(seleccionado, otro);

                msg += $"{otro.Nombre}: {dist:F2} km\n";

                var camino = grafo.CaminoMinimo(seleccionado, otro);
                if (camino.Length < 2)
                    continue;

                for (int i = 0; i < camino.Length - 1; i++)
                {
                    var p1 = new PointLatLng(camino[i].Latitud, camino[i].Longitud);
                    var p2 = new PointLatLng(camino[i + 1].Latitud, camino[i + 1].Longitud);

                    var route = new GMapRoute(new[] { p1, p2 }, $"tramo-{i}");
                    route.Stroke = new Pen(Color.MediumPurple, 3);

                    capaRutas.Routes.Add(route);
                }
            }

            mapa.Zoom++; mapa.Zoom--; // refrescar mapa

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
   public class GMapTextMarker : GMapMarker
    {
        public string Texto;
        private Font fuente = new Font("Segoe UI", 10, FontStyle.Bold);

        public GMapTextMarker(PointLatLng p, string texto) : base(p)
        {
            Texto = texto;
            Offset = new Point(0, 0);
        }

        public override void OnRender(Graphics g)
        {
            SizeF tam = g.MeasureString(Texto, fuente);
            Rectangle rect = new Rectangle(
                (int)LocalPosition.X - 5,
                (int)LocalPosition.Y - 5,
                (int)tam.Width + 10,
                (int)tam.Height + 10
            );

            // Fondo blanco
            g.FillRectangle(Brushes.White, rect);

            // Borde negro
            g.DrawRectangle(Pens.Black, rect);

            // Texto negro
            g.DrawString(Texto, fuente, Brushes.Black, rect.X + 5, rect.Y + 5);
        }
    }

    public class FotoMarker : GMapMarker
    {
        private readonly Bitmap imagen;
        private readonly int size;

        public FotoMarker(PointLatLng pos, Bitmap img, int size = 60) : base(pos)
        {
            this.imagen = img;
            this.size = size;

            // 🔹 Muy importante: definir el tamaño real del marcador
            Size = new Size(size, size);

            // 🔹 Centrar la imagen respecto al punto del marker
            Offset = new Point(-size / 2, -size / 2);
        }

        public override void OnRender(Graphics g)
        {
            g.DrawImage(imagen, LocalPosition.X, LocalPosition.Y, size, size);
        }
    }



}

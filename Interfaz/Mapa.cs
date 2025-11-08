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
        private Button btnZoomIn;
        private Button btnZoomOut;

        public MapaForm()
        {
            Text = "Mapa Genealógico";
            Width = 1000;
            Height = 700;

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

            marcadores = new GMapOverlay("marcadores");
            mapa.Overlays.Add(marcadores);

            btnZoomIn = new Button { Text = "+", Width = 30, Height = 30, Left = 10, Top = 10 };
            btnZoomOut = new Button { Text = "-", Width = 30, Height = 30, Left = 10, Top = 50 };
            btnZoomIn.Click += (s, e) => { if (mapa.Zoom < mapa.MaxZoom) mapa.Zoom++; };
            btnZoomOut.Click += (s, e) => { if (mapa.Zoom > mapa.MinZoom) mapa.Zoom--; };

            Controls.Add(mapa);
            Controls.Add(btnZoomIn);
            Controls.Add(btnZoomOut);

            var marcador = new GMarkerGoogle(
                new PointLatLng(9.934739, -84.087502),
                GMarkerGoogleType.red_dot
            )
            {
                ToolTipText = "Familiar: Juan Pérez\nSan José, Costa Rica"
            };
            marcadores.Markers.Add(marcador);
        }
    }
}

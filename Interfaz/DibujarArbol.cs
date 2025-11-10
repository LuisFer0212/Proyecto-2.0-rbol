using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    public class TreeCanvas : Control
    {
        public TreeCanvas()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Theme.Card;
            ForeColor = Theme.TextPrimary;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Theme.Card);

            // Título del área
            using (var titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Theme.TextPrimary))
            {
                g.DrawString("Vista del árbol (interfaz, sin lógica)", titleFont, titleBrush, 16, 12);
            }

            // Ejemplo estático de 5 nodos conectados (solo UI de muestra)
            // Posiciones base
            var root = new Point(Width/2, 110);
            var c1   = new Point(Width/2 - 200, 250);
            var c2   = new Point(Width/2 + 200, 250);
            var g1   = new Point(c1.X - 120, 390);
            var g2   = new Point(c1.X + 120, 390);

            // Líneas (conectores)
            using var pen = new Pen(Theme.Line, 2);
            g.DrawLine(pen, root, c1);
            g.DrawLine(pen, root, c2);
            g.DrawLine(pen, c1, g1);
            g.DrawLine(pen, c1, g2);

            // Dibujo de “cards” para cada nodo (círculo + nombre + mini-foto placeholder)
            DrawNode(g, root, "Raíz",     "foto_root");
            DrawNode(g, c1,   "Hijo A",   "foto_A");
            DrawNode(g, c2,   "Hijo B",   "foto_B");
            DrawNode(g, g1,   "Nieto A1", "foto_A1");
            DrawNode(g, g2,   "Nieto A2", "foto_A2");
        }

        private void DrawNode(Graphics g, Point center, string name, string photoKey)
        {
            int r = 36; // radio del círculo
            var circleRect = new Rectangle(center.X - r, center.Y - r, r*2, r*2);

            using var circleBrush = new SolidBrush(Color.FromArgb(220, Theme.Btn));
            using var borderPen   = new Pen(Theme.BtnHover, 3);
            g.FillEllipse(circleBrush, circleRect);
            g.DrawEllipse(borderPen, circleRect);

            // Placeholder de “foto” (inicial)
            string initial = string.IsNullOrWhiteSpace(name) ? "?" : name.Trim()[0].ToString().ToUpperInvariant();
            using var f = new Font("Segoe UI", 12, FontStyle.Bold);
            var sz = g.MeasureString(initial, f);
            var ip = new PointF(center.X - sz.Width/2, center.Y - sz.Height/2);
            using var textOnCircle = new SolidBrush(Color.White);
            g.DrawString(initial, f, textOnCircle, ip);

            // Nombre debajo
            using var nameFont = new Font("Segoe UI", 10, FontStyle.Regular);
            using var nameBrush = new SolidBrush(Theme.TextPrimary);
            var nameSize = g.MeasureString(name, nameFont);
            var namePt = new PointF(center.X - nameSize.Width/2, center.Y + r + 8);
            g.DrawString(name, nameFont, nameBrush, namePt);
        }
    }
}

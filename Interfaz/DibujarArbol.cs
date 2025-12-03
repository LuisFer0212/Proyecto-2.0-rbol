using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    public class TreeCanvas : Control
    {
        private readonly ArbolGenealogico arbol;

        private readonly ListaEnlazada<NodoVisual> nodosVisuales = new ListaEnlazada<NodoVisual>();

        private Point desplazamiento = Point.Empty;
        private bool arrastrando = false;
        private Point ultimoMouse;
        private bool arrastreConMovimiento = false;
        private bool omitirProximoClickIzquierdo = false;

        private class NodoVisual
        {
            public Familiar Familiar { get; }
            public Point Centro { get; set; }
            public int Radio { get; }

            public NodoVisual(Familiar familiar, Point centro, int radio)
            {
                Familiar = familiar;
                Centro = centro;
                Radio = radio;
            }

            public bool ContienePunto(Point p)
            {
                int dx = p.X - Centro.X;
                int dy = p.Y - Centro.Y;
                return dx * dx + dy * dy <= Radio * Radio;
            }
        }

        private struct NodoNivel
        {
            public Familiar Familiar;
            public int Nivel;

            public NodoNivel(Familiar familiar, int nivel)
            {
                Familiar = familiar;
                Nivel = nivel;
            }
        }

        public TreeCanvas(ArbolGenealogico arbol)
        {
            this.arbol = arbol ?? throw new ArgumentNullException(nameof(arbol));

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

            nodosVisuales.Clear();

            if (!arbol.TieneMiembros || arbol.Raiz == null)
            {
                DibujarMensajeSinDatos(g);
                return;
            }

            DibujarArbol(g);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                arrastrando = true;
                arrastreConMovimiento = false;
                ultimoMouse = e.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!arrastrando)
                return;

            int dx = e.X - ultimoMouse.X;
            int dy = e.Y - ultimoMouse.Y;

            if (dx != 0 || dy != 0)
            {
                arrastreConMovimiento = true;

                desplazamiento.X += dx;
                desplazamiento.Y += dy;

                ultimoMouse = e.Location;

                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!arrastrando)
                return;

            if (arrastreConMovimiento && e.Button == MouseButtons.Left)
            {
                omitirProximoClickIzquierdo = true;
            }

            arrastrando = false;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && omitirProximoClickIzquierdo)
            {
                omitirProximoClickIzquierdo = false;
                return;
            }

            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Right)
            {
                if (!arbol.TieneMiembros)
                    return;

                NodoVisual? seleccionadoDerecho = null;

                foreach (var nodo in nodosVisuales)
                {
                    if (nodo.ContienePunto(e.Location))
                    {
                        seleccionadoDerecho = nodo;
                        break;
                    }
                }

                if (seleccionadoDerecho != null)
                {
                    using (var ventana = new VerFamiliar(seleccionadoDerecho.Familiar))
                    {
                        ventana.ShowDialog(FindForm());
                    }
                }

                return;
            }

            if (e.Button != MouseButtons.Left)
                return;

            if (!arbol.TieneMiembros)
            {
                using (var ventana = new GestionarFamilia(arbol, null, true))
                {
                    if (ventana.ShowDialog(FindForm()) == DialogResult.OK)
                        Invalidate();
                }
                return;
            }

            NodoVisual? seleccionado = null;
            foreach (var nodo in nodosVisuales)
            {
                if (nodo.ContienePunto(e.Location))
                {
                    seleccionado = nodo;
                    break;
                }
            }

            if (seleccionado != null)
            {
                using (var ventana = new GestionarFamilia(arbol, seleccionado.Familiar, false))
                {
                    if (ventana.ShowDialog(FindForm()) == DialogResult.OK)
                        Invalidate();
                }
            }
        }

        private void DibujarMensajeSinDatos(Graphics g)
        {
            using var fuente = new Font("Segoe UI", 11, FontStyle.Italic);
            using var pincel = new SolidBrush(Theme.TextPrimary);

            string texto = "No hay integrantes en el árbol.\n" +
                           "Haga clic en el área en blanco para registrar el primero.";

            var tamaño = g.MeasureString(texto, fuente);
            float x = (Width - tamaño.Width) / 2f;
            float y = (Height - tamaño.Height) / 2f;

            g.DrawString(texto, fuente, pincel, x, y);
        }

        private void DibujarArbol(Graphics g)
        {
            if (arbol.Raiz == null)
                return;

            var niveles = CalcularNiveles();

            int indiceNivel = 0;
            foreach (var kvp in niveles)
            {
                var familiaresNivel = kvp.Value;
                int yBase = 150 + indiceNivel * 160;

                var grupos = AgruparPorParejas(familiaresNivel);

                int espacioEntreGrupos = 170;
                int anchoTotal = (grupos.Count - 1) * espacioEntreGrupos;
                int inicioX = (Width / 2) - (anchoTotal / 2);

                foreach (var grupo in grupos)
                {
                    if (grupo.Count == 1)
                    {
                        var fam = grupo[0];

                        var centro = new Point(
                            inicioX + desplazamiento.X,
                            yBase + desplazamiento.Y
                        );

                        AgregarNodoVisualSiNoExiste(fam, centro);
                    }
                    else if (grupo.Count == 2)
                    {
                        int separacion = 90;
                        var fam1 = grupo[0];
                        var fam2 = grupo[1];

                        var centro1 = new Point(inicioX - separacion / 2 + desplazamiento.X, yBase + desplazamiento.Y);
                        var centro2 = new Point(inicioX + separacion / 2 + desplazamiento.X, yBase + desplazamiento.Y);

                        AgregarNodoVisualSiNoExiste(fam1, centro1);
                        AgregarNodoVisualSiNoExiste(fam2, centro2);
                    }
                    else
                    {
                        int xGrupo = inicioX - (grupo.Count - 1) * 40;

                        foreach (var fam in grupo)
                        {
                            var centro = new Point(
                                xGrupo + desplazamiento.X,
                                yBase + desplazamiento.Y
                            );

                            AgregarNodoVisualSiNoExiste(fam, centro);
                            xGrupo += 80;
                        }
                    }

                    inicioX += espacioEntreGrupos;
                }

                indiceNivel++;
            }

            DibujarConexiones(g);

            foreach (var nodo in nodosVisuales)
            {
                DibujarNodo(g, nodo);
            }
        }

        private void AgregarNodoVisualSiNoExiste(Familiar fam, Point centro)
        {
            foreach (var n in nodosVisuales)
            {
                if (ReferenceEquals(n.Familiar, fam))
                {
                    n.Centro = centro;
                    return;
                }
            }

            nodosVisuales.Agregar(new NodoVisual(fam, centro, 35));
        }

        private ListaEnlazada<ListaEnlazada<Familiar>> AgruparPorParejas(ListaEnlazada<Familiar> familiaresNivel)
        {
            var grupos = new ListaEnlazada<ListaEnlazada<Familiar>>();
            var usados = new HashSet<Familiar>();

            foreach (var fam in familiaresNivel)
            {
                if (usados.Contains(fam))
                    continue;

                if (fam.Pareja != null &&
                    familiaresNivel.ContainsValue(fam.Pareja) &&
                    !usados.Contains(fam.Pareja))
                {
                    var grupo = new ListaEnlazada<Familiar>();
                    grupo.Agregar(fam);
                    grupo.Agregar(fam.Pareja);

                    grupos.Agregar(grupo);

                    usados.Add(fam);
                    usados.Add(fam.Pareja);
                }
                else
                {
                    var grupo = new ListaEnlazada<Familiar>();
                    grupo.Agregar(fam);

                    grupos.Agregar(grupo);

                    usados.Add(fam);
                }
            }

            return grupos;
        }

        private Dictionary<int, ListaEnlazada<Familiar>> CalcularNiveles()
        {
            var niveles = new Dictionary<int, ListaEnlazada<Familiar>>();
            var visitados = new HashSet<Familiar>();
            var cola = new Cola<NodoNivel>();

            if (arbol.Raiz == null)
                return niveles;

            cola.Enqueue(new NodoNivel(arbol.Raiz, 0));
            visitados.Add(arbol.Raiz);

            while (cola.Count > 0)
            {
                var actual = cola.Dequeue();
                var fam = actual.Familiar;
                int nivel = actual.Nivel;

                if (!niveles.ContainsKey(nivel))
                    niveles[nivel] = new ListaEnlazada<Familiar>();

                if (!niveles[nivel].ContainsValue(fam))
                    niveles[nivel].Agregar(fam);

                var hijo = fam.PrimerHijo;
                while (hijo != null)
                {
                    if (!visitados.Contains(hijo))
                    {
                        visitados.Add(hijo);
                        cola.Enqueue(new NodoNivel(hijo, nivel + 1));
                    }

                    hijo = hijo.HermanoDerecho;
                }

                if (fam.Pareja != null && !visitados.Contains(fam.Pareja))
                {
                    visitados.Add(fam.Pareja);
                    cola.Enqueue(new NodoNivel(fam.Pareja, nivel));
                }
            }

            return niveles;
        }

        private void DibujarConexiones(Graphics g)
        {
            if (nodosVisuales.Count == 0)
                return;

            var mapa = new Dictionary<Familiar, NodoVisual>();
            foreach (var nodo in nodosVisuales)
                mapa[nodo.Familiar] = nodo;

            Color bordePrincipal = Color.FromArgb(0, 153, 51);
            Color bordePolitica = Color.FromArgb(192, 64, 0);

            using var penPareja = new Pen(Theme.Line, 2);
            using var penConsanguineo = new Pen(bordePrincipal, 3);

            var parejasMarcadas = new HashSet<Familiar>();
            foreach (var nodo in nodosVisuales)
            {
                var fam = nodo.Familiar;
                if (fam.Pareja == null || parejasMarcadas.Contains(fam))
                    continue;

                if (mapa.TryGetValue(fam.Pareja, out var nodoPareja))
                {
                    g.DrawLine(penPareja, nodo.Centro, nodoPareja.Centro);
                    parejasMarcadas.Add(fam);
                    parejasMarcadas.Add(fam.Pareja);
                }
            }

            foreach (var nodoHijo in nodosVisuales)
            {
                var hijo = nodoHijo.Familiar;
                if (hijo.Progenitor == null)
                    continue;

                if (!mapa.TryGetValue(hijo.Progenitor, out var nodoPadre))
                    continue;

                Point origenSuperior;

                if (hijo.Progenitor.Pareja != null &&
                    mapa.TryGetValue(hijo.Progenitor.Pareja, out var nodoParejaH))
                {
                    int unionX = (nodoPadre.Centro.X + nodoParejaH.Centro.X) / 2;
                    int unionY = nodoPadre.Centro.Y + nodoPadre.Radio + 6;

                    var puntoUnion = new Point(unionX, unionY);

                    var penPadreUnion = nodoPadre.Familiar.EsFamiliaPolitica ? penPareja : penConsanguineo;
                    var penParejaUnion = nodoParejaH.Familiar.EsFamiliaPolitica ? penPareja : penConsanguineo;

                    g.DrawLine(penPadreUnion,
                        new Point(nodoPadre.Centro.X, nodoPadre.Centro.Y + nodoPadre.Radio),
                        puntoUnion);

                    g.DrawLine(penParejaUnion,
                        new Point(nodoParejaH.Centro.X, nodoParejaH.Centro.Y + nodoParejaH.Radio),
                        puntoUnion);

                    origenSuperior = puntoUnion;
                }
                else
                {
                    origenSuperior = new Point(
                        nodoPadre.Centro.X,
                        nodoPadre.Centro.Y + nodoPadre.Radio
                    );
                }

                var destinoInferior = new Point(
                    nodoHijo.Centro.X,
                    nodoHijo.Centro.Y - nodoHijo.Radio
                );

                g.DrawLine(penConsanguineo, origenSuperior, destinoInferior);
            }
        }

        private void DibujarNodo(Graphics g, NodoVisual nodo)
        {
            int r = nodo.Radio;
            var rect = new Rectangle(nodo.Centro.X - r, nodo.Centro.Y - r, r * 2, r * 2);

            Color bordePrincipal = Color.FromArgb(0, 153, 51);
            Color bordePolitica = Color.FromArgb(192, 64, 0);

            Color colorBorde = nodo.Familiar.EsFamiliaPolitica ? bordePolitica : bordePrincipal;

            bool dibujoFoto = false;
            if (!string.IsNullOrWhiteSpace(nodo.Familiar.RutaFoto) &&
                File.Exists(nodo.Familiar.RutaFoto))
            {
                try
                {
                    using var imagen = Image.FromFile(nodo.Familiar.RutaFoto);
                    using (var ruta = new GraphicsPath())
                    {
                        ruta.AddEllipse(rect);
                        g.SetClip(ruta);
                        g.DrawImage(imagen, rect);
                    }

                    dibujoFoto = true;
                }
                catch { dibujoFoto = false; }
            }

            if (!dibujoFoto)
            {
                using var relleno = new SolidBrush(Color.FromArgb(220, Theme.Btn));
                g.FillEllipse(relleno, rect);
            }

            using (var borde = new Pen(colorBorde, 4))
            {
                g.ResetClip();
                g.DrawEllipse(borde, rect);
            }

            string nombre = nodo.Familiar.Nombre;

            var estiloNombre = nodo.Familiar.EsFamiliaPolitica ? FontStyle.Regular : FontStyle.Underline;

            using var fuenteNombre = new Font("Segoe UI", 7, estiloNombre);
            using var pincelNombre = new SolidBrush(Theme.TextPrimary);

            var tamañoNombre = g.MeasureString(nombre, fuenteNombre);
            var puntoNombre = new PointF(
                nodo.Centro.X - tamañoNombre.Width / 2f,
                nodo.Centro.Y + r + 6
            );

            g.DrawString(nombre, fuenteNombre, pincelNombre, puntoNombre);
        }
    }
}

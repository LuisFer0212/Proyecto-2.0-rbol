// Archivo: DibujarArbol.cs
// Lienzo donde se dibuja el árbol genealógico en la interfaz.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Proyecto_2_Arbol
{
    // Control que pinta los nodos del árbol y responde a los clics del usuario.
    public class TreeCanvas : Control
    {
        // Referencia al árbol de la parte de lógica.
        private readonly ArbolGenealogico arbol;

        // Lista de nodos tal y como se dibujan en pantalla.
        private readonly List<NodoVisual> nodosVisuales = new List<NodoVisual>();

        // Representación visual de un familiar en el lienzo.
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

        // Estructura interna para el recorrido por niveles.
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

        // Redibuja el contenido del control.
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

        // Manejo de clics para crear el primer familiar o agregar hijo/pareja.
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button != MouseButtons.Left)
                return;

            // Si aún no hay miembros, cualquier clic crea el primero.
            if (!arbol.TieneMiembros)
            {
                using (var ventana = new GestionarFamilia(arbol, null, true))
                {
                    if (ventana.ShowDialog(FindForm()) == DialogResult.OK)
                        Invalidate();
                }
                return;
            }

            // Buscar si se hizo clic sobre algún nodo.
            NodoVisual? seleccionado = null;
            foreach (var nodo in nodosVisuales)
            {
                if (nodo.ContienePunto(e.Location))
                {
                    seleccionado = nodo;
                    break;
                }
            }

            // Si se hizo clic en un familiar, se abre la ventana para agregar hijo o pareja.
            if (seleccionado != null)
            {
                using (var ventana = new GestionarFamilia(arbol, seleccionado.Familiar, false))
                {
                    if (ventana.ShowDialog(FindForm()) == DialogResult.OK)
                        Invalidate();
                }
            }
        }

        // Mensaje cuando todavía no se ha registrado ningún familiar.
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

        // Calcula niveles, acomoda parejas juntas y dibuja todo el árbol.
        private void DibujarArbol(Graphics g)
        {
            if (arbol.Raiz == null)
                return;

            var niveles = CalcularNiveles();

            // Primero ubicamos la posición de todos los nodos.
            int indiceNivel = 0;
            foreach (var kvp in niveles)
            {
                var familiaresNivel = kvp.Value;
                int y = 150 + indiceNivel * 160;

                var grupos = AgruparPorParejas(familiaresNivel);

                int espacioEntreGrupos = 170;
                int anchoTotal = (grupos.Count - 1) * espacioEntreGrupos;
                int inicioX = (Width / 2) - (anchoTotal / 2);

                foreach (var grupo in grupos)
                {
                    if (grupo.Count == 1)
                    {
                        var fam = grupo[0];
                        var centro = new Point(inicioX, y);
                        AgregarNodoVisualSiNoExiste(fam, centro);
                    }
                    else if (grupo.Count == 2)
                    {
                        int separacion = 90;
                        var fam1 = grupo[0];
                        var fam2 = grupo[1];

                        var centro1 = new Point(inicioX - separacion / 2, y);
                        var centro2 = new Point(inicioX + separacion / 2, y);

                        AgregarNodoVisualSiNoExiste(fam1, centro1);
                        AgregarNodoVisualSiNoExiste(fam2, centro2);
                    }

                    inicioX += espacioEntreGrupos;
                }

                indiceNivel++;
            }

            // Luego se dibujan las conexiones (parejas e hijos).
            DibujarConexiones(g);

            // Por último los círculos con la foto y los nombres.
            foreach (var nodo in nodosVisuales)
            {
                DibujarNodo(g, nodo);
            }
        }

        // Si el familiar ya existe en la lista, solo se actualiza su centro.
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

            nodosVisuales.Add(new NodoVisual(fam, centro, 35));
        }

        // Agrupa los familiares de un nivel para que las parejas queden juntas.
        private List<List<Familiar>> AgruparPorParejas(List<Familiar> familiaresNivel)
        {
            var grupos = new List<List<Familiar>>();
            var usados = new HashSet<Familiar>();

            foreach (var fam in familiaresNivel)
            {
                if (usados.Contains(fam))
                    continue;

                if (fam.Pareja != null &&
                    familiaresNivel.Contains(fam.Pareja) &&
                    !usados.Contains(fam.Pareja))
                {
                    grupos.Add(new List<Familiar> { fam, fam.Pareja });
                    usados.Add(fam);
                    usados.Add(fam.Pareja);
                }
                else
                {
                    grupos.Add(new List<Familiar> { fam });
                    usados.Add(fam);
                }
            }

            return grupos;
        }

        // Calcula el nivel (generación) de cada familiar a partir de la raíz.
        private Dictionary<int, List<Familiar>> CalcularNiveles()
        {
            var niveles = new Dictionary<int, List<Familiar>>();
            var visitados = new HashSet<Familiar>();
            var cola = new Queue<NodoNivel>();

            if (arbol.Raiz == null)
                return niveles;

            cola.Enqueue(new NodoNivel(arbol.Raiz, 0));
            visitados.Add(arbol.Raiz);

            while (cola.Count > 0)
            {
                var elemento = cola.Dequeue();
                var familiar = elemento.Familiar;
                int nivel = elemento.Nivel;

                if (!niveles.TryGetValue(nivel, out var listaNivel))
                {
                    listaNivel = new List<Familiar>();
                    niveles[nivel] = listaNivel;
                }

                if (!listaNivel.Contains(familiar))
                    listaNivel.Add(familiar);

                // Hijos en el siguiente nivel (PrimerHijo / HermanoDerecho).
                var hijo = familiar.PrimerHijo;
                while (hijo != null)
                {
                    if (!visitados.Contains(hijo))
                    {
                        visitados.Add(hijo);
                        cola.Enqueue(new NodoNivel(hijo, nivel + 1));
                    }
                    hijo = hijo.HermanoDerecho;
                }

                // Pareja en el mismo nivel (familia política).
                if (familiar.Pareja != null && !visitados.Contains(familiar.Pareja))
                {
                    visitados.Add(familiar.Pareja);
                    cola.Enqueue(new NodoNivel(familiar.Pareja, nivel));
                }
            }

            return niveles;
        }

        // Dibuja líneas entre parejas y desde la pareja (o el padre) hacia los hijos.
        private void DibujarConexiones(Graphics g)
        {
            if (nodosVisuales.Count == 0)
                return;

            var mapa = new Dictionary<Familiar, NodoVisual>();
            foreach (var nodo in nodosVisuales)
            {
                mapa[nodo.Familiar] = nodo;
            }

            // Colores de familia de sangre y familia política (mismos que en el borde).
            Color bordePrincipal = Color.FromArgb(0, 153, 51); // verde fuerte
            Color bordePolitica = Color.FromArgb(192, 64, 0);  // naranja rojizo

            using var penPareja = new Pen(Theme.Line, 2);            // gris: pareja y uniones de política
            using var penConsanguineo = new Pen(bordePrincipal, 3);  // verde: descendencia de sangre

            // Línea horizontal entre parejas (gris).
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

            // Líneas hacia los hijos.
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
                    // Punto de unión entre la pareja.
                    int unionX = (nodoPadre.Centro.X + nodoParejaH.Centro.X) / 2;
                    int unionY = nodoPadre.Centro.Y + nodoPadre.Radio + 6;

                    var puntoUnion = new Point(unionX, unionY);

                    // Desde el punto de unión a cada integrante:
                    //   - verde si es familia de sangre
                    //   - gris si es familia política
                    var penPadreUnion = nodoPadre.Familiar.EsFamiliaPolitica ? penPareja : penConsanguineo;
                    var penParejaUnion = nodoParejaH.Familiar.EsFamiliaPolitica ? penPareja : penConsanguineo;

                    g.DrawLine(
                        penPadreUnion,
                        new Point(nodoPadre.Centro.X, nodoPadre.Centro.Y + nodoPadre.Radio),
                        puntoUnion
                    );

                    g.DrawLine(
                        penParejaUnion,
                        new Point(nodoParejaH.Centro.X, nodoParejaH.Centro.Y + nodoParejaH.Radio),
                        puntoUnion
                    );

                    // Desde el punto de unión hacia el hijo siempre en verde
                    // porque los hijos son parte de la familia de sangre.
                    origenSuperior = puntoUnion;
                }
                else
                {
                    // Solo hay un progenitor conocido: el tramo hacia el hijo es verde.
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

        // Dibuja el círculo del nodo, la foto y el nombre debajo.
        private void DibujarNodo(Graphics g, NodoVisual nodo)
        {
            int r = nodo.Radio;
            var rect = new Rectangle(nodo.Centro.X - r, nodo.Centro.Y - r, r * 2, r * 2);

            // Colores para diferenciar familia principal y familia política.
            Color bordePrincipal = Color.FromArgb(0, 153, 51); // verde fuerte
            Color bordePolitica = Color.FromArgb(192, 64, 0);  // naranja rojizo

            Color colorBorde = nodo.Familiar.EsFamiliaPolitica
                ? bordePolitica
                : bordePrincipal;

            // Intento de cargar la foto asociada al familiar.
            bool dibujoFoto = false;
            if (!string.IsNullOrWhiteSpace(nodo.Familiar.RutaFoto) &&
                File.Exists(nodo.Familiar.RutaFoto))
            {
                try
                {
                    using (var imagen = Image.FromFile(nodo.Familiar.RutaFoto))
                    {
                        using var rutaCircular = new GraphicsPath();
                        rutaCircular.AddEllipse(rect);

                        g.SetClip(rutaCircular);
                        g.DrawImage(imagen, rect);
                        g.ResetClip();

                        dibujoFoto = true;
                    }
                }
                catch
                {
                    dibujoFoto = false;
                }
            }

            // Si no hay foto válida, se dibuja un círculo de fondo.
            if (!dibujoFoto)
            {
                using var relleno = new SolidBrush(Color.FromArgb(220, Theme.Btn));
                g.FillEllipse(relleno, rect);
            }

            // Borde grueso del círculo según el tipo de familiar.
            using (var borde = new Pen(colorBorde, 4))
            {
                g.DrawEllipse(borde, rect);
            }

            string nombre = nodo.Familiar.Nombre ?? string.Empty;
            string inicial = string.IsNullOrWhiteSpace(nombre)
                ? "?"
                : nombre.Trim()[0].ToString().ToUpperInvariant();

            // Si no se dibujó foto, se pone la inicial en el centro del círculo.
            if (!dibujoFoto)
            {
                using var fuenteInicial = new Font("Segoe UI", 12, FontStyle.Bold);
                var tamañoInicial = g.MeasureString(inicial, fuenteInicial);
                var puntoInicial = new PointF(
                    nodo.Centro.X - tamañoInicial.Width / 2f,
                    nodo.Centro.Y - tamañoInicial.Height / 2f
                );

                using var pincelBlanco = new SolidBrush(Color.White);
                g.DrawString(inicial, fuenteInicial, pincelBlanco, puntoInicial);
            }

            // Nombre debajo del nodo.
            FontStyle estiloNombre = nodo.Familiar.EsFamiliaPolitica
                ? FontStyle.Regular
                : FontStyle.Underline;

            using var fuenteNombre = new Font("Segoe UI", 10, estiloNombre);
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

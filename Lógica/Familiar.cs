// Lógica.Familiar.cs
// Archivo: Familiar.cs
// Clase que representa a una persona dentro del árbol genealógico.

using System;

namespace Proyecto_2_Arbol
{
    public class Familiar
    {
        // Datos básicos del familiar.
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Edad { get; set; }

        // Indica si la persona ya falleció.
        // Si está fallecido, la edad se entiende como la edad al morir.
        public bool Fallecido { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string RutaFoto { get; set; }

        // Indica si este nodo se considera familia política
        // para efectos de visualización (parejas de la familia de sangre).
        public bool EsFamiliaPolitica { get; internal set; }

        // Progenitor directo en el árbol (madre/padre que aparece como padre de este nodo).
        public Familiar? Progenitor { get; internal set; }

        // Primer hijo de este familiar. Si no tiene hijos, queda en null.
        public Familiar? PrimerHijo { get; internal set; }

        // Siguiente hijo del mismo progenitor (hermano en el mismo nivel).
        public Familiar? HermanoDerecho { get; internal set; }

        // Pareja actual.
        public Familiar? Pareja { get; internal set; }

        public Familiar(
            string nombre,
            string cedula,
            DateTime fechaNacimiento,
            int edad,
            double latitud,
            double longitud,
            string rutaFoto)
        {
            // Guarda el nombre tal cual lo escribe la persona.
            Nombre = nombre;

            // Guarda la cédula que se está usando para identificar al familiar.
            Cedula = cedula;

            // Guarda la fecha en la que nació el familiar.
            FechaNacimiento = fechaNacimiento;

            // Guarda la edad actual (o edad al morir, si está fallecido).
            Edad = edad;

            // Guarda la ubicación de residencia para usarla en el mapa.
            Latitud = latitud;
            Longitud = longitud;

            // Guarda la ruta del archivo de la foto seleccionada.
            RutaFoto = rutaFoto;

            // De entrada se asume que forma parte de la familia de sangre.
            EsFamiliaPolitica = false;

            // De entrada se asume que la persona está viva hasta que se indique lo contrario.
            Fallecido = false;
        }

        // Marca este familiar y su pareja como familia política para efectos de dibujo.
        internal void AsignarFamiliaPoliticaConPareja()
        {
            // Marca este nodo como política.
            EsFamiliaPolitica = true;

            // Si tiene pareja, también se marca como política.
            if (Pareja != null)
            {
                Pareja.EsFamiliaPolitica = true;
            }
        }

        // Recorre el árbol desde la raíz limpiando la marca de familia política.
        internal static void ResetearMarcasDeFamiliaPoliticaDesdeRaiz(Familiar raiz)
        {
            if (raiz == null)
                return;

            // Limpia la marca en este nodo.
            raiz.EsFamiliaPolitica = false;

            // Limpia también la marca en la pareja si existe.
            if (raiz.Pareja != null)
            {
                raiz.Pareja.EsFamiliaPolitica = false;
            }

            // Recorre todos los hijos en profundidad y resetea la marca.
            var hijo = raiz.PrimerHijo;
            while (hijo != null)
            {
                ResetearMarcasDeFamiliaPoliticaDesdeRaiz(hijo);
                hijo = hijo.HermanoDerecho;
            }
        }

        // Agrega un hijo a este familiar encadenándolo con PrimerHijo y HermanoDerecho.
        internal void AgregarHijo(Familiar hijo)
        {
            // Indica quién es el progenitor de este nuevo hijo.
            hijo.Progenitor = this;

            // Los hijos siempre se consideran familia de sangre.
            hijo.EsFamiliaPolitica = false;

            // Si no hay hijos aún, este pasa a ser el primero.
            if (PrimerHijo == null)
            {
                PrimerHijo = hijo;
                return;
            }

            // Si ya hay hijos, se recorre la lista de hermanos hasta el último.
            var actual = PrimerHijo;
            while (actual.HermanoDerecho != null)
            {
                actual = actual.HermanoDerecho;
            }

            // Se engancha el nuevo hijo al final de la lista de hermanos.
            actual.HermanoDerecho = hijo;
        }
    }
}

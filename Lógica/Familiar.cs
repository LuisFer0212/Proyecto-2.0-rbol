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
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string RutaFoto { get; set; }

        // Indica si este nodo corresponde a familia política (parejas).
        // Por defecto es falso; se marca en true cuando se registra como pareja.
        public bool EsFamiliaPolitica { get; internal set; }

        // Progenitor directo en el árbol.
        public Familiar? Progenitor { get; internal set; }

        // Primer hijo de este familiar. Si no tiene hijos, queda en null.
        public Familiar? PrimerHijo { get; internal set; }

        // Siguiente hijo del mismo progenitor (hermano en el mismo nivel).
        public Familiar? HermanoDerecho { get; internal set; }

        // Pareja actual. Se usa para mostrar la familia política al lado.
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
            Nombre = nombre;
            Cedula = cedula;
            FechaNacimiento = fechaNacimiento;
            Edad = edad;
            Latitud = latitud;
            Longitud = longitud;
            RutaFoto = rutaFoto;
            EsFamiliaPolitica = false; // por defecto se asume consanguíneo
        }

        // Agrega un hijo a este familiar encadenándolo con PrimerHijo y HermanoDerecho.
        internal void AgregarHijo(Familiar hijo)
        {
            hijo.Progenitor = this;
            hijo.EsFamiliaPolitica = false; // los hijos forman parte de la familia principal

            // Si no hay hijos aún, este se vuelve el primero.
            if (PrimerHijo == null)
            {
                PrimerHijo = hijo;
                return;
            }

            // Si ya hay hijos, se recorre hasta el último hermano.
            var actual = PrimerHijo;
            while (actual.HermanoDerecho != null)
            {
                actual = actual.HermanoDerecho;
            }

            actual.HermanoDerecho = hijo;
        }
    }
}

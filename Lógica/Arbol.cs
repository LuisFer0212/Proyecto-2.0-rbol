// Archivo: ArbolGenealogico.cs
// Clase que administra el árbol genealógico completo.

using System;

namespace Proyecto_2_Arbol
{
    public class ArbolGenealogico
    {
        // Primer integrante del árbol.
        public Familiar? Raiz { get; private set; }

        // Indica si el árbol ya tiene al menos un miembro.
        public bool TieneMiembros => Raiz != null;

        // Registra el primer integrante del árbol.
        public void AgregarMiembroInicial(Familiar familiar)
        {
            if (familiar == null)
                throw new ArgumentNullException(nameof(familiar));

            if (Raiz != null)
                throw new InvalidOperationException("Ya existe un miembro inicial en el árbol.");

            // La raíz se considera parte de la familia de sangre.
            familiar.EsFamiliaPolitica = false;
            Raiz = familiar;
        }

        // Agrega un hijo a un familiar ya existente.
        public void AgregarHijo(Familiar progenitor, Familiar hijo)
        {
            if (progenitor == null)
                throw new ArgumentNullException(nameof(progenitor));

            if (hijo == null)
                throw new ArgumentNullException(nameof(hijo));

            progenitor.AgregarHijo(hijo);
        }

        // Registra una pareja.
        // Solo se marca como familia política si la pareja es de un descendiente de sangre.
        public void AgregarPareja(Familiar persona, Familiar pareja)
        {
            if (persona == null)
                throw new ArgumentNullException(nameof(persona));

            if (pareja == null)
                throw new ArgumentNullException(nameof(pareja));

            if (persona.Pareja != null)
                throw new InvalidOperationException("Esta persona ya tiene una pareja registrada.");

            // Descendiente de sangre: no es político y tiene progenitor distinto de null.
            bool esDescendienteDeSangre = !persona.EsFamiliaPolitica && persona.Progenitor != null;

            // Solo las parejas de esos descendientes se marcan como familia política.
            pareja.EsFamiliaPolitica = esDescendienteDeSangre;

            persona.Pareja = pareja;
            pareja.Pareja = persona;
        }

        // Borra el árbol completo para poder empezar uno nuevo.
        public void Limpiar()
        {
            Raiz = null;
        }
        // =============================
        // NUEVO: Integración con Grafo
        // =============================
        private readonly GrafoResidencias grafo = new GrafoResidencias();

        // Devuelve todos los familiares del árbol (se usa en mapa y estadísticas)
        public Familiar[] ObtenerTodosLosFamiliares()
        {
            grafo.ConstruirGrafoDesdeArbol(this);
            return grafo.ObtenerTodosLosFamiliares();
        }

        // Devuelve el par más lejano y el promedio de distancias
        public (Familiar cercano1, Familiar cercano2, Familiar lejano1, Familiar lejano2, double distanciaPromedio)
        ObtenerEstadisticasGrafo()
        {
            grafo.ConstruirGrafoDesdeArbol(this);

            var (c1, c2, _) = grafo.ParMasCercano();
            var (l1, l2, _) = grafo.ParMasLejano();
            double promedio = grafo.DistanciaPromedio();

            return (c1, c2, l1, l2, promedio);
        }

    }
}

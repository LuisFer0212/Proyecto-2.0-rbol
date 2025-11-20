// Lógica.Arbol.cs
// Archivo: ArbolGenealogico.cs
// Clase que administra el árbol genealógico completo.

using System;
using System.Collections.Generic;
using System.Linq;

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

        // -------------------------------------------------------------
        // Métodos nuevos para cálculo de distancias entre familiares
        // -------------------------------------------------------------

        // Obtiene todos los familiares del árbol en una lista.
        public List<Familiar> ObtenerTodosLosFamiliares()
        {
            var lista = new List<Familiar>();
            if (Raiz != null)
                RecorrerRecursivo(Raiz, lista);
            return lista;
        }

        // Recorre el árbol recursivamente para llenar la lista de familiares.
        private void RecorrerRecursivo(Familiar f, List<Familiar> lista)
        {
            lista.Add(f);

            if (f.PrimerHijo != null)
            {
                var hijo = f.PrimerHijo;
                while (hijo != null)
                {
                    RecorrerRecursivo(hijo, lista);
                    hijo = hijo.HermanoDerecho;
                }
            }

            if (f.Pareja != null && !lista.Contains(f.Pareja))
            {
                lista.Add(f.Pareja);
            }
        }

        // Devuelve el par de familiares más cercano y más lejano, y la distancia promedio.
        public (Familiar f1, Familiar f2, double distanciaMax) ParMasCercanoYLejano(out double distanciaPromedio)
        {
            var lista = ObtenerTodosLosFamiliares();
            double minDist = double.MaxValue;
            double maxDist = double.MinValue;
            double sumaDist = 0;
            int contador = 0;

            Familiar? fMin1 = null, fMin2 = null;
            Familiar? fMax1 = null, fMax2 = null;

            for (int i = 0; i < lista.Count; i++)
            {
                for (int j = i + 1; j < lista.Count; j++)
                {
                    double d = GeoHelper.DistanciaKm(
                        lista[i].Latitud, lista[i].Longitud,
                        lista[j].Latitud, lista[j].Longitud
                    );

                    sumaDist += d;
                    contador++;

                    if (d < minDist)
                    {
                        minDist = d;
                        fMin1 = lista[i];
                        fMin2 = lista[j];
                    }
                    if (d > maxDist)
                    {
                        maxDist = d;
                        fMax1 = lista[i];
                        fMax2 = lista[j];
                    }
                }
            }

            distanciaPromedio = contador > 0 ? sumaDist / contador : 0;

            // Retorna el par más lejano y su distancia
            return (fMax1!, fMax2!, maxDist);
        }
    }
}

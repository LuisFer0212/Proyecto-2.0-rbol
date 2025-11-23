// Archivo: GrafoResidencias.cs
// Grafo de residencias implementado 100% desde cero usando tu ListaEnlazada propia.

using System;

namespace Proyecto_2_Arbol
{
    public class GrafoResidencias
    {
        private readonly ListaEnlazada<Familiar> nodos;
        private double[,] matrizDistancias;

        public GrafoResidencias()
        {
            nodos = new ListaEnlazada<Familiar>();
            matrizDistancias = new double[0, 0];
        }

        // ==========================================================
        // AGREGAR NODO
        // ==========================================================
        public void AgregarNodo(Familiar f)
        {
            if (!nodos.Contiene(f))
            {
                nodos.Agregar(f);
                RecalcularMatriz();
            }
        }

        // ==========================================================
        // CONSTRUIR GRAFO DESDE EL ÁRBOL
        // ==========================================================
        public void ConstruirGrafoDesdeArbol(ArbolGenealogico arbol)
        {
            // Limpiar nodos actuales
            var arr = nodos.AArray();
            nodos.Clear();

            if (arbol.Raiz == null)
                return;

            var cola = new Queue<Familiar>();
            cola.Enqueue(arbol.Raiz);

            while (cola.Count > 0)
            {
                var actual = cola.Dequeue();
                AgregarNodo(actual);

                // Hijos
                var hijo = actual.PrimerHijo;
                while (hijo != null)
                {
                    cola.Enqueue(hijo);
                    hijo = hijo.HermanoDerecho;
                }

                // Pareja
                if (actual.Pareja != null)
                    AgregarNodo(actual.Pareja);
            }

            RecalcularMatriz();
        }

        // ==========================================================
        // RECONSTRUIR MATRIZ
        // ==========================================================
        private void RecalcularMatriz()
        {
            var arr = nodos.AArray();
            int n = arr.Length;

            matrizDistancias = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        matrizDistancias[i, j] = 0;
                    else
                        matrizDistancias[i, j] = GeoHelper.DistanciaKm(
                            arr[i].Latitud, arr[i].Longitud,
                            arr[j].Latitud, arr[j].Longitud
                        );
                }
            }
        }

        // ==========================================================
        // OBTENER TODOS LOS FAMILIARES
        // ==========================================================
        public Familiar[] ObtenerTodosLosFamiliares()
        {
            return nodos.AArray();
        }

        // ==========================================================
        // OBTENER DISTANCIA ENTRE DOS NODOS
        // ==========================================================
        public double ObtenerDistancia(Familiar a, Familiar b)
        {
            var arr = nodos.AArray();
            int i = Array.IndexOf(arr, a);
            int j = Array.IndexOf(arr, b);

            if (i == -1 || j == -1)
                return double.PositiveInfinity;

            return matrizDistancias[i, j];
        }

        // ==========================================================
        // PAR MÁS LEJANO
        // ==========================================================
        public (Familiar A, Familiar B, double Distancia) ParMasLejano()
        {
            var arr = nodos.AArray();

            double max = -1;
            Familiar? f1 = null;
            Familiar? f2 = null;

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    double d = matrizDistancias[i, j];
                    if (d > max)
                    {
                        max = d;
                        f1 = arr[i];
                        f2 = arr[j];
                    }
                }
            }

            return (f1!, f2!, max);
        }

        // ==========================================================
        // PAR MÁS CERCANO
        // ==========================================================
        public (Familiar A, Familiar B, double Distancia) ParMasCercano()
        {
            var arr = nodos.AArray();

            double min = double.MaxValue;
            Familiar? f1 = null;
            Familiar? f2 = null;

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    double d = matrizDistancias[i, j];
                    if (d < min)
                    {
                        min = d;
                        f1 = arr[i];
                        f2 = arr[j];
                    }
                }
            }

            return (f1!, f2!, min);
        }

        // ==========================================================
        // DISTANCIA PROMEDIO
        // ==========================================================
        public double DistanciaPromedio()
        {
            var arr = nodos.AArray();

            double suma = 0;
            int total = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    suma += matrizDistancias[i, j];
                    total++;
                }
            }

            if (total == 0)
                return 0;

            return suma / total;
        }
    }
}

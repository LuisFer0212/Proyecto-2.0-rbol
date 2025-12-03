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
            var arr = nodos.AArray();
            nodos.Clear();

            if (arbol.Raiz == null)
                return;

            var cola = new Cola<Familiar>();
            cola.Enqueue(arbol.Raiz);

            while (cola.Count > 0)
            {
                var actual = cola.Dequeue();
                AgregarNodo(actual);

                var hijo = actual.PrimerHijo;
                while (hijo != null)
                {
                    cola.Enqueue(hijo);
                    hijo = hijo.HermanoDerecho;
                }

                if (actual.Pareja != null)
                {
                    if (!nodos.Contiene(actual.Pareja))
                    {
                        cola.Enqueue(actual.Pareja);
                        AgregarNodo(actual.Pareja);
                    }
                }
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

        public Familiar[] ObtenerTodosLosFamiliares()
        {
            return nodos.AArray();
        }

        public double ObtenerDistancia(Familiar a, Familiar b)
        {
            var arr = nodos.AArray();
            int i = Array.IndexOf(arr, a);
            int j = Array.IndexOf(arr, b);

            if (i == -1 || j == -1)
                return double.PositiveInfinity;

            return matrizDistancias[i, j];
        }

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

        // =============================
        // DIJKSTRA: Distancia mínima
        // =============================
        public double DistanciaMinima(Familiar origen, Familiar destino)
        {
            var arr = nodos.AArray();
            int n = arr.Length;

            int start = Array.IndexOf(arr, origen);
            int end = Array.IndexOf(arr, destino);

            if (start == -1 || end == -1)
                return double.PositiveInfinity;

            double[] dist = new double[n];
            bool[] visitado = new bool[n];

            for (int i = 0; i < n; i++)
                dist[i] = double.PositiveInfinity;

            dist[start] = 0;

            for (int iter = 0; iter < n - 1; iter++)
            {
                int u = -1;
                double min = double.PositiveInfinity;

                for (int i = 0; i < n; i++)
                {
                    if (!visitado[i] && dist[i] < min)
                    {
                        min = dist[i];
                        u = i;
                    }
                }

                if (u == -1)
                    break;

                visitado[u] = true;

                for (int v = 0; v < n; v++)
                {
                    if (!visitado[v])
                    {
                        double peso = matrizDistancias[u, v];
                        if (dist[u] + peso < dist[v])
                        {
                            dist[v] = dist[u] + peso;
                        }
                    }
                }
            }

            return dist[end];
        }

        // =============================
        // CAMINO MÍNIMO usando ListaEnlazada
        // =============================
        public Familiar[] CaminoMinimo(Familiar origen, Familiar destino)
        {
            var arr = nodos.AArray();
            int n = arr.Length;

            int start = Array.IndexOf(arr, origen);
            int end = Array.IndexOf(arr, destino);

            if (start == -1 || end == -1)
                return Array.Empty<Familiar>();

            double[] dist = new double[n];
            int[] previo = new int[n];
            bool[] visitado = new bool[n];

            for (int i = 0; i < n; i++)
            {
                dist[i] = double.PositiveInfinity;
                previo[i] = -1;
            }

            dist[start] = 0;

            for (int iter = 0; iter < n - 1; iter++)
            {
                int u = -1;
                double min = double.PositiveInfinity;

                for (int i = 0; i < n; i++)
                {
                    if (!visitado[i] && dist[i] < min)
                    {
                        min = dist[i];
                        u = i;
                    }
                }

                if (u == -1)
                    break;

                visitado[u] = true;

                for (int v = 0; v < n; v++)
                {
                    if (!visitado[v])
                    {
                        double peso = matrizDistancias[u, v];

                        if (dist[u] + peso < dist[v])
                        {
                            dist[v] = dist[u] + peso;
                            previo[v] = u;
                        }
                    }
                }
            }

            // ============================
            // RECONSTRUCCIÓN DEL CAMINO
            // ============================

            var camino = new ListaEnlazada<Familiar>();
            int actual = end;

            while (actual != -1)
            {
                camino.Agregar(arr[actual]);
                actual = previo[actual];
            }

            // Convertimos a arreglo
            var resultado = camino.AArray();

            // Invertimos manualmente el camino
            Array.Reverse(resultado);

            return resultado;
        }
    }
}

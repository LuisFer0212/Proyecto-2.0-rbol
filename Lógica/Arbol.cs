// Archivo: ArbolGenealogico.cs
// Clase que administra el árbol genealógico completo.

using System;
using System.IO;
using System.Text.Json;

namespace Proyecto_2_Arbol
{
    public class ArbolGenealogico
    {
        // Nombre del archivo donde se guarda el árbol en disco.
        private const string ArchivoPersistencia = "arbol_genealogico.json";

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

            // Marca la raíz como parte de la familia de sangre.
            familiar.EsFamiliaPolitica = false;
            Raiz = familiar;

            // Guarda el árbol actualizado en disco.
            GuardarEnArchivo();
        }

        // Agrega un hijo a un familiar ya existente.
        public void AgregarHijo(Familiar progenitor, Familiar hijo)
        {
            if (progenitor == null)
                throw new ArgumentNullException(nameof(progenitor));

            if (hijo == null)
                throw new ArgumentNullException(nameof(hijo));

            // Verifica que la edad del hijo sea menor que la del progenitor.
            if (hijo.Edad >= progenitor.Edad)
                throw new InvalidOperationException("La edad del hijo debe ser menor que la del progenitor.");

            // Enlaza el nuevo hijo al progenitor.
            progenitor.AgregarHijo(hijo);

            // Guarda el árbol actualizado en disco.
            GuardarEnArchivo();
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

            // Guarda el árbol actualizado en disco.
            GuardarEnArchivo();
        }

        // Borra el árbol completo para poder empezar uno nuevo.
        public void Limpiar()
        {
            // Deja la raíz en null para vaciar el árbol.
            Raiz = null;

            // Actualiza el archivo en disco para reflejar que ya no hay árbol.
            GuardarEnArchivo();
        }

        // =============================
        // Integración con Grafo
        // =============================

        // Grafo que se usa para cálculos de distancias entre familiares.
        private readonly GrafoResidencias grafo = new GrafoResidencias();

        // Devuelve todos los familiares del árbol (se usa en mapa y estadísticas).
        public Familiar[] ObtenerTodosLosFamiliares()
        {
            grafo.ConstruirGrafoDesdeArbol(this);
            return grafo.ObtenerTodosLosFamiliares();
        }

        // Devuelve el par más cercano, el más lejano y el promedio de distancias.
        public (Familiar cercano1, Familiar cercano2, Familiar lejano1, Familiar lejano2, double distanciaPromedio)
        ObtenerEstadisticasGrafo()
        {
            grafo.ConstruirGrafoDesdeArbol(this);

            var (c1, c2, _) = grafo.ParMasCercano();
            var (l1, l2, _) = grafo.ParMasLejano();
            double promedio = grafo.DistanciaPromedio();

            return (c1, c2, l1, l2, promedio);
        }

        // =============================
        // Persistencia en archivo
        // =============================

        // Guarda el árbol genealógico completo en un archivo JSON.
        public void GuardarEnArchivo()
        {
            try
            {
                // Si no hay raíz, se borra cualquier archivo previo.
                if (Raiz == null)
                {
                    string rutaVacia = ObtenerRutaArchivo();
                    if (File.Exists(rutaVacia))
                    {
                        File.Delete(rutaVacia);
                    }
                    return;
                }

                // Construye una lista con todos los familiares alcanzables desde la raíz.
                var lista = new ListaEnlazada<Familiar>();
                RecorrerArbol(Raiz, lista);

                Familiar[] familiares = lista.AArray();

                // Busca el índice de la raíz dentro del arreglo.
                int indiceRaiz = BuscarIndice(Raiz, familiares);
                if (indiceRaiz < 0)
                {
                    // Si por alguna razón no se encuentra la raíz, no se guarda nada.
                    return;
                }

                // Construye los DTO que se van a serializar.
                var dto = new ArbolPersistenteDTO();
                dto.RaizId = indiceRaiz;
                dto.Familiares = CrearDTOs(familiares);

                string json = JsonSerializer.Serialize(dto);
                string ruta = ObtenerRutaArchivo();

                File.WriteAllText(ruta, json);
            }
            catch
            {
                // Si ocurre algún error de IO, se omite para no romper la aplicación.
                // El árbol en memoria sigue funcionando normal.
            }
        }

        // Carga el árbol genealógico desde el archivo JSON si existe.
        public void CargarDesdeArchivo()
        {
            try
            {
                string ruta = ObtenerRutaArchivo();

                // Si no hay archivo, se deja el árbol vacío.
                if (!File.Exists(ruta))
                {
                    Raiz = null;
                    return;
                }

                string json = File.ReadAllText(ruta);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Raiz = null;
                    return;
                }

                var dto = JsonSerializer.Deserialize<ArbolPersistenteDTO>(json);
                if (dto == null || dto.Familiares == null || dto.Familiares.Length == 0)
                {
                    Raiz = null;
                    return;
                }

                // Crea todos los objetos Familiar sin relaciones.
                Familiar[] familiares = new Familiar[dto.Familiares.Length];
                for (int i = 0; i < dto.Familiares.Length; i++)
                {
                    FamiliarPersistenteDTO f = dto.Familiares[i];

                    // Crea el familiar con los datos básicos.
                    var nuevo = new Familiar(
                        f.Nombre ?? string.Empty,
                        f.Cedula ?? string.Empty,
                        f.FechaNacimiento,
                        f.Edad,
                        f.Latitud,
                        f.Longitud,
                        f.RutaFoto ?? string.Empty
                    );

                    // Restaura si se marcó como familia política.
                    nuevo.EsFamiliaPolitica = f.EsFamiliaPolitica;

                    familiares[i] = nuevo;
                }

                // Segunda pasada: reconstruye las relaciones entre los familiares.
                for (int i = 0; i < dto.Familiares.Length; i++)
                {
                    FamiliarPersistenteDTO f = dto.Familiares[i];
                    Familiar actual = familiares[i];

                    if (f.ProgenitorId >= 0 && f.ProgenitorId < familiares.Length)
                    {
                        actual.Progenitor = familiares[f.ProgenitorId];
                    }

                    if (f.PrimerHijoId >= 0 && f.PrimerHijoId < familiares.Length)
                    {
                        actual.PrimerHijo = familiares[f.PrimerHijoId];
                    }

                    if (f.HermanoDerechoId >= 0 && f.HermanoDerechoId < familiares.Length)
                    {
                        actual.HermanoDerecho = familiares[f.HermanoDerechoId];
                    }

                    if (f.ParejaId >= 0 && f.ParejaId < familiares.Length)
                    {
                        actual.Pareja = familiares[f.ParejaId];
                    }
                }

                // Asegura que las parejas queden enlazadas en ambos sentidos.
                for (int i = 0; i < dto.Familiares.Length; i++)
                {
                    FamiliarPersistenteDTO f = dto.Familiares[i];
                    if (f.ParejaId >= 0 && f.ParejaId < familiares.Length)
                    {
                        Familiar a = familiares[i];
                        Familiar b = familiares[f.ParejaId];

                        if (!ReferenceEquals(a.Pareja, b))
                        {
                            a.Pareja = b;
                        }

                        if (!ReferenceEquals(b.Pareja, a))
                        {
                            b.Pareja = a;
                        }
                    }
                }

                // Fija la raíz según el índice guardado.
                if (dto.RaizId >= 0 && dto.RaizId < familiares.Length)
                {
                    Raiz = familiares[dto.RaizId];
                }
                else
                {
                    Raiz = null;
                }
            }
            catch
            {
                // Si algo falla al leer o deserializar, se deja el árbol vacío.
                Raiz = null;
            }
        }

        // Devuelve la ruta completa del archivo de persistencia.
        private static string ObtenerRutaArchivo()
        {
            // Usa la carpeta donde está corriendo el ejecutable.
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, ArchivoPersistencia);
        }

        // Recorre el árbol a partir de un nodo y agrega cada familiar a la lista si aún no está.
        private void RecorrerArbol(Familiar? actual, ListaEnlazada<Familiar> acumulador)
        {
            if (actual == null)
            {
                return;
            }

            // Evita agregar el mismo familiar más de una vez.
            if (!acumulador.Contiene(actual))
            {
                acumulador.Agregar(actual);
            }
            else
            {
                return;
            }

            // Recorre todos los hijos de este familiar.
            Familiar? hijo = actual.PrimerHijo;
            while (hijo != null)
            {
                RecorrerArbol(hijo, acumulador);
                hijo = hijo.HermanoDerecho;
            }

            // Recorre también a la pareja si existe.
            if (actual.Pareja != null)
            {
                RecorrerArbol(actual.Pareja, acumulador);
            }
        }

        // Busca el índice de un familiar dentro de un arreglo por referencia.
        private int BuscarIndice(Familiar? familiar, Familiar[] arreglo)
        {
            if (familiar == null)
            {
                return -1;
            }

            for (int i = 0; i < arreglo.Length; i++)
            {
                if (ReferenceEquals(arreglo[i], familiar))
                {
                    return i;
                }
            }

            return -1;
        }

        // Construye los DTO a partir del arreglo de familiares.
        private FamiliarPersistenteDTO[] CrearDTOs(Familiar[] familiares)
        {
            var resultado = new FamiliarPersistenteDTO[familiares.Length];

            for (int i = 0; i < familiares.Length; i++)
            {
                Familiar f = familiares[i];

                var dto = new FamiliarPersistenteDTO
                {
                    Nombre = f.Nombre,
                    Cedula = f.Cedula,
                    FechaNacimiento = f.FechaNacimiento,
                    Edad = f.Edad,
                    Latitud = f.Latitud,
                    Longitud = f.Longitud,
                    RutaFoto = f.RutaFoto,
                    EsFamiliaPolitica = f.EsFamiliaPolitica,
                    ProgenitorId = BuscarIndice(f.Progenitor, familiares),
                    PrimerHijoId = BuscarIndice(f.PrimerHijo, familiares),
                    HermanoDerechoId = BuscarIndice(f.HermanoDerecho, familiares),
                    ParejaId = BuscarIndice(f.Pareja, familiares)
                };

                resultado[i] = dto;
            }

            return resultado;
        }

        // DTO principal que representa el árbol en disco.
        private class ArbolPersistenteDTO
        {
            public int RaizId { get; set; }
            public FamiliarPersistenteDTO[]? Familiares { get; set; }
        }

        // DTO que guarda los datos de cada familiar en el archivo.
        private class FamiliarPersistenteDTO
        {
            public string? Nombre { get; set; }
            public string? Cedula { get; set; }
            public DateTime FechaNacimiento { get; set; }
            public int Edad { get; set; }
            public double Latitud { get; set; }
            public double Longitud { get; set; }
            public string? RutaFoto { get; set; }
            public bool EsFamiliaPolitica { get; set; }
            public int ProgenitorId { get; set; }
            public int PrimerHijoId { get; set; }
            public int HermanoDerechoId { get; set; }
            public int ParejaId { get; set; }
        }
    }
}

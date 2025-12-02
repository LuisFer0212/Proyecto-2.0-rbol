using Xunit;
using Proyecto_2_Arbol;

namespace Proyecto_2_Arbol.Tests
{
    public class FamiliarTests
    {
        // ⭐⭐⭐ YA TIENES ESTAS DOS (las dejo para completar la clase)

        [Fact]
        public void AgregarHijo_CuandoNoTieneHijos_LoAsignaComoPrimerHijo()
        {
            var padre = new Familiar("Padre", "101", DateTime.Now, 40, 0, 0, "foto.jpg");
            var hijo = new Familiar("Hijo1", "202", DateTime.Now, 10, 0, 0, "foto.jpg");

            padre.AgregarHijo(hijo);

            Assert.Same(hijo, padre.PrimerHijo);
            Assert.Same(padre, hijo.Progenitor);
            Assert.False(hijo.EsFamiliaPolitica);
        }

        [Fact]
        public void AgregarHijo_CuandoYaTieneHijo_LoAgregaAlFinalDeLaCadena()
        {
            var padre = new Familiar("Padre", "101", DateTime.Now, 40, 0, 0, "foto.jpg");
            var hijoMayor = new Familiar("HijoMayor", "202", DateTime.Now, 15, 0, 0, "foto.jpg");
            var hijoMenor = new Familiar("HijoMenor", "303", DateTime.Now, 5, 0, 0, "foto.jpg");

            padre.AgregarHijo(hijoMayor);
            padre.AgregarHijo(hijoMenor);

            Assert.Same(hijoMayor, padre.PrimerHijo);
            Assert.Same(hijoMenor, hijoMayor.HermanoDerecho);
            Assert.Same(padre, hijoMenor.Progenitor);
        }

        [Fact]
        public void AgregarMiembroInicial_AsignaRaizCorrectamente()
        {
            var arbol = new ArbolGenealogico();
            var persona = new Familiar("Ana", "1", DateTime.Now, 30, 0, 0, "foto.jpg");

            arbol.AgregarMiembroInicial(persona);

            Assert.Equal(persona, arbol.Raiz);
            Assert.True(arbol.TieneMiembros);
            Assert.False(persona.EsFamiliaPolitica);

        }

        [Fact]
        public void AgregarHijo_AsignaPrimerHijoCorrectamente()
        {
            var arbol = new ArbolGenealogico();
            var padre = new Familiar("Padre", "101", DateTime.Now, 50, 0, 0, "foto.jpg");
            var hijo = new Familiar("Hijo", "202", DateTime.Now, 10, 0, 0, "foto.jpg");

            arbol.AgregarMiembroInicial(padre);
            arbol.AgregarHijo(padre, hijo);

            Assert.Equal(hijo, padre.PrimerHijo);
            Assert.Equal(padre, hijo.Progenitor);
        }

        [Fact]
        public void AgregarPareja_EnlazaParejasCorrectamente()
        {
            var arbol = new ArbolGenealogico();
            var persona = new Familiar("A", "1", DateTime.Now, 40, 0, 0, "foto.jpg");
            var pareja = new Familiar("B", "2", DateTime.Now, 38, 0, 0, "foto.jpg");

            arbol.AgregarMiembroInicial(persona);
            arbol.AgregarPareja(persona, pareja);

            Assert.Equal(pareja, persona.Pareja);
            Assert.Equal(persona, pareja.Pareja);
        }

        [Fact]
        public void AgregarHijo_SinHijos_AsignaPrimerHijo()
        {
            var padre = new Familiar("Padre", "101", DateTime.Now, 40, 0, 0, "foto.jpg");
            var hijo = new Familiar("Hijo", "202", DateTime.Now, 10, 0, 0, "foto.jpg");

            padre.AgregarHijo(hijo);

            Assert.Equal(hijo, padre.PrimerHijo);
            Assert.Equal(padre, hijo.Progenitor);
            Assert.False(hijo.EsFamiliaPolitica);
        }

         [Fact]
        public void AgregarHijo_ConHijoPrevio_EncadenaComoHermanoDerecho()
        {
            var padre = new Familiar("Padre", "101", DateTime.Now, 40, 0, 0, "foto.jpg");
            var hijo1 = new Familiar("Hijo1", "202", DateTime.Now, 15, 0, 0, "foto.jpg");
            var hijo2 = new Familiar("Hijo2", "303", DateTime.Now, 10, 0, 0, "foto.jpg");

            padre.AgregarHijo(hijo1);
            padre.AgregarHijo(hijo2);

            Assert.Equal(hijo1, padre.PrimerHijo);
            Assert.Equal(hijo2, hijo1.HermanoDerecho);
            Assert.Equal(padre, hijo2.Progenitor);
        }

        [Fact]
        public void DistanciaKm_MismaUbicacion_RetornaCero()
        {
            double lat = 10.0;
            double lon = -84.0;

            double distancia = GeoHelper.DistanciaKm(lat, lon, lat, lon);

            Assert.Equal(0, distancia, 5); // tolerancia de 5 decimales
        }

        [Fact]
        public void DistanciaKm_EsSimetrica()
        {
            double lat1 = 10.0;
            double lon1 = -84.0;

            double lat2 = 11.0;
            double lon2 = -83.0;

            double d1 = GeoHelper.DistanciaKm(lat1, lon1, lat2, lon2);
            double d2 = GeoHelper.DistanciaKm(lat2, lon2, lat1, lon1);

            Assert.Equal(d1, d2, 5);
        }

        [Fact]
        public void ObtenerDistancia_EntreDosNodosDevuelveDistanciaCorrecta()
        {
            var grafo = new GrafoResidencias();

            var f1 = new Familiar("A", "1", DateTime.Now, 20, 10.0, -84.0, "");
            var f2 = new Familiar("B", "2", DateTime.Now, 22, 10.1, -84.1, "");

            grafo.AgregarNodo(f1);
            grafo.AgregarNodo(f2);

            double d = grafo.ObtenerDistancia(f1, f2);

            Assert.InRange(d, 14, 18); // distancia aproximada 15 km
        }

        [Fact]
        public void ParMasCercano_DevuelveLaParejaCorrecta()
        {
            var grafo = new GrafoResidencias();

            var a = new Familiar("A", "1", DateTime.Now, 20, 0, 0, "");
            var b = new Familiar("B", "2", DateTime.Now, 22, 0.05, 0.05, ""); // más cerca
            var c = new Familiar("C", "3", DateTime.Now, 30, 10, 10, "");

            grafo.AgregarNodo(a);
            grafo.AgregarNodo(b);
            grafo.AgregarNodo(c);

            var (n1, n2, dist) = grafo.ParMasCercano();

            Assert.True((n1 == a && n2 == b) || (n1 == b && n2 == a));
            Assert.True(dist < 10); // debe ser la distancia pequeña
        }

        [Fact]
        public void AArray_ConvierteListaAArrayCorrectamente()
        {
            var lista = new ListaEnlazada<int>();

            lista.Agregar(10);
            lista.Agregar(20);
            lista.Agregar(30);

            var arr = lista.AArray();

            Assert.Equal(3, arr.Length);
            Assert.Equal(10, arr[0]);
            Assert.Equal(20, arr[1]);
            Assert.Equal(30, arr[2]);
        }

        [Fact]
        public void Agregar_Elementos_SeAgreganEnOrdenCorrecto()
        {
            var lista = new ListaEnlazada<string>();

            lista.Agregar("A");
            lista.Agregar("B");
            lista.Agregar("C");

            Assert.Equal(3, lista.Count);
            Assert.Equal("A", lista.Get(0));
            Assert.Equal("B", lista.Get(1));
            Assert.Equal("C", lista.Get(2));
        }
    }
}
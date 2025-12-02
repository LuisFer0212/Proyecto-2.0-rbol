// Archivo: ListaEnlazada.cs
// Implementación propia de una lista enlazada simple desde cero.

using System;

namespace Proyecto_2_Arbol
{
    public class NodoLista<T>
    {
        public T Valor;
        public NodoLista<T>? Siguiente;

        public NodoLista(T valor)
        {
            Valor = valor;
            Siguiente = null;
        }
    }

    public class ListaEnlazada<T>
    {
        private NodoLista<T>? cabeza;
        private int cantidad;

        public ListaEnlazada()
        {
            cabeza = null;
            cantidad = 0;
        }

        public int Count => cantidad;

        // Agregar un elemento al final de la lista
        public void Agregar(T item)
        {
            var nuevo = new NodoLista<T>(item);

            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                var actual = cabeza;
                while (actual.Siguiente != null)
                    actual = actual.Siguiente;

                actual.Siguiente = nuevo;
            }

            cantidad++;
        }

        // Verifica si un valor ya existe en la lista (Comparación por referencia)
        public bool Contiene(T item)
        {
            var actual = cabeza;
            while (actual != null)
            {
                if (ReferenceEquals(actual.Valor, item))
                    return true;

                actual = actual.Siguiente;
            }
            return false;
        }

        // Obtener elemento por índice
        public T Get(int index)
        {
            if (index < 0 || index >= cantidad)
                throw new IndexOutOfRangeException();

            int i = 0;
            var actual = cabeza;

            while (actual != null)
            {
                if (i == index)
                    return actual.Valor;

                i++;
                actual = actual.Siguiente;
            }

            throw new Exception("Error inesperado en la lista.");
        }

        // Convertir lista a array para recorridos
        public T[] AArray()
        {
            T[] array = new T[cantidad];
            var actual = cabeza;
            int i = 0;

            while (actual != null)
            {
                array[i++] = actual.Valor;
                actual = actual.Siguiente;
            }

            return array;
        }
        public void Clear()
        {
            cabeza = null;
            cantidad = 0;
        }
    }
}

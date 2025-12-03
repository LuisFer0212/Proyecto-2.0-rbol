// Archivo: ListaEnlazada.cs
// Implementación propia de una lista enlazada desde cero,
// ampliada para soportar foreach, indexación, inserciones, eliminación, etc.

using System;
using System.Collections;
using System.Collections.Generic;

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

    public class ListaEnlazada<T> : IEnumerable<T>
    {
        private NodoLista<T>? cabeza;
        private int cantidad;

        public ListaEnlazada()
        {
            cabeza = null;
            cantidad = 0;
        }

        public int Count => cantidad;

        // ============================================================
        // AGREGAR AL FINAL
        // ============================================================
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
                while (actual!.Siguiente != null)
                    actual = actual.Siguiente;

                actual.Siguiente = nuevo;
            }

            cantidad++;
        }

        // ============================================================
        // INSERTAR EN POSICIÓN
        // ============================================================
        public void InsertAt(int index, T item)
        {
            if (index < 0 || index > cantidad)
                throw new IndexOutOfRangeException();

            var nuevo = new NodoLista<T>(item);

            if (index == 0)
            {
                nuevo.Siguiente = cabeza;
                cabeza = nuevo;
            }
            else
            {
                var actual = cabeza;
                for (int i = 0; i < index - 1; i++)
                    actual = actual!.Siguiente;

                nuevo.Siguiente = actual!.Siguiente;
                actual.Siguiente = nuevo;
            }

            cantidad++;
        }

        // ============================================================
        // ELIMINAR EN POSICIÓN
        // ============================================================
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= cantidad)
                throw new IndexOutOfRangeException();

            if (index == 0)
            {
                cabeza = cabeza!.Siguiente;
            }
            else
            {
                var actual = cabeza;
                for (int i = 0; i < index - 1; i++)
                    actual = actual!.Siguiente;

                actual!.Siguiente = actual.Siguiente!.Siguiente;
            }

            cantidad--;
        }

        // ============================================================
        // BUSCAR ÍNDICE DE VALOR
        // ============================================================
        public int IndexOf(T item)
        {
            int i = 0;
            var actual = cabeza;

            while (actual != null)
            {
                if (Equals(actual.Valor, item))
                    return i;

                actual = actual.Siguiente;
                i++;
            }

            return -1;
        }

        // ============================================================
        // CONTAINS POR VALOR
        // ============================================================
        public bool ContainsValue(T item)
        {
            var actual = cabeza;
            while (actual != null)
            {
                if (Equals(actual.Valor, item))
                    return true;

                actual = actual.Siguiente;
            }
            return false;
        }

        // ============================================================
        // CONTAINS POR REFERENCIA (tu versión original)
        // ============================================================
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

        // ============================================================
        // GET POR ÍNDICE (tu versión original)
        // ============================================================
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

                actual = actual.Siguiente;
                i++;
            }

            throw new Exception("Error inesperado en la lista.");
        }

        // ============================================================
        // INDEXADOR (permite grupo[0], grupo[1], etc.)
        // ============================================================
        public T this[int index]
        {
            get => Get(index);
            set
            {
                if (index < 0 || index >= cantidad)
                    throw new IndexOutOfRangeException();

                int i = 0;
                var actual = cabeza;

                while (actual != null)
                {
                    if (i == index)
                    {
                        actual.Valor = value;
                        return;
                    }
                    actual = actual.Siguiente;
                    i++;
                }
            }
        }

        // ============================================================
        // REVERSE (invierte la lista)
        // ============================================================
        public void Reverse()
        {
            NodoLista<T>? prev = null;
            var actual = cabeza;

            while (actual != null)
            {
                var siguiente = actual.Siguiente;
                actual.Siguiente = prev;
                prev = actual;
                actual = siguiente;
            }

            cabeza = prev;
        }

        // ============================================================
        // ToArray()
        // ============================================================
        public T[] AArray()
        {
            T[] arr = new T[cantidad];
            var actual = cabeza;
            int i = 0;

            while (actual != null)
            {
                arr[i++] = actual.Valor;
                actual = actual.Siguiente;
            }

            return arr;
        }

        // ============================================================
        // CLEAR
        // ============================================================
        public void Clear()
        {
            cabeza = null;
            cantidad = 0;
        }

        // ============================================================
        // IEnumerable<T> → permite foreach sin usar List<>
        // ============================================================
        public IEnumerator<T> GetEnumerator()
        {
            var actual = cabeza;

            while (actual != null)
            {
                yield return actual.Valor;
                actual = actual.Siguiente;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

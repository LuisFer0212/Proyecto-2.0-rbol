using System;
using System.Collections;
using System.Collections.Generic;

namespace Proyecto_2_Arbol
{
    public class NodoCola<T>
    {
        public T Valor;
        public NodoCola<T>? Siguiente;

        public NodoCola(T valor)
        {
            Valor = valor;
            Siguiente = null;
        }
    }

    public class Cola<T> : IEnumerable<T>
    {
        private NodoCola<T>? frente;
        private NodoCola<T>? final;
        private int cantidad;

        public Cola()
        {
            frente = null;
            final = null;
            cantidad = 0;
        }

        public int Count => cantidad;
        public bool IsEmpty => cantidad == 0;

        // ============================================
        // ENQUEUE - Insertar al final O(1)
        // ============================================
        public void Enqueue(T item)
        {
            var nuevo = new NodoCola<T>(item);

            if (IsEmpty)
            {
                frente = nuevo;
                final = nuevo;
            }
            else
            {
                final!.Siguiente = nuevo;
                final = nuevo;
            }

            cantidad++;
        }

        // ============================================
        // DEQUEUE - Remover del frente O(1)
        // ============================================
        public T Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("La cola está vacía.");

            var valor = frente!.Valor;
            frente = frente.Siguiente;

            if (frente == null)
                final = null;

            cantidad--;
            return valor;
        }

        // ============================================
        // PEEK - Ver el primer elemento sin removerlo
        // ============================================
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("La cola está vacía.");

            return frente!.Valor;
        }

        // ============================================
        // CLEAR
        // ============================================
        public void Clear()
        {
            frente = null;
            final = null;
            cantidad = 0;
        }

        // ============================================
        // IEnumerable (para foreach)
        // ============================================
        public IEnumerator<T> GetEnumerator()
        {
            var actual = frente;

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

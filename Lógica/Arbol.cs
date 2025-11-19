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

        // Registra una pareja. No se trata como hijo, solo se enlazan ambos.
        public void AgregarPareja(Familiar persona, Familiar pareja)
        {
            if (persona == null)
                throw new ArgumentNullException(nameof(persona));

            if (pareja == null)
                throw new ArgumentNullException(nameof(pareja));

            if (persona.Pareja != null)
                throw new InvalidOperationException("Esta persona ya tiene una pareja registrada.");

            persona.Pareja = pareja;
            pareja.Pareja = persona;
        }

        // Borra el árbol completo para poder empezar uno nuevo.
        public void Limpiar()
        {
            Raiz = null;
        }
    }
}

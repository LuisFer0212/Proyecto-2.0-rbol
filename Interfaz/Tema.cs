using System.Drawing;

namespace Proyecto_2_Arbol
{
    // Clase estática que centraliza los colores utilizados en la interfaz de la aplicación.
    public static class Theme
    {
        // Color de fondo principal de las ventanas.
        public static Color BgMain => ColorTranslator.FromHtml("#F4F6FA");

        // Color de fondo del panel lateral de menú.
        public static Color BgPane => ColorTranslator.FromHtml("#283142");

        // Color base de los botones.
        public static Color Btn => ColorTranslator.FromHtml("#4C6EF5");

        // Color de los botones cuando el puntero pasa por encima.
        public static Color BtnHover => ColorTranslator.FromHtml("#3B5BDB");

        // Color principal del texto sobre fondos claros.
        public static Color TextPrimary => Color.Black;

        // Color del texto que se muestra sobre el panel lateral oscuro.
        public static Color TextOnPane => Color.White;

        // Color de fondo de tarjetas o paneles internos.
        public static Color Card => Color.White;

        // Color de líneas de separación o bordes suaves.
        public static Color Line => ColorTranslator.FromHtml("#C9D1D9");
    }
}

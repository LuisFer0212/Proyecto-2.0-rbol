using System.Drawing;

namespace Proyecto_2_Arbol
{
    public static class Theme
    {
        public static bool DarkMode { get; private set; } = false;

        // Cambia entre modo claro y oscuro
        public static void ToggleMode()
        {
            DarkMode = !DarkMode;
        }

        // =======================
        // PALETA – MODO CLARO
        // =======================
        private static readonly Color LightBgMain = ColorTranslator.FromHtml("#F4F6FA");
        private static readonly Color LightBgPane = ColorTranslator.FromHtml("#283142");
        private static readonly Color LightBtn = ColorTranslator.FromHtml("#4C6EF5");
        private static readonly Color LightBtnHover = ColorTranslator.FromHtml("#3B5BDB");
        private static readonly Color LightTextPrimary = Color.Black;
        private static readonly Color LightTextOnPane = Color.White;
        private static readonly Color LightCard = Color.White;
        private static readonly Color LightLine = ColorTranslator.FromHtml("#C9D1D9");

        // =======================
        // PALETA – MODO OSCURO
        // =======================
        private static readonly Color DarkBgMain = ColorTranslator.FromHtml("#0D1117");
        private static readonly Color DarkBgPane = ColorTranslator.FromHtml("#161B22");
        private static readonly Color DarkBtn = ColorTranslator.FromHtml("#238636");
        private static readonly Color DarkBtnHover = ColorTranslator.FromHtml("#2EA043");
        private static readonly Color DarkTextPrimary = Color.White;
        private static readonly Color DarkTextOnPane = Color.White;
        private static readonly Color DarkCard = ColorTranslator.FromHtml("#21262D");
        private static readonly Color DarkLine = ColorTranslator.FromHtml("#30363D");

        // =====================
        // PROPIEDADES PÚBLICAS
        // =====================
        public static Color BgMain => DarkMode ? DarkBgMain : LightBgMain;
        public static Color BgPane => DarkMode ? DarkBgPane : LightBgPane;
        public static Color Btn => DarkMode ? DarkBtn : LightBtn;
        public static Color BtnHover => DarkMode ? DarkBtnHover : LightBtnHover;
        public static Color TextPrimary => DarkMode ? DarkTextPrimary : LightTextPrimary;
        public static Color TextOnPane => DarkMode ? DarkTextOnPane : LightTextOnPane;
        public static Color Card => DarkMode ? DarkCard : LightCard;
        public static Color Line => DarkMode ? DarkLine : LightLine;
    }
}

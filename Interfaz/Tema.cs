using System.Drawing;

namespace Proyecto_2_Arbol
{
    public static class Theme
    {
        public static bool Dark = false;

        public static Color BgMain      => Dark ? ColorTranslator.FromHtml("#1F2430") : ColorTranslator.FromHtml("#F4F6FA");
        public static Color BgPane      => Dark ? ColorTranslator.FromHtml("#171B24") : ColorTranslator.FromHtml("#283142");
        public static Color Btn         => Dark ? ColorTranslator.FromHtml("#3D6BF2") : ColorTranslator.FromHtml("#4C6EF5");
        public static Color BtnHover    => Dark ? ColorTranslator.FromHtml("#3357CC") : ColorTranslator.FromHtml("#3B5BDB");
        public static Color TextPrimary => Dark ? Color.WhiteSmoke : Color.Black;
        public static Color TextOnPane  => Color.White;
        public static Color Card        => Dark ? ColorTranslator.FromHtml("#222838") : Color.White;
        public static Color Line        => Dark ? ColorTranslator.FromHtml("#4A5568") : ColorTranslator.FromHtml("#C9D1D9");
    }
}

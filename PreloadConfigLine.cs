using System;
using System.Drawing;

namespace PreloadAlert
{
    public enum PreloadCategory
    {
        Unknown = 0,
        Essence,
        Shrine,
        Strongbox,
        Exile,
        Azmeri,
        Expedition,
        Abyss,
        Incursion,
        Misc,
        Custom,
    }

    public class PreloadConfigLine : ConfigLineBase
    {
        public Func<Color> FastColor;
        public PreloadCategory Category { get; set; } = PreloadCategory.Unknown;
    }

    public abstract class ConfigLineBase
    {
        public string Text { get; set; }
        public Color? Color { get; set; }

        public override bool Equals(object obj)
        {
            return Text == ((ConfigLineBase) obj).Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }
}

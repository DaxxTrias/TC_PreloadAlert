using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Tracks preloads observed in a specific zone to handle cross-area contamination
    /// </summary>
    public class ZonePreloadData
    {
        public string ZoneId { get; set; }
        public string ZoneName { get; set; }
        public DateTime LastSeen { get; set; }
        public Dictionary<string, PreloadConfigLine> Alerts { get; set; } = new Dictionary<string, PreloadConfigLine>();
        public HashSet<string> ObservedPreloads { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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

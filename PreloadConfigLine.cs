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
        public Func<Color>? FastColor;
        public PreloadCategory Category { get; set; } = PreloadCategory.Unknown;
    }

    /// <summary>
    /// Tracks preloads observed in a specific zone to handle cross-area contamination
    /// </summary>
    public class ZonePreloadData
    {
        public string ZoneId { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;
        public DateTime LastSeen { get; set; }
        public Dictionary<string, PreloadConfigLine> Alerts { get; set; } = new Dictionary<string, PreloadConfigLine>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> ObservedPreloads { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public abstract class ConfigLineBase
    {
        public string Text { get; set; } = string.Empty;
        public Color? Color { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ConfigLineBase other && Text == other.Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }
}

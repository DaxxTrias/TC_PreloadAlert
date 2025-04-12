using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using System.Drawing;
using System.Numerics;

namespace PreloadAlert
{
    public class PreloadAlertSettings : ISettings
    {
        public PreloadAlertSettings()
        {
            Enable = new ToggleNode(true);
            Essence = new ToggleNode(true);
            Strongboxes = new ToggleNode(true);
            Shrines = new ToggleNode(true);
        }

        public ToggleNode Enable { get; set; }
        [Menu("Reparse Preloads", "Run the parser multiple times in a zone instead of only once on load")]
        public ToggleNode ReparsePreloads { get; set; } = new ToggleNode(true);
        [Menu("Reparse Delay", "How many seconds between reparsing events")]
        [ConditionalDisplay(nameof(ReparsePreloads), true)]
        public RangeNode<int> ReparseDelay { get; set; } = new(60, 15, 300);
        public ColorNode BackgroundColor { get; set; } = Color.FromArgb(0, 0, 0);
        public ColorNode DefaultTextColor { get; set; } = Color.FromArgb(210, 210, 210);
        public ColorNode AreaTextColor { get; set; } = Color.FromArgb(150, 200, 250);
        [Menu("Display Position", "X and Y coordinates to draw on screen")]
        public RangeNode<Vector2> DisplayPosition { get; set; } = new(new Vector2(1040, 0), Vector2.Zero, Vector2.One * 4000);
        [Menu("Show Shrines", "Enable or Disable the showing of Shrines")]

        #region Shrine Settings
        public ToggleNode Shrines { get; set; }
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfGreed { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfRegeneration { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfResistance { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfAcceleration { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfGloom { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfCrit { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfCorruption { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfFire { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfEnduring { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfLightning { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfCold { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfDivine { get; set; } = Color.FromArgb(255, 255, 255);
        #endregion

        #region Essence Settings
        [Menu("Show Essences", "Enable or Disable the showing of Essence(s)")]
        public ToggleNode Essence { get; set; }
        //public RangeNode<int> TextSize { get; set; }
        //public ToggleNode ParallelParsing { get; set; } = new ToggleNode(true);
        //public ToggleNode LoadOnlyMetadata { get; set; } = new ToggleNode(true);
        //public ColorNode CadiroTrader { get; set; }
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfLightning { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfLightning { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfSpeed { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfSpeed { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfPhysical { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfPhysical { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfAttack { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfAttack { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfLife { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfLife { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfChaos { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfChaos { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfCasting { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfCasting { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfCold { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfCold { get; set; } =  Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfMana { get; set; } =   Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfMana { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfDefence { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfDefence { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfAttributes { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfAttributes { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode EssenceOfFire { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Essence), true)]
        public ColorNode GreaterEssenceOfFire { get; set; } = Color.FromArgb(255, 255, 255);
        #endregion

        #region Strongbox Settings
        [Menu("Show Strongboxes", "Enable or Disable the showing of Strongboxes")]
        public ToggleNode Strongboxes { get; set; }
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode ArcanistStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode ArtisanStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode CartographerStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode DivinerStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode GemcutterStrongbox { get; set; } = Color.FromArgb(255, 255, 27);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode JewellerStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode BlacksmithStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode ArmourerStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode OrnateStrongbox { get; set; } = Color.FromArgb(190, 95, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode LargeStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode EpicStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode ResearchStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode BasicStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public ColorNode ArcaneStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        #endregion
    }
}

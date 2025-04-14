using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

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
            Exiles = new ToggleNode(true);
            Azmeri = new ToggleNode(true);
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

        public ToggleNode Shrines { get; set; }
        [ConditionalDisplay(nameof(Shrines), true)]
        public ShrineSettings ShrineColors { get; set; } = new ShrineSettings();

        [Menu("Show Essences", "Enable or Disable the showing of Essence(s)")]
        public ToggleNode Essence { get; set; }
        [ConditionalDisplay(nameof(Essence), true)]
        public EssenceSettings EssenceColors { get; set; } = new EssenceSettings();

        [Menu("Show Strongboxes", "Enable or Disable the showing of Strongboxes")]
        public ToggleNode Strongboxes { get; set; }
        [ConditionalDisplay(nameof(Strongboxes), true)]
        public StrongboxSettings StrongboxColors { get; set; } = new StrongboxSettings();

        [Menu("Show Exiles", "Enable or Disable the showing of Rogue Exiles")]
        public ToggleNode Exiles { get; set; }
        [ConditionalDisplay(nameof(Exiles), true)]
        public ExileSettings ExileColors { get; set; } = new ExileSettings();

        [Menu("Show Azmeri", "Enable or Disable the showing of Azmeri wisps")]
        public ToggleNode Azmeri { get; set; }
        [ConditionalDisplay(nameof(Azmeri), true)]
        public AzmeriSettings AzmeriColors { get; set; } = new AzmeriSettings();
    }
    [Submenu]
    public class ExileSettings
    {
        public ColorNode Sondar { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Doran { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Ulfred { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Hesperia { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Nyassa { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Clara { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Vasa { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Adrienne { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Bronnach { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Raok { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Taua { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Marauder1 { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Huntress1 { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Huntress2 { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Duelist { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Mercenary2 { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Templar { get; set; } = Color.FromArgb(226, 162, 0);
        public ColorNode Druid { get; set; } = Color.FromArgb(226, 162, 0);
    }

    [Submenu]
    public class StrongboxSettings
    {
        public ColorNode ArcanistStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ArtisanStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode CartographerStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode DivinerStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GemcutterStrongbox { get; set; } = Color.FromArgb(255, 255, 27);
        public ColorNode JewellerStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode BlacksmithStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ArmourerStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode OrnateStrongbox { get; set; } = Color.FromArgb(190, 95, 255);
        public ColorNode LargeStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EpicStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ResearchStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode BasicStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ArcaneStrongbox { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode IxchelsTormentStrongbox { get; set; } = Color.FromArgb(228, 110, 0);
    }

    [Submenu]
    public class ShrineSettings
    {
        public ColorNode ShrineOfGreed { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfRegeneration { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfResistance { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfAcceleration { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfGloom { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfCrit { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfCorruption { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfFire { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfEnduring { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfLightning { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfCold { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode ShrineOfDivine { get; set; } = Color.FromArgb(255, 255, 255);
    }

    [Submenu]
    public class EssenceSettings
    {
        public ColorNode EssenceOfLightning { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfLightning { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfSpeed { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfSpeed { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfPhysical { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfPhysical { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfAttack { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfAttack { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfLife { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfLife { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfChaos { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfChaos { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfCasting { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfCasting { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfCold { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfCold { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfMana { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfMana { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfDefence { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfDefence { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfAttributes { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfAttributes { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode EssenceOfFire { get; set; } = Color.FromArgb(255, 255, 255);
        public ColorNode GreaterEssenceOfFire { get; set; } = Color.FromArgb(255, 255, 255);
    }

    [Submenu]
    public class AzmeriSettings
    {
        public ColorNode Delwyn { get; set; } = Color.FromArgb(90, 210, 0);
        public ColorNode OxWild { get; set; } = Color.FromArgb(90, 210, 0);
        public ColorNode BearWild { get; set; } = Color.FromArgb(90, 210, 0);
        public ColorNode BoarWild { get; set; } = Color.FromArgb(90, 210, 0);
        public ColorNode StagWild { get; set; } = Color.FromArgb(90, 210, 0);
        public ColorNode StagVivid { get; set; } = Color.FromArgb(90, 210, 0);
        public ColorNode CatVivid { get; set; } = Color.FromArgb(90, 210, 0);
        public ColorNode SerpentPrimal { get; set; } = Color.FromArgb(90, 210, 0);
        public ColorNode OwlPrimal { get; set; } = Color.FromArgb(90, 210, 0);
    }
}

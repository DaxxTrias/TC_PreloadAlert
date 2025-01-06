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
            //Masters = new ToggleNode(true);
            //Exiles = new ToggleNode(true);
            //PerandusBoxes = new ToggleNode(true);
            //CorruptedArea = new ToggleNode(true);
            //CorruptedTitle = new ToggleNode(true);
            //Bestiary = new ToggleNode(true);
            //TextSize = new RangeNode<int>(16, 10, 50);
            //BackgroundColor = new ColorBGRA(0, 0, 0, 255);
            //DefaultTextColor = new ColorBGRA(210, 210, 210, 255);
            //AreaTextColor = new ColorBGRA(150, 200, 250, 255);
            //CorruptedAreaColor = new ColorBGRA(208, 31, 144, 255);

            //CadiroTrader = new ColorBGRA(255, 128, 0, 255);
            //PerandusChestStandard = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestRarity = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestQuantity = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestCoins = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestJewellery = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestGems = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestCurrency = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestInventory = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestDivinationCards = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestKeepersOfTheTrove = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestUniqueItem = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestMaps = new ColorBGRA(153, 255, 51, 255);
            //PerandusChestFishing = new ColorBGRA(153, 255, 51, 255);
            //PerandusManorUniqueChest = new ColorBGRA(153, 255, 51, 255);
            //PerandusManorCurrencyChest = new ColorBGRA(153, 255, 51, 255);
            //PerandusManorMapsChest = new ColorBGRA(153, 255, 51, 255);
            //PerandusManorJewelryChest = new ColorBGRA(153, 255, 51, 255);
            //PerandusManorDivinationCardsChest = new ColorBGRA(153, 255, 51, 255);
            //PerandusManorLostTreasureChest = new ColorBGRA(153, 255, 51, 255);

            //MasterZana = new ColorBGRA(255, 2550, 0, 255);
            //MasterCatarina = new ColorBGRA(100, 255, 255, 255);
            //MasterTora = new ColorBGRA(100, 255, 255, 255);
            //MasterVorici = new ColorBGRA(100, 255, 255, 255);
            //MasterHaku = new ColorBGRA(100, 255, 255, 255);
            //MasterElreon = new ColorBGRA(100, 255, 255, 255);
            //MasterVagan = new ColorBGRA(100, 255, 255, 255);
            //MasterKrillson = new ColorBGRA(255, 0, 255, 255);

            //ArcanistStrongbox = new ColorBGRA(255, 0, 255, 255);
            //ArtisanStrongbox = new ColorBGRA(210, 210, 210, 255);
            //CartographerStrongbox = new ColorBGRA(255, 255, 0, 255);
            //DivinerStrongbox = new ColorBGRA(255, 0, 255, 255);
            //GemcutterStrongbox = new ColorBGRA(27, 162, 155, 255);
            //JewellerStrongbox = new ColorBGRA(210, 210, 210, 255);
            //BlacksmithStrongbox = new ColorBGRA(210, 210, 210, 255);
            //ArmourerStrongbox = new ColorBGRA(210, 210, 210, 255);
            //OrnateStrongbox = new ColorBGRA(210, 210, 210, 255);
            //LargeStrongbox = new ColorBGRA(210, 210, 210, 255);
            //PerandusStrongbox = new ColorBGRA(175, 96, 37, 255);
            //KaomStrongbox = new ColorBGRA(175, 96, 37, 255);
            //MalachaiStrongbox = new ColorBGRA(175, 96, 37, 255);
            //EpicStrongbox = new ColorBGRA(175, 96, 37, 255);
            //SimpleStrongbox = new ColorBGRA(210, 210, 210, 255);

            //OrraGreengate = new ColorBGRA(254, 192, 118, 255);
            //ThenaMoga = new ColorBGRA(254, 192, 118, 255);
            //AntalieNapora = new ColorBGRA(254, 192, 118, 255);
            //TorrOlgosso = new ColorBGRA(254, 192, 118, 255);
            //ArmiosBell = new ColorBGRA(254, 192, 118, 255);
            //ZacharieDesmarais = new ColorBGRA(254, 192, 118, 255);
            //MinaraAnenima = new ColorBGRA(254, 192, 118, 255);
            //IgnaPhoenix = new ColorBGRA(254, 192, 118, 255);
            //JonahUnchained = new ColorBGRA(254, 192, 118, 255);
            //DamoiTui = new ColorBGRA(254, 192, 118, 255);
            //XandroBlooddrinker = new ColorBGRA(254, 192, 118, 255);
            //VickasGiantbone = new ColorBGRA(254, 192, 118, 255);
            //EoinGreyfur = new ColorBGRA(254, 192, 118, 255);
            //TinevinHighdove = new ColorBGRA(254, 192, 118, 255);
            //MagnusStonethorn = new ColorBGRA(254, 192, 118, 255);
            //IonDarkshroud = new ColorBGRA(254, 192, 118, 255);
            //AshLessard = new ColorBGRA(254, 192, 118, 255);
            //WilorinDemontamer = new ColorBGRA(254, 192, 118, 255);
            //AugustinaSolaria = new ColorBGRA(254, 192, 118, 255);
            //DenaLorenni = new ColorBGRA(254, 192, 118, 255);
            //VanthAgiel = new ColorBGRA(254, 192, 118, 255);
            //LaelFuria = new ColorBGRA(254, 192, 118, 255);
            //OyraOna = new ColorBGRA(254, 192, 118, 255);
            //BoltBrownfur = new ColorBGRA(254, 192, 118, 255);
            //AilentiaRac = new ColorBGRA(254, 192, 118, 255);
            //UlyssesMorvant = new ColorBGRA(254, 192, 118, 255);
            //AurelioVoidsinger = new ColorBGRA(254, 192, 118, 255);
        }

        public ToggleNode Enable { get; set; }
        [Menu("Reparse Delay", "How many milliseconds between reparsing events")]
        public RangeNode<int> ReparseDelay { get; set; } = new(60000, 100, 180000);
        public ColorNode BackgroundColor { get; set; } = Color.FromArgb(0, 0, 0);
        public ColorNode DefaultTextColor { get; set; } = Color.FromArgb(210, 210, 210);
        public ColorNode AreaTextColor { get; set; } = Color.FromArgb(150, 200, 250);
        [Menu("Display Position", "X and Y coordinates to draw on screen")]
        public RangeNode<Vector2> DisplayPosition { get; set; } = new(new Vector2(1040, 0), Vector2.Zero, Vector2.One * 4000);
        [Menu("Show Essences", "Enable or Disable the showing of Essence(s)")]

        public ToggleNode Shrines { get; set; }
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfSmoke { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfRegeneration { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfResistance { get; set; } = Color.FromArgb(255, 255, 255);
        [ConditionalDisplay(nameof(Shrines), true)]
        public ColorNode ShrineOfAcceleration { get; set; } = Color.FromArgb(255, 255, 255);

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

        //public ColorNode PerandusChestStandard { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestRarity { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestQuantity { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestCoins { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestJewellery { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestGems { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestCurrency { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestInventory { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestDivinationCards { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestKeepersOfTheTrove { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestUniqueItem { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestMaps { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusChestFishing { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusManorUniqueChest { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusManorCurrencyChest { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusManorMapsChest { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusManorJewelryChest { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusManorDivinationCardsChest { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ColorNode PerandusManorLostTreasureChest { get; set; } = System.Drawing.Color.FromArgb(255, 255, 0);
        //public ToggleNode PerandusBoxes { get; set; }
        //public ToggleNode CorruptedArea { get; set; }
        //public ToggleNode CorruptedTitle { get; set; }
        //public ToggleNode Masters { get; set; }
        //public ToggleNode Exiles { get; set; }
        //public ColorNode CorruptedAreaColor { get; set; } = System.Drawing.Color.FromArgb(208, 31, 144);
        //public ColorNode MasterZana { get; set; }
        //public ColorNode MasterCatarina { get; set; }
        //public ColorNode MasterTora { get; set; }
        //public ColorNode MasterVorici { get; set; }
        //public ColorNode MasterHaku { get; set; }
        //public ColorNode MasterElreon { get; set; }
        //public ColorNode MasterVagan { get; set; }
        //public ColorNode MasterKrillson { get; set; }
        //public ColorNode OrraGreengate { get; set; }
        //public ColorNode ThenaMoga { get; set; }
        //public ColorNode AntalieNapora { get; set; }
        //public ColorNode TorrOlgosso { get; set; }
        //public ColorNode ArmiosBell { get; set; }
        //public ColorNode ZacharieDesmarais { get; set; }
        //public ColorNode MinaraAnenima { get; set; }
        //public ColorNode IgnaPhoenix { get; set; }
        //public ColorNode JonahUnchained { get; set; }
        //public ColorNode DamoiTui { get; set; }
        //public ColorNode XandroBlooddrinker { get; set; }
        //public ColorNode VickasGiantbone { get; set; }
        //public ColorNode EoinGreyfur { get; set; }
        //public ColorNode TinevinHighdove { get; set; }
        //public ColorNode MagnusStonethorn { get; set; }
        //public ColorNode IonDarkshroud { get; set; }
        //public ColorNode AshLessard { get; set; }
        //public ColorNode WilorinDemontamer { get; set; }
        //public ColorNode AugustinaSolaria { get; set; }
        //public ColorNode DenaLorenni { get; set; }
        //public ColorNode VanthAgiel { get; set; }
        //public ColorNode LaelFuria { get; set; }
        //public ColorNode OyraOna { get; set; }
        //public ColorNode BoltBrownfur { get; set; }
        //public ColorNode AilentiaRac { get; set; }
        //public ColorNode UlyssesMorvant { get; set; }
        //public ColorNode AurelioVoidsinger { get; set; }
        //public ToggleNode Bestiary { get; set; }
    }
}

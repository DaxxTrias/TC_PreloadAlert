using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExileCore2;
using ExileCore2.PoEMemory;
using ExileCore2.Shared;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using ImGuiNET;
using Newtonsoft.Json;
using Vector2 = System.Numerics.Vector2;
using RectangleF = ExileCore2.Shared.RectangleF;

namespace PreloadAlert
{
    public class PreloadAlert : BaseSettingsPlugin<PreloadAlertSettings>
    {
        private const string PRELOAD_ALERTS = "config/preload_alerts.txt";
        private const string PRELOAD_ALERTS_PERSONAL = "config/preload_alerts_personal.txt";
        private const string PreloadStart = "preload-start.png";
        private const string PreloadEnd = "preload-end.png";
        private const string PreloadNew = "preload-new.png";
        public static Dictionary<string, PreloadConfigLine> Essences;
        public static Dictionary<string, PreloadConfigLine> Shrines;
        public static Dictionary<string, PreloadConfigLine> Strongboxes;
        public static Dictionary<string, PreloadConfigLine> Preload;
        //public static Dictionary<string, PreloadConfigLine> PerandusLeague;
        //public static Dictionary<string, PreloadConfigLine> Bestiary;
        public static Color AreaNameColor;
        private readonly object _locker = new object();
        private Dictionary<string, PreloadConfigLine> alertStrings;
        private bool canRender;
        private DebugInformation debugInformation;
        private List<PreloadConfigLine> DrawAlerts = new List<PreloadConfigLine>();
        private bool essencefound;
        private bool shrinefound;
        private readonly List<long> filesPtr = new List<long>();
        private bool foundSpecificPerandusChest;
        private bool holdKey = false;
        private bool isAreaChanged = false;
        private bool isLoading;
        private Vector2 lastLine;
        private Dictionary<string, PreloadConfigLine> personalAlertStrings;
        private readonly List<string> PreloadDebug = new List<string>();
        private Action PreloadDebugAction;
        private bool working;
        private CancellationTokenSource cancellationTokenSource;

        public PreloadAlert()
        {
            Order = -40;
        }

        private Dictionary<string, PreloadConfigLine> alerts { get; } = new Dictionary<string, PreloadConfigLine>();
        private Action<string, Color> AddPreload => ExternalPreloads;

        //Need more test because different result with old method. Most of diff its Art/ and others but sometimes see Metadata/parti...Probably async loads
        //private void ParseByFiles(Dictionary<string, FileInformation> dictionary)
        //{
        //    if (working) return;
        //    working = true;

        //    Task.Run(() =>
        //    {
        //        debugInformation.TickAction(() =>
        //        {
        //            if (Settings.ParallelParsing)
        //            {
        //                Parallel.ForEach(dictionary, pair =>
        //                {
        //                    var text = pair.Key;
        //                    if (Settings.LoadOnlyMetadata && text[0] != 'M') return;
        //                    if (text.Contains("@")) text = text.Split('@')[0];
        //                    CheckForPreload(text);
        //                });
        //            }
        //            else
        //            {
        //                foreach (var pair in dictionary)
        //                {
        //                    var text = pair.Key;
        //                    if (Settings.LoadOnlyMetadata && text[0] != 'M') continue;
        //                    if (text.Contains("@")) text = text.Split('@')[0];
        //                    CheckForPreload(text);
        //                }
        //            }

        //            lock (_locker)
        //            {
        //                DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
        //            }
        //        });

        //        working = false;
        //    });
        //}

        public override void DrawSettings()
        {
            if (ImGui.Button("Dump preloads"))
            {
                Directory.CreateDirectory(Path.Combine(DirectoryFullName, "Dumps"));
                var path = Path.Combine(DirectoryFullName, "Dumps",
                    $"{GameController.Area.CurrentArea.Name}.txt");

				DebugWindow.LogMsg(path);

				File.WriteAllLines(path, PreloadDebug);
            }

            if (ImGui.Button("Dump grouped preloads"))
            {
                var groupBy = PreloadDebug.OrderBy(x => x).GroupBy(x => x.IndexOf('/'));
                var serializeObject = JsonConvert.SerializeObject(groupBy, Formatting.Indented);

                // Replace invalid characters in the file name
                var areaName = string.Join("_", GameController.Area.CurrentArea.Name.Split(Path.GetInvalidFileNameChars()));
                var path = Path.Combine(DirectoryFullName, "Dumps",
                    $"{areaName} ({DateTime.Now:yyyy-MM-dd_HH-mm-ss}).txt");

                DebugWindow.LogMsg($"Dumped Preloads to: {path}");
                //DebugWindow.LogMsg($"GroupBy Count: {groupBy.Count()}");
                //DebugWindow.LogMsg($"Serialized Object: {serializeObject}");

                File.WriteAllText(path, serializeObject);
            }

            if (ImGui.Button("Show all preloads"))
            {
                var groupBy = PreloadDebug.OrderBy(x => x).GroupBy(x => x.IndexOf('/')).ToList();
                var result = new Dictionary<string, List<string>>(groupBy.Count);

                foreach (var gr in groupBy)
                {
                    var g = gr.ToList();

                    if (gr.Key != -1)
                    {
                        var list = new List<string>(g.Count);
                        result[g.First().Substring(0, gr.Key)] = list;

                        foreach (var str in g)
                        {
                            list.Add(str);
                        }
                    }
                    else
                    {
                        var list = new List<string>(g.Count);
                        var key = gr.Key.ToString();
                        result[key] = list;

                        foreach (var str in g)
                        {
                            list.Add(str);
                        }
                    }
                }

                groupBy = null;

                PreloadDebugAction = () =>
                {
                    foreach (var res in result)
                    {
                        if (ImGui.TreeNode(res.Key))
                        {
                            foreach (var str in res.Value)
                            {
                                ImGui.Text(str);
                            }

                            ImGui.TreePop();
                        }
                    }

                    ImGui.Separator();

                    if (alerts.Count > 0)
                    {
                        if (ImGui.TreeNode("DrawAlerts"))
                        {
                            foreach (var alert in DrawAlerts)
                            {
                                ImGui.TextColored((alert.FastColor?.Invoke() ?? alert.Color ?? Settings.DefaultTextColor).ToImguiVec4(),
                                    $"{alert.Text}");
                            }

                            ImGui.TreePop();
                        }
                    }
					ImGui.Text($"Area Change Count: {GameController.Game.AreaChangeCount}");

					if (ImGui.Button("Close")) PreloadDebugAction = null;
                };
            }

            base.DrawSettings();
        }

        private void ExternalPreloads(string text, Color color)
        {
            if (working)
            {
                Task.Run(async () =>
                {
                    var tries = 0;

                    while (working && tries < 20)
                    {
                        await Task.Delay(200);
                        tries++;
                    }

                    if (!working && tries < 20)
                    {
                        alerts.Add(text, new PreloadConfigLine {Text = text, FastColor = () => color});

                        lock (_locker)
                        {
                            DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
                        }
                    }
                });
            }
            else
            {
                alerts.Add(text, new PreloadConfigLine {Text = text, FastColor = () => color});

                lock (_locker)
                {
                    DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
                }
            }
        }

        public override void OnLoad()
        {
            alertStrings = LoadConfig("config/preload_alerts.txt");
            SetupPredefinedConfigs();
            try
            {
                string preloadStartPath = Path.Combine(DirectoryFullName, PreloadStart);
                string preloadEndPath = Path.Combine(DirectoryFullName, PreloadEnd);
                string preloadNewPath = Path.Combine(DirectoryFullName, PreloadNew);

                Graphics.InitImage(PreloadStart, preloadStartPath);
                Graphics.InitImage(PreloadEnd, preloadEndPath);
                Graphics.InitImage(PreloadNew, preloadNewPath);

                DebugWindow.LogMsg($"Loaded images: {preloadStartPath}, {preloadEndPath}, {preloadNewPath}");
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Error initializing images: {ex.Message}");
            }

            if (File.Exists(PRELOAD_ALERTS_PERSONAL))
                alertStrings = alertStrings.MergeLeft(LoadConfig(PRELOAD_ALERTS_PERSONAL));
            else
                File.Create(PRELOAD_ALERTS_PERSONAL);
        }

        public override bool Initialise()
        {
            GameController.PluginBridge.SaveMethod($"{nameof(PreloadAlert)}.{nameof(AddPreload)}", AddPreload);
            AreaNameColor = Settings.AreaTextColor;
            debugInformation = new DebugInformation("Preload alert parsing", false);
            /*GameController.Files.LoadedFiles += (sender, dictionary) =>
            {
                ParseByFiles(dictionary);
            };*/

            GameController.LeftPanel.WantUse(() => Settings.Enable);
            AreaChange(GameController.Area.CurrentArea);
            return true;
        }

        public override void AreaChange(AreaInstance area)
        {
            isLoading = true;
            alerts.Clear();

            lock (_locker)
            {
                DrawAlerts.Clear();
            }
            PreloadDebugAction = null;
            if (GameController.Area.CurrentArea.IsHideout || GameController.Area.CurrentArea.IsTown)
            {
                isLoading = false;
                return;
            }
            Parse();
            StartPeriodicCheck();

            isLoading = false;
        }

        private void Parse()
        {
            if (GameController.Area.CurrentArea.IsHideout || GameController.Area.CurrentArea.IsTown)
                return;

            if (!working)
            {
                working = true;
                PreloadDebug.Clear();

                Task.Run(() =>
                {
                    debugInformation.TickAction(() =>
                    {
                        try
                        {
                            var memory = GameController.Memory;
                            FilesFromMemory filesFromMemory = new FilesFromMemory(memory);
                            var AllFiles = filesFromMemory.GetAllFiles();
                            if (AllFiles == null)
                                return;

                            int areaChangeCount = GameController.Game.AreaChangeCount;
                            foreach (var file in AllFiles)
                            {
                                if (file.Value.ChangeCount == areaChangeCount)
                                {
                                    var text = file.Key;
                                    if (text.Contains('@')) text = text.Split('@')[0];

                                    lock (_locker)
                                    {
                                        PreloadDebug.Add(text);
                                    }

                                    CheckForPreload(text);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            DebugWindow.LogError($"{nameof(PreloadAlert)} -> {e}");
                        }

                        lock (_locker)
                        {
                            DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
                        }
                    });

                    working = false;
                });
            }
        }

        private void StartPeriodicCheck()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            if (!Settings.ReparsePreloads)
                return;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (GameController.Area.CurrentArea.IsTown || GameController.Area.CurrentArea.IsHideout)
                        continue;

                    await Task.Delay(Settings.ReparseDelay.Value * 1000, token);
                    if (!token.IsCancellationRequested)
                    {
                        Parse();
                    }
                }
            }, token);
        }

        public override void Tick()
        {
            canRender = true;

            if (!Settings.Enable || GameController.Area.CurrentArea != null && GameController.Area.CurrentArea.IsTown ||
                GameController.IsLoading || !GameController.InGame)
            {
                canRender = false;
                return;
            }

            if (GameController.Game.IngameState.IngameUi.StashElement.IsVisibleLocal)
            {
                canRender = false;
                return;
            }

            var UIHover = GameController.Game.IngameState.UIHover;
            var miniMap = GameController.Game.IngameState.IngameUi.Map.SmallMiniMap;

            if (Settings.Enable.Value && UIHover?.Tooltip != null && UIHover.IsValid && UIHover.Address != 0x00 &&
                UIHover.Tooltip.Address != 0x00 && UIHover.Tooltip.IsVisibleLocal &&
                UIHover.Tooltip.GetClientRectCache.Intersects(miniMap.GetClientRectCache))
            {
                canRender = false;
                return;
            }

            if (UIHover?.Tooltip != null && (!UIHover.IsValid || UIHover.Address == 0x00 || UIHover.Tooltip.Address == 0x00 ||
                                             !UIHover.Tooltip.IsVisibleLocal))
                canRender = true;

            if (Input.GetKeyState(Keys.F5)) AreaChange(GameController.Area.CurrentArea);

            return;
        }

        public override void Render()
        {
            const string loadingText = "Loading...";
            PreloadDebugAction?.Invoke();
            if (!canRender) 
                return;

            var drawPoint = Settings.DisplayPosition.Value;

            var textSize = isLoading
                ? Graphics.MeasureText(loadingText)
                : DrawAlerts.Select(x => Graphics.MeasureText(x.Text)).ToList()
                    switch { var s => new Vector2(s.DefaultIfEmpty(Vector2.Zero).Max(x => x.X), s.Sum(x => x.Y)) };
            var bounds = new RectangleF(drawPoint.X - textSize.X - 55,
                drawPoint.Y, textSize.X + 60, textSize.Y);

            try
            {
                Graphics.DrawImage("preload-new.png", bounds, Settings.BackgroundColor);
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Error drawing image {PreloadNew}: {ex.Message}");
            }

            if (isLoading)
            {
                lastLine = Graphics.DrawText(loadingText, drawPoint, Color.Orange, FontAlign.Right);
                drawPoint.Y += lastLine.Y;
            }
            else
            {
                foreach (var line in DrawAlerts)
                {
                    lastLine = Graphics.DrawText(line.Text, drawPoint,
                        line.FastColor?.Invoke() ?? line.Color ?? Settings.DefaultTextColor, FontAlign.Right);

                    drawPoint.Y += lastLine.Y;
                }
            }

            GameController.LeftPanel.StartDrawPoint = drawPoint;
        }

        public Dictionary<string, PreloadConfigLine> LoadConfig(string path)
        {
            return LoadConfigBase(path, 3).ToDictionary(line => line[0], line =>
            {
                var preloadAlerConfigLine = new PreloadConfigLine {Text = line[1], Color = line.ConfigColorValueExtractor(2)};
                return preloadAlerConfigLine;
            });
        }

        protected static IEnumerable<string[]> LoadConfigBase(string path, int columnsCount = 2)
        {
            return File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line) && line.IndexOf(';') >= 0 && !line.StartsWith("#"))
                .Select(line => line.Split(new[] {';'}, columnsCount).Select(parts => parts.Trim()).ToArray());
        }

        private void SetupPredefinedConfigs()
        {
            Essences = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModLightning1",
                    new PreloadConfigLine {Text = "Lightning Essence", FastColor = () => Settings.EssenceOfLightning}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModLightning2",
                    new PreloadConfigLine {Text = "Greater Lightning Essence", FastColor = () => Settings.GreaterEssenceOfLightning}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModSpeed1",
                    new PreloadConfigLine {Text = "Speed Essence", FastColor = () => Settings.EssenceOfSpeed}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModSpeed2e",
                    new PreloadConfigLine {Text = "Greater Speed Essence", FastColor = () => Settings.GreaterEssenceOfSpeed}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModPhysical1",
                    new PreloadConfigLine {Text = "Physical Essence", FastColor = () => Settings.EssenceOfPhysical}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModPhysical2",
                    new PreloadConfigLine {Text = "Greater Physical Essence", FastColor = () => Settings.GreaterEssenceOfPhysical}
                },
                {
                    // Essence of battle?
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModAttack1",
                    new PreloadConfigLine {Text = "Attack Essence", FastColor = () => Settings.EssenceOfAttack}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModAttack2",
                    new PreloadConfigLine {Text = "Greater Attack Essence", FastColor = () => Settings.GreaterEssenceOfAttack}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModLife1",
                    new PreloadConfigLine {Text = "Life Essence", FastColor = () => Settings.EssenceOfLife}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModLife2",
                    new PreloadConfigLine {Text = "Greater Life Essence", FastColor = () => Settings.GreaterEssenceOfLife}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModChaos1",
                    new PreloadConfigLine {Text = "Chaos Essence", FastColor = () => Settings.EssenceOfChaos}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModChaos2",
                    new PreloadConfigLine {Text = "Greater Chaos Essence", FastColor = () => Settings.GreaterEssenceOfChaos}
                },
                {
                    // Essence of battle?
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModCaster1",
                    new PreloadConfigLine {Text = "Caster Essence", FastColor = () => Settings.EssenceOfCasting}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModCaster2",
                    new PreloadConfigLine {Text = "Greater Caster Essence", FastColor = () => Settings.GreaterEssenceOfCasting}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModCold1",
                    new PreloadConfigLine {Text = "Cold Essence", FastColor = () => Settings.EssenceOfCold}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModCold2",
                    new PreloadConfigLine {Text = "Greater Cold Essence", FastColor = () => Settings.GreaterEssenceOfCold}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModMana1",
                    new PreloadConfigLine {Text = "Mana Essence", FastColor = () => Settings.EssenceOfMana}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModMana2",
                    new PreloadConfigLine {Text = "Greater Mana Essence", FastColor = () => Settings.GreaterEssenceOfMana}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModDefences1",
                    new PreloadConfigLine {Text = "Defence Essence", FastColor = () => Settings.EssenceOfDefence}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModDefences2",
                    new PreloadConfigLine {Text = "Greater Defence Essence", FastColor = () => Settings.GreaterEssenceOfDefence}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModAttribute1",
                    new PreloadConfigLine {Text = "Attributes Essence", FastColor = () => Settings.EssenceOfAttributes}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModAttribute2",
                    new PreloadConfigLine {Text = "Greater Attributes Essence", FastColor = () => Settings.GreaterEssenceOfAttributes}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModFire1",
                    new PreloadConfigLine {Text = "Fire Essence", FastColor = () => Settings.EssenceOfFire}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModFire2",
                    new PreloadConfigLine {Text = "Greater Fire Essence", FastColor = () => Settings.GreaterEssenceOfFire}
                },
            };

            Shrines = new Dictionary<string, PreloadConfigLine>
            {
                // Jan-6-2025: shrine assets loaded with .epk are confirmed loaded in, but are not always in the preload.
                // Certain shrines (fire/cold/light) spawn monsters and these can be detected in the preload.
                // in the absence of the above two scenarios, a generic shrine asset is always preloaded (Metadata/Shrines/Shrine)
                {
                    "Metadata/Effects/Environment/shrine/plus/plus.epk",
                    new PreloadConfigLine {Text = "Regeneration Shrine", FastColor = () => Settings.ShrineOfRegeneration}
                },
                {
                    "Metadata/Effects/Environment/shrine/resistance/resist.epk",
                    new PreloadConfigLine {Text = "Resistance Shrine", FastColor = () => Settings.ShrineOfResistance}
                },
                {
                    "Metadata/Effects/Environment/shrine/smoke/smoke.epk", // avarice
                    new PreloadConfigLine {Text = "Greed Shrine", FastColor = () => Settings.ShrineOfGreed}
                },
                {
                    "Metadata/Effects/Environment/shrine/run_fast/runfast.epk", // acceleration
                    new PreloadConfigLine {Text = "Acceleration Shrine", FastColor = () => Settings.ShrineOfAcceleration}
                },
                {
                    "Metadata/Effects/Environment/shrine/summon/summon.epk", // chaos damage
                    new PreloadConfigLine {Text = "Gloom Shrine", FastColor = () => Settings.ShrineOfGloom}
                },
                {
                    "Metadata/Effects/Environment/shrine/sword/sword.epk", // critical
                    new PreloadConfigLine {Text = "Diamond Shrine", FastColor = () => Settings.ShrineOfCrit}
                },
                {
                    "Metadata/Effects/Environment/shrine/curse/rig.epk", // corrupted shrine
                    new PreloadConfigLine {Text = "Tainted Shrine", FastColor = () => Settings.ShrineOfCorruption}
                },
                {
                    //"Metadata/Effects/Environment/shrine/fire/fire.epk", // alternate data. not a preload; is a confirmation
                    "Metadata/Monsters/Daemon/Shrines/ShrineFireDaemon_", // meteoric shrine shrine
                    new PreloadConfigLine {Text = "Meteoric Shrine", FastColor = () => Settings.ShrineOfFire}
                },
                {
                    "Metadata/Effects/Environment/shrine/massive/massive.epk", // meteoric shrine shrine
                    new PreloadConfigLine {Text = "Enduring Shrine", FastColor = () => Settings.ShrineOfEnduring}
                },
                {
                    //"Metadata/Effects/Environment/shrine/lightning/lightning.epk", // alternate data. not a preload; is a confirmation
                    "Metadata/Monsters/Daemon/Shrines/ShrineLightningDaemon", // lightning  tempest shrine
                    new PreloadConfigLine {Text = "Lightning Shrine", FastColor = () => Settings.ShrineOfLightning}
                },
                {
                    //"Metadata/Effects/Environment/shrine/ice/ice.epk", // alternate data. not a preload; is a confirmation
                    "Metadata/Monsters/Daemon/Shrines/ShrineColdDaemon", // cold shrine
                    new PreloadConfigLine {Text = "Cold Shrine", FastColor = () => Settings.ShrineOfCold}
                },
                {
                    "Metadata/Effects/Environment/shrine/god_mode/godmode.epk",
                    new PreloadConfigLine {Text = "Divine Shrine", FastColor = () => Settings.ShrineOfDivine}
                },

            };

            #region perandus
            //PerandusLeague = new Dictionary<string, PreloadConfigLine>
            //{
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestStandard",
            //        new PreloadConfigLine {Text = "Perandus Chest", FastColor = () => Settings.PerandusChestStandard}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestRarity",
            //        new PreloadConfigLine {Text = "Perandus Cache", FastColor = () => Settings.PerandusChestRarity}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestQuantity",
            //        new PreloadConfigLine {Text = "Perandus Hoard", FastColor = () => Settings.PerandusChestQuantity}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestCoins",
            //        new PreloadConfigLine {Text = "Perandus Coffer", FastColor = () => Settings.PerandusChestCoins}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestJewellery",
            //        new PreloadConfigLine {Text = "Perandus Jewellery Box", FastColor = () => Settings.PerandusChestJewellery}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestGems",
            //        new PreloadConfigLine {Text = "Perandus Safe", FastColor = () => Settings.PerandusChestGems}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestCurrency",
            //        new PreloadConfigLine {Text = "Perandus Treasury", FastColor = () => Settings.PerandusChestCurrency}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestInventory",
            //        new PreloadConfigLine {Text = "Perandus Wardrobe", FastColor = () => Settings.PerandusChestInventory}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestDivinationCards",
            //        new PreloadConfigLine {Text = "Perandus Catalogue", FastColor = () => Settings.PerandusChestDivinationCards}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestKeepersOfTheTrove",
            //        new PreloadConfigLine {Text = "Perandus Trove", FastColor = () => Settings.PerandusChestKeepersOfTheTrove}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestUniqueItem",
            //        new PreloadConfigLine {Text = "Perandus Locker", FastColor = () => Settings.PerandusChestUniqueItem}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestMaps",
            //        new PreloadConfigLine {Text = "Perandus Archive", FastColor = () => Settings.PerandusChestMaps}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusChestFishing",
            //        new PreloadConfigLine {Text = "Perandus Tackle Box", FastColor = () => Settings.PerandusChestFishing}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusManorUniqueChest",
            //        new PreloadConfigLine {Text = "Cadiro's Locker", FastColor = () => Settings.PerandusManorUniqueChest}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusManorCurrencyChest",
            //        new PreloadConfigLine {Text = "Cadiro's Treasury", FastColor = () => Settings.PerandusManorCurrencyChest}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusManorMapsChest",
            //        new PreloadConfigLine {Text = "Cadiro's Archive", FastColor = () => Settings.PerandusManorMapsChest}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusManorJewelryChest",
            //        new PreloadConfigLine {Text = "Cadiro's Jewellery Box", FastColor = () => Settings.PerandusManorJewelryChest}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusManorDivinationCardsChest",
            //        new PreloadConfigLine {Text = "Cadiro's Catalogue", FastColor = () => Settings.PerandusManorDivinationCardsChest}
            //    },
            //    {
            //        "Metadata/Chests/PerandusChests/PerandusManorLostTreasureChest",
            //        new PreloadConfigLine {Text = "Grand Perandus Vault", FastColor = () => Settings.PerandusManorLostTreasureChest}
            //    }
            //};
            #endregion

            Strongboxes = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "Metadata/Chests/StrongBoxes/Arcanist", // this might have been replaced by researcher's strongbox
                    new PreloadConfigLine {Text = "Arcanist's Strongbox", FastColor = () => Settings.ArcanistStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Artisan",
                    new PreloadConfigLine {Text = "Artisan's Strongbox", FastColor = () => Settings.ArtisanStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Cartographer", // probably doesnt exist now since theyre waystones?
                    new PreloadConfigLine {Text = "Cartographer's Strongbox", FastColor = () => Settings.CartographerStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Diviner",
                    new PreloadConfigLine {Text = "Diviner's Strongbox", FastColor = () => Settings.DivinerStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/StrongboxDivination",
                    new PreloadConfigLine {Text = "Diviner's Strongbox", FastColor = () => Settings.DivinerStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Gemcutter",
                    new PreloadConfigLine {Text = "Gemcutter's Strongbox", FastColor = () => Settings.GemcutterStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Jeweller",
                    new PreloadConfigLine {Text = "Jeweller's Strongbox", FastColor = () => Settings.JewellerStrongbox}
                },
                {
                    //"Metadata/Chests/StrongBoxes/Arsenal", poe1
                    "Metadata/Chests/StrongBoxes/MartialStrongbox",
                    new PreloadConfigLine {Text = "Blacksmith's Strongbox", FastColor = () => Settings.BlacksmithStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/CasterStrongbox",
                    new PreloadConfigLine {Text = "Arcane Strongbox", FastColor = () => Settings.ArcaneStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/ResearchStrongbox", // maybe replaced arcanist?
                    new PreloadConfigLine {Text = "Researcher's Strongbox", FastColor = () => Settings.ResearchStrongbox}
                },
                {
                    //"Metadata/Chests/StrongBoxes/Armory", poe1
                    "Metadata/Chests/StrongBoxes/ArmourerStrongbox",
                    new PreloadConfigLine {Text = "Armourer's Strongbox", FastColor = () => Settings.ArmourerStrongbox}
                },
                {
                    //"Metadata/Chests/StrongBoxes/Ornate", poe1
                    "Metadata/Chests/StrongBoxes/OrnateStrongbox",
                    new PreloadConfigLine {Text = "Ornate Strongbox", FastColor = () => Settings.OrnateStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Large",
                    new PreloadConfigLine {Text = "Large Strongbox", FastColor = () => Settings.LargeStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/BasicStrongbox", // simple from poe1 replaced by basic in poe2
                    new PreloadConfigLine {Text = "Basic Strongbox", FastColor = () => Settings.BasicStrongbox}
                },
                {
                    "Metadata/Chests/CopperChests/CopperChestEpic3",
                    new PreloadConfigLine {Text = "Epic Chest", FastColor = () => Settings.EpicStrongbox}
                },
                //{
                //    "Metadata/Chests/StrongBoxes/PerandusBox",
                //    new PreloadConfigLine {Text = "Perandus Strongbox", FastColor = () => Settings.PerandusStrongbox}
                //},
                //{
                //    "Metadata/Chests/StrongBoxes/KaomBox",
                //    new PreloadConfigLine {Text = "Kaom Strongbox", FastColor = () => Settings.KaomStrongbox}
                //},
                //{
                //    "Metadata/Chests/StrongBoxes/MalachaisBox",
                //    new PreloadConfigLine {Text = "Malachai Strongbox", FastColor = () => Settings.MalachaiStrongbox}
                //}
            };

            #region masters
            //Preload = new Dictionary<string, PreloadConfigLine>
            //{
            //    {"Wild/StrDexInt", new PreloadConfigLine {Text = "Zana, Master Cartographer", FastColor = () => Settings.MasterZana}},
            //    {"Wild/Int", new PreloadConfigLine {Text = "Catarina, Master of the Dead", FastColor = () => Settings.MasterCatarina}},
            //    {"Wild/Dex", new PreloadConfigLine {Text = "Tora, Master of the Hunt", FastColor = () => Settings.MasterTora}},
            //    {"Wild/DexInt", new PreloadConfigLine {Text = "Vorici, Master Assassin", FastColor = () => Settings.MasterVorici}},
            //    {"Wild/Str", new PreloadConfigLine {Text = "Haku, Armourmaster", FastColor = () => Settings.MasterHaku}},
            //    {"Wild/StrInt", new PreloadConfigLine {Text = "Elreon, Loremaster", FastColor = () => Settings.MasterElreon}},
            //    {"Wild/Fish", new PreloadConfigLine {Text = "Krillson, Master Fisherman", FastColor = () => Settings.MasterKrillson}},
            //    {
            //        "MasterStrDex1",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (2HSword)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {"MasterStrDex2", new PreloadConfigLine {Text = "Vagan, Weaponmaster (Staff)", FastColor = () => Settings.MasterVagan}},
            //    {"MasterStrDex3", new PreloadConfigLine {Text = "Vagan, Weaponmaster (Bow)", FastColor = () => Settings.MasterVagan}},
            //    {
            //        "MasterStrDex4",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (DaggerRapier)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {"MasterStrDex5", new PreloadConfigLine {Text = "Vagan, Weaponmaster (Blunt)", FastColor = () => Settings.MasterVagan}},
            //    {
            //        "MasterStrDex6",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (Blades)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {
            //        "MasterStrDex7",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (SwordAxe)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {
            //        "MasterStrDex8",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (Punching)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {
            //        "MasterStrDex9",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (Flickerstrike)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {
            //        "MasterStrDex10",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (Elementalist)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {
            //        "MasterStrDex11",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (Cyclone)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {
            //        "MasterStrDex12",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (PhysSpells)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {
            //        "MasterStrDex13",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (Traps)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {
            //        "MasterStrDex14",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (RighteousFire)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {
            //        "MasterStrDex15",
            //        new PreloadConfigLine {Text = "Vagan, Weaponmaster (CastOnHit)", FastColor = () => Settings.MasterVagan}
            //    },
            //    {"ExileDuelist1", new PreloadConfigLine {Text = "Exile Torr Olgosso", FastColor = () => Settings.TorrOlgosso}},
            //    {"ExileDuelist2", new PreloadConfigLine {Text = "Exile Armios Bell", FastColor = () => Settings.ArmiosBell}},
            //    {
            //        "ExileDuelist4",
            //        new PreloadConfigLine {Text = "Exile Zacharie Desmarais", FastColor = () => Settings.ZacharieDesmarais}
            //    },
            //    {"ExileDuelist5", new PreloadConfigLine {Text = "Exile Oyra Ona", FastColor = () => Settings.OyraOna}},
            //    {"ExileMarauder1", new PreloadConfigLine {Text = "Exile Jonah Unchained", FastColor = () => Settings.JonahUnchained}},
            //    {"ExileMarauder2", new PreloadConfigLine {Text = "Exile Damoi Tui", FastColor = () => Settings.DamoiTui}},
            //    {
            //        "ExileMarauder3",
            //        new PreloadConfigLine {Text = "Exile Xandro Blooddrinker", FastColor = () => Settings.XandroBlooddrinker}
            //    },
            //    {"ExileMarauder5", new PreloadConfigLine {Text = "Exile Vickas Giantbone", FastColor = () => Settings.VickasGiantbone}},
            //    {"ExileMarauder6__", new PreloadConfigLine {Text = "Exile Bolt Brownfur", FastColor = () => Settings.BoltBrownfur}},
            //    {"ExileRanger1", new PreloadConfigLine {Text = "Exile Orra Greengate", FastColor = () => Settings.OrraGreengate}},
            //    {"ExileRanger2", new PreloadConfigLine {Text = "Exile Thena Moga", FastColor = () => Settings.ThenaMoga}},
            //    {"ExileRanger3", new PreloadConfigLine {Text = "Exile Antalie Napora", FastColor = () => Settings.AntalieNapora}},
            //    {"ExileRanger5", new PreloadConfigLine {Text = "Exile Ailentia Rac", FastColor = () => Settings.AilentiaRac}},
            //    {"ExileScion2", new PreloadConfigLine {Text = "Exile Augustina Solaria", FastColor = () => Settings.AugustinaSolaria}},
            //    {"ExileScion3", new PreloadConfigLine {Text = "Exile Lael Furia", FastColor = () => Settings.LaelFuria}},
            //    {"ExileScion4", new PreloadConfigLine {Text = "Exile Vanth Agiel", FastColor = () => Settings.VanthAgiel}},
            //    {"ExileShadow1_", new PreloadConfigLine {Text = "Exile Ion Darkshroud", FastColor = () => Settings.IonDarkshroud}},
            //    {"ExileShadow2", new PreloadConfigLine {Text = "Exile Ash Lessard", FastColor = () => Settings.AshLessard}},
            //    {
            //        "ExileShadow4",
            //        new PreloadConfigLine {Text = "Exile Wilorin Demontamer", FastColor = () => Settings.WilorinDemontamer}
            //    },
            //    {"ExileShadow5", new PreloadConfigLine {Text = "Exile Ulysses Morvant", FastColor = () => Settings.UlyssesMorvant}},
            //    {"ExileTemplar1", new PreloadConfigLine {Text = "Exile Eoin Greyfur", FastColor = () => Settings.EoinGreyfur}},
            //    {"ExileTemplar2", new PreloadConfigLine {Text = "Exile Tinevin Highdove", FastColor = () => Settings.TinevinHighdove}},
            //    {
            //        "ExileTemplar4",
            //        new PreloadConfigLine {Text = "Exile Magnus Stonethorn", FastColor = () => Settings.MagnusStonethorn}
            //    },
            //    {
            //        "ExileTemplar5",
            //        new PreloadConfigLine {Text = "Exile Aurelio Voidsinger", FastColor = () => Settings.AurelioVoidsinger}
            //    },
            //    {"ExileWitch1", new PreloadConfigLine {Text = "Exile Minara Anenima", FastColor = () => Settings.MinaraAnenima}},
            //    {"ExileWitch2", new PreloadConfigLine {Text = "Exile Igna Phoenix", FastColor = () => Settings.IgnaPhoenix}},
            //    {"ExileWitch4", new PreloadConfigLine {Text = "Exile Dena Lorenni", FastColor = () => Settings.DenaLorenni}}
            //};

            //Old stuff from bestiary league
            //Bestiary = new Dictionary<string, PreloadConfigLine>();
        }
        #endregion

        private void CheckForPreload(string text)
        {
            if (alertStrings == null || alerts == null || _locker == null)
                return;

            if (alertStrings.ContainsKey(text))
            {
                lock (_locker)
                {
                    alerts[alertStrings[text].Text] = alertStrings[text];
                }

                return;
            }

            #region corrupted area
            //if (text.Contains("Metadata/Terrain/Doodads/vaal_sidearea_effects/soulcoaster.ao"))
            //{
            //    if (Settings.CorruptedTitle)
            //    {
            //        // using corrupted titles so set the color here, XpRatePlugin will grab the color to use when drawing the title.
            //        AreaNameColor = Settings.CorruptedAreaColor;
            //        GameController.Area.CurrentArea.AreaColorName = AreaNameColor;
            //    }
            //    else
            //    {
            //        // not using corrupted titles, so throw it in a preload alert
            //        lock (_locker)
            //        {
            //            alerts[text] = new PreloadConfigLine {Text = "Corrupted Area", FastColor = () => Settings.CorruptedAreaColor};
            //        }
            //    }

            //    return;
            //}
            #endregion

            if (Settings.Essence)
            {
                var essence_alert = Essences.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value)
                    .FirstOrDefault();

                if (essence_alert != null)
                {
                    essencefound = true;

                    if (alerts.ContainsKey("Remnant of Corruption"))

                        //TODO: TEST ESSENCE
                    {
                        lock (_locker)
                        {
                            alerts.Remove("Remnant of Corruption");
                        }
                    }

                    lock (_locker)
                    {
                        alerts[essence_alert.Text] = essence_alert;
                    }

                    return;
                }

                #region remnant of corruption
                //if (!essencefound && text.Contains("MiniMonolith"))
                //{
                //    lock (_locker)
                //    {
                //        alerts["Remnant of Corruption"] = new PreloadConfigLine
                //        {
                //            Text = "Remnant of Corruption",
                //            FastColor = () => Settings.RemnantOfCorruption
                //        };
                //    }
                //}
                #endregion
            }

            if (Settings.Shrines)
            {
                var shrine_alert = Shrines.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value)
                    .FirstOrDefault();

                if (shrine_alert != null)
                {
                    shrinefound = true;

                    lock (_locker)
                    {
                        alerts[shrine_alert.Text] = shrine_alert;
                    }

                    return;
                }
                #region remnant of corruption
                //if (!essencefound && text.Contains("MiniMonolith"))
                //{
                //    lock (_locker)
                //    {
                //        alerts["Remnant of Corruption"] = new PreloadConfigLine
                //        {
                //            Text = "Remnant of Corruption",
                //            FastColor = () => Settings.RemnantOfCorruption
                //        };
                //    }
                //}
                #endregion
            }

            #region perandus
            //var perandus_alert = PerandusLeague.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
            //    .Select(kv => kv.Value).FirstOrDefault();

            //if (perandus_alert != null && Settings.PerandusBoxes)
            //{
            //    foundSpecificPerandusChest = true;

            //    if (alerts.ContainsKey("Unknown Perandus Chest"))
            //    {
            //        lock (_locker)
            //        {
            //            alerts.Remove("Unknown Perandus Chest");
            //        }
            //    }

            //    lock (_locker)
            //    {
            //        alerts.Add(perandus_alert.Text, perandus_alert);
            //    }

            //    return;
            //}

            //if (Settings.PerandusBoxes && !foundSpecificPerandusChest && text.StartsWith("Metadata/Chests/PerandusChests"))
            //{
            //    lock (_locker)
            //    {
            //        alerts["Unknown Perandus Chest"] = new PreloadConfigLine
            //        {
            //            Text = "Unknown Perandus Chest", FastColor = () => Settings.PerandusChestStandard
            //        };
            //    }
            //}
            #endregion perandus

            var _alert = Strongboxes.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value)
                .FirstOrDefault();

            if (_alert != null && Settings.Strongboxes)
            {
                lock (_locker)
                {
                    alerts[_alert.Text] = _alert;
                }

                return;
            }

            #region exile
            //var alert = Preload.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value)
            //    .FirstOrDefault();

            //if (alert != null && Settings.Exiles)
            //{
            //    lock (_locker)
            //    {
            //        alerts[alert.Text] = alert;
            //    }
            //}
            #endregion
        }
    }
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> MergeLeft<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> other)
        {
            foreach (var kvp in other)
            {
                source[kvp.Key] = kvp.Value;
            }
            return source;
        }
    }
}

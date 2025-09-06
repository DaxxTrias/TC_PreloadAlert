using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using ExileCore2;
using ExileCore2.PoEMemory;
using ExileCore2.Shared;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using ImGuiNET;
using Newtonsoft.Json;
using System.Globalization;
using Vector2 = System.Numerics.Vector2;
using RectangleF = ExileCore2.Shared.RectangleF;
using ExileCore2.PoEMemory.Components;

namespace PreloadAlert
{
    public class PreloadAlert : BaseSettingsPlugin<PreloadAlertSettings>
    {
        private const string PRELOAD_ALERTS = "config/preload_alerts.txt";
        private const string PRELOAD_ALERTS_PERSONAL = "config/preload_alerts_personal.txt";
        private const string PreloadStart = "preload-start.png";
        private const string PreloadEnd = "preload-end.png";
        private const string PreloadNew = "preload-new.png";
        private const string PreloadNewBase64 = "iVBORw0KGgoAAAANSUhEUgAAAlgAAAAPCAYAAAA4crG6AAAHeElEQVR4nO1d7Y7bIBDEvvd/5Dr9cbJEp7OzH+CEJB6pSg7DsguYHQ9WurV1kfFtg8+H0V6VbaJeX4fZ3gzbu9HHBm2w7U6+n/V3qPtD7J7tts7nnfS7Q9878WsjPrDvylbUplUW6VPZVbatMus7a6vGpreHPllt+r/Z2rJiQ3+s+lZ/2E61tepGrntl6npkX6i0UfWyfVb2rtE6yt7IPj/bv2rb2bnqGT4/E1eMj5VHvw3lscBkvQrOgB5Bfx7wybB19VgCeHSDiHb6v/E6q3+WH45fkXKMDftmZf34PUi98/PofFSLp4/D8qP/u19XOI/MZ7ShksQGNtQYKnvNuH441/GaVcbWEotZ+cfaRXzJkpYZsO4p9NOLxfPT2xMYuYrsI7PIVRbZeJ6Jq2NviT1+FUTX07Mx26ebXP2L0lisSrCa8eSs6p5QBKBPzJEndWyvkqq1wC270U3/4dg4SFmD+tiW2WTJTxEApVZgbGqd9eOHxIn1i/Fa9jKqjUUOI2uQEQar3WYQuBGC5K2tCjn0/IjeB56ackWiysRxRb+z+8qOUXRur+i7gmeQuJn2ViUeNxm6Blmx5x+sSrAyweAAbCTBZRJYnygw+VtSoZVQdzE5HqGL2NjJURVLdPgd2zB/LAUoShiVImjBIkeKvFp+qPhVXcueBUXu2bqLbITPekKuEDEPMzf6GePwrLG0lNlou+r1iL3qnHxq0n431ewVuAnbL4YenFZWsE54NwOqMZFkrAgXqhCMTDHbSkmykpc63uq/W2rSqWBZShOSwZ4knv/wOE/5gwoT87URuwg13sqP1vnv+W0RO6vvCnAuvLVS3dhnJ+Em1mXEbuQBZQbYHK648eO9nBmDq8drpI9IuxH/ow9fswnRyBpalXjMXkc3JozvygRLHRP1sJKy2misxMdIj6fCsITfCJFhR2r4Yjj69gBbLB5rDjfjmiJRKs6HMT7M7x3qRRKQR6rQzwOuM/WNzbPnR+SdNNaejZ0ieMq+ImZeH5GNwJvnCKx1W1WOLV9GSWqmzxECzIj7KDLq74j/DFG19V2P2z5NxZqtHKu9/hvxcQpW9OaOSnj9zWwdjXjJTxGwiF1sf4iF3CtXKlEf3SfzxUpyFjH1jhjO7+xFdvQJ48D4sA/LZ+U/XttFbFYsbN6ihA/LI+vQs68Sz4ykq0iyIkh9eYXYzSBHWeKb6TOiVmdtjKCS4Gb1H02wo0nYax8les/A6srOrWIthtUVrGiyYoge10T7UQkPyU0jJMMiGqwvVJN6pQr9Zu9TYYyKdGHCtuyw5LsbKhz72QDLdoTgqtj6vzEpRNQi5Y+FqsqDZSreTB8eSa5ulmrNV8uuIoevRJXQRTAj0c2e/5kY2eOrqCpnKys7s9XAW72agBUJ1kYIQaSuByQX7Em9/8xs7jskul55Yv15CbBPvEiyLHKE/WyGX3gjejcmG6deqVIKolpfSBhZn4wQsjizKiRe9whNdo1Z6zej+lyZFGf4ZyFyz0b7qCpY1eTJyiJjMJrYZiazZybGldSlDEZ9/hZ151axBvHuP9PANkDvqVslXq+O9UTOjueQAESOhrz+DkI2TttM4VLkx/ID321ihMwjn0yBwzr4G2GKRLGyvl3vs/czCBa5UOvNqsvK1bhnNixrfrLKbkTZrPqSrYOoJOgsgYvat8hcVtGsJKUVE9lMJXc1jPi9MumYNR9RgeNbUJ7zFQlWdlKzwXuLRx25KOLF2u1GMvN+ywnr9Rv9D/R7dJ8MHnFhsSFBaqQ+K2fHopZi5o0p2lbz1tuwjicZooqNRVIqxxuRstFjNM+vKAlRcxSx4cVR3cBVu+pYVo8jGeHPxrVqIruKWL8rPKX71ZhJ/m716hf40JzC6r+DVb3BldzvHdkhAUAbeOTWjAl4QH3WhpEGJB/siK//rt7BOkgb3CQi6sz5fRdzwwiHGkeEuqG9o0isi30wJcyy1cdq+at8YfPLrmNZhqx5BCNKMqI2vbY9rPtqFrKq19VJUe0XUVydGCvx3+rF/4g8VL0KM/1iD8bfBtzHS2Px7i+5RwgD1h8BOzJtagwSpdM/fDEcCYBFBlU9VueHEDlMwNjOu+bFijasOliXjQcSQwRT+HrCGSHT6N95HY9KGRS5t9YK6yvyhKRUJrWuPR9b8F6bhcxxWlVVwvrV2KLqqvWA9ArMJEaV+LOorvtRVHxemXDchHgukFN8lILVEovZUk6yxyweQfBUqh74Mjq2VwoD+w+Iz7YH9NX3w5SbP8SvZowtHjFuxF9GZBUha2R8sA4jdowA4LUG750pf5gCyGDdSBm1Bsss35FAK+XHIovMrmfLsp9BlLR4SmU1yV2RTKrKT1QtHMGozauJwZXq3VXEoeKztX/f+Gx8lIKFgXgJkbVXpKt6LNInUK8de9G6dROlbKjjKJa8USVj/fUkDBU3K3FHnywjKiPbmNRL7lZfVn9IRhv5nrHF6ini1ow4MsnXu+YRMMtWhiQwOxGCZvXj+VlNcs9UEirJtKKyKWTirRLFqm/fRDaie94rsLK69u6orfHW2l9mJqzsooib2wAAAABJRU5ErkJggg==";
        public static Dictionary<string, PreloadConfigLine> Essences;
        public static Dictionary<string, PreloadConfigLine> Shrines;
        public static Dictionary<string, PreloadConfigLine> Strongboxes;
        public static Dictionary<string, PreloadConfigLine> Exiles;
        public static Dictionary<string, PreloadConfigLine> AzmeriLeague;
        public static Dictionary<string, PreloadConfigLine> ExpeditionLeague;
        public static Dictionary<string, PreloadConfigLine> Misc;
        public static Dictionary<string, PreloadConfigLine> Abyss;
        private const string AbyssSmallKey = "Metadata/Chests/Abyss/AbyssChestSmallMagic";
        public static Color AreaNameColor;
        private readonly object _locker = new object();
        private Dictionary<string, PreloadConfigLine> alertStrings;
        private bool canRender;
        private DebugInformation debugInformation;
        private List<PreloadConfigLine> DrawAlerts = new List<PreloadConfigLine>();
        private bool essencefound;
        private bool shrinefound;
        private bool isLoading;
        private Vector2 lastLine;
        private readonly List<string> PreloadDebug = new List<string>();
        private Action PreloadDebugAction;
        private bool working;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationTokenSource debugDummyCts;
        private CancellationTokenSource parseCts;
        private HashSet<string> _personalExplicitColorKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly string GenericShrinePath = "Metadata/Shrines/Shrine";
        private HashSet<string> _lastPreloadKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private CancellationTokenSource _expShrineProbeCts;

        public PreloadAlert()
        {
            Order = -40;
        }

        private Dictionary<string, PreloadConfigLine> alerts { get; } = new Dictionary<string, PreloadConfigLine>();
        private Action<string, Color> AddPreload => ExternalPreloads;

        private void ResetCaches()
        {
            lock (_locker)
            {
                alerts.Clear();
                DrawAlerts.Clear();
                PreloadDebug.Clear();
            }
            PreloadDebugAction = null;
            _expShrineProbeCts?.Cancel();
            _expShrineProbeCts = null;
        }

        private void ScheduleInitialParseRetry()
        {
            var token = parseCts?.Token ?? CancellationToken.None;
            Task.Run(async () =>
            {
                try { await Task.Delay(1000, token); } catch { return; }
                if (token.IsCancellationRequested) return;
                if (!GameController.Area.CurrentArea.IsTown && !GameController.Area.CurrentArea.IsHideout)
                {
                    if (!working && alerts.Count == 0)
                        Parse(token);
                }

                try { await Task.Delay(2000, token); } catch { return; }
                if (token.IsCancellationRequested) return;
                if (!GameController.Area.CurrentArea.IsTown && !GameController.Area.CurrentArea.IsHideout)
                {
                    if (!working && alerts.Count == 0)
                        Parse(token);
                }
            }, token);
        }

        public override void DrawSettings()
        {
            if (ImGui.CollapsingHeader("Debug"))
            {
                if (ImGui.Button("Dump preloads"))
                {
                    Directory.CreateDirectory(Path.Combine(DirectoryFullName, "Dumps"));

                    var areaName = string.Join("_", GameController.Area.CurrentArea.Name.Split(Path.GetInvalidFileNameChars()));
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    var path = Path.Combine(DirectoryFullName, "Dumps",
                        $"{areaName}_{timestamp}.txt");

                    DebugWindow.LogMsg(path);

                    File.WriteAllLines(path, PreloadDebug);
                }

                if (ImGui.Button("Dump grouped preloads"))
                {
                    var groupBy = PreloadDebug.OrderBy(x => x).GroupBy(x => x.IndexOf('/'));
                    var serializeObject = JsonConvert.SerializeObject(groupBy, Formatting.Indented);

                    // Replace invalid characters in the file name and append current time to avoid overwriting  
                    var areaName = string.Join("_", GameController.Area.CurrentArea.Name.Split(Path.GetInvalidFileNameChars()));
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    var path = Path.Combine(DirectoryFullName, "Dumps",
                        $"{areaName}_{timestamp}.txt");

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

                if (ImGui.Button("Test draw dummy alert"))
                {
                    // Create a temporary blank alert to confirm rendering
                    debugDummyCts?.Cancel();
                    debugDummyCts = new CancellationTokenSource();
                    var token = debugDummyCts.Token;

                    lock (_locker)
                    {
                        alerts["This is a Test"] = new PreloadConfigLine { Text = "This is a Test", Color = Settings.DefaultTextColor, Category = PreloadCategory.Custom };
                        DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
                    }

                    Task.Run(async () =>
                    {
                        try { await Task.Delay(15000, token); }
                        catch { /* cancelled */ }

                        if (!token.IsCancellationRequested)
                        {
                            lock (_locker)
                            {
                                alerts.Remove("This is a Test");
                                DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
                            }
                        }
                    }, token);
                }

                if (ImGui.Button("Open dump folder"))
                {
                    try
                    {
                        var dumps = Path.Combine(DirectoryFullName, "Dumps");
                        Directory.CreateDirectory(dumps);
                        Process.Start(new ProcessStartInfo { FileName = dumps, UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        DebugWindow.LogError($"Failed to open dump folder: {ex.Message}");
                    }
                }

                if (ImGui.Button("Reset default preload config"))
                {
                    try
                    {
                        var globalConfigDir = ConfigDirectory;
                        Directory.CreateDirectory(globalConfigDir);
                        var globalMainPath = Path.Combine(globalConfigDir, Path.GetFileName(PRELOAD_ALERTS));
                        var globalPersonalPath = Path.Combine(globalConfigDir, Path.GetFileName(PRELOAD_ALERTS_PERSONAL));

                        if (File.Exists(globalMainPath))
                        {
                            var backupPath = globalMainPath + "." + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".bak";
                            try
                            {
                                File.Copy(globalMainPath, backupPath, false);
                                DebugWindow.LogMsg($"Backed up current default config to: {backupPath}");
                            }
                            catch
                            {
                                // ignore backup failures
                            }
                        }

                        // Reset caches and cancel any in-flight parse before regenerating
                        parseCts?.Cancel();
                        ResetCaches();

                        GenerateDefaultMainConfig(globalMainPath);
                        alertStrings = LoadConfig(globalMainPath);
                        BindConfigColorsToBuiltIns();

                        // Ensure personal stub exists
                        if (!File.Exists(globalPersonalPath))
                        {
                            WritePersonalConfigHeader(globalPersonalPath);
                            DebugWindow.LogMsg($"Created personal preload config stub at: {globalPersonalPath}");
                        }

                        DebugWindow.LogMsg($"Regenerated default config at: {globalMainPath} ({alertStrings.Count} entries).");
                        parseCts = new CancellationTokenSource();
                        Parse(parseCts.Token);
                        ScheduleInitialParseRetry();
                    }
                    catch (Exception ex)
                    {
                        DebugWindow.LogError($"Failed to regenerate default config: {ex.Message}");
                    }
                    
                }
            }

            if (ImGui.Button("Open config folder"))
            {
                try
                {
                    Directory.CreateDirectory(ConfigDirectory);
                    Process.Start(new ProcessStartInfo { FileName = ConfigDirectory, UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    DebugWindow.LogError($"Failed to open config directory: {ex.Message}");
                }
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
                        alerts.Add(text, new PreloadConfigLine {Text = text, FastColor = () => color, Category = PreloadCategory.Custom});

                        lock (_locker)
                        {
                            DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
                        }
                    }
                });
            }
            else
            {
                alerts.Add(text, new PreloadConfigLine {Text = text, FastColor = () => color, Category = PreloadCategory.Custom});

                lock (_locker)
                {
                    DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
                }
            }
        }

        public override void OnLoad()
        {
            // Determine legacy (plugin-local) and global config locations
            var legacyConfigDir = Path.Combine(DirectoryFullName, "config");
            var legacyMainPath = Path.Combine(legacyConfigDir, Path.GetFileName(PRELOAD_ALERTS));
            var legacyPersonalPath = Path.Combine(legacyConfigDir, Path.GetFileName(PRELOAD_ALERTS_PERSONAL));

            var globalConfigDir = ConfigDirectory;
            var globalMainPath = Path.Combine(globalConfigDir, Path.GetFileName(PRELOAD_ALERTS));
            var globalPersonalPath = Path.Combine(globalConfigDir, Path.GetFileName(PRELOAD_ALERTS_PERSONAL));

            try
            {
                Directory.CreateDirectory(globalConfigDir);
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to ensure global config directory: {globalConfigDir}. {ex.Message}");
            }

            // Migrate legacy configs to global if global is missing (main only; personal is read-only, no writes)
            try
            {
                if (!File.Exists(globalMainPath) && File.Exists(legacyMainPath))
                {
                    File.Copy(legacyMainPath, globalMainPath, false);
                    DebugWindow.LogMsg($"Migrated PreloadAlert main config to global: {globalMainPath}");
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to migrate main config to global: {ex.Message}");
            }

            try
            {
                if (!File.Exists(globalPersonalPath) && !File.Exists(legacyPersonalPath))
                {
                    WritePersonalConfigHeader(globalPersonalPath);
                    DebugWindow.LogMsg($"Created personal preload config stub at: {globalPersonalPath}");
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to create personal preload stub: {ex.Message}");
            }

            DebugWindow.LogMsg($"PreloadAlert config directory: {globalConfigDir}\n  Main: {globalMainPath} {(File.Exists(globalMainPath) ? "(exists)" : "(missing)")}\n  Personal: {globalPersonalPath} {(File.Exists(globalPersonalPath) ? "(exists)" : "(missing)")}");

            // Load main config (global)
            try
            {
                if (File.Exists(globalMainPath))
                {
                    alertStrings = LoadConfig(globalMainPath);
                }
                else
                {
                    DebugWindow.LogMsg($"No preload config found at {globalMainPath}. Using built-in alerts only.");
                    alertStrings = new Dictionary<string, PreloadConfigLine>();
                }

                if (alertStrings == null || !alertStrings.Any())
                {
                    DebugWindow.LogMsg($"No entries loaded from {globalMainPath}. Using built-in alerts only.");
                    alertStrings = new Dictionary<string, PreloadConfigLine>();
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to load alert strings from {globalMainPath}: {ex.Message}");
                alertStrings = new Dictionary<string, PreloadConfigLine>();
            }

            SetupPredefinedConfigs();

            // If main config is missing or empty, generate from built-ins into global path
            try
            {
                if (!File.Exists(globalMainPath) || alertStrings.Count == 0)
                {
                    GenerateDefaultMainConfig(globalMainPath);
                    alertStrings = LoadConfig(globalMainPath);
                    DebugWindow.LogMsg($"Generated default preload config with {alertStrings.Count} entries at {globalMainPath}");
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to generate default preload config: {ex.Message}");
            }

            // Append any newly added built-ins to an existing main config
            try
            {
                if (File.Exists(globalMainPath) && SyncMainConfigWithBuiltIns(globalMainPath))
                {
                    alertStrings = LoadConfig(globalMainPath);
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to sync built-in entries to main config: {ex.Message}");
            }

            // After built-ins are established and main config is present, bind config lines without explicit colors
            try
            {
                BindConfigColorsToBuiltIns();
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to bind config colors to built-ins: {ex.Message}");
            }

            // Initialize images (kept in plugin directory)
            try
            {
                string preloadStartPath = Path.Combine(DirectoryFullName, PreloadStart);
                string preloadEndPath = Path.Combine(DirectoryFullName, PreloadEnd);
                string preloadNewPath = Path.Combine(DirectoryFullName, PreloadNew);

                // Ensure preload-new.png exists by materializing from embedded base64 when missing
                if (!File.Exists(preloadNewPath))
                {
                    try
                    {
                        var bytes = Convert.FromBase64String(PreloadNewBase64);
                        Directory.CreateDirectory(DirectoryFullName);
                        File.WriteAllBytes(preloadNewPath, bytes);
                        DebugWindow.LogMsg($"Embedded {PreloadNew} created at {preloadNewPath}");
                    }
                    catch (Exception ex)
                    {
                        DebugWindow.LogError($"Failed to create embedded {PreloadNew}: {ex.Message}");
                    }
                }

                Graphics.InitImage(PreloadStart, preloadStartPath);
                Graphics.InitImage(PreloadEnd, preloadEndPath);
                Graphics.InitImage(PreloadNew, preloadNewPath);

                DebugWindow.LogMsg($"Loaded images: {preloadStartPath}, {preloadEndPath}, {preloadNewPath}");
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Error initializing images: {ex.Message}");
            }

            // Load personal config read-only (prefer global; fallback to legacy) and merge over main only if it has entries
            try
            {
                Dictionary<string, PreloadConfigLine> personal = null;
                string personalPathUsed = null;

                if (File.Exists(globalPersonalPath) && new FileInfo(globalPersonalPath).Length > 0)
                {
                    personal = LoadConfig(globalPersonalPath);
                    personalPathUsed = globalPersonalPath;
                }
                else if (File.Exists(legacyPersonalPath) && new FileInfo(legacyPersonalPath).Length > 0)
                {
                    personal = LoadConfig(legacyPersonalPath);
                    personalPathUsed = legacyPersonalPath;
                }

                if (personal != null && personal.Count > 0)
                {
                    _personalExplicitColorKeys = new HashSet<string>(
                        personal.Where(p => p.Value.Color.HasValue && p.Value.Color.Value.ToArgb() != 0)
                                .Select(p => p.Key),
                        StringComparer.OrdinalIgnoreCase);
                    alertStrings = alertStrings.MergeLeft(personal);
                    DebugWindow.LogMsg($"Loaded personal preload config from: {personalPathUsed} ({personal.Count} entries). Overrides applied.");
                }
                else
                {
                    _personalExplicitColorKeys.Clear();
                }

                // Re-bind after personal overrides are applied
                BindConfigColorsToBuiltIns();
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to load personal preload config: {ex.Message}");
            }
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
            // Clear any stale state and perform initial parse with a retry to avoid warm-up races
            parseCts?.Cancel();
            ResetCaches();
            AreaChange(GameController.Area.CurrentArea);
            ScheduleInitialParseRetry();
            return true;
        }

        public override void AreaChange(AreaInstance area)
        {
            isLoading = true;
            // Cancel any in-flight parse from the previous area and clear current drawings atomically
            parseCts?.Cancel();
            ResetCaches();
            if (GameController.Area.CurrentArea.IsHideout || GameController.Area.CurrentArea.IsTown)
            {
                isLoading = false;
                return;
            }
            parseCts = new CancellationTokenSource();
            Parse(parseCts.Token);
            if (Settings.ReparsePreloads)
            {
                StartPeriodicCheck();
            }
            // Also schedule a one-time quick retry after area change to improve reliability
            ScheduleInitialParseRetry();

            isLoading = false;
        }

        private void Parse(CancellationToken token = default)
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
                        if (token.IsCancellationRequested) return;
                        try
                        {
                            var memory = GameController.Memory;
                            if (memory == null)
                            {
                                DebugWindow.LogError($"{nameof(PreloadAlert)}: GameController.Memory is null; aborting preload parse.");
                                working = false;
                                return;
                            }

                            // DebugWindow.LogMsg($"{nameof(PreloadAlert)}: Creating FilesFromMemory...");
                            var sw = Stopwatch.StartNew();
                            FilesFromMemory filesFromMemory = new FilesFromMemory(memory);
                            // DebugWindow.LogMsg($"{nameof(PreloadAlert)}: FilesFromMemory created.");

                            var allFiles = filesFromMemory.GetAllFiles();
                            sw.Stop();
                            if (allFiles == null)
                            {
                                DebugWindow.LogError($"{nameof(PreloadAlert)}: FilesFromMemory.GetAllFiles() returned null after {sw.ElapsedMilliseconds} ms");
                                working = false;
                                return;
                            }

                            var totalCount = allFiles.Count;
                            // DebugWindow.LogMsg($"{nameof(PreloadAlert)}: GetAllFiles() returned {totalCount} entries in {sw.ElapsedMilliseconds} ms");
                            var sample = allFiles.Keys.Take(5).ToList();
                            if (sample.Count > 0)
                            {
                                // DebugWindow.LogMsg($"{nameof(PreloadAlert)}: Sample file keys: {string.Join(", ", sample)}");
                            }

                            // DebugWindow.LogMsg($"{nameof(PreloadAlert)}: Filtering files to 'Metadata/' prefix; ignoring ChangeCount.");
                            foreach (var file in allFiles)
                            {
                                if (token.IsCancellationRequested) return;
                                var text = file.Key;
                                if (!text.StartsWith("Metadata/", StringComparison.OrdinalIgnoreCase))
                                    continue;

                                if (text.Contains('@'))
                                {
                                    var splitText = text.Split('@');
                                    if (splitText.Length > 0)
                                    {
                                        text = splitText[0];
                                    }
                                }

                                lock (_locker)
                                {
                                    PreloadDebug.Add(text);
                                }

                                CheckForPreload(text);
                            }

                            // Snapshot keys for post-processing (e.g., generic shrine buff detection)
                            _lastPreloadKeys = new HashSet<string>(allFiles.Keys, StringComparer.OrdinalIgnoreCase);
                            DetectAndApplySpecialShrines();
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            DebugWindow.LogError($"{nameof(PreloadAlert)} -> ArgumentOutOfRangeException: {ex.Message}");
                        }
                        catch (Exception e)
                        {
                            DebugWindow.LogError($"{nameof(PreloadAlert)} -> {e}");
                        }

                        if (token.IsCancellationRequested) return;
                        // Apply suppression rules (e.g., remove small Abyss if specific exists)
                        ApplyAlertSuppressions();

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
                    {
                        await Task.Delay(250, token);
                        continue;
                    }

                    if (!Settings.ReparsePreloads || working)
                    {
                        await Task.Delay(250, token);
                        continue;
                    }

                    await Task.Delay(Settings.ReparseDelay.Value * 1000, token);
                    if (!token.IsCancellationRequested)
                    {
                        Parse(parseCts?.Token ?? CancellationToken.None);
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

            var visibleAlerts = DrawAlerts.Where(a => IsCategoryEnabled(a.Category)).ToList();

            var textSize = isLoading
                ? Graphics.MeasureText(loadingText)
                : visibleAlerts.Select(x => Graphics.MeasureText(x.Text)).ToList()
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
                foreach (var line in visibleAlerts)
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
                var parsed = ParseRgba(line.Length > 2 ? line[2] : null);
                Color? color = parsed.ToArgb() == 0 ? (Color?)null : parsed;
                var preloadAlerConfigLine = new PreloadConfigLine {Text = line[1], Color = color};
                return preloadAlerConfigLine;
            });
        }

        private static Color ParseRgba(string? rgba)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rgba)) return Color.Empty;
                var s = rgba.Trim();

                // CSV: r,g,b[,a]
                if (s.Contains(','))
                {
                    var parts = s.Split(',');
                    if (parts.Length < 3) return Color.Empty;
                    int r = int.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
                    int g = int.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
                    int b = int.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);
                    int a = parts.Length >= 4 ? int.Parse(parts[3].Trim(), CultureInfo.InvariantCulture) : 255;
                    return Color.FromArgb(a, r, g, b);
                }

                // HEX: AARRGGBB or RRGGBB (case-insensitive, supports # or 0x prefixes)
                var h = s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? s.Substring(2) : s;
                if (h.StartsWith('#')) h = h.Substring(1);
                h = h.Replace("_", string.Empty).Trim();

                byte aHex = 0xFF, rHex, gHex, bHex;
                if (h.Length == 8)
                {
                    aHex = byte.Parse(h.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    rHex = byte.Parse(h.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    gHex = byte.Parse(h.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    bHex = byte.Parse(h.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    return Color.FromArgb(aHex, rHex, gHex, bHex);
                }
                if (h.Length == 6)
                {
                    rHex = byte.Parse(h.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    gHex = byte.Parse(h.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    bHex = byte.Parse(h.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    return Color.FromArgb(aHex, rHex, gHex, bHex);
                }
                if (h.Length == 4) // ARGB short
                {
                    aHex = byte.Parse(new string(h[0], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    rHex = byte.Parse(new string(h[1], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    gHex = byte.Parse(new string(h[2], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    bHex = byte.Parse(new string(h[3], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    return Color.FromArgb(aHex, rHex, gHex, bHex);
                }
                if (h.Length == 3) // RGB short
                {
                    rHex = byte.Parse(new string(h[0], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    gHex = byte.Parse(new string(h[1], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    bHex = byte.Parse(new string(h[2], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    return Color.FromArgb(aHex, rHex, gHex, bHex);
                }

                return Color.Empty;
            }
            catch
            {
                return Color.Empty;
            }
        }

        protected static IEnumerable<string[]> LoadConfigBase(string path, int columnsCount = 2)
        {
            if (!File.Exists(path))
                return Enumerable.Empty<string[]>();

            return File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line) && line.IndexOf(';') >= 0 && !line.StartsWith("#"))
                .Select(line => line.Split(new[] {';'}, columnsCount).Select(parts => parts.Trim()).ToArray());
        }

        private void SetupPredefinedConfigs()
        {
            Essences = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModLightning1",
                    new PreloadConfigLine {Text = "Lightning Essence", FastColor = () => Settings.EssenceColors.EssenceOfLightning}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModLightning2",
                    new PreloadConfigLine {Text = "Greater Lightning Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfLightning}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModSpeed1",
                    new PreloadConfigLine {Text = "Speed Essence", FastColor = () => Settings.EssenceColors.EssenceOfSpeed}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModSpeed2e",
                    new PreloadConfigLine {Text = "Greater Speed Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfSpeed}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModPhysical1",
                    new PreloadConfigLine {Text = "Physical Essence", FastColor = () => Settings.EssenceColors.EssenceOfPhysical}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModPhysical2",
                    new PreloadConfigLine {Text = "Greater Physical Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfPhysical}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModAttack1",
                    new PreloadConfigLine {Text = "Attack Essence", FastColor = () => Settings.EssenceColors.EssenceOfAttack}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModAttack2",
                    new PreloadConfigLine {Text = "Greater Attack Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfAttack}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModLife1",
                    new PreloadConfigLine {Text = "Life Essence", FastColor = () => Settings.EssenceColors.EssenceOfLife}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModLife2",
                    new PreloadConfigLine {Text = "Greater Life Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfLife}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModChaos1",
                    new PreloadConfigLine {Text = "Chaos Essence", FastColor = () => Settings.EssenceColors.EssenceOfChaos}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModChaos2",
                    new PreloadConfigLine {Text = "Greater Chaos Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfChaos}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModCaster1",
                    new PreloadConfigLine {Text = "Caster Essence", FastColor = () => Settings.EssenceColors.EssenceOfCasting}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModCaster2",
                    new PreloadConfigLine {Text = "Greater Caster Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfCasting}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModCold1",
                    new PreloadConfigLine {Text = "Cold Essence", FastColor = () => Settings.EssenceColors.EssenceOfCold}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModCold2",
                    new PreloadConfigLine {Text = "Greater Cold Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfCold}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModMana1",
                    new PreloadConfigLine {Text = "Mana Essence", FastColor = () => Settings.EssenceColors.EssenceOfMana}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModMana2",
                    new PreloadConfigLine {Text = "Greater Mana Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfMana}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModDefences1",
                    new PreloadConfigLine {Text = "Defence Essence", FastColor = () => Settings.EssenceColors.EssenceOfDefence}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModDefences2",
                    new PreloadConfigLine {Text = "Greater Defence Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfDefence}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModAttribute1",
                    new PreloadConfigLine {Text = "Attributes Essence", FastColor = () => Settings.EssenceColors.EssenceOfAttributes}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModAttribute2",
                    new PreloadConfigLine {Text = "Greater Attributes Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfAttributes}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModFire1",
                    new PreloadConfigLine {Text = "Fire Essence", FastColor = () => Settings.EssenceColors.EssenceOfFire}
                },
                {
                    "Metadata/Monsters/EssenceModDaemons/MonsterEssenceModFire2",
                    new PreloadConfigLine {Text = "Greater Fire Essence", FastColor = () => Settings.EssenceColors.GreaterEssenceOfFire}
                },
            };
            SetCategory(Essences, PreloadCategory.Essence);

            Shrines = new Dictionary<string, PreloadConfigLine>
            {
                // Jan-6-2025: shrine assets loaded with .epk are confirmed loaded in, but are not always in the preload.
                // Certain shrines (fire/cold/light) spawn monsters and these can be detected in the preload.
                // in the absence of the above two scenarios, a generic shrine asset is always preloaded (Metadata/Shrines/Shrine)
                // enlightenment shrine is a good example. it only uses a generic shrine metadata tag
                {
                    "Metadata/Effects/Environment/shrine/plus/plus.epk",
                    new PreloadConfigLine {Text = "Regeneration Shrine", FastColor = () => Settings.ShrineColors.ShrineOfRegeneration}
                },
                {
                    "Metadata/Effects/Environment/shrine/resistance/resist.epk",
                    new PreloadConfigLine {Text = "Resistance Shrine", FastColor = () => Settings.ShrineColors.ShrineOfResistance}
                },
                {
                    //"Metadata/Effects/Environment/shrine/smoke/smoke.epk", // avarice 0.1.x
                    "Metadata/Effects/Environment/shrine/greed/greed.epk", // formerly avarice, changed in 0.2 to always be greed?
                    new PreloadConfigLine {Text = "Greed Shrine", FastColor = () => Settings.ShrineColors.ShrineOfGreed}
                },
                {
                    "Metadata/Effects/Environment/shrine/run_fast/runfast.epk", // acceleration
                    new PreloadConfigLine {Text = "Acceleration Shrine", FastColor = () => Settings.ShrineColors.ShrineOfAcceleration}
                },
                {
                    "Metadata/Effects/Environment/shrine/summon/summon.epk", // chaos damage
                    new PreloadConfigLine {Text = "Gloom Shrine", FastColor = () => Settings.ShrineColors.ShrineOfGloom}
                },
                {
                    "Metadata/Effects/Environment/shrine/sword/sword.epk", // critical
                    new PreloadConfigLine {Text = "Diamond Shrine", FastColor = () => Settings.ShrineColors.ShrineOfCrit}
                },
                {
                    "Metadata/Effects/Environment/shrine/curse/rig.epk", // corrupted shrine
                    new PreloadConfigLine {Text = "Tainted Shrine", FastColor = () => Settings.ShrineColors.ShrineOfCorruption}
                },
                {
                    //"Metadata/Effects/Environment/shrine/fire/fire.epk", // alternate data. not a preload; is a confirmation
                    "Metadata/Monsters/Daemon/Shrines/ShrineFireDaemon_", // meteoric shrine shrine
                    new PreloadConfigLine {Text = "Meteoric Shrine", FastColor = () => Settings.ShrineColors.ShrineOfFire}
                },
                {
                    "Metadata/Effects/Environment/shrine/massive/massive.epk", // meteoric shrine shrine
                    new PreloadConfigLine {Text = "Enduring Shrine", FastColor = () => Settings.ShrineColors.ShrineOfEnduring}
                },
                {
                    //"Metadata/Effects/Environment/shrine/lightning/lightning.epk", // alternate data. not a preload; is a confirmation
                    "Metadata/Monsters/Daemon/Shrines/ShrineLightningDaemon", // lightning  tempest shrine
                    new PreloadConfigLine {Text = "Lightning Shrine", FastColor = () => Settings.ShrineColors.ShrineOfLightning}
                },
                {
                    //"Metadata/Effects/Environment/shrine/ice/ice.epk", // alternate data. not a preload; is a confirmation
                    "Metadata/Monsters/Daemon/Shrines/ShrineColdDaemon", // cold shrine
                    new PreloadConfigLine {Text = "Cold Shrine", FastColor = () => Settings.ShrineColors.ShrineOfCold}
                },
                {
                    "Metadata/Effects/Environment/shrine/god_mode/godmode.epk",
                    new PreloadConfigLine {Text = "Divine Shrine", FastColor = () => Settings.ShrineColors.ShrineOfDivine}
                },
            };
            SetCategory(Shrines, PreloadCategory.Shrine);

            ExpeditionLeague = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "Metadata/Monsters/LeagueExpedition/NPC/ExpeditionRog",
                    new PreloadConfigLine {Text = "Rog [Items crafter]", FastColor = () => Settings.ExpeditionColors.Rog}
                },
                {
                    "Metadata/Monsters/LeagueExpedition/NPC/ExpeditionTujen",
                    new PreloadConfigLine {Text = "Tujen [Currency gambler]", FastColor = () => Settings.ExpeditionColors.Tujen}
                },
                {
                    "Metadata/Monsters/LeagueExpedition/NPC/ExpeditionGwennen",
                    new PreloadConfigLine {Text = "Gwennen [Items gambler]", FastColor = () => Settings.ExpeditionColors.Gwennen}
                },
                {
                    "Metadata/Monsters/LeagueExpedition/NPC/ExpeditionDannig",
                    new PreloadConfigLine {Text = "Dannig [Exchange items]", FastColor = () => Settings.ExpeditionColors.Dannig}
                },
            };
            SetCategory(ExpeditionLeague, PreloadCategory.Expedition);

            AzmeriLeague = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "Metadata/Monsters/NPC/Torment/TormentNPC",
                    new PreloadConfigLine {Text = "Delwyn [Hunter]", FastColor = () => Settings.AzmeriColors.Delwyn}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheOxWild",
                    new PreloadConfigLine {Text = "Spirit of Ox (Wild)", FastColor = () => Settings.AzmeriColors.OxWild}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheBearWild",
                    new PreloadConfigLine {Text = "Spirit of Bear (Wild)", FastColor = () => Settings.AzmeriColors.BearWild}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheBoarWild",
                    new PreloadConfigLine {Text = "Spirit of Boar (Wild)", FastColor = () => Settings.AzmeriColors.BoarWild}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheStagVivid",
                    new PreloadConfigLine {Text = "Spirit of Stag (Vivid)", FastColor = () => Settings.AzmeriColors.StagVivid}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheWolfVivid",
                    new PreloadConfigLine {Text = "Spirit of Wolf (Vivid)", FastColor = () => Settings.AzmeriColors.StagWild}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheCatVivid",
                    new PreloadConfigLine {Text = "Spirit of Cat (Vivid)", FastColor = () => Settings.AzmeriColors.CatVivid}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheSerpentPrimal",
                    new PreloadConfigLine {Text = "Spirit of Serpent (Primal)", FastColor = () => Settings.AzmeriColors.SerpentPrimal}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheOwlPrimal",
                    new PreloadConfigLine {Text = "Spirit of Owl (Primal)", FastColor = () => Settings.AzmeriColors.OwlPrimal}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheRabbitSacred_",
                    new PreloadConfigLine {Text = "Spirit of Rabbit (Sacred)", FastColor = () => Settings.AzmeriColors.RabbitSacred}
                },
                {
                    "Metadata/Monsters/TormentedSpirits/TormentedSpiritoftheCunningFox",
                    new PreloadConfigLine {Text = "Spirit of Cunning Fox", FastColor = () => Settings.AzmeriColors.CunningFox}
                }
            };
            SetCategory(AzmeriLeague, PreloadCategory.Azmeri);

            Strongboxes = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "Metadata/Chests/StrongBoxes/Arcanist", // this might have been replaced by researcher's strongbox
                    new PreloadConfigLine {Text = "Arcanist's Strongbox", FastColor = () => Settings.StrongboxColors.ArcanistStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Artisan",
                    new PreloadConfigLine {Text = "Artisan's Strongbox", FastColor = () => Settings.StrongboxColors.ArtisanStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Cartographer", // probably doesnt exist now since theyre waystones?
                    new PreloadConfigLine {Text = "Cartographer's Strongbox", FastColor = () => Settings.StrongboxColors.CartographerStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Diviner",
                    new PreloadConfigLine {Text = "Diviner's Strongbox", FastColor = () => Settings.StrongboxColors.DivinerStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/StrongboxDivination",
                    new PreloadConfigLine {Text = "Diviner's Strongbox", FastColor = () => Settings.StrongboxColors.DivinerStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Gemcutter",
                    new PreloadConfigLine {Text = "Gemcutter's Strongbox", FastColor = () => Settings.StrongboxColors.GemcutterStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Jeweller",
                    new PreloadConfigLine {Text = "Jeweller's Strongbox", FastColor = () => Settings.StrongboxColors.JewellerStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/MartialStrongbox",
                    new PreloadConfigLine {Text = "Blacksmith's Strongbox", FastColor = () => Settings.StrongboxColors.BlacksmithStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/CasterStrongbox",
                    new PreloadConfigLine {Text = "Arcane Strongbox", FastColor = () => Settings.StrongboxColors.ArcaneStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/ResearchStrongboxLow",
                    new PreloadConfigLine {Text = "Researcher's Strongbox", FastColor = () => Settings.StrongboxColors.ResearchStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/ResearchStrongbox",
                    new PreloadConfigLine {Text = "Researcher's Strongbox", FastColor = () => Settings.StrongboxColors.ResearchStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/ResearchStrongboxHigh",
                    new PreloadConfigLine {Text = "Researcher's Strongbox", FastColor = () => Settings.StrongboxColors.ResearchStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/ArmourerStrongbox",
                    new PreloadConfigLine {Text = "Armourer's Strongbox", FastColor = () => Settings.StrongboxColors.ArmourerStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/OrnateStrongbox",
                    new PreloadConfigLine {Text = "Ornate Strongbox", FastColor = () => Settings.StrongboxColors.OrnateStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/Large",
                    new PreloadConfigLine {Text = "Large Strongbox", FastColor = () => Settings.StrongboxColors.LargeStrongbox}
                },
                // Metadata/Chests/StrongBoxes/LargeStrongboxLow for ogham's unique strongbox (unconfirmed)
                {
                    "Metadata/Chests/StrongBoxes/BasicStrongbox",
                    new PreloadConfigLine {Text = "Basic Strongbox", FastColor = () => Settings.StrongboxColors.BasicStrongbox}
                },
                {
                    "Metadata/Chests/CopperChests/CopperChestEpic3",
                    new PreloadConfigLine {Text = "Epic Chest", FastColor = () => Settings.StrongboxColors.EpicStrongbox}
                },
                // Special Strongboxes
                {
                    // base-type can be blacksmith or armourer's
                    // Metadata/Chests/StrongBoxes/Unique/UniqueVaalStrongboxInteractionObject // ixchel torment unique box
                    "Metadata/Monsters/Strongbox/Daemon/SummonVaalMonstersDaemon",
                    new PreloadConfigLine {Text = "Ixchel's Torment Strongbox", FastColor = () => Settings.StrongboxColors.IxchelsTormentStrongbox}
                },
            };
            SetCategory(Strongboxes, PreloadCategory.Strongbox);

            Exiles = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "ExileMonk1", 
                    new PreloadConfigLine {Text = "Sondar the Stormbinder (Monk Exile)", FastColor = () => Settings.ExileColors.Sondar}
                },
                {
                    "ExileMonk2", 
                    new PreloadConfigLine {Text = "Doran, the Deft (Monk Exile)", FastColor = () => Settings.ExileColors.Doran}
                },
                {
                    "ExileMercenary1", 
                    new PreloadConfigLine {Text = "Ulfred, the Afflicted (Merc Exile)", FastColor = () => Settings.ExileColors.Ulfred}
                },
                {
                    "ExileSorceress2", 
                    new PreloadConfigLine {Text = "Hesperia, Arcane Tempest (Sorc Exile)", FastColor = () => Settings.ExileColors.Hesperia}
                },
                {
                    "ExileSorceress1", 
                    new PreloadConfigLine {Text = "Nyassa, Flaming Hand (Sorc Exile)", FastColor = () => Settings.ExileColors.Nyassa}
                },
                {
                    "ExileWitch2", 
                    new PreloadConfigLine {Text = "Clara the Curse Weaver (Witch Exile)", FastColor = () => Settings.ExileColors.Clara}
                },
                {
                    "ExileWitch1", 
                    new PreloadConfigLine {Text = "Vasa, Death Akhara (Witch Exile)", FastColor = () => Settings.ExileColors.Vasa}
                },
                {
                    "ExileRanger2", 
                    new PreloadConfigLine {Text = "Adrienne, the Malignant Rose (Ranger Exile)", FastColor = () => Settings.ExileColors.Adrienne}
                },
                {
                    "ExileRanger1", 
                    new PreloadConfigLine {Text = "Bronnach the Manhunter (Ranger Exile)", FastColor = () => Settings.ExileColors.Bronnach}
                },
                {
                    "ExileWarrior2", new PreloadConfigLine {Text = "Raok, the Bloodthirsty (Warrior Exile)", FastColor = () => Settings.ExileColors.Raok}
                },
                {
                    "ExileWarrior1", new PreloadConfigLine {Text = "Taua, the Ruthless (Warrior Exile)", FastColor = () => Settings.ExileColors.Taua}
                },
                {
                    "ExileMarauder1", new PreloadConfigLine {Text = "Marauder Exile", FastColor = () => Settings.ExileColors.Marauder1}
                },
                {
                    "ExileHuntress1", new PreloadConfigLine {Text = "Huntress Exile #1", FastColor = () => Settings.ExileColors.Huntress1}
                },
                {
                    "ExileHuntress2", new PreloadConfigLine {Text = "Huntress Exile #2", FastColor = () => Settings.ExileColors.Huntress2}
                },
                {
                    "ExileDuelist1", new PreloadConfigLine {Text = "Duelist Exile", FastColor = () => Settings.ExileColors.Duelist}
                },
                {
                    "ExileMercenary2", new PreloadConfigLine {Text = "Mercenary Exile", FastColor = () => Settings.ExileColors.Mercenary2}
                },
                {
                    "ExileTemplar1", new PreloadConfigLine {Text = "Templar Exile", FastColor = () => Settings.ExileColors.Templar}
                },
                {
                    "ExileDruid1", new PreloadConfigLine {Text = "Druid Exile", FastColor = () => Settings.ExileColors.Druid}
                }
            };
            SetCategory(Exiles, PreloadCategory.Exile);

            Misc = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "Metadata/Chests/MarakethSanctum/GoldChestRadiusJewels1",
                    new PreloadConfigLine { Text = "Gold Time-Lost Cache", FastColor = () => Settings.MiscColors.TimeLostCache }
                },
            };
            SetCategory(Misc, PreloadCategory.Misc);

            Abyss = new Dictionary<string, PreloadConfigLine>
            {
                // TODO: Metadata/MiscellaneousObjects/Abyss/AbyssPlinth can be used to count the total # of chests in an instance (unconfirmed)
                // TODO: small magic abyssal trove can also be Metadata/Chests/Abyss/AbyssChestGeneric & Metadata/Chests/Abyss/AbyssChestFinalGeneric & Metadata/Chests/Abyss/AbyssLargeChestFinalGeneric
                // why though? does every chest have multiple variants?
                {
                    "Metadata/Chests/Abyss/AbyssChestSmallMagic", // Abyssal Trove (low level only?)
                    new PreloadConfigLine { Text = "Abyss", FastColor = () => Settings.AbyssColors.AbyssSmall }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestGeneric", // Abyssal Trove
                    new PreloadConfigLine { Text = "Abyssal Trove", FastColor = () => Settings.AbyssColors.AbyssGeneric }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestFinalGeneric", // Abyssal Trove
                    new PreloadConfigLine { Text = "Abyssal Trove", FastColor = () => Settings.AbyssColors.AbyssGeneric }
                },
                {
                    "Metadata/Chests/Abyss/AbyssLargeChestFinalGeneric", // Abyssal Trove (large)
                    new PreloadConfigLine { Text = "Abyssal Trove Large", FastColor = () => Settings.AbyssColors.AbyssGeneric }
                },
                {
                    "Metadata/Chests/Abyss/AbyssLargeChestRareFinalGeneric", // Abyssal Trove (rare-large)
                    new PreloadConfigLine { Text = "Abyssal Trove Large (rare)", FastColor = () => Settings.AbyssColors.AbyssGeneric }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestArmour", // Abyssal Armoury (small?)
                    new PreloadConfigLine { Text = "Abyssal Armoury", FastColor = () => Settings.AbyssColors.AbyssArmour }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestFinalArmour", // Large Abyssal Armoury
                    new PreloadConfigLine { Text = "Abyssal Armoury", FastColor = () => Settings.AbyssColors.AbyssArmour }
                },
                {
                    "Metadata/Chests/Abyss/AbyssLargeChestFinalArmour", // Large Abyssal Armoury
                    new PreloadConfigLine { Text = "Abyssal Armoury (large)", FastColor = () => Settings.AbyssColors.AbyssArmour }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestCurrency", // Abyssal Currency
                    new PreloadConfigLine { Text = "Abyssal Coffer", FastColor = () => Settings.AbyssColors.AbyssCurrency }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestFinalCurrency", // Abyssal Currency
                    new PreloadConfigLine { Text = "Abyssal Coffer", FastColor = () => Settings.AbyssColors.AbyssCurrency }
                },
                {
                    "Metadata/Chests/Abyss/AbyssLargeChestFinalCurrency", // Abyssal Currency
                    new PreloadConfigLine { Text = "Abyssal Coffer", FastColor = () => Settings.AbyssColors.AbyssCurrency }
                },
                {
                    "Metadata/Chests/Abyss/AbyssLargeChestRareFinalCurrency", // Abyssal Currency
                    new PreloadConfigLine { Text = "Abyssal Coffer", FastColor = () => Settings.AbyssColors.AbyssCurrency }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestRareFinalCurrency", // Abyssal Currency
                    new PreloadConfigLine { Text = "Abyssal Coffer (rare)", FastColor = () => Settings.AbyssColors.AbyssCurrency }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestFinalWeapons", // Abyssal Arsenal
                    new PreloadConfigLine { Text = "Abyssal Arsenal", FastColor = () => Settings.AbyssColors.AbyssWeapons }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestRareFinalWeapons", // Abyssal Arsenal (rare)
                    new PreloadConfigLine { Text = "Abyssal Arsenal Large (rare)", FastColor = () => Settings.AbyssColors.AbyssWeapons }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestRareWeapons", // Abyssal Arsenal (rare)
                    new PreloadConfigLine { Text = "Abyssal Arsenal (rare)", FastColor = () => Settings.AbyssColors.AbyssWeapons }
                },
                {
                    "Metadata/Chests/Abyss/AbyssChestWeapons", // Abyssal Arsenal
                    new PreloadConfigLine { Text = "Abyssal Arsenal", FastColor = () => Settings.AbyssColors.AbyssWeapons }
                }

            };
            SetCategory(Abyss, PreloadCategory.Abyss);
        }

        private void CheckForPreload(string text)
        {
            if (alertStrings == null || alerts == null || _locker == null)
                return;

            // Prefix-based match for configured entries to support wildcard-like keys
            if (alertStrings.Count > 0)
            {
                foreach (var kv in alertStrings)
                {
                    if (text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        var configKey = kv.Key;
                        var line = kv.Value;

                        // Handle Abyss suppression even when alerts are coming from config
                        if (line.Category == PreloadCategory.Abyss)
                        {
                            // If a specific Abyss chest is detected, remove the small baseline alert
                            if (!configKey.Equals(AbyssSmallKey, StringComparison.OrdinalIgnoreCase))
                            {
                                if (TryGetDisplayTextForKey(AbyssSmallKey, out var smallText))
                                {
                                    lock (_locker)
                                    {
                                        TryRemoveAlertByTextInsensitive(smallText);
                                    }
                                }
                            }
                            else
                            {
                                // This is the small baseline. Do not add it if any specific Abyss alert is already present
                                var hasSpecific = AnyAbyssSpecificPresent();
                                if (hasSpecific)
                                    return;
                            }
                        }

                        lock (_locker)
                        {
                            alerts[line.Text] = line;
                        }
                        return;
                    }
                }
            }

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

            if (Settings.Strongboxes)
            {
                var _alert = Strongboxes.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                    .Select(kv => kv.Value).FirstOrDefault();
                if (_alert != null && Settings.Strongboxes)
                {
                    lock (_locker)
                    {
                        alerts[_alert.Text] = _alert;
                    }

                    return;
                }
            }

            if (Settings.Exiles)
            {
                var alert = Exiles.Where(kv => text.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                    .Select(kv => kv.Value).FirstOrDefault();
                if (alert != null)
                {
                    lock (_locker)
                    {
                        alerts[alert.Text] = alert;
                    }
                }
            }

            if (Settings.Azmeri)
            {
                var azmeri_alert = AzmeriLeague.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                    .Select(kv => kv.Value).FirstOrDefault();
                if (azmeri_alert != null)
                {
                    lock (_locker)
                    {
                        alerts[azmeri_alert.Text] = azmeri_alert;
                    }
                }
            }

            if (Settings.Misc)
            {
                var misc_alert = Misc.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                    .Select(kv => kv.Value).FirstOrDefault();
                if (misc_alert != null)
                {
                    lock (_locker)
                    {
                        alerts[misc_alert.Text] = misc_alert;
                    }
                }
            }

            if (Settings.Abyss)
            {
                var match = Abyss.FirstOrDefault(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase));
                if (!match.Equals(default(KeyValuePair<string, PreloadConfigLine>)))
                {
                    var matchedKey = match.Key;
                    var matched = match.Value;
                    lock (_locker)
                    {
                        // If a specific Abyss chest is detected, remove the small baseline alert
                        if (!matchedKey.Equals(AbyssSmallKey, StringComparison.OrdinalIgnoreCase))
                        {
                            if (TryGetDisplayTextForKey(AbyssSmallKey, out var smallText))
                            {
                                TryRemoveAlertByTextInsensitive(smallText);
                            }
                            alerts[matched.Text] = matched;
                        }
                        else
                        {
                            // Only add the small baseline if no specific Abyss alert is already present
                            var hasSpecific = AnyAbyssSpecificPresent();
                            if (!hasSpecific)
                                alerts[matched.Text] = matched;
                        }
                    }
                }
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
        }

        private static string FormatColor(Color c) => $"{c.R},{c.G},{c.B},{c.A}";

        private IEnumerable<KeyValuePair<string, PreloadConfigLine>> EnumerateBuiltIns()
        {
            foreach (var kv in Essences) yield return kv;
            foreach (var kv in Shrines) yield return kv;
            foreach (var kv in Strongboxes) yield return kv;
            foreach (var kv in Exiles) yield return kv;
            foreach (var kv in AzmeriLeague) yield return kv;
            foreach (var kv in ExpeditionLeague) yield return kv;
            foreach (var kv in Abyss) yield return kv;
            foreach (var kv in Misc) yield return kv;
        }

        private bool SyncMainConfigWithBuiltIns(string path)
        {
            try
            {
                if (!File.Exists(path)) return false;
                var existing = LoadConfig(path);
                var existingKeys = new HashSet<string>(existing.Keys, StringComparer.OrdinalIgnoreCase);
                var newLines = new List<string>();
                foreach (var kv in EnumerateBuiltIns())
                {
                    if (!existingKeys.Contains(kv.Key))
                    {
                        // Write without color so it stays bound to live settings via built-ins
                        newLines.Add($"{kv.Key};{kv.Value.Text}");
                    }
                }
                if (newLines.Count == 0) return false;
                File.AppendAllLines(path, newLines);
                // DebugWindow.LogMsg($"PreloadAlert: Added {newLines.Count} new built-in entries to {path}");
                return true;
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"PreloadAlert: Failed to sync built-in entries to {path}: {ex.Message}");
                return false;
            }
        }

        private void GenerateDefaultMainConfig(string path)
        {
            var lines = new List<string>
            {
                "# PreloadAlert main config",
                "# Format: <Prefix or FullKey> ; <Display Text> ; <R,G,B,A>",
                "# Lines here are matched as prefix (case-insensitive). Personal overrides win.",
                "# Tip: omit the color to use live colors from the settings menu.",
            };

            foreach (var kv in EnumerateBuiltIns())
            {
                // Write without color so built-ins provide live menu colors
                lines.Add($"{kv.Key};{kv.Value.Text}");
            }

            File.WriteAllLines(path, lines);
        }

        private void WritePersonalConfigHeader(string path)
        {
            var header = new[]
            {
                "# PreloadAlert personal config (overrides)",
                "# Format: <Prefix or FullKey> ; <Display Text> ; <Color>",
                "# Color formats:",
                "#   - CSV RGBA: r,g,b[,a] (alpha optional; defaults to 255)",
                "#   - Hex: #RRGGBB or #AARRGGBB (0x prefix also supported)",
                "# If you omit the color, the plugin uses Default Text Color from settings.",
                "# Examples:",
                "#   CSV: Metadata/Monsters/UniqueBoss/SomeBoss;Scary Boss;255,64,64,255",
                "#   Hex: Metadata/MiscellaneousObjects/Checkpoint;Checkpoint;#FFFF00",
            };
            File.WriteAllLines(path, header);
        }

        private void ApplyAlertSuppressions()
        {
            lock (_locker)
            {
                if (Abyss != null)
                {
                    var hasSpecific = AnyAbyssSpecificPresent();
                    if (hasSpecific && TryGetDisplayTextForKey(AbyssSmallKey, out var smallText))
                    {
                        TryRemoveAlertByTextInsensitive(smallText);
                        // Also remove any Abyss entry that uses the baseline text regardless of its source
                        var baselineText = GetAbyssBaselineText();
                        var keysToRemove = alerts.Where(kv => kv.Value.Category == PreloadCategory.Abyss && kv.Key.Equals(baselineText, StringComparison.OrdinalIgnoreCase))
                                                 .Select(kv => kv.Key)
                                                 .ToList();
                        foreach (var k in keysToRemove)
                        {
                            alerts.Remove(k);
                        }
                    }
                }
            }
        }

        private void BindConfigColorsToBuiltIns()
        {
            if (alertStrings == null || alertStrings.Count == 0) return;
            var builtIns = EnumerateBuiltIns().ToList();
            foreach (var kv in alertStrings.ToList())
            {
                var configKey = kv.Key;
                var line = kv.Value;
                if (line == null) continue;

                var hasExplicitColor = line.Color.HasValue && line.Color.Value.ToArgb() != 0;
                var personalOverride = _personalExplicitColorKeys.Contains(configKey);
                // If personal has explicit color, respect it. Otherwise, always bind to menu FastColor
                if (!personalOverride)
                {
                    var match = builtIns.FirstOrDefault(b => b.Key.StartsWith(configKey, StringComparison.OrdinalIgnoreCase));
                    if (!match.Equals(default(KeyValuePair<string, PreloadConfigLine>)))
                    {
                        line.FastColor = match.Value.FastColor;
                        if (line.Category == PreloadCategory.Unknown)
                            line.Category = match.Value.Category;
                        // If built-in had a fixed color, copy it too (rare; most use FastColor)
                        if ((!line.Color.HasValue || line.Color.Value.ToArgb() == 0) && match.Value.Color.HasValue && match.Value.Color.Value.ToArgb() != 0)
                            line.Color = match.Value.Color;
                    }
                }
            }
        }

        private static void SetCategory(Dictionary<string, PreloadConfigLine> map, PreloadCategory category)
        {
            if (map == null) return;
            foreach (var v in map.Values)
                v.Category = category;
        }

        private bool IsCategoryEnabled(PreloadCategory category)
        {
            return category switch
            {
                PreloadCategory.Essence => Settings.Essence.Value,
                PreloadCategory.Shrine => Settings.Shrines.Value,
                PreloadCategory.Strongbox => Settings.Strongboxes.Value,
                PreloadCategory.Exile => Settings.Exiles.Value,
                PreloadCategory.Azmeri => Settings.Azmeri.Value,
                PreloadCategory.Expedition => Settings.Expedition.Value,
                PreloadCategory.Abyss => Settings.Abyss.Value,
                PreloadCategory.Misc => Settings.Misc.Value,
                PreloadCategory.Custom => true,
                _ => true,
            };
        }

        // Resolve the display text used for a given config/built-in key
        private bool TryGetDisplayTextForKey(string key, out string text)
        {
            text = null;
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (alertStrings != null && alertStrings.Count > 0)
                {
                    if (alertStrings.TryGetValue(key, out var line) && line != null && !string.IsNullOrEmpty(line.Text))
                    {
                        text = line.Text;
                        return true;
                    }
                    // Fallback: allow prefix-equality in case user shortened/extended the key in config
                    foreach (var kv in alertStrings)
                    {
                        if (kv.Key.Equals(key, StringComparison.OrdinalIgnoreCase) || key.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.IsNullOrEmpty(kv.Value?.Text))
                            {
                                text = kv.Value.Text;
                                return true;
                            }
                        }
                    }
                }

                // Fallback to built-in mapping
                var builtIn = EnumerateBuiltIns().FirstOrDefault(b => b.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (!builtIn.Equals(default(KeyValuePair<string, PreloadConfigLine>)))
                {
                    text = builtIn.Value.Text;
                    return true;
                }
            }
            return false;
        }

        // Helper: obtain the display text used for the Abyss baseline (small) entry
        private string GetAbyssBaselineText()
        {
            if (TryGetDisplayTextForKey(AbyssSmallKey, out var text) && !string.IsNullOrWhiteSpace(text))
                return text;
            if (Abyss != null && Abyss.TryGetValue(AbyssSmallKey, out var line) && !string.IsNullOrWhiteSpace(line?.Text))
                return line.Text;
            return "Abyss";
        }

        // Check if any non-small Abyss alert is already queued for drawing
        private bool AnyAbyssSpecificPresent()
        {
            // Fast path: inspect current alerts for any Abyss entry whose text is not the baseline
            var baseline = GetAbyssBaselineText();
            if (alerts != null && alerts.Count > 0)
            {
                foreach (var entry in alerts)
                {
                    var v = entry.Value;
                    if (v?.Category == PreloadCategory.Abyss && !string.Equals(v.Text, baseline, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            // Fallback: consult built-in keys
            foreach (var kv in Abyss)
            {
                if (kv.Key.Equals(AbyssSmallKey, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (TryGetDisplayTextForKey(kv.Key, out var specificText))
                {
                    // Case-insensitive presence check
                    if (alerts.ContainsKey(specificText) || alerts.Keys.Any(k => k.Equals(specificText, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }
                else
                {
                    // Fall back to built-in text if config not found
                    var text = kv.Value.Text;
                    if (alerts.ContainsKey(text) || alerts.Keys.Any(k => k.Equals(text, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }
            }
            return false;
        }

        // Safely remove an alert by its display text, case-insensitive
        private bool TryRemoveAlertByTextInsensitive(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            if (alerts.Remove(text)) return true;
            var key = alerts.Keys.FirstOrDefault(k => k.Equals(text, StringComparison.OrdinalIgnoreCase));
            if (key != null)
            {
                alerts.Remove(key);
                return true;
            }
            return false;
        }

        // Identify and handle special shrine cases that cannot be derived from a single path match
        private void DetectAndApplySpecialShrines()
        {
            try
            {
                if (_lastPreloadKeys == null || _lastPreloadKeys.Count == 0) return;
                if (!Settings.Shrines.Value) return;

                // Never alert on the generic shrine metadata directly
                if (TryGetDisplayTextForKey(GenericShrinePath, out var genericText))
                {
                    lock (_locker)
                    {
                        TryRemoveAlertByTextInsensitive(genericText);
                    }
                }

                // If the generic shrine metadata is present (prefix), probe for experience shrine via entity buffs
                var hasGenericShrine = _lastPreloadKeys.Any(k => k.StartsWith(GenericShrinePath, StringComparison.OrdinalIgnoreCase));
                // DebugWindow.LogMsg($"{nameof(PreloadAlert)}: Generic shrine present={hasGenericShrine}; preloads={_lastPreloadKeys.Count}");
                if (hasGenericShrine)
                    StartExperienceShrineProbe();
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"{nameof(PreloadAlert)}: DetectAndApplySpecialShrines failed: {ex.Message}");
            }
        }

        private void StartExperienceShrineProbe()
        {
            // Avoid duplicate probes and avoid re-adding if already present
            lock (_locker)
            {
                if (alerts.ContainsKey("Experience Shrine"))
                    return;
            }
            if (_expShrineProbeCts != null && !_expShrineProbeCts.IsCancellationRequested)
                return;

            _expShrineProbeCts = new CancellationTokenSource();
            var token = _expShrineProbeCts.Token;
            Task.Run(async () =>
            {
                try
                {
                    // Try quickly first
                    if (TryDetectExperienceShrineFromEntities())
                    {
                        // DebugWindow.LogMsg($"{nameof(PreloadAlert)}: Experience Shrine found immediately via entity buffs.");
                        AddExperienceShrineAlert();
                        return;
                    }

                    // Probe for a short window to accommodate entity initialization and buff application
                    var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(8);
                    while (!token.IsCancellationRequested && DateTime.UtcNow < deadline)
                    {
                        await Task.Delay(250, token);
                        if (token.IsCancellationRequested) break;
                        if (TryDetectExperienceShrineFromEntities())
                        {
                            DebugWindow.LogMsg($"{nameof(PreloadAlert)}: Experience Shrine found during probe.");
                            AddExperienceShrineAlert();
                            break;
                        }
                    }
                    if (!token.IsCancellationRequested)
                        DebugWindow.LogMsg($"{nameof(PreloadAlert)}: Experience Shrine probe ended (timeout or no match).");
                }
                catch
                {
                    // ignore
                }
            }, token);
        }

        private void AddExperienceShrineAlert()
        {
            var expShrine = new PreloadConfigLine
            {
                Text = "Experience Shrine",
                FastColor = () => Settings.ShrineColors.ShrineOfExperience,
                Category = PreloadCategory.Shrine
            };
            lock (_locker)
            {
                alerts[expShrine.Text] = expShrine;
                DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
            }
            // DebugWindow.LogMsg($"{nameof(PreloadAlert)}: Added alert 'Experience Shrine'.");
        }

        // Detect Experience/Enlightenment shrine by inspecting live shrine entities and their buffs
        private bool TryDetectExperienceShrineFromEntities()
        {
            try
            {
                var entities = GameController?.EntityListWrapper?.OnlyValidEntities;
                if (entities == null) return false;
                foreach (var e in entities)
                {
                    if (e == null || !e.IsValid) continue;
                    if (!e.TryGetComponent<Shrine>(out var shrine)) continue;
                    if (shrine?.IsAvailable != true) continue;

                    // Fast path: direct entity buffs
                    var directBuffs = e.Buffs;
                    if (directBuffs != null && directBuffs.Count > 0)
                    {
                        for (int i = 0; i < directBuffs.Count; i++)
                        {
                            var b = directBuffs[i];
                            if (BuffIsExperience(b)) return true;
                        }
                    }

                    // Fallback: Buffs component list
                    // var buffsComp = e.GetComponent<Buffs>();
                    // if (buffsComp?.BuffsList != null)
                    // {
                    //     foreach (var b in buffsComp.BuffsList)
                    //     {
                    //         if (BuffIsExperience(b)) return true;
                    //     }
                    // }
                }
            }
            catch
            {
                // ignore, fall back to no detection
            }
            return false;
        }

        // Match an experience/enlightenment style shrine buff by name
        private static bool BuffIsExperience(ExileCore2.PoEMemory.Components.Buff buff)
        {
            if (buff == null) return false;
            var name = buff.Name;
            if (string.IsNullOrWhiteSpace(name)) return false;
            return name.StartsWith("shrine_experience", StringComparison.OrdinalIgnoreCase);
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

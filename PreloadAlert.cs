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
        private const string PreloadNewBase64 = "iVBORw0KGgoAAAANSUhEUgAAAlgAAAAPCAYAAAA4crG6AAAHeElEQVR4nO1d7Y7bIBDEvvd/5Dr9cbJEp7OzH+CEJB6pSg7DsguYHQ9WurV1kfFtg8+H0V6VbaJeX4fZ3gzbu9HHBm2w7U6+n/V3qPtD7J7tts7nnfS7Q9878WsjPrDvylbUplUW6VPZVbatMus7a6vGpreHPllt+r/Z2rJiQ3+s+lZ/2E61tepGrntl6npkX6i0UfWyfVb2rtE6yt7IPj/bv2rb2bnqGT4/E1eMj5VHvw3lscBkvQrOgB5Bfx7wybB19VgCeHSDiHb6v/E6q3+WH45fkXKMDftmZf34PUi98/PofFSLp4/D8qP/u19XOI/MZ7ShksQGNtQYKnvNuH441/GaVcbWEotZ+cfaRXzJkpYZsO4p9NOLxfPT2xMYuYrsI7PIVRbZeJ6Jq2NviT1+FUTX07Mx26ebXP2L0lisSrCa8eSs6p5QBKBPzJEndWyvkqq1wC270U3/4dg4SFmD+tiW2WTJTxEApVZgbGqd9eOHxIn1i/Fa9jKqjUUOI2uQEQar3WYQuBGC5K2tCjn0/IjeB56ackWiysRxRb+z+8qOUXRur+i7gmeQuJn2ViUeNxm6Blmx5x+sSrAyweAAbCTBZRJYnygw+VtSoZVQdzE5HqGL2NjJURVLdPgd2zB/LAUoShiVImjBIkeKvFp+qPhVXcueBUXu2bqLbITPekKuEDEPMzf6GePwrLG0lNlou+r1iL3qnHxq0n431ewVuAnbL4YenFZWsE54NwOqMZFkrAgXqhCMTDHbSkmykpc63uq/W2rSqWBZShOSwZ4knv/wOE/5gwoT87URuwg13sqP1vnv+W0RO6vvCnAuvLVS3dhnJ+Em1mXEbuQBZQbYHK648eO9nBmDq8drpI9IuxH/ow9fswnRyBpalXjMXkc3JozvygRLHRP1sJKy2misxMdIj6fCsITfCJFhR2r4Yjj69gBbLB5rDjfjmiJRKs6HMT7M7x3qRRKQR6rQzwOuM/WNzbPnR+SdNNaejZ0ieMq+ImZeH5GNwJvnCKx1W1WOLV9GSWqmzxECzIj7KDLq74j/DFG19V2P2z5NxZqtHKu9/hvxcQpW9OaOSnj9zWwdjXjJTxGwiF1sf4iF3CtXKlEf3SfzxUpyFjH1jhjO7+xFdvQJ48D4sA/LZ+U/XttFbFYsbN6ihA/LI+vQs68Sz4ykq0iyIkh9eYXYzSBHWeKb6TOiVmdtjKCS4Gb1H02wo0nYax8les/A6srOrWIthtUVrGiyYoge10T7UQkPyU0jJMMiGqwvVJN6pQr9Zu9TYYyKdGHCtuyw5LsbKhz72QDLdoTgqtj6vzEpRNQi5Y+FqsqDZSreTB8eSa5ulmrNV8uuIoevRJXQRTAj0c2e/5kY2eOrqCpnKys7s9XAW72agBUJ1kYIQaSuByQX7Em9/8xs7jskul55Yv15CbBPvEiyLHKE/WyGX3gjejcmG6deqVIKolpfSBhZn4wQsjizKiRe9whNdo1Z6zej+lyZFGf4ZyFyz0b7qCpY1eTJyiJjMJrYZiazZybGldSlDEZ9/hZ151axBvHuP9PANkDvqVslXq+O9UTOjueQAESOhrz+DkI2TttM4VLkx/ID321ihMwjn0yBwzr4G2GKRLGyvl3vs/czCBa5UOvNqsvK1bhnNixrfrLKbkTZrPqSrYOoJOgsgYvat8hcVtGsJKUVE9lMJXc1jPi9MumYNR9RgeNbUJ7zFQlWdlKzwXuLRx25KOLF2u1GMvN+ywnr9Rv9D/R7dJ8MHnFhsSFBaqQ+K2fHopZi5o0p2lbz1tuwjicZooqNRVIqxxuRstFjNM+vKAlRcxSx4cVR3cBVu+pYVo8jGeHPxrVqIruKWL8rPKX71ZhJ/m716hf40JzC6r+DVb3BldzvHdkhAUAbeOTWjAl4QH3WhpEGJB/siK//rt7BOkgb3CQi6sz5fRdzwwiHGkeEuqG9o0isi30wJcyy1cdq+at8YfPLrmNZhqx5BCNKMqI2vbY9rPtqFrKq19VJUe0XUVydGCvx3+rF/4g8VL0KM/1iD8bfBtzHS2Px7i+5RwgD1h8BOzJtagwSpdM/fDEcCYBFBlU9VueHEDlMwNjOu+bFijasOliXjQcSQwRT+HrCGSHT6N95HY9KGRS5t9YK6yvyhKRUJrWuPR9b8F6bhcxxWlVVwvrV2KLqqvWA9ArMJEaV+LOorvtRVHxemXDchHgukFN8lILVEovZUk6yxyweQfBUqh74Mjq2VwoD+w+Iz7YH9NX3w5SbP8SvZowtHjFuxF9GZBUha2R8sA4jdowA4LUG750pf5gCyGDdSBm1Bsss35FAK+XHIovMrmfLsp9BlLR4SmU1yV2RTKrKT1QtHMGozauJwZXq3VXEoeKztX/f+Gx8lIKFgXgJkbVXpKt6LNInUK8de9G6dROlbKjjKJa8USVj/fUkDBU3K3FHnywjKiPbmNRL7lZfVn9IRhv5nrHF6ini1ow4MsnXu+YRMMtWhiQwOxGCZvXj+VlNcs9UEirJtKKyKWTirRLFqm/fRDaie94rsLK69u6orfHW2l9mJqzsooib2wAAAABJRU5ErkJggg==";
        public static Dictionary<string, PreloadConfigLine> Essences;
        public static Dictionary<string, PreloadConfigLine> Shrines;
        public static Dictionary<string, PreloadConfigLine> Strongboxes;
        public static Dictionary<string, PreloadConfigLine> Exiles;
        public static Dictionary<string, PreloadConfigLine> AzmeriLeague;
        public static Dictionary<string, PreloadConfigLine> ExpeditionLeague;
        public static Dictionary<string, PreloadConfigLine> Misc;
        public static Dictionary<string, PreloadConfigLine> Abyss;
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
        //
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
        //
        //            lock (_locker)
        //            {
        //                DrawAlerts = alerts.OrderBy(x => x.Value.Text).Select(x => x.Value).ToList();
        //            }
        //        });
        //
        //        working = false;
        //    });
        //}

        public override void DrawSettings()
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
                    alerts["This is a Test"] = new PreloadConfigLine { Text = "This is a Test", Color = Settings.DefaultTextColor };
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
            var configDir = Path.Combine(DirectoryFullName, "config");
            var preloadAlertsPath = Path.Combine(configDir, Path.GetFileName(PRELOAD_ALERTS));
            var preloadAlertsPersonalPath = Path.Combine(configDir, Path.GetFileName(PRELOAD_ALERTS_PERSONAL));

            try
            {
                Directory.CreateDirectory(configDir);
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to ensure config directory: {configDir}. {ex.Message}");
            }

            try
            {
                if (File.Exists(preloadAlertsPath))
                {
                    alertStrings = LoadConfig(preloadAlertsPath);
                }
                else
                {
                    DebugWindow.LogMsg($"No preload config found at {preloadAlertsPath}. Using built-in alerts only.");
                    alertStrings = new Dictionary<string, PreloadConfigLine>();
                }

                if (alertStrings == null || !alertStrings.Any())
                {
                    DebugWindow.LogMsg($"No entries loaded from {preloadAlertsPath}. Using built-in alerts only.");
                    alertStrings = new Dictionary<string, PreloadConfigLine>();
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to load alert strings from {preloadAlertsPath}: {ex.Message}");
                alertStrings = new Dictionary<string, PreloadConfigLine>();
            }

            SetupPredefinedConfigs();

            // If main config is missing or empty, generate from built-in dictionaries (prefix-capable)
            try
            {
                if (!File.Exists(preloadAlertsPath) || alertStrings.Count == 0)
                {
                    GenerateDefaultMainConfig(preloadAlertsPath);
                    alertStrings = LoadConfig(preloadAlertsPath);
                    DebugWindow.LogMsg($"Generated default preload config with {alertStrings.Count} entries at {preloadAlertsPath}");
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to generate default preload config: {ex.Message}");
            }

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

            try
            {
                if (File.Exists(preloadAlertsPersonalPath))
                {
                    var personal = LoadConfig(preloadAlertsPersonalPath);
                    alertStrings = alertStrings.MergeLeft(personal);
                }
                else
                {
                    WritePersonalConfigHeader(preloadAlertsPersonalPath);
                }

                // Regenerate header if personal exists but is empty
                if (new FileInfo(preloadAlertsPersonalPath).Length == 0)
                {
                    WritePersonalConfigHeader(preloadAlertsPersonalPath);
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Failed to load or create personal preload config at {preloadAlertsPersonalPath}: {ex.Message}");
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
            if (Settings.ReparsePreloads)
            {
                StartPeriodicCheck();
            }

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
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            DebugWindow.LogError($"{nameof(PreloadAlert)} -> ArgumentOutOfRangeException: {ex.Message}");
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
                    new PreloadConfigLine {Text = "Spirit of Stag (Wild)", FastColor = () => Settings.AzmeriColors.StagWild}
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
                    //Metadata/Monsters/Strongbox/Daemon/SummonVaalMonstersDaemon // might be spawner for ixchel torment unique box
                    //Metadata/Chests/StrongBoxes/Unique/UniqueVaalStrongboxInteractionObject // ixchel torment unique box
                    "Metadata/Chests/StrongBoxes/MartialStrongbox",
                    new PreloadConfigLine {Text = "Blacksmith's Strongbox", FastColor = () => Settings.StrongboxColors.BlacksmithStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/CasterStrongbox",
                    new PreloadConfigLine {Text = "Arcane Strongbox", FastColor = () => Settings.StrongboxColors.ArcaneStrongbox}
                },
                {
                    "Metadata/Chests/StrongBoxes/ResearchStrongboxHigh",
                    new PreloadConfigLine {Text = "Researcher's Strongbox", FastColor = () => Settings.StrongboxColors.ResearchStrongbox}
                },
                {
                    //Metadata/Monsters/Strongbox/Daemon/SummonVaalMonstersDaemon // might be spawner for ixchel torment unique box
                    //Metadata/Chests/StrongBoxes/Unique/UniqueVaalStrongboxInteractionObject // ixchel torment unique box
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
                    "Metadata/Monsters/Strongbox/Daemon/SummonVaalMonstersDaemon",
                    new PreloadConfigLine {Text = "Ixchel's Torment Strongbox", FastColor = () => Settings.StrongboxColors.IxchelsTormentStrongbox}
                },
            };

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

            Misc = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "Metadata/Chests/MarakethSanctum/GoldChestRadiusJewels1",
                    new PreloadConfigLine { Text = "Gold Time-Lost Cache", FastColor = () => Settings.MiscColors.TimeLostCache }
                },
            };

            Abyss = new Dictionary<string, PreloadConfigLine>
            {
                {
                    "Metadata/Chests/Abyss/AbyssChestSmallMagic",
                    new PreloadConfigLine { Text = "Abyss (small)", FastColor = () => Settings.AbyssColors.AbyssSmall }
                },
            };
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
                        lock (_locker)
                        {
                            alerts[kv.Value.Text] = kv.Value;
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
                var abyss_alert = Abyss.Where(kv => text.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                    .Select(kv => kv.Value).FirstOrDefault();
                if (abyss_alert != null)
                {
                    lock (_locker)
                    {
                        alerts[abyss_alert.Text] = abyss_alert;
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

        private void GenerateDefaultMainConfig(string path)
        {
            var lines = new List<string>
            {
                "# PreloadAlert main config",
                "# Format: <Prefix or FullKey> ; <Display Text> ; <R,G,B,A>",
                "# Lines here are matched as prefix (case-insensitive). Personal overrides win.",
            };

            IEnumerable<KeyValuePair<string, PreloadConfigLine>> AllBuiltIns()
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

            foreach (var kv in AllBuiltIns())
            {
                var color = kv.Value.FastColor?.Invoke() ?? kv.Value.Color ?? Settings.DefaultTextColor;
                lines.Add($"{kv.Key};{kv.Value.Text};{FormatColor(color)}");
            }

            File.WriteAllLines(path, lines);
        }

        private void WritePersonalConfigHeader(string path)
        {
            var header = new[]
            {
                "# PreloadAlert personal config (overrides)",
                "# Format: <Prefix or FullKey> ; <Display Text> ; <R,G,B,A>",
                "# Example:",
                "# Metadata/Monsters/UniqueBoss/SomeBoss;Scary Boss;255,64,64,255",
            };
            File.WriteAllLines(path, header);
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

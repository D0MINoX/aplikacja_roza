using System.Collections.Concurrent;
using System.Diagnostics;

namespace MauiApp1.Services
{
    public class MeditationAudioService
    {
        public const string AutoDownloadAudioPreferenceKey = "AutoDownloadAudio";

        private static readonly Dictionary<string, string> MysteryFolders = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Zwiastowanie Najświętszej Maryi Pannie"] = "01_Zwiastowanie",
            ["Nawiedzenie św. Elżbiety"] = "02_Nawiedzenie",
            ["Narodzenie Pana Jezusa"] = "03_Narodzenie",
            ["Ofiarowanie Pana Jezusa w świątyni"] = "04_Ofiarowanie",
            ["Odnalezienie Pana Jezusa w świątyni"] = "05_Odnalezienie",
            ["Chrzest Pana Jezusa w Jordanie"] = "06_Chrzest_w_Jordanie",
            ["Objawienie się Pana Jezusa w Kanie Galilejskiej"] = "07_Kana_Galilejska",
            ["Głoszenie Królestwa Bożego i wzywanie do nawrócenia"] = "08_Gloszenie_Krolestwa",
            ["Przemienienie na górze Tabor"] = "09_Przemienienie_Tabor",
            ["Ustanowienie Eucharystii"] = "10_Ustanowienie_Eucharystii",
            ["Modlitwa Pana Jezusa w Ogrójcu"] = "11_Modlitwa_w_Ogrojcu",
            ["Biczowanie Pana Jezusa"] = "12_Biczowanie",
            ["Cierniem ukoronowanie Pana Jezusa"] = "13_Cierniem_ukoronowanie",
            ["Dźwiganie krzyża na Kalwarię"] = "14_Droga_krzyzowa",
            ["Ukrzyżowanie i śmierć Pana Jezusa"] = "15_Ukrzyzowanie",
            ["Zmartwychwstanie Pana Jezusa"] = "16_Zmartwychwstanie",
            ["Wniebowstąpienie Pana Jezusa"] = "17_Wniebowstapienie",
            ["Zesłanie Ducha Świętego"] = "18_Zeslanie_Ducha_Swietego",
            ["Wniebowzięcie Najświętszej Maryi Panny"] = "19_Wniebowziecie",
            ["Ukoronowanie Najświętszej Maryi Panny na Królową Nieba i Ziemi"] = "20_Ukoronowanie"
        };

        private const long MinValidAudioBytes = 10_000;
        private readonly HttpClient _httpClient;
        private readonly string _cacheRoot;
        private readonly ConcurrentDictionary<string, byte> _activeMysteryDownloads = new(StringComparer.OrdinalIgnoreCase);

        public MeditationAudioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _cacheRoot = Path.Combine(FileSystem.AppDataDirectory, "audio_cache");
        }

        public bool IsMysteryFullyCached(string mysteryTitle)
        {
            if (string.IsNullOrWhiteSpace(mysteryTitle))
                return false;

            for (int day = 0; day <= 31; day++)
            {
                if (GetCachedFilePath(mysteryTitle, day) == null)
                    return false;
            }
            return true;
        }

        public void EnsureMysteryAudioDownloadedInBackground(string mysteryTitle, int priorityDay = -1)
        {
            if (string.IsNullOrWhiteSpace(mysteryTitle))
                return;

            string key = ResolveFolderKey(mysteryTitle);
            if (IsMysteryFullyCached(mysteryTitle))
                return;

            if (!_activeMysteryDownloads.TryAdd(key, 0))
                return;

            _ = Task.Run(async () =>
            {
                try
                {
                    await DownloadAllAudioForMysteryAsync(mysteryTitle, priorityDay);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"### BŁĄD POBIERANIA AUDIO TAJEMNICY: {ex.Message}");
                }
                finally
                {
                    _activeMysteryDownloads.TryRemove(key, out _);
                }
            });
        }

        public async Task<int> DownloadAllAudioForMysteryAsync(string mysteryTitle, int priorityDay = -1, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(mysteryTitle))
                return 0;

            var daysOrder = new List<int>(32);
            if (priorityDay is >= 0 and <= 31)
                daysOrder.Add(priorityDay);
            for (int day = 0; day <= 31; day++)
            {
                if (day != priorityDay)
                    daysOrder.Add(day);
            }

            int downloadedCount = 0;
            int consecutiveFailures = 0;

            foreach (int day in daysOrder)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Jeśli użytkownik wyłączył przełącznik w trakcie pobierania, przerwij grzecznie
                if (!Preferences.Default.Get(AutoDownloadAudioPreferenceKey, false))
                    break;

                if (GetCachedFilePath(mysteryTitle, day) != null)
                {
                    consecutiveFailures = 0;
                    continue;
                }

                string? path = await GetOrDownloadAudioPathAsync(mysteryTitle, day, cancellationToken);
                if (path != null)
                {
                    downloadedCount++;
                    consecutiveFailures = 0;
                }
                else
                {
                    consecutiveFailures++;
                    // Jeśli np. brak sieci lub serwer nie ma jeszcze plików audio, nie męcz 32 razy
                    if (consecutiveFailures >= 3)
                        break;
                }
            }

            return downloadedCount;
        }

        public string? GetCachedFilePath(string mysteryTitle, int day)
        {
            if (day < 0 || day > 31 || string.IsNullOrWhiteSpace(mysteryTitle))
                return null;

            string folder = ResolveFolderKey(mysteryTitle);
            string path = Path.Combine(_cacheRoot, folder, $"dzien_{day:D2}.mp3");
            if (File.Exists(path) && new FileInfo(path).Length >= MinValidAudioBytes)
                return path;

            return null;
        }

        public async Task<string?> GetOrDownloadAudioPathAsync(string mysteryTitle, int day, CancellationToken cancellationToken = default)
        {
            if (day < 0 || day > 31 || string.IsNullOrWhiteSpace(mysteryTitle))
                return null;

            string? cached = GetCachedFilePath(mysteryTitle, day);
            if (cached != null)
                return cached;

            string folder = ResolveFolderKey(mysteryTitle);
            string mysteryDir = Path.Combine(_cacheRoot, folder);
            Directory.CreateDirectory(mysteryDir);

            string finalPath = Path.Combine(mysteryDir, $"dzien_{day:D2}.mp3");
            string tempPath = Path.Combine(mysteryDir, $".dzien_{day:D2}.{Guid.NewGuid():N}.tmp");

            try
            {
                string url = $"api/meditations/audio?date={day}&title={Uri.EscapeDataString(mysteryTitle.Trim())}";
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(25));

                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"### AUDIO API ({response.StatusCode}) dla {mysteryTitle} dzień {day}");
                    return null;
                }

                await using (var remoteStream = await response.Content.ReadAsStreamAsync(timeoutCts.Token))
                await using (var localStream = File.Create(tempPath))
                {
                    await remoteStream.CopyToAsync(localStream, timeoutCts.Token);
                }

                if (new FileInfo(tempPath).Length < MinValidAudioBytes)
                {
                    SafeDelete(tempPath);
                    return null;
                }

                File.Move(tempPath, finalPath, overwrite: true);
                return finalPath;
            }
            catch (OperationCanceledException)
            {
                SafeDelete(tempPath);
                throw;
            }
            catch (Exception ex)
            {
                SafeDelete(tempPath);
                Debug.WriteLine($"### BŁĄD POBIERANIA AUDIO: {ex.Message}");
                return null;
            }
        }

        private static string ResolveFolderKey(string mysteryTitle)
        {
            if (MysteryFolders.TryGetValue(mysteryTitle.Trim(), out string? known))
                return known;

            string trimmed = mysteryTitle.Trim();
            string safe = trimmed.Replace(" ", "_");
            return safe.Length > 24 ? safe.Substring(0, 24) : safe;
        }

        private static void SafeDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
            }
        }
    }
}

// ============================================================
// AudioPlayer.cs
// Audio — Background music and sound effects.
// Knows nothing about the board: Program tells it what to play.
// Every file is optional: if it is missing or the audio device is
// unavailable (e.g. in Docker), the game simply runs silently.
// ============================================================

using SFML.Audio;

namespace Learn2Slither.Audio;

public sealed class AudioPlayer : IDisposable
{
    const string MusicPath      = "assets/audio/game.ogg";
    const string MenuMusicPath      = "assets/audio/start-menu.ogg";
    const string GreenApplePath = "assets/audio/green-apple.wav";
    const string RedApplePath   = "assets/audio/red-apple.wav";
    const string GameOverPath   = "assets/audio/game-over.wav";
    const string WinPath        = "assets/audio/win.wav";

    const float MusicVolume  = 40f;  // 0 to 100
    const float EffectVolume = 80f;

    readonly Music? _music;
    readonly Music? _menuMusic;
    readonly LoadedSound? _greenApple;
    readonly LoadedSound? _redApple;
    readonly LoadedSound? _gameOver;
    readonly LoadedSound? _win;

    public AudioPlayer()
    {
        _music      = TryLoadMusic(MusicPath);
        _menuMusic  = TryLoadMusic(MenuMusicPath);
        _greenApple = TryLoadSound(GreenApplePath);
        _redApple   = TryLoadSound(RedApplePath);
        _gameOver   = TryLoadSound(GameOverPath);
        _win        = TryLoadSound(WinPath);
    }

    #region Public interface

    public void PlayMusic()  => _music?.Play();
    public void StopMusic()  => _music?.Stop();

    public void PlayMenuMusic()  => _menuMusic?.Play();
    public void StopMenuMusic()  => _menuMusic?.Stop();

    public void PlayGreenApple() => _greenApple?.Sound.Play();
    public void PlayRedApple()   => _redApple?.Sound.Play();
    public void PlayGameOver()   => _gameOver?.Sound.Play();
    public void PlayWin()   => _win?.Sound.Play();

    #endregion

    #region Loading

    sealed record LoadedSound(SoundBuffer Buffer, Sound Sound);

    static Music? TryLoadMusic(string relativePath)
    {
        string? path = FindFile(relativePath);
        if (path == null)
            return null;

        try
        {
            var music = new Music(path)
            {
                Volume = MusicVolume,
                Loop = true
            };

            return music;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"[Audio] Failed to load {path}: {e.Message}");
            return null;
        }
    }

    static LoadedSound? TryLoadSound(string relativePath)
    {
        string? path = FindFile(relativePath);
        if (path == null)
            return null;

        try
        {
            var buffer = new SoundBuffer(path);
            var sound = new Sound(buffer) { Volume = EffectVolume };
            return new LoadedSound(buffer, sound);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"[Audio] Failed to load {path}: {e.Message}");
            return null;
        }
    }

    static string? FindFile(string relativePath)
    {
        string path = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (File.Exists(path))
            return path;

        Console.Error.WriteLine($"[Audio] File not found: {path}");
        return null;
    }

    #endregion

    #region Cleanup

    public void Dispose()
    {
        _music?.Stop();
        _music?.Dispose();

        _menuMusic?.Stop();
        _menuMusic?.Dispose();

        foreach (var loaded in new[] { _greenApple, _redApple, _gameOver, _win })
        {
            loaded?.Sound.Dispose();
            loaded?.Buffer.Dispose();
        }
    }

    #endregion
}
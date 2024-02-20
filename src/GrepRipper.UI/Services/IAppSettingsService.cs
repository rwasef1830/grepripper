namespace GrepRipper.UI.Services;

public interface IAppSettingsService
{
    void Save<TSettings>(TSettings settings) where TSettings : class;
    TSettings LoadOrCreate<TSettings>() where TSettings : class, new();
}

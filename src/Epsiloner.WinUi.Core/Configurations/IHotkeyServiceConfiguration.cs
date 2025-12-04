using Epsiloner.WinUi.Services;

namespace Epsiloner.WinUi.Configurations;

/// <summary>
/// (Optional) Provides extra configuration for <see cref="IHotkeysService"/>.
/// </summary>
public interface IHotkeyServiceConfiguration
{
    void Configure(IHotkeysService services);
}
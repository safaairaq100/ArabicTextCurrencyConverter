using Microsoft.Extensions.DependencyInjection;

namespace ArabicTextCurrencyConverter;

public static class ArabicCurrencyServiceExtensions
{
    public static IServiceCollection AddArabicCurrencyService(this IServiceCollection services)
    {
        services.AddSingleton<IArabicCurrencyService, ArabicCurrencyService>();
        return services;
    }

    public static IArabicCurrencyService UseArabicTextCurrency(this IServiceProvider provider)
        => provider.GetRequiredService<IArabicCurrencyService>();
}
namespace ArabicTextCurrencyConverter;

public interface IArabicCurrencyService
{
    string Convert(double number);
    string Convert(double number,
        string mainUnit, string mainUnitDual, string mainUnitPlural,
        string subUnit, string subUnitDual, string subUnitPlural);

    IArabicCurrencyService SetDefaultCurrency(string mainUnit, string subUnit, bool useThreeDecimal = false);
    IArabicCurrencyService SetCurrencyForms(
        string singular, string dual, string plural,
        string subSingular, string subDual, string subPlural);

    /// <summary>
    /// Using classical Arabic instead of colloquial Arabic
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    IArabicCurrencyService UseFormalArabic(bool enable);

    /// <summary>
    /// لا غير [amount in words] فقط
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    IArabicCurrencyService UseAmountLimiter(bool enable); 
}
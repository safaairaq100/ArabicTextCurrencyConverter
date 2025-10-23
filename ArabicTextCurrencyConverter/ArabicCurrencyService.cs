namespace ArabicTextCurrencyConverter
{
    public class ArabicCurrencyService : IArabicCurrencyService
    {
        private string _mainUnit = "دينار";
        private string _mainUnitDual = "ديناران";
        private string _mainUnitPlural = "دنانير";

        private string _subUnit = "فلس";
        private string _subUnitDual = "فلسان";
        private string _subUnitPlural = "فلوس";

        private bool _useThreeDecimal;
        private bool _useFormalArabic;
        private bool _useAmountLimiter;

        private static readonly string[] Units =
        {
            "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة",
            "ستة", "سبعة", "ثمانية", "تسعة"
        };

        private static readonly string[] Tens =
        {
            "", "عشرة", "عشرون", "ثلاثون", "أربعون", "خمسون",
            "ستون", "سبعون", "ثمانون", "تسعون"
        };

        private static readonly string[] Hundreds =
        {
            "", "مئة", "مئتان", "ثلاثمائة", "أربعمائة",
            "خمسمائة", "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة"
        };

        private static readonly string[] Scales =
        {
            "", "ألف", "مليون", "مليار"
        };

        public IArabicCurrencyService UseFormalArabic(bool enable)
        {
            _useFormalArabic = enable;
            return this;
        }

        public IArabicCurrencyService UseAmountLimiter(bool enable)
        {
            _useAmountLimiter = enable;
            return this;
        }

        public IArabicCurrencyService SetDefaultCurrency(string mainUnit, string subUnit, bool useThreeDecimal = false)
        {
            _mainUnit = mainUnit;
            _subUnit = subUnit;
            _useThreeDecimal = useThreeDecimal;
            return this;
        }

        public IArabicCurrencyService SetCurrencyForms(
            string singular, string dual, string plural,
            string subSingular, string subDual, string subPlural)
        {
            _mainUnit = singular;
            _mainUnitDual = dual;
            _mainUnitPlural = plural;
            _subUnit = subSingular;
            _subUnitDual = subDual;
            _subUnitPlural = subPlural;
            return this;
        }

        public string Convert(double number)
        {
            return Convert(number, _mainUnit, _mainUnitDual, _mainUnitPlural,
                           _subUnit, _subUnitDual, _subUnitPlural);
        }

        public string Convert(double number,
            string mainUnit, string mainUnitDual, string mainUnitPlural,
            string subUnit, string subUnitDual, string subUnitPlural)
        {
            if (number == 0)
                return ApplyWrapper(FormatUnit($"صفر {mainUnit}"));

            if (_useThreeDecimal && number > 999_999_999_999.999 ||
                !_useThreeDecimal && number > 999_999_999_999.99)
                return "قيمة كبيرة جداً";

            var formatted = number.ToString(_useThreeDecimal ? "0.000" : "0.00");
            var parts = formatted.Split('.');
            var integerPart = long.Parse(parts[0]);
            var decimalPart = int.Parse(parts[1]);

            string integerText = ConvertNumber(integerPart);
            string decimalText = decimalPart > 0 ? ConvertNumber(decimalPart) : "";

            string mainCurrency = GetCurrencyForm(integerPart, mainUnit, mainUnitDual, mainUnitPlural);
            string subCurrency = GetCurrencyForm(decimalPart, subUnit, subUnitDual, subUnitPlural);

            if (decimalPart >= 11)
                subCurrency = subUnit;

            string formattedMain = $"{integerText} {mainCurrency}";
            if (integerPart == 1)
                formattedMain = $"{mainCurrency} واحد";
            else if (integerPart == 2)
                formattedMain = mainCurrency;

            string result;
            if (decimalPart > 0)
                result = $"{formattedMain} و{decimalText} {subCurrency}";
            else
                result = formattedMain;

            result = _useFormalArabic ? ApplyFormalArabic(result) : result;
            return ApplyWrapper(result);
        }

        private string ConvertNumber(long number)
        {
            if (number == 0)
                return "صفر";

            var parts = new List<string>();
            int scaleIndex = 0;

            while (number > 0)
            {
                int group = (int)(number % 1000);
                if (group > 0)
                {
                    string groupText = ConvertGroup(group);
                    string scaleText = Scales[scaleIndex];

                    if (scaleIndex > 0)
                    {
                        if (group == 1)
                        {
                            groupText = scaleText;
                            scaleText = "";
                        }
                        else if (group == 2)
                        {
                            groupText = scaleText switch
                            {
                                "ألف" => "ألفان",
                                "مليون" => "مليونان",
                                "مليار" => "ملياران",
                                _ => groupText + " " + scaleText
                            };
                            scaleText = "";
                        }
                        else if (group > 2 && group < 11)
                        {
                            scaleText = scaleText switch
                            {
                                "ألف" => "آلاف",
                                "مليون" => "ملايين",
                                "مليار" => "مليارات",
                                _ => scaleText
                            };
                        }
                    }

                    if (!string.IsNullOrEmpty(scaleText))
                        groupText += $" {scaleText}";

                    parts.Insert(0, groupText);
                }

                number /= 1000;
                scaleIndex++;
            }

            return string.Join(" و", parts);
        }

        private static string ConvertGroup(int number)
        {
            var parts = new List<string>();

            int hundreds = number / 100;
            int remainder = number % 100;
            int tens = remainder / 10;
            int units = remainder % 10;

            if (hundreds > 0)
                parts.Add(Hundreds[hundreds]);

            if (remainder > 0)
            {
                if (remainder == 11)
                    parts.Add("أحد عشر");
                else if (remainder == 12)
                    parts.Add("اثنا عشر");
                else if (remainder > 12 && remainder < 20)
                    parts.Add($"{Units[remainder - 10]} عشر");
                else
                {
                    if (units > 0 && !(tens == 0 && (units == 1 || units == 2)))
                        parts.Add(Units[units]);
                    if (tens > 0)
                        parts.Add(Tens[tens]);
                }
            }

            return string.Join(" و", parts);
        }

        private static string GetCurrencyForm(long value, string singular, string dual, string plural)
        {
            if (value == 0)
                return singular;
            if (value == 1)
                return singular;
            if (value == 2)
                return dual;
            if (value >= 3 && value <= 10)
                return plural;
            return singular;
        }

     


        private string ApplyWrapper(string text)
        {
            if (_useAmountLimiter)
                return $"فقط {text} لا غير";
            return text;
        }

        private string ApplyFormalArabic(string text)
        {
            // Add tanwīn fatḥa to last currency words
            text = text.Replace(" دينار", " دينارًا");
            text = text.Replace(" فلس", " فلسًا");
            text = text.Replace(" ريال", " ريالًا");
            text = text.Replace(" هللة", " هللةً");
            text = text.Replace(" قرش", " قرشًا");
            text = text.Replace(" جنيه", " جنيهًا");
            text = text.Replace(" ليرة", " ليرةً");
            text = text.Replace(" درهم", " درهمًا");
            text = text.Replace(" دراهم", " دراهمًا");
            text = text.Replace(" سنت", " سنتًا");
            text = text.Replace(" سنتات", " سنتاتٍ");
            text = text.Replace(" دولار", " دولاراً");
            text = text.Replace(" يورو", " يوروًا");
            text = text.Replace(" جنيهًا إسترلينيًا", " جنيهًا إسترلينيًا");
            return text;
        }

        private string FormatUnit(string text)
        {
            return _useFormalArabic ? ApplyFormalArabic(text) : text;
        }
    }
}
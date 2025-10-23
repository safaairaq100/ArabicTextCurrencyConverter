using ArabicTextCurrencyConverter;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Xunit.Abstractions;

namespace ArabicCurrencyConverter.Tests
{
    public class ArabicCurrencyTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IArabicCurrencyService _converter;

        public ArabicCurrencyTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var services = new ServiceCollection();
            services.AddArabicCurrencyService();
            var provider = services.BuildServiceProvider();

            _converter = provider.GetRequiredService<IArabicCurrencyService>()
                .SetCurrencyForms("دينار", "ديناران", "دنانير", "فلس", "فلسان", "فلوس")
                .UseAmountLimiter(true);
        }

        [Fact(DisplayName = "Generate Arabic Currency Conversion Report")]
        public void GenerateCurrencyTestFile()
        {
            var tests = new (double value, string description)[]
            {
                (0, "Zero value"),
                (1, "Single"),
                (2, "Dual"),
                (3, "Few"),
                (11, "11–19 range"),
                (21, "Compound 21"),
                (99, "Tens compound"),
                (100, "Hundred"),
                (200, "Two hundred"),
                (300, "Three hundred"),
                (1000, "One thousand"),
                (2000, "Two thousand"),
                (5000, "Few thousands"),
                (11000, "Thousands 11"),
                (25000, "Thousands 25"),
                (125000, "Hundred-thousand"),
                (1000000, "One million"),
                (2000000, "Two millions"),
                (5500000, "Multi-million"),
                (1234567.89, "Full complex with decimal"),
                (1000000000, "One billion"),
                (2000000000, "Two billions"),
                (987654321.75, "All groups used"),
                (100.05, "Subunit <10"),
                (500.5, "Subunit in tens"),
                (100.75, "Subunit compound"),
                (1500.75, "Thousands + decimal"),
                (100000000000.99, "Upper range valid"),
                (1000000000000.00, "Overflow limit"),
                (0.5, "Fractional only"),
            };

            var sb = new StringBuilder();
            sb.AppendLine("Arabic Currency Converter Test Results");
            sb.AppendLine(new string('=', 60));
            sb.AppendLine();

            foreach (var test in tests)
            {
                string result = _converter.Convert(test.value);
                string line = $"{test.value,-15} → {result}";
                sb.AppendLine(line);
            }

            sb.AppendLine();
            sb.AppendLine("Custom Currency Examples:");
            sb.AppendLine("--------------------------");

            string saudi = _converter.Convert(
                1234.75,
                "ريال", "ريالان", "ريالات",
                "هللة", "هللتان", "هللات");
            sb.AppendLine($"1234.75 → {saudi}");

            string egypt = _converter.Convert(
                987654.32,
                "جنيه", "جنيهان", "جنيهات",
                "قرش", "قرشان", "قروش");
            sb.AppendLine($"987654.32 → {egypt}");

            var path = Path.Combine(AppContext.BaseDirectory, "CurrencyTestResults.txt");
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

            // Output for visibility during test runs
            Console.OutputEncoding = Encoding.UTF8;
            _testOutputHelper.WriteLine(sb.ToString());
            _testOutputHelper.WriteLine($"Test results saved to: {path}");

            // Assert that at least one key phrase exists
            Assert.Contains("دينار", sb.ToString());
            Assert.Contains("فقط", sb.ToString());

        }
    }
}

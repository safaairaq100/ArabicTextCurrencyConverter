# Arabic Currency Converter for .NET 8

### Convert numbers to grammatically correct Arabic text with currency units  
Supports **singular, dual, plural**, **formal Arabic (tanwīn)**, and **official formatting** such as  
> **"فقط ألف ومئتان وأربعة وثلاثون دينارًا وخمسة وسبعون فلسًا لا غير"**

---

## 📦 Overview

`ArabicTextCurrencyConverter` is a lightweight .NET 8 library that converts numeric values into **Arabic text currency strings**.  
It fully supports Arabic grammar rules (singular/dual/plural), Arabic scales (ألف / مليون / مليار), and integrates cleanly with **Dependency Injection (DI)**.

It’s ideal for:
- Invoices and financial systems  
- Banking, cheque, and contract printing  
- ERP, accounting, or payment platforms  

---

## 🚀 Features

✅ Converts any number (up to 999,999,999,999.999)  
✅ Arabic grammar support for:
   - Singular (دينار)
   - Dual (ديناران)
   - Plural (دنانير)
✅ Subunit support (فلس / هللة / قرش / …)  
✅ Optional **formal Arabic** mode → adds tanwīn (ً)  
✅ Optional **amount limiter** → adds “فقط ... لا غير”  
✅ Full **Dependency Injection** support  
✅ Configurable main & sub currencies per call  

---

## 💻 Installation

Add reference in your project:

```bash
dotnet add package ArabicTextCurrencyConverter
```
```csharp
using ArabicTextCurrencyConverter;
```

If using dependency injection (recommended for ASP.NET Core):

```csharp
using Microsoft.Extensions.DependencyInjection;
```

## Usage

### 1. Register the Service

Add the Arabic currency service to your DI container:

```csharp
services.AddArabicCurrencyService();
```

### 2. Resolve and Use the Service

```csharp
var currencyService = serviceProvider.UseArabicTextCurrency();
string arabicText = currencyService.Convert(123.45);
Console.WriteLine(arabicText); // Example output: "مئة و ثلاثة و عشرون دينار و خمسة و أربعون فلس"
```

### 3. Configuring Currency Units

#### Set default currency units

```csharp
currencyService.SetDefaultCurrency("ريال", "هللة");
```

#### Set singular, dual, and plural forms for main and subunits

```csharp
currencyService.SetCurrencyForms(
    singular: "جنيه",
    dual: "جنيهان",
    plural: "جنيهات",
    subSingular: "قرش",
    subDual: "قرشان",
    subPlural: "قروش"
);
```

### 4. Formal Arabic Output

Add tanwīn fatḥa to currency units for formal output:

```csharp
currencyService.UseFormalArabic(true);
```

### 5. Limiting Amount Output

Wrap output with "فقط ... لا غير":

```csharp
currencyService.UseAmountLimiter(true);
```

### 6. Using Three Decimal Places

By default, the service supports two decimal places. To support three:

```csharp
currencyService.SetDefaultCurrency("دينار", "فلس", useThreeDecimal: true);
```

## Example

```csharp
// Startup.cs or Program.cs
services.AddArabicCurrencyService();

// Somewhere in your code
var currencyService = serviceProvider.UseArabicTextCurrency()
    .SetDefaultCurrency("ريال", "هللة")
    .SetCurrencyForms("ريال", "ريالان", "ريالات", "هللة", "هللتان", "هللات")
    .UseFormalArabic(true)
    .UseAmountLimiter(true);

string result = currencyService.Convert(1023.75);
// Output: "فقط ألف و ثلاثة و عشرون ريالًا و خمسة و سبعون هللةً لا غير"
```

## API Reference

### IServiceCollection Extension

- `AddArabicCurrencyService()`: Registers the service as a singleton.

### IServiceProvider Extension

- `UseArabicTextCurrency()`: Resolves the currency service.

### ArabicCurrencyService Methods

- `Convert(double number)`: Converts a number to Arabic currency text using the configured units.
- `Convert(double number, string mainUnit, string mainUnitDual, string mainUnitPlural, string subUnit, string subUnitDual, string subUnitPlural)`: Converts using custom units for this call.
- `SetDefaultCurrency(string mainUnit, string subUnit, bool useThreeDecimal = false)`: Sets main and subunits plus decimal precision.
- `SetCurrencyForms(string singular, string dual, string plural, string subSingular, string subDual, string subPlural)`: Sets all forms of main/subunits.
- `UseFormalArabic(bool enable)`: Enables formal Arabic formatting.
- `UseAmountLimiter(bool enable)`: Wraps output with "فقط ... لا غير".

## Notes

- The service supports large numbers up to 999,999,999,999.99 (or .999 if three decimals enabled).
- If the number is zero, output will be "صفر {currency}".
- Formal Arabic modifies the currency word endings for proper grammar.

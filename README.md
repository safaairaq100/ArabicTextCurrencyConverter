# Arabic Currency Converter for .NET 8

### Convert numbers to grammatically correct Arabic text with currency units  
Supports **singular, dual, plural**, **formal Arabic (tanwīn)**, and **official formatting** such as  
> **"فقط ألف ومئتان وأربعة وثلاثون دينارًا وخمسة وسبعون فلسًا لا غير"**

---

## 📦 Overview

`ArabicCurrencyConverter` is a lightweight .NET 8 library that converts numeric values into **Arabic text currency strings**.  
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

## 🧱 Installation

Add reference in your project:

```bash
dotnet add package ArabicCurrencyConverter

# Banka API 🏦

Bu proje, C# ve .NET teknolojileri kullanılarak geliştirilmiş, veritabanı bağlantılı bir REST API uygulamasıdır. Amaç Entity Framework Core ve SQLite kullanarak "Code First" yaklaşımıyla temel bankacılık işlemlerini simüle etmektir.

## 🚀 Teknolojiler
- **Platform:** .NET (C#)
- **Veritabanı:** SQLite
- **ORM:** Entity Framework Core
- **Dokümantasyon:** Scalar UI

## ⚡ Özellikler
- ✅ Hesap oluşturma ve bakiye sorgulama
- ✅ Para yatırma ve çekme (Validation kuralları ile)
- ✅ Veritabanı kalıcılığı (Uygulama kapansa bile veriler saklanır)
- ✅ Dependency Injection ve Controller yapısı

## 🛠 Kurulum
Projeyi klonladıktan sonra terminalde şu komutu çalıştırarak veritabanını oluşturabilirsiniz:
dotnet ef database update
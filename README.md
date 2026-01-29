# Banka API (Web API + SQLite) 🏦

Bu proje, **.NET** ve **Entity Framework Core** kullanılarak geliştirilmiş, **İlişkisel Veritabanı (Relational Database)** yapısına sahip profesyonel bir RESTful API simülasyonudur.

Sadece bakiye takibi yapmakla kalmaz, hesap hareketlerini (Transaction History) kayıt altına alır ve güvenli veri yönetimi için **GUID** (Globally Unique Identifier) standartlarını kullanır.

## 🚀 Kullanılan Teknolojiler
- **Framework:** .NET Core / .NET 8
- **Veritabanı:** SQLite
- **ORM:** Entity Framework Core (Code-First)
- **Kimlik Yönetimi:** GUID (Benzersiz Kimlik Yapısı)
- **Dokümantasyon:** Scalar UI / Swagger

## ⚡ Temel Özellikler
- **🏗 İlişkisel Veri Modeli (One-to-Many):** Bir hesap ve ona bağlı çoklu işlem geçmişi yapısı kurgulanmıştır.
- **🛡 GUID Altyapısı:** Sıralı `int` ID'ler yerine, tahmin edilemez ve güvenli `GUID` yapısına geçilmiştir.
- **📜 İşlem Geçmişi (Transaction History):** Para yatırma ve çekme işlemleri tarihçesiyle birlikte veritabanında saklanır.
- **✅ DTO Mantığı:** API yanıtlarında döngüsel başvuruyu (Circular Reference) önleyen özel veri dönüşümü uygulanmıştır.

## 🔌 API Uç Noktaları (Endpoints)

| Metot | İstek Adresi (URL) | Açıklama |
| :--- | :--- | :--- |
| `GET` | `/api/Banka` | Hesap bilgilerini ve **geçmiş işlem dökümünü** getirir. |
| `POST` | `/api/Banka/yatir` | Bakiyeyi artırır ve `Para Yatırma` fişi keser. |
| `POST` | `/api/Banka/cek` | Bakiyeyi düşürür ve `Para Çekme` fişi keser. |

## 🛠 Kurulum ve Çalıştırma

Projeyi bilgisayarınıza indirdikten sonra veritabanını oluşturmak için terminalde şu komutları sırasıyla çalıştırın:

```bash
# 1. Gerekli paketlerin yüklü olduğundan emin olun
dotnet restore

# 2. Veritabanını ve tabloları oluşturun (Migration)
dotnet ef database update

# 3. Projeyi ayağa kaldırın
dotnet run
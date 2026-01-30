# 🏦 Banka API - .NET Web API

Modern bankacılık işlemleri için geliştirilmiş, JWT tabanlı güvenli kimlik doğrulama sistemi içeren RESTful API projesi.

## 🚀 Teknolojiler

- **.NET 10.0** - Web API Framework
- **Entity Framework Core** - ORM (Code-First)
- **SQLite** - Veritabanı
- **JWT (JSON Web Token)** - Kimlik Doğrulama
- **BCrypt** - Şifre Hashleme
- **Swagger/OpenAPI** - API Dokümantasyonu

## ⚡ Özellikler

### 🔐 Güvenlik
- JWT Bearer Token tabanlı kimlik doğrulama
- BCrypt ile güvenli şifre hashleme
- Role-based authorization (Müşteri/Admin)
- Swagger'da token desteği

### 🏗 Veri Modeli
- **Kullanıcılar** - Kimlik doğrulama ve yetkilendirme
- **Hesaplar** - Banka hesapları (GUID tabanlı)
- **İşlemler** - Para yatırma/çekme işlem geçmişi (One-to-Many ilişki)

### 📊 İşlevler
- Kullanıcı kaydı ve giriş sistemi
- Hesap oluşturma ve sorgulama
- Para transferi (Hesaplar arası)
- Para yatırma/çekme
- İşlem geçmişi takibi

## 🔌 API Endpoint'leri

### 🔓 Kimlik Doğrulama (Auth)
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | `/api/Auth/register` | Yeni kullanıcı kaydı |
| POST | `/api/Auth/login` | Kullanıcı girişi (JWT token alır) |

### 🏦 Banka İşlemleri (Protected)
| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| GET | `/api/Banka` | Tüm hesapları listele | 🔒 JWT |
| POST | `/api/Banka/hesap-olustur` | Yeni hesap aç | 🔒 JWT |
| POST | `/api/Banka/para-transfer` | Hesaplar arası transfer | 🔒 JWT |
| POST | `/api/Banka/yatir` | Para yatır | 🔒 JWT |
| POST | `/api/Banka/cek` | Para çek | 🔒 JWT |

## 📦 Kurulum

### 1️⃣ Projeyi İndirin
```bash
git clone <repo-url>
cd BankaApi/BankaApi
```

### 2️⃣ Bağımlılıkları Yükleyin
```bash
dotnet restore
```

### 3️⃣ Veritabanını Oluşturun
```bash
dotnet ef database update
```
> Not: Connection string `appsettings.json` içinde tanımlıdır (SQLite - `banka.db`)

### 4️⃣ Uygulamayı Çalıştırın
```bash
dotnet run
```

Uygulama şu adreste çalışacaktır: **http://localhost:5212**

## 🔧 Yapılandırma

`appsettings.json` dosyasında aşağıdaki ayarları yapılandırabilirsiniz:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=banka.db"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here"
  }
}
```

## 📝 Kullanım Örneği

### 1. Kullanıcı Kaydı
```http
POST /api/Auth/register
Content-Type: application/json

{
  "kullaniciAdi": "ahmet",
  "sifre": "123456"
}
```

### 2. Giriş Yapma
```http
POST /api/Auth/login
Content-Type: application/json

{
  "kullaniciAdi": "ahmet",
  "sifre": "123456"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

### 3. Hesap Oluşturma (Token Gerekli)
```http
POST /api/Banka/hesap-olustur
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "ad": "Ahmet",
  "soyad": "Yılmaz",
  "bakiye": 1000
}
```

### 4. Para Transferi
```http
POST /api/Banka/para-transfer
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "gonderenHesapId": "guid-here",
  "aliciHesapId": "guid-here",
  "tutar": 500
}
```

## 🗂 Proje Yapısı

```
BankaApi/
├── Controllers/
│   ├── AuthController.cs      # Kimlik doğrulama
│   └── BankaController.cs     # Banka işlemleri
├── Models/
│   ├── Kullanici.cs          # Kullanıcı entity
│   ├── Hesap.cs              # Hesap entity
│   └── Islem.cs              # İşlem entity
├── Dtos/
│   ├── AuthDtos.cs           # Auth DTO'ları
│   ├── HesapOlusturDto.cs    # Hesap DTO
│   └── ParaTransferiDto.cs   # Transfer DTO
├── Migrations/               # EF Core migrations
├── BankaDbContext.cs         # Database context
├── Program.cs                # Uygulama giriş noktası
└── appsettings.json          # Yapılandırma
```

## 🧪 Test (Swagger ile)

1. Uygulamayı başlatın: `dotnet run`
2. Swagger arayüzüne gidin: http://localhost:5212/swagger
3. `/api/Auth/register` ile kullanıcı oluşturun
4. `/api/Auth/login` ile token alın
5. Sağ üstteki **Authorize** 🔒 butonuna tıklayın
6. `Bearer [token]` formatında token'ı yapıştırın
7. Diğer endpoint'leri test edin

## 🛡 Güvenlik Notları

- Şifreler BCrypt ile hashlenip saklanır
- JWT secret key production'da güçlü olmalı
- Token süresi `ClockSkew = TimeSpan.Zero` ile ayarlanmıştır
- HTTPS kullanımı önerilir (Production)

## 📄 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

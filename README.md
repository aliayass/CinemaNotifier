# 🎬 CinemaNotifier - Elazığ Sinema Botu

## 📝 Hikayesi
Bir sinemasever olarak Elazığ'da üniversite okumanın kendine has zorlukları var. En büyük dertlerimden biri, heyecanla beklediğim bazı filmlerin şehre sadece 1-2 günlüğüne gelip gitmesiydi. Dersler, sınavlar derken bazen o karmaşada vizyona giren filmleri kontrol etmeyi unutuyordum ve çok istediğim filmleri kaçırıyordum.

"Bunu kaçırmamam lazım!" dediğim filmleri bir daha ıskalamamak için bu projeyi geliştirdim. Artık vizyonu sürekli takip etmeme gerek yok; bu bot benim yerime her gün kontrol ediyor ve Elazığ'a gelen filmleri bana bildiriyor. 🍿

## 🚀 Ne Yapıyor?
Bu bot, `sinemalar.com` üzerinden özellikle **Elazığ Cinepoint (Elysium Park)** sinemasını tarar.
- **Otomatik Kontrol:** Her gün öğlen **12:00** ve **16:00**'da vizyondaki filmleri kontrol eder.
- **Anında Bildirim:** Bulduğu film listesini Telegram üzerinden anında cebime gönderir.
- **Açılış Kontrolü:** Bot çalıştırıldığı an dahi bir kontrol yapıp durum raporu verir.

## 🛠️ Kullanılan Teknolojiler
Bu proje **.NET 9** kullanılarak geliştirilmiş bir **Worker Service** (Arka Plan Servisi) uygulamasıdır.

*   **Platform:** .NET 9.0 (C#)
*   **HtmlAgilityPack:** Web kazıma (Scraping) işlemleri için kullanıldı. Sinema sayfasındaki HTML yapısını analiz edip film isimlerini ayıklar.
*   **Telegram.Bot:** Telegram API ile iletişim kurmak ve bildirim göndermek için kullanıldı.
*   **Microsoft.Extensions.Hosting:** Uygulamanın bir arka plan servisi olarak stabil çalışmasını sağlar.

## ⚙️ Kurulum ve Çalıştırma

### Gereksinimler
*   .NET 9.0 SDK

### Yapılandırma
`appsettings.json` dosyasında Telegram ayarlarını yapmanız gerekir:
```json
  "Telegram": {
    "BotToken": "SENIN_BOT_TOKENIN",
    "TargetChatId": "SENIN_CHAT_ID"
  }
```

### Çalıştırma
Projeyi çalıştırmak için terminalde şu komutu vermeniz yeterli:
```powershell
dotnet run
```

---
*Elazığ'da artık film kaçırmak yok!* 🎥

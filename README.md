# EmailNotification Service

The **EmailNotification** service is a .NET 8 background worker that listens for email events via RabbitMQ and sends emails using an SMTP provider. It supports structured HTML emails and multiple attachments.

## ✨ Features

- 📬 Listens for `EmailRequestedEvent` on RabbitMQ
- 💌 Sends HTML emails using SMTP
- 📎 Supports multiple Base64-encoded file attachments
- 📃 Configurable via `appsettings.json`
- 🪵 Logs output using Serilog (to console + rolling log files)

## 🛠 Configuration

Example `appsettings.json` (with placeholder values):

```json
{
  "EVENTBUS": {
    "Provider": "RABBITMQ",
    "Servers": [ "localhost" ],
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "",
    "QueueName": "",
    "UseDurableExchange": true,
    "Retry": {
      "RequeueOnError": true
    }
  },
  "SmtpSettings": {
    "Host": "your-smtp-host.com",
    "Port": 587,
    "EnableSsl": true,
    "From": "noreply@yourdomain.com",
    "Username": "your-username",
    "Password": "your-password"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/email-notification-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  }
}
```

## 🚀 Running Locally

```bash
dotnet restore
dotnet build
dotnet run
```

> Make sure RabbitMQ is running and your SMTP credentials are valid.

## 📂 Project Structure

- `Models/` – Event handlers and models
- `Services/` – SMTP email sending logic
- `Program.cs` – Dependency injection and Serilog setup

## 🧪 Testing Locally

You can trigger a test email via:

```
POST /api/emailtest
Content-Type: multipart/form-data
```

Fields:
- `To`: recipient email
- `Subject`: email subject
- `Body`: message body (HTML)
- `Attachments`: (optional) file(s)

## 📄 License

This project is licensed under the [MIT License](LICENSE).
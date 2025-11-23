# 🍽️ Caterly - ASP.NET MVC Catering Platform

![ASP.NET MVC](https://img.shields.io/badge/ASP.NET-MVC%20Web%20App-purple)
![.NET Core](https://img.shields.io/badge/.NET-6.0-blue)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red)
![Web Application](https://img.shields.io/badge/Platform-Web%20App-green)

A complete **ASP.NET MVC Web Application** for online catering services with multi-role authentication, booking system, and real-time messaging.

## ✨ Features

### 👥 User Features
- **User Registration & Login** - Secure authentication system
- **Browse Caterers** - Search and filter by categories
- **Book Services** - Easy reservation system
- **Message Caterers** - Direct communication
- **Manage Profile** - Update personal information

### 🏢 Caterer Features  
- **Menu Management** - Add/Edit menu items
- **Booking Management** - Handle customer reservations
- **Customer Messages** - Respond to inquiries
- **Business Profile** - Manage service details

### ⚙️ Admin Features
- **Category Management** - Organize service categories
- **User Management** - Oversee all platform users
- **Booking Monitoring** - Track all reservations
- **System Analytics** - Platform overview

## 🛠️ Built With

- **Framework**: ASP.NET Core MVC
- **Backend**: C# .NET 6
- **Database**: SQL Server + Entity Framework Core
- **Frontend**: Razor Views, Bootstrap, JavaScript
- **Authentication**: ASP.NET Core Identity

## 🚀 Quick Start

### Prerequisites
- .NET 6.0 SDK
- SQL Server
- Visual Studio 2022

### Installation & Run
```bash
# Clone the repository
git clone https://github.com/zainish24/caterly-mvc-webapp.git

# Navigate to project directory
cd caterly-mvc-webapp

# Update connection string in appsettings.json
# Run database migrations
dotnet ef database update

# Launch the application
dotnet run
```

## 📁 Project Structure
```
Caterly-MVC/
├── Controllers/          # MVC Controllers
├── Models/              # Data Models & Entities
├── Views/               # Razor Pages
├── Data/                # DbContext & Migrations
├── wwwroot/             # Static Files (CSS, JS, Images)
└── Services/            # Business Logic Layer
```

## ⚙️ Configuration

1. **Update database connection** in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CaterlyDB;Trusted_Connection=true;"
  }
}
```

2. **For development**, create `appsettings.Development.json` with your local SQL Server details

## 🎯 Key Features

- **Multi-Role System** (User, Caterer, Admin)
- **MVC Architecture** - Clean separation of concerns
- **Entity Framework** - Efficient data management
- **Responsive UI** - Works on all devices
- **Secure Authentication** - ASP.NET Identity

---

**Repository**: `caterly-mvc-webapp`  
**Technology**: ASP.NET Core MVC Web Application  
**Database**: SQL Server with Entity Framework  

**Built with ❤️ using ASP.NET MVC | Professional Catering Solution**
**⭐ If this project helps you, please give it a star!**

---

This README clearly indicates it's an **ASP.NET MVC Web Application** while being professional, concise, and highlighting all key features! 🚀

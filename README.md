# RoleBasedAuthSystem

A modern, secure, role-based authentication & authorization system built with **ASP.NET Core MVC (.NET 8 / .NET 9)**, **Entity Framework Core**, **SQL Server**, and **ASP.NET Core Identity**.

Supports three roles: **SuperAdmin**, **Admin**, and **User**, with full role-based access control, profile management, admin tools, dark/light mode, calendar UI, external login (Google & Microsoft), animated UI elements, and more.

## ✨ Features

### Core Authentication & Authorization
- Registration, Login, Logout, Password hashing
- Role-based authorization (`[Authorize(Roles = "...")]`)
- Three built-in roles: SuperAdmin, Admin, User
- Automatic seeding of roles + default SuperAdmin user
- Role-based redirection after login (e.g. SuperAdmin → /SuperAdmin/Dashboard)

### User & Profile Management
- User profile (Full Name + Avatar upload)
- Avatar displayed in navbar when logged in
- Change password, profile picture update

### Admin / SuperAdmin Tools
- Manage Users (view, edit, lock/unlock, delete)
- Search + pagination on user list
- Lock/unlock user accounts (with confirmation)
- Stats cards on SuperAdmin dashboard (total users, role distribution, new this month, locked accounts)
- Calendar UI (FullCalendar) for events, maintenance, tasks

### Modern & Animated UI
- Glassmorphism design + animated gradient background
- Dark / Light mode toggle (persists in localStorage)
- Staggered card entrance animations
- Animated Floating Action Button (FAB)
- Toast notifications, skeleton loading, hover effects
- Responsive layout with Bootstrap 5 + Bootstrap Icons

### External Authentication
- Google OAuth login
- Microsoft Account login (personal accounts)
- Easy to extend to Microsoft Entra ID (Azure AD)

### Security
- Password policies, lockout after failed attempts
- Anti-forgery tokens, HTTPS enforced
- External login auto-provisioning (new users created automatically)

## Tech Stack

- Backend: ASP.NET Core MVC (.NET 8 / .NET 9)
- Database: SQL Server + Entity Framework Core
- Authentication: ASP.NET Core Identity
- Frontend: Bootstrap 5, Bootstrap Icons, FullCalendar v6
- Animations: Pure CSS + Intersection Observer + minimal JavaScript

## Getting Started

### Prerequisites

- .NET 8 SDK (or .NET 9)
- SQL Server (LocalDB or full instance)

### Installation

1. Clone the repository

```bash
git clone https://github.com/yourusername/RoleBasedAuthSystem.git
cd RoleBasedAuthSystem

RoleBasedAuthSystem/
├── Controllers/            → Account, Admin, SuperAdmin, User, Calendar...
├── Data/                   → ApplicationDbContext.cs
├── Models/                 → ApplicationUser, ViewModels (Login, Profile, etc.)
├── Services/               → Seeder.cs (roles + default user)
├── Views/                  → Account, Admin, SuperAdmin, User, Shared/_Layout
├── wwwroot/                → css, js, images/default-avatar.png
├── Program.cs              → Main entry point + configuration
└── appsettings.json        → Connection strings + secrets


{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=RoleBasedAuthDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id.apps.googleusercontent.com",
      "ClientSecret": "GOCSPX-your-google-client-secret"
    },
    "Microsoft": {
      "ClientId": "your-microsoft-client-id",
      "ClientSecret": "your-microsoft-client-secret"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}

Entity framework

dotnet ef migrations add InitialCreate
dotnet ef database update

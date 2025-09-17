# 🎬 Movie Ticket Booking – Frontend (ASP.NET MVC)

This is the **frontend of the Movie Ticket Booking System**, built using **ASP.NET MVC (.NET 6)** with clean separation of Admin and User areas.  
The system allows users to browse movies, book tickets, and manage bookings, while admins can manage theatres, screens, movies, genres, and showtimes.

---

## ✨ Features

### 👤 User
- Register / Login (JWT-based authentication)
- Browse all movies with search & filtering
- Book tickets for available showtimes
- View & manage own bookings
- Responsive UI with modern styling

### 🛠️ Admin
- Dashboard for quick insights
- Manage Theatres, Screens, and Showtimes
- Add, edit, delete Movies with genres
- View bookings and user activities
- Role-based access (Admin / User)

---

## 🏗️ Tech Stack

- **Framework:** ASP.NET MVC 6
- **Frontend:** Razor Views, Custom CSS, Bootstrap-free UI
- **Backend API:** .NET Web API (separate repo / project)
- **Database:** SQL Server (via EF Core)
- **Authentication:** JWT (JSON Web Token)
- **IDE:** Visual Studio 2022

---

## 📂 Project Structure
Movie_management_system/
├── Areas/
│ ├── Admin/ # Admin controllers, views
│ └── User/ # User controllers, views
├── Controllers/ # Common controllers (Login, Register, etc.)
├── DTOs/ # Data Transfer Objects
├── wwwroot/ # Static assets (css, js, images)
├── Movie_management_system.csproj
└── Program.cs


---

## 🚀 Getting Started

### Prerequisites
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- .NET 6 SDK
- SQL Server

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/Prit-Kanani/MovieTicketBooking_Frontend.git
Open the solution in Visual Studio 2022

Restore dependencies:
dotnet restore

Update database (if EF migrations are used):

dotnet ef database update

Run the project:

dotnet run


Roadmap

 Implement seat selection in booking

 Add payment gateway integration

 Add movie reviews & ratings

 Improve UI/UX with animations

🤝 Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss.

📜 License

This project is licensed under the MIT License – feel free to use and modify.


---

⚡ Question:  
Do you also want me to create a **`README.md` file inside the repo automatically** (so you just commit it), or do you wan

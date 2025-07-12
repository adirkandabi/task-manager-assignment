# TaskManagerAPI

This is a simple RESTful API for managing projects and tasks, built with **ASP.NET Core**.  
It supports user authentication via **AWS Cognito**, and implements role-based access control (admin/user).

## Tech Stack

- **Backend**: C# (.NET 8 Web API)
- **Database**: SQL Server (via Entity Framework Core)
- **Authentication**: AWS Cognito (User Pool)
- **Testing**: xUnit, Moq, FluentAssertions
- **Logging & Error Handling**: Built-in middleware & try-catch

---

## Features

### Core Functionality

- **User Authentication** (via AWS Cognito)
- **Projects**: Create, Read, Update, Delete
- **Tasks**: Create, Read, Update, Delete
- **Pagination** support on list endpoints
- **Role-based Access Control** (admin vs regular user)

### Unit Testing

All core logic is fully tested:

- `ProjectService` and `TaskService` have full unit test coverage
- `ProjectRepository` and `TaskRepository` are tested using in-memory EF Core database

---

## Getting Started

### Run Locally

```bash
git clone https://github.com/adirkandabi/task-manager-assignment.git
cd TaskManagerAPI
dotnet build
dotnet run
```

# Employee Management System

A full-stack web application for managing employees using Angular 19 (frontend) and ASP.NET Core .NET 8 Web API (backend).

---

## Backend Setup (.NET 8 API)

### 1. Prerequisites

* Visual Studio 2022
* SQL Server or SQL Server Express
* .NET 8 SDK

### 2. Configuration

1. **Clone the Repository**:

   ```
   git clone https://github.com/EchoSNS/Employee-Management-System.git
   ```

2. **Update Connection String**:
   In `appsettings.json` & `appsettings.Development.json`:

   ```
   "ConnectionStrings": {
     "DefaultConnection": "Server=CHANGE_TO_YOUR_SERVERNAME;Database=EmployeeManagementDB_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
   }
   ```

3. **Apply Migrations**:

   ```
   Add-Migration InitialCreate
   Update-Database
   ```

4. **Run the API**:

   The API should be accessible at: `https://localhost:49531`

---

## Frontend Setup (Angular 19)

### 1. Prerequisites

* Node.js ≥ v18.19.0
* Angular CLI v19

### 2. Installation & Setup

1. **Install Dependencies**:

   ```
   npm install
   ```

3. **Update Environment File**:
   In `src/environments/environment.ts`:

   ```ts
   export const environment = {
     production: false,
     apiUrl: 'https://localhost:7072'
   };
   ```

### 3. Run the Angular App

```
ng serve
```

Open in browser: [http://localhost:4200](http://localhost:49531)

---

## API Endpoints Summary

| Method | Endpoint              | Description         |
| ------ | --------------------- | ------------------- |
| GET    | `/api/employees`      | Get all employees   |
| GET    | `/api/employees/{id}` | Get employee by ID  |
| POST   | `/api/employees`      | Create new employee |
| PUT    | `/api/employees/{id}` | Update employee     |
| DELETE | `/api/employees/{id}` | Delete employee     |

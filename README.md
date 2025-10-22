# TodoApp

A full-stack To-Do List application built with **React** (frontend), **ASP.NET Core Web API** (backend), and **MongoDB Atlas** (database).

---

## Features

- Add, edit, delete, and view to-do items
- Prevents duplicate titles
- Status tracking with enforced transitions:  
  `Pending → In Progress → Completed`
- Filter todos by status
- Responsive UI (desktop and mobile)
- Error handling for duplicate titles and invalid status transitions

---

## Tech Stack

- **Frontend:** React (TypeScript, Hooks)
- **Backend:** ASP.NET Core Web API (C#)
- **Database:** MongoDB Atlas (Free Tier)

---

## Getting Started

### Prerequisites

- Node.js (v18+ recommended)
- .NET 8 SDK
- MongoDB Atlas account

---

### 1. Clone the Repository

```sh
git clone https://github.com/patelseth/TodoApp.git
cd TodoApp
```

---

### 2. Setup MongoDB Atlas

- Create a free cluster at [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
- Create a database and collection for todos
- Get your connection string

---

### 3. Configure the Backend

- In `backend/Infrastructure/` (or via environment variables), set your MongoDB connection string and other configuration.

---

### 4. Run the Backend

```sh
cd backend/src
dotnet run
```
The API will run on `http://localhost:5000`.

---

### 5. Run the Frontend

```sh
cd frontend/src
npm install
npm start
```
The React app will run on `http://localhost:3000`.

---

### 6. API Proxy

The React app is configured to proxy API requests to the backend.  
No extra setup is needed if both servers are running locally.

---

## Testing

Backend tests are located in `backend/tests/`, parallel to `backend/src/`.

Run backend tests:

```sh
cd backend/tests
dotnet test
```

---

## Project Structure

```
TodoApp/
├── backend/
│   ├── src/
│   │   ├── Api/                # ASP.NET Core Web API
│   │   ├── Application/        # Application logic and requests
│   │   ├── Domain/             # Domain entities and exceptions
│   |   ├── Infrastructure/     # Configuration and infrastructure
│   └── tests/                  # Backend tests
├── frontend/
│   └── src/
│       ├── api/            # API calls
│       ├── components/     # React components
│       ├── hooks/          # Custom hooks
│       ├── models/         # TypeScript models
│       ├── pages/          # Page components
│       ├── styles/         # CSS
│       ├── App.tsx
│       └── index.tsx
├── README.md
└── ...
```

---

## Status Transitions

- Only these transitions are allowed:
  - `Pending` → `In Progress`
  - `In Progress` → `Completed`
- All other transitions are rejected and covered by backend tests.

---

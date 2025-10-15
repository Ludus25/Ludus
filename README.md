# Ludus


Ludus is a full-stack web application consisting of a **.NET backend** and a **React frontend**.  
The project is fully containerized with Docker for easy setup and deployment.

---

## 🧩 Backend

The backend is composed of multiple **.NET 8 microservices** that handle the core business logic of the system.  
It uses **MySQL**, **PostgreSQL**, and **Redis** as databases and caching layers.

### ▶️ Run backend with Docker

To start the backend services, run the following command from the root directory:

```bash
docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up -d --build
```

This will build and start all backend containers in detached mode.

---

## 🎨 Frontend

The frontend is built with **React** and communicates with the backend via **REST APIs** and **WebSockets**.  
It provides the user interface and real-time features for the Ludus platform.

### ▶️ Run frontend locally

1. Navigate to the frontend directory  
2. Install all dependencies:

   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm run dev
   ```
The application will be available at [http://localhost:5173](http://localhost:5173) (or as configured).

---

## ⚙️ Tech Stack

- **Backend:** .NET 8 (microservices), MySQL, PostgreSQL, Redis, Docker  
- **Frontend:** React, Vite  
- **Communication:** REST API, WebSockets  
- **Containerization:** Docker Compose  

---

## 🚀 Getting Started

1. Clone the repository  
2. Start the backend using Docker Compose  
3. Run the frontend locally  
4. Enjoy exploring **Ludus** 🎮

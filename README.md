# TaskManager API — Learning Project

This repo is a **practice project** to learn software testing in .NET Core:

* **Unit tests** (isolated business logic)
* **Integration tests** (real API + DB)
* **Load tests** (simulating concurrent users)

The goal isn’t production readiness — it’s to get hands-on with the full testing pipeline.

---

## 1. Project Structure

```
TaskManager.sln
│
├── TaskManager.Api           # Minimal ASP.NET Core Web API
│   ├── Data/AppDbContext.cs  # EF Core DbContext
│   └── Program.cs            # Endpoints, DB setup, Swagger
│
├── TaskManager.Domain        # Domain model & services
│   ├── Models/TaskItem.cs
│   └── Services/TaskService.cs
│
├── TaskManager.Tests.Unit        # Unit tests (xUnit + FluentAssertions + Moq)
│   └── TaskServiceTests.cs
│
├── TaskManager.Tests.Integration # Integration tests (API + in-memory SQLite)
│   ├── CustomWebApplicationFactory.cs
│   └── TasksApiTests.cs
│
└── loadtest
    └── tasks_k6.js           # k6 load test script
```

---

## 2. Running the API

```bash
dotnet run --project TaskManager.Api
```

Swagger UI will be available at [http://localhost:5000/swagger](http://localhost:5000/swagger).

> ℹ️ The API uses SQLite by default (`tasks.db`). Tables are auto-created on startup.

---

## 3. Unit Testing Guidelines

* **Scope:** One class/method at a time, no DB, no network.
* **Tooling:** xUnit + FluentAssertions (+ Moq for mocks).
* **Examples:**

  * Validate task creation rules (`TaskService`).
  * Check edge cases: empty title, duplicate title.

Run unit tests:

```bash
dotnet test TaskManager.Tests.Unit
```

**Tips:**

* Tests should be fast (<100ms).
* Avoid dependencies: mock or pass in delegates.
* Name tests clearly: `MethodName_Should_DoSomething_When_Condition`.

---

## 4. Integration Testing Guidelines

* **Scope:** Multiple components working together (API + DB).
* **Tooling:** `WebApplicationFactory<Program>` + in-memory SQLite.
* **Examples:**

  * POST a task, then GET it by ID.
  * GET all tasks returns a list.

Run integration tests:

```bash
dotnet test TaskManager.Tests.Integration
```

**Tips:**

* Use a shared in-memory SQLite connection so schema persists.
* Call `EnsureCreated()` in test setup.
* Focus on **end-to-end behavior** (status codes, JSON payloads).

---

## 5. Load Testing Guidelines

* **Scope:** System under stress (many users, concurrent requests).
* **Tooling:** [k6](https://k6.io/).
* **Script:** [`loadtest/tasks_k6.js`](./loadtest/tasks_k6.js).

Run the API first, then in another terminal:

```bash
k6 run loadtest/tasks_k6.js --env BASE_URL=http://localhost:5000
```

**Tips:**

* SQLite isn’t great under concurrency → expect some `database is locked` errors.
* That’s a *feature*, not a bug — it shows DB bottlenecks.
* For “serious” load testing, switch to Postgres or SQL Server.
* Adjust VUs (virtual users) and thresholds in the script to simulate scenarios.

---

## 6. Common Issues & Fixes

* **`no such table: Tasks`**
  → Call `EnsureCreated()` on startup or run EF Core migrations.

* **`database is locked`** (SQLite)
  → Reduce concurrent writes in load tests, enable WAL + busy\_timeout, or use a server DB.

* **Long-running tests**
  → Use `AsNoTracking()` for read queries in EF to reduce overhead.

---

## 7. Learning Checklist

* [ ] Run unit tests, add a new rule (e.g. max title length), and test it.
* [ ] Write an integration test for DELETE endpoint.
* [ ] Run k6 with 50 VUs, note failure rate.
* [ ] Switch from SQLite to Postgres in Docker, rerun load test, compare results.
* [ ] Add CI (GitHub Actions) to run unit + integration tests on push.

---

## 8. Key Takeaways

* **Unit tests** = fast feedback on business logic.
* **Integration tests** = confidence API + DB + routing work together.
* **Load tests** = uncover bottlenecks and stability issues.

Remember: the point isn’t to “perfect” the API, but to **practice the testing workflow** end-to-end.

---

Happy testing 🚀

unit:
	dotnet test TaskManager.Tests.Unit -v minimal

integration:
	dotnet test TaskManager.Tests.Integration -v minimal

api:
	dotnet run --project TaskManager.Api

load:
	k6 run loadtest/tasks_k6.js --env BASE_URL=http://localhost:5135
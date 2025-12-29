# Скрипт для запуска всех тестов

Write-Host "Запуск тестов..." -ForegroundColor Green

dotnet test TestProject/TestProject.csproj --verbosity minimal --logger "console;verbosity=detailed"

Write-Host "`nТесты завершены!" -ForegroundColor Green

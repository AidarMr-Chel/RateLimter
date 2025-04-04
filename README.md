# 📘 API Gateway Rate Limiting — README

## 🧭 Общая цель проекта

Разработка отказоустойчивого API Gateway с гибким механизмом ограничения входящего трафика (rate limiting). Система должна уметь фильтровать трафик по IP, региону, user-agent и другим параметрам, а также визуализировать текущую нагрузку и управляться через удобную админ-панель без перезапуска.

## 📌 Ключевые возможности

- Rate Limiting с фильтрами
- Circuit Breaker + Retry Logic
- Админ-панель для управления лимитами и логами
- Redis — хранилище счётчиков и конфигурации
- REST API для сбора метрик и логов
- Frontend: Dashboard + Log Viewer + Limit Admin
- Нагрузочное тестирование через Load Simulator

## 📈 Сценарии использования

### Сценарий 1: Защита от DDoS

Gateway блокирует злоумышленника по IP/региону, возвращает 429 и не нагружает backend.

### Сценарий 2: Онлайн-редактирование лимитов

Админ снижает лимит по региону через UI без рестарта Gateway.

### Сценарий 3: Тестирование под нагрузкой

Разработчик запускает симулятор → Gateway регулирует трафик → админка показывает поведение системы.

---

## 🧱 Архитектура и компоненты

### 1. Load Simulator

- Генерирует нагрузку на Gateway
- Настраиваемый rps, регион, длительность

### 2. API Gateway

- Middleware: Rate Limiting, Retry, Circuit Breaker
- Прокси через HttpClient

### 3. Rate Limit Config

- JSON-конфиг → Redis → Live-обновление

### 4. Redis

- INCR + EXPIRE для счётчиков
- Хранение конфигов

### 5. Logging & Metrics

- Serilog → файл (последние 1000 записей)
- Содержит: время, IP, регион, статус, задержка

### 6. MonitoringService

- REST API: `/logs`, `/metrics`, `/limits`
- CRUD по лимитам, агрегация логов

### 7. Frontend (React Admin Panel)

- Dashboard (графики по RPS)
- Log Viewer (таблица логов)
- Limit Admin (изменение лимитов)

---

## ✅ MVP: Реализуемое

- Rate Limiting (2 стратегии)
- Retry Logic / Circuit Breaker
- JSON-конфиг + Redis
- Симулятор нагрузки (.NET Console App)
- REST API + UI для управления
- README + DESIGN.md + архитектурная схема

---

## 🚀 Расширение функционала (в будущем)

- Балансировка между несколькими Gateway
- Полезная нагрузка (настоящий backend)
- Redis репликация, отказоустойчивость
- Адаптивные лимиты (на основе метрик)
- Prometheus + Grafana
- БД для логов и метрик
- Поддержка 4 стратегий лимитирования

---

## 🧠 Выбор технологий

| Компонент   | Технология                       |
| ----------- | -------------------------------- |
| Backend     | .NET 8 / ASP.NET Core            |
| Frontend    | React + Recharts                 |
| Хранилище   | Redis                            |
| Логирование | Serilog                          |
| Метрики     | Prometheus + Grafana (в будущем) |
| Конфиг      | JSON                             |
| Нагрузка    | .NET Console App                 |

---

## 🛠 Конкретная реализация

- `RateLimitingMiddleware` в Gateway
- `IRateLimitingStrategy` (расширяемо)
- `IRateLimitStore` (обертка над Redis)
- REST API для логов и лимитов
- React SPA с компонентами: Dashboard, Log Viewer, Limit Editor

---

## 🧩 Расширяемость

- Подключаемые стратегии RL через интерфейс
- Возможность заменить Redis (интерфейс `IRateLimitStore`)
- Плагинная Middleware-архитектура
- JWT / API Key (в будущем)

---

## 🧪 Тестирование

- Unit-тесты на Rate Limiting, Retry, CB
- Интеграционные тесты симуляции
- Стресс-тесты (до 10.000 RPS)

---

## 🔐 Безопасность *(в продвинутой версии)*

- Авторизация в админке (JWT, роли)
- HTTPS-only
- Защита REST API от неавторизованных вызовов
- Валидация данных на всех уровнях

---

## 🚀 CI/CD и деплой

- `docker-compose.yml` для запуска Redis + Gateway + MonitoringService
- README-инструкция по сборке и запуску
- GitHub Actions (в будущем): build, test, push Docker image

---

## 📚 Документация

- `README.md` — текущий документ
- `DESIGN.md` — внутренняя архитектура, схемы и описания
- Архитектурная схема (draw\.io или png)
- Демонстрационный сценарий (описан в README)

---

## 🔧 Как запустить

```bash
# Сборка проекта
cd Gateway && dotnet build
cd MonitoringService && dotnet build

# Запуск Redis + сервисов
docker-compose up

# Запуск симулятора нагрузки
cd LoadSimulator
./LoadSimulator --rps 500 --duration 30 --region us-east
```

---

## 🧑‍💻 Контакты / авторство

Проект разработан в рамках учебного задания по .NET. Авторы: [].


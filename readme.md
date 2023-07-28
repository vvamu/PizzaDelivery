# PizzaDelivery. ASP WebAPI project. - GIT

**WebApi to delivery pizza.** 

---

Database: WebAPI - MsSQL , Web App - PSQL.

**Project project placed by Docker-Compose.**

**Roles:** - Admin can execute CRUD operations with Pizza, Promocodes. Check info about Users, Orders.
           - User can execute CRUD operations with ShoppingCartItem, create Order and add Promocode.

---

**Project components:**

- EmailProvider - Library import Service to send emails
- ExternalService - Library import Service to add ExternalConnections to user (Google, Facebook, Vkontakte)
- PizzaDelivery.Application - Library import Services to work with DB
- PizzaDelivery.Domain - Library import DB models
- PizzaDelivery.Persistence - Library import configuration DB
- PizzaDelivery - Web App project
- PizzaDeliveryApi - WebAPI project
- PizzaDelivery.Tests - XUnit tests to WebAPI

---

**Docker-compose components**

- pizza-delivery-api
- pizza-delivery-app
- sql-server-db
- postgres_db

---

Tags: Microservice architecture, Email API, Vkontakte API - Oauth2, Google API, Facebook API, Docker, Docker-compose, AutoMapper, Pagination, Option Pattern, FluentValidation, HTTPS, SQL Queries, Serilog, HostedServices, ExceptionHandlingMiddleware, XUnit

---

To check project:
1. clone this repository
2. open cmd in project folder
3. enter - docker-compose build
4. enter - docker-compose up

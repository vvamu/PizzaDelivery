# PizzaDelivery. ASP WebAPI project.

WebApi to delivery pizza. 

Database: Sqlite.
**Roles:** - Admin can execute CRUD operations with Pizza, Promocodes. Check info about Users, Orders.
           - User can execute CRUD operations with ShoppingCartItem, create Order and add Promocode.
**Project components:**

- PizzaDelivery.Application - Library import Services to work with DB
- PizzaDelivery.Domain - Library import DB models
- PizzaDelivery.Persistence - Library import configuration DB
- PizzaDelivery - Web App project
- PizzaDeliveryApi - WebAPI project
- PizzaDelivery.Tests - Tests to WebAPI
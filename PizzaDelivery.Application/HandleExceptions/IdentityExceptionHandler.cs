//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace PizzaDelivery.Application.HandleExceptions;
//public class IdentityExceptionHandler : IExceptionHandler
//{
//    public IActionResult HandleException(ExceptionContext context)
//    {
//        if (context.Exception is IdentityException identityException)
//        {
//            // Handle the Identity exception and return an appropriate response
//            // You can customize the response based on the specific exception type

//            // Example: Return a BadRequest response with the exception message
//            return new BadRequestObjectResult(identityException.Message);
//        }

//        // If it's not an Identity exception, let other exception handlers handle it
//        return null;
//    }
//}
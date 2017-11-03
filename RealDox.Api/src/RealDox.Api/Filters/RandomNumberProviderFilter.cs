using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RealDox.Api.Services;

namespace RealDox.Api.Filters
{
    public class RandomNumberProviderFilter : IActionFilter
    {
        private readonly RandomNumberService _randomService;

        public RandomNumberProviderFilter(RandomNumberService random)
        {
            _randomService = random;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.ActionArguments["randomNumber"] = _randomService.GetRandomNumber();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}

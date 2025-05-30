using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace project.Models.Services
{
    public class BudgetService
    {
        private readonly IConfiguration _configuration;
        public BudgetService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Domain.Constants
{
    public static class CurrencyConstants
    {
        public static readonly HashSet<string> AcceptedCurrencies = new()
        {
            "EUR", "USD", "GBP", "CHF"
        };
    }
}

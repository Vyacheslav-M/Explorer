using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string Name { get; set; }
        public string Sort { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string name, string sort)
        {
            if (name == null)
            {
                Name = name;
            }
            else
            {
                if (Sort == sort)
                {
                    Name = name + "\\";
                }
                else
                {
                    Name = name;
                }
            }

            if (sort == null)
            {
                Sort = "up";
            }
            else if (sort == "up")
            {
                Sort = "down";
            }
            else if (sort == "down")
            {
                Sort = "up";
            }
        }
    }
}


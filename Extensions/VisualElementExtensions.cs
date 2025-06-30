using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeClient.Extensions
{
    public static class VisualElementExtensions
    {
        public static Page? GetParentPage(this Element element)
        {
            while (element != null)
            {
                if (element is Page page)
                    return page;

                element = element.Parent;
            }

            return null;
        }
    }
}

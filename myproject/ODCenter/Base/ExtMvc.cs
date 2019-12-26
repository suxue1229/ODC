using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ODCenter.Base
{
    public static class ExtMvc
    {
        public static HtmlString IconStatus(Boolean status)
        {
            return new HtmlString(
                String.Format("<span class=\"glyphicon glyphicon-md {0}\" style=\"color:{1};\"></span>",
                status ? "glyphicon-ok" : "glyphicon-remove", status ? "green" : "red"
                ));
        }

        public static HtmlString IconUser(String user)
        {
            return new HtmlString(String.Format("<span class=\"glyphicon glyphicon-md glyphicon-user\" title=\"{0}\"></span>", user));
        }

        public static SelectListItem[] SelectList(Type enumType)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (Enum val in Enum.GetValues(enumType))
            {
                items.Add(new SelectListItem()
                {
                    Text = val.Description(),
                    Value = val.ToString()
                });
            }
            return items.ToArray();
        }

        public static IEnumerable<SelectListItem> GetSelectList(this Guid[] institutes, Guid current)
        {
            return (from id in institutes
                    let institute = DbProvider.Institutes[id.ToString("N")]
                    where DbProvider.Institutes.ContainsKey(id.ToString("N"))
                    orderby institute.Name
                    select new SelectListItem
                    {
                        Text = institute.Name,
                        Value = institute.Id.ToString("N"),
                        Selected = institute.Id == current
                    });
        }
    }
}
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearServer.Razor
{
   public class CustomTemplate<T> : TemplateBase<T>
    {
        public new T Model
        {
            get { return base.Model; }
            set { base.Model = value; }

        }

        public CustomTemplate()
        {

        }
    }
}

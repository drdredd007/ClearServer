using System.Net;
using ClearServerCore.Core.Database;

namespace ClearServerCore.Core.Utils
{
    public class CustomTemplate<T>
    {
        public new T Model
        {
            get { return Model; }
            set { Model = value; }
        }
    }

    public class UserModel
    {
        public bool isAuth { get; set; }
        public User user { get; set; }
    }

    public class ErrorModel
    {
        public int Code;

        public string ErrorMessage => ((HttpStatusCode) this.Code).ToString();
    } 
}
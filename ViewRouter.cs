using System;
using System.Reflection;

//http://dotnetslackers.com/Community/blogs/haissam/archive/2007/07/25/Call-a-function-using-Reflection.aspx

namespace Maussoft.Mvc
{
    public class ViewRouter<TSession> where TSession : new()
    {
        string viewNamespace;

        public ViewRouter(string viewNamespace)
        {
            this.viewNamespace = viewNamespace;
        }

        private string Invoke(WebContext<TSession> context, Type routedClass)
        {
            View<TSession> view = Activator.CreateInstance(routedClass) as View<TSession>;
            if (view == null)
            {
                Console.WriteLine("ViewRouter: object {0} could not be created.", routedClass.FullName);
                return null;
            }
            Console.WriteLine("ViewRouter: object {0} was created.", routedClass.FullName);

            string result = view.Render(context);

            Console.WriteLine("ViewRouter: invoked {0}.Render()", routedClass.FullName);

            return result;
        }

        private string Match(WebContext<TSession> context, string prefix, string className)
        {
            Type routedClass = null;
            Assembly assembly = Assembly.GetEntryAssembly();

            Console.WriteLine("ViewRouter: try {0}.Render()", className);

            routedClass = assembly.GetType(prefix + '.' + className);
            if (routedClass == null)
            {
                Console.WriteLine("ViewRouter: class {0} does not exist.", prefix + '.' + className);
                return null;
            }
            Console.WriteLine("ViewRouter: class {0} found.", prefix + '.' + className);

            return this.Invoke(context, routedClass);
        }

        public string Route(WebContext<TSession> context)
        {
            if (context.Sent) return null;
            if (context.View == null) return null; //Route to 404?

            string[] parts = context.View.Split('.');
            string className = null;

            string prefix = viewNamespace;
            for (int i = parts.Length - 1; i >= 0; i--)
            {

                className = (String.Join(".", parts, 0, i) + '.' + parts[parts.Length - 1]).Trim('.');

                var result = this.Match(context, prefix, className);
                if (result != null)
                {
                    return result;
                }

            }
            return null;
        }

    }
}
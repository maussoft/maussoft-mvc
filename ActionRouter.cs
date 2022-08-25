using System;
using System.Reflection;
using System.Text.RegularExpressions;

//http://dotnetslackers.com/Community/blogs/haissam/archive/2007/07/25/Call-a-function-using-Reflection.aspx

namespace Maussoft.Mvc
{
    public class ActionRouter<TSession> where TSession : new()
    {
        string controllerNamespace;

        public ActionRouter(string controllerNamespace)
        {
            this.controllerNamespace = controllerNamespace;
        }

        private Boolean Invoke(WebContext<TSession> context, string className, Type routedClass, MethodInfo routedMethod, object[] parameters)
        {
            object routedObject = Activator.CreateInstance(routedClass);
            if (routedObject == null)
            {
                Console.WriteLine("ActionRouter: object {0} could not be created.", routedClass.FullName);
                return false;
            }
            Console.WriteLine("ActionRouter: object {0} was created.", routedClass.FullName);

            context.View = className + '.' + routedMethod.Name;
            context.Controller = className;
            context.Action = routedMethod.Name;
            routedMethod.Invoke(routedObject, parameters);
            Console.WriteLine("ActionRouter: invoked {0}.{1}(...)", routedClass.FullName, routedMethod.Name);

            return true;
        }

        private object[] GetParameters(WebContext<TSession> context, ParameterInfo[] parameterList, ArraySegment<string> arguments)
        {
            object[] parameters = new object[parameterList.Length];

            parameters[0] = context;
            for (int p = 1; p < parameterList.Length; p++)
            {

                if (arguments.Offset + p - 1 < arguments.Array.Length)
                {

                    string[] ints = new string[] { "System.Byte", "System.SByte", "System.Int32", "System.UInt32", "System.Int16", "System.UInt16", "System.Int64", "System.UInt64" };

                    try
                    {

                        if (parameterList[p].ParameterType.ToString() == "System.String")
                        {
                            parameters[p] = arguments.Array[arguments.Offset + p - 1];
                        }
                        else if (Array.IndexOf(ints, parameterList[p].ParameterType.ToString()) >= 0)
                        {
                            string value = arguments.Array[arguments.Offset + p - 1];
                            value = Regex.Replace(value, @"[^0-9]+", "");
                            parameters[p] = Convert.ChangeType(value, parameterList[p].ParameterType);
                        }
                        else
                        {
                            Console.WriteLine("ActionRouter: non-compatible type for argument #{0}: {1} ({2})", p, parameterList[p].Name, parameterList[p].ParameterType);
                            return null;
                        }

                    }
                    catch (System.FormatException)
                    {
                        Console.WriteLine("ActionRouter: conversion failed for argument #{0}: {1} ({2})", p, parameterList[p].Name, parameterList[p].ParameterType);
                        return null;
                    }

                    Console.WriteLine("ActionRouter: converted string to '{2}' for argument #{0}: {1}", p, parameterList[p].Name, parameterList[p].ParameterType);

                }
                else
                {
                    if (parameterList[p].IsOptional)
                    {
                        parameters[p] = parameterList[p].DefaultValue;
                        Console.WriteLine("ActionRouter: default value added for missing argument #{0}: {1}", p, parameterList[p].Name);
                    }
                    else
                    {
                        Console.WriteLine("ActionRouter: missing non-optional argument #{0}: {1}", p, parameterList[p].Name);
                        return null;
                    }
                }
            }

            return parameters;
        }

        private Boolean Match(WebContext<TSession> context, string prefix, string className, string methodName, ArraySegment<string> arguments)
        {
            Type routedClass = null;
            MethodInfo routedMethod = null;
            object[] parameters = null;
            ParameterInfo[] parameterList = null;
            Assembly assembly = Assembly.GetEntryAssembly();

            Console.WriteLine("ActionRouter: try {0}.{1}(...{2})", className, methodName, arguments.Count);

            routedClass = assembly.GetType(prefix + '.' + className);
            if (routedClass == null)
            {
                Console.WriteLine("ActionRouter: class {0} does not exist.", prefix + '.' + className);
                return false;
            }
            Console.WriteLine("ActionRouter: class {0} found.", prefix + '.' + className);

            routedMethod = routedClass.GetMethod(methodName);
            if (routedMethod == null)
            {
                Console.WriteLine("Routing: method {0}.{1} does not exist.", className, methodName);
                return false;
            }
            Console.WriteLine("ActionRouter: method {0}.{1} found.", className, methodName);

            parameterList = routedMethod.GetParameters();

            if (parameterList.Length < 1)
            {
                Console.WriteLine("ActionRouter: first argument not found");
                return false;
            }
            Console.WriteLine("ActionRouter: first argument found");

            if (parameterList[0].ParameterType != context.GetType())
            {
                Console.WriteLine("ActionRouter: first argument type mismatch {0} != {1}", parameterList[0].ParameterType, context.GetType());
                return false;
            }
            Console.WriteLine("ActionRouter: first argument type matches");

            if (arguments.Count > parameterList.Length - 1)
            {
                Console.WriteLine("ActionRouter: argument count too high: {0} > {1}", arguments.Count, parameterList.Length - 1);
                return false;
            }
            Console.WriteLine("ActionRouter: argument count not too high");

            parameters = GetParameters(context, parameterList, arguments);
            if (parameters == null)
            {
                Console.WriteLine("ActionRouter: invalid argument encountered");
                return false;
            }

            return this.Invoke(context, className, routedClass, routedMethod, parameters);
        }

        public Boolean Route(WebContext<TSession> context)
        {
            string[] parts = context.Url.TrimStart('/').Split('/');

            string className = null;
            string methodName = null;
            ArraySegment<string> arguments = new ArraySegment<string>();

            string prefix = this.controllerNamespace;
            for (int i = parts.Length; i >= 1; i--)
            {

                if (i < parts.Length)
                {
                    className = String.Join(".", parts, 0, i);
                    methodName = parts[i];
                    arguments = new ArraySegment<string>(parts, i + 1, parts.Length - (i + 1));

                    if (this.Match(context, prefix, className, methodName, arguments))
                    {
                        return true;
                    }
                }

                className = String.Join(".", parts, 0, i);
                methodName = "Index";
                arguments = new ArraySegment<string>(parts, i, parts.Length - i);

                if (this.Match(context, prefix, className, methodName, arguments))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
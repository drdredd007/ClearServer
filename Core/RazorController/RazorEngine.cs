using ClearServer;
using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClearServerCore.Core.RazorController
{
    public static class RazorEngine
    {
        public static RazorLightEngine Engine;

        public static void Init()
        {
            Engine = new RazorLightEngineBuilder().UseEmbeddedResourcesProject(typeof(Server)).UseMemoryCachingProvider().Build();
            Engine.Options.DynamicTemplates.Add("header", File.ReadAllText(@"C:\Users\drdre\source\repos\ClearServer\View\Layout\Header.cshtml"));
        }
    }
}

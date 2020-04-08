using System;
using System.Text.Json;


namespace Quarrel.Dummy
{
    public class Class1
    {
        public void M() 
        {
            var je1 = new JsonElement();
            var je2 = new JsonElement();
            var diffs = JsonDiff.OfElements(je1, je2);
        }

    }
}

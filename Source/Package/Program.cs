using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Package
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var path = @"C:\Users\Dingp\source\repos\Accelerider\Source\Build\Release";

            var fileTags = Directory
                .GetFiles(path)
                .Select(item => Path.GetFileName(item))
                .Select(item => $"<File Name=\"{item}\" />");

            var temp = string.Join(Environment.NewLine, fileTags);
        }
    }
}

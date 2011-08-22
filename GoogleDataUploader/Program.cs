using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;
using System.Diagnostics;

namespace GoogleDataUploader
{
    class Program
    {

        public Program(List<string> args)
        {
            Console.WriteLine("Google Data Uploader v1.0 - Pablo Carballude");
            if (args.Any(x => x == "--merge" || x == "-m"))
                Merge(args[1], args[2]);
            else if (args.Any(x => x == "--unmerge" || x == "-u"))
                UnMerge(args[1]);
            else if (args.Any(x => x == "--help" || x == "-h"))
                ShowHelp();
            else
                Console.WriteLine("\nUnrecognized option.\nUse --help for usage info.");
        }

        private void Merge(string head, string tail)
        {
            CreateZip(tail, tail + ".zip");            
            var file = TagLib.File.Create(head);
            file.Tag.Title = tail.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Last();
            file.Tag.Album = "Data";
            file.Tag.Performers = new string[] { "Google Data Uploader" };
            file.Save();            
            var headFileBytes = File.ReadAllBytes(head);            
            var headBytes = Enumerable.Range(0, _HEAD_LENGTH).Select(x => headFileBytes[x]);
            var tailBytes = File.ReadAllBytes(tail + ".zip"); ;
            File.WriteAllBytes(tail + ".mp3", headBytes.Concat(tailBytes).ToArray());
            File.Delete(tail + ".zip");
            Console.WriteLine("\nFile has been camouflaged! You can now upload it to Google Music ;)\n");
        }

        private void ExtractZip(string file)
        {
            Process zip = new Process();
            zip.StartInfo.FileName = "7za.exe";
            zip.StartInfo.Arguments = "x -y \"" + file + "\"";
            zip.Start();
            zip.WaitForExit();
        }

        private void CreateZip(string file, string name)
        {
            var zip = new ZipFile();
            zip.AddFile(file);
            zip.Save(name);
        }

        private static int _HEAD_LENGTH = 102400;

        private void UnMerge(string file)
        {
            ExtractZip(file);
            Console.WriteLine("\nFile has been revealed! Enjoy ;)\n");
        }

        public void ShowHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\n-m / --merge <mp3> <data>\tCreates a fake mp3 ready to be uploaded to Google Music");
            Console.WriteLine("\n-u / --unmerge <mergedFile>\tTakes a fake mp3 and extracts its data");
            Console.WriteLine("\n--help\t\t Shows this message");
        }

        static void Main(string[] args)
        {
            new Program(args.ToList());
        }
    }
}

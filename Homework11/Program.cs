namespace Homework11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] directories = ["c:\\Otus\\TestDir1", "c:\\Otus\\TestDir2"];

            foreach (var d in directories)
            {
                if (!Directory.Exists(d))
                {
                    Directory.CreateDirectory(d);
                }

                for (int i = 1; i <= 10; i++)
                {
                    string path = $"{d}\\File{i}";

                    File.Create(path).Dispose();

                    try
                    {
                        using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                        {
                            writer.WriteLine(Path.GetFileName(path));
                            writer.WriteLine(DateTime.Now);
                        }

                        using (StreamReader reader = new StreamReader(path))
                        {
                            var content = reader.ReadLine();
                            var dateTime = reader.ReadLine();
                            Console.WriteLine($"{Path.GetFullPath(path)}: {content} + {dateTime}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}

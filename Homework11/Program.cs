using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Homework11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] directories = ["c:\\Otus\\TestDir1", "c:\\Otus\\TestDir2"];

            foreach (var d in directories)
            {
                var directory = new DirectoryInfo(d);

                if (!directory.Exists)
                {
                    directory.Create();
                }

                for (int i = 1; i <= 10; i++)
                {
                    string path = $"{d}\\File{i}";

                    File.Create(path).Dispose();

                    try
                    {
                        using (StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                        {
                            if (HasWriteAccess(path))
                            {
                                writer.WriteLine(Path.GetFileName(path));
                                writer.WriteLine(DateTime.Now);
                            }   
                        }

                        using (StreamReader reader = new StreamReader(path))
                        {
                            if (HasReadAccess(path))
                            {
                                var content = reader.ReadLine();
                                var dateTime = reader.ReadLine();
                                Console.WriteLine($"{Path.GetFullPath(path)}: {content} + {dateTime}");
                            }      
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        static bool HasReadAccess(string filePath)
        {
            return HasAccess(filePath, FileSystemRights.ReadData);
        }

        // Method to check if the user has write access using ACLs
        static bool HasWriteAccess(string filePath)
        {
            return HasAccess(filePath, FileSystemRights.WriteData);
        }

        // Method to check if the user has specific access rights using ACLs
        static bool HasAccess(string filePath, FileSystemRights accessRight)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                AuthorizationRuleCollection rules = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));

                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (principal.IsInRole(rule.IdentityReference.Value))
                    {
                        if ((rule.FileSystemRights & accessRight) == accessRight)
                        {
                            if (rule.AccessControlType == AccessControlType.Allow)
                            {
                                return true; // User has the required access
                            }
                        }
                    }
                }
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false; // No access due to restricted permissions
            }
        }
    }
}

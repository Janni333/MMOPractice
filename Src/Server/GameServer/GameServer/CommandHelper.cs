using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class CommandHelper
    {
        public static void Run()
        {
            bool run = true;
            while (run)
            {
                Console.Write(">");
                string line = Console.ReadLine().ToLower().Trim();
                try
                {
                    string[] cmd = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    switch (cmd[0])
                    {
                        case "addexp":
                            AddExp(int.Parse(cmd[1]), int.Parse(cmd[2]));
                            break;
                        case "exit":
                            run = false;
                            break;
                        default:
                            Help();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }
                
            }
        }

        public static void Help()
        {
            Console.Write(@"
Help:
    exit                        Exit Game Server
    help                        Show Help
    addexp <charid> <exp>       Add Exp for certain char
");
        }

        public static void AddExp(int charid, int exp)
        {
            var cha = Managers.CharacterManager.Instance.GetCharacter(charid);
            if (cha == null)
            {
                Console.WriteLine("Char{0} not found", charid);
                return;
            }
            cha.AddExp(exp);
        }
    }
}

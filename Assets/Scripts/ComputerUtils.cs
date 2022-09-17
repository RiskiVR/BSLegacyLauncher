// From https://github.com/ComputerElite/ComputerUtils/blob/dd623c0b1d2c7063bea27b83558df7ecadc6d416/ComputerUtils/ComputerUtils.CommandLine.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComputerUtils.CommandLine
{
    public class ParsedCommand
    {
        public string absoluteApplicationPath = "";
        public string command = "";
        public string argsStr = "";
        public List<string> args = new List<string>();
        public bool success = false;
        public string commandParseError = "";

        public override string ToString()
        {
            return "execute " + absoluteApplicationPath + " with args:\n" + String.Join("\n", args);
        }

        public ParsedCommand(string command)
        {
            ParsedCommand c = parseCommand(command);
            this.absoluteApplicationPath = c.absoluteApplicationPath;
            this.command = c.command;
            this.argsStr = c.argsStr;
            this.args = c.args;
            this.success = c.success;
            this.commandParseError = c.commandParseError;
        }
        public ParsedCommand() { }

        public static ParsedCommand parseCommand(string command, bool removeFirstArgument = false, string location = "")
        {
            ParsedCommand c = new ParsedCommand();
            Console.WriteLine("Parsing " + command);
            int quotationMarks = 0;
            string currentCommand = "";
            foreach (string s in command.Split(' '))
            {
                string tmp = s;
                if (tmp.StartsWith("\""))
                {
                    string add = "\"";
                    if (quotationMarks == 0) tmp = tmp.Substring(1);
                    else add = "";
                    quotationMarks += (add + tmp).TakeWhile(ch => ch == '"').Count();
                }
                if (tmp.EndsWith("\""))
                {
                    quotationMarks -= tmp.Reverse().TakeWhile(ch => ch == '"').Count();
                    if (quotationMarks == 0) tmp = tmp.Substring(0, tmp.Length - 1);
                }
                currentCommand += (currentCommand == "" ? "" : " ") + tmp;
                if (quotationMarks == 0)
                {
                    c.args.Add(currentCommand);
                    currentCommand = "";
                }
                if (quotationMarks < 0)
                {
                    c.success = false;
                    c.commandParseError = "too many quotation marks";
                    return c;
                }
            }
            if (quotationMarks > 0)
            {
                c.success = false;
                c.commandParseError = "too little quotation marks";
            }
            else c.success = true;
            if (c.args.Count >= 1 && removeFirstArgument)
            {
                c.absoluteApplicationPath = (Path.IsPathRooted(c.args[0]) || File.Exists(c.args[0]) || File.Exists(c.args[0] + ".exe")) ? c.args[0] : location + (location.EndsWith("\\") ? "" : "\\") + c.args[0];
                c.command = c.args[0];
                c.args.RemoveAt(0);
                foreach (string s in c.args) c.argsStr += "\"" + s + "\" ";
            }
            return c;
        }
    }

    public class CommandLineArgument
    {
        public List<string> aliases = new List<string>();
        public bool isToggle = false;
        public string description = "";
        public string defaultValue = "";
        public string valueName = "value";
    }

    public class CommandLineCommandContainer
    {
        public List<CommandLineArgument> arguments = new List<CommandLineArgument>();
        public string[] parsedCommand = null;

        public CommandLineCommandContainer(string arguments)
        {
            parsedCommand = new ParsedCommand(arguments).args.ToArray();
        }

        public CommandLineCommandContainer(string[] arguments)
        {
            parsedCommand = arguments;
        }

        public void AddCommandLineArgument(List<string> aliases, bool isToggle = false, string description = "no description", string valueName = "value", string defaultValue = "")
        {
            CommandLineArgument a = new CommandLineArgument();
            a.aliases = aliases;
            a.isToggle = isToggle;
            a.description = description;
            a.valueName = valueName;
            a.defaultValue = defaultValue;
            arguments.Add(a);
        }

        public bool HasArgument(string name, bool ignoreCase = true)
        {
            foreach (string s in parsedCommand)
            {
                foreach (CommandLineArgument a in arguments)
                {
                    if (a.aliases.FirstOrDefault(x => (ignoreCase ? x.ToLower() : x) == (ignoreCase ? name.ToLower() : name)) != null && a.aliases.FirstOrDefault(x => (ignoreCase ? x.ToLower() : x) == (ignoreCase ? s.ToLower() : s)) != null) return true;
                }
                if (ignoreCase && s.ToLower() == name.ToLower() || s == name) return true;
            }
            return false;
        }

        public string GetValue(string name, bool ignoreCase = true)
        {
            for (int i = 0; i < parsedCommand.Length; i++)
            {
                //Console.WriteLine(i + " + 1 > " + parsedCommand.Length + " : " + (parsedCommand.Length > i + 1) + "   " + name + " == " + parsedCommand[i] + " : " + (name == parsedCommand[i]));
                foreach (CommandLineArgument a in arguments)
                {
                    if (a.aliases.FirstOrDefault(x => (ignoreCase ? x.ToLower() : x) == (ignoreCase ? name.ToLower() : name)) != null && a.aliases.FirstOrDefault(x => (ignoreCase ? x.ToLower() : x) == (ignoreCase ? parsedCommand[i].ToLower() : parsedCommand[i])) != null && parsedCommand.Length > i + 1) return parsedCommand[i + 1];
                }
                if ((ignoreCase && parsedCommand[i].ToLower() == name.ToLower() || parsedCommand[i] == name) && parsedCommand.Length > i + 1) return parsedCommand[i + 1];
            }
            foreach (CommandLineArgument a in arguments)
            {
                if (ignoreCase && a.aliases.FirstOrDefault(x => (ignoreCase ? x.ToLower() : x) == name) != null && !a.isToggle && a.defaultValue != "") return a.defaultValue;
            }
            return "";
        }

        public void ShowHelp(string exeName = "", string extraInfo = "")
        {
            Console.ForegroundColor = ConsoleColor.White;
            Dictionary<int, List<CommandLineArgument>> args = new Dictionary<int, List<CommandLineArgument>>();
            Console.WriteLine();
            Console.Write((exeName.Contains(" ") ? "\"" + exeName + "\"" : exeName) + " ");
            int length = 0;
            foreach (CommandLineArgument arg in arguments)
            {
                if (String.Join(" / ", arg.aliases).Length > length) length = String.Join(" / ", arg.aliases).Length;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("<");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(arg.aliases[0]);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(arg.isToggle ? "" : " " + arg.valueName);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("> ");
            }
            Console.WriteLine();
            Console.WriteLine();
            foreach (CommandLineArgument arg in arguments)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(String.Join(" / ", arg.aliases).PadRight(length) + " :   ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(arg.description);
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (!arg.isToggle && arg.defaultValue != "") Console.Write("   default: " + arg.defaultValue);
                Console.Write("\n");
            }
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine();
            Console.WriteLine(extraInfo);
            Console.WriteLine();
        }
    }
}
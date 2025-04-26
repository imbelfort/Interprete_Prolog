using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ProyectoProlog
{
    class PrologInterpreter
    {
        private List<string> facts;
        private List<string> rules;

        public PrologInterpreter()
        {
            facts = new List<string>();
            rules = new List<string>();
        }

        public void ClearKnowledgeBase()
        {
            facts.Clear();
            rules.Clear();
        }

        public List<string> GetFacts()
        {
            return facts;
        }

        public List<string> GetRules()
        {
            return rules;
        }

        // Eliminar espacios al inicio y final de una cadena
        private string Trim(string str)
        {
            int first = str.IndexOfAny(new char[] { ' ', '\t', '\n', '\r' }, 0);
            if (first == -1)
                return str;

            first = str.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            if (first == str.Length)
                return "";

            int last = str.Length - 1;
            while (last >= 0 && (char.IsWhiteSpace(str[last]) || str[last] == '.'))
                last--;

            return str.Substring(first, last - first + 1);
        }

        public void AddClause(string clause)
        {
            clause = Trim(clause);
            if (clause.Contains(":-"))
            {
                rules.Add(clause);
            }
            else
            {
                facts.Add(clause);
            }
        }

        public bool Evaluate(string query)
        {
            query = Trim(query);

            if (query == "fail")
            {
                return false;
            }

            // Buscar en hechos
            foreach (string fact in facts)
            {
                if (fact == query)
                {
                    return true;
                }
            }

            // Buscar en reglas
            foreach (string rule in rules)
            {
                int pos = rule.IndexOf(":-");
                if (pos != -1)
                {
                    string head = Trim(rule.Substring(0, pos));
                    if (head == query)
                    {
                        string body = Trim(rule.Substring(pos + 2));

                        // Verificar si hay un corte
                        if (body.Contains("!"))
                        {
                            string[] parts = body.Split(',');
                            foreach (string part in parts)
                            {
                                string trimmedPart = Trim(part);
                                if (trimmedPart == "!") continue;
                                if (!Evaluate(trimmedPart))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }

                        // Evaluar cada parte del cuerpo
                        string[] bodyParts = body.Split(',');
                        bool allTrue = true;
                        foreach (string part in bodyParts)
                        {
                            string trimmedPart = Trim(part);
                            if (!Evaluate(trimmedPart))
                            {
                                allTrue = false;
                                break;
                            }
                        }
                        if (allTrue) return true;
                    }
                }
            }
            return false;
        }


        public bool SaveToFile(string filename)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(filename))
                {
                    file.WriteLine("% Hechos:");
                    foreach (string fact in facts)
                    {
                        file.WriteLine(fact + ".");
                    }

                    file.WriteLine("\n% Reglas:");
                    foreach (string rule in rules)
                    {
                        file.WriteLine(rule + ".");
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadFromFile(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    return false;
                }

                facts.Clear();
                rules.Clear();

                string[] lines = File.ReadAllLines(filename);

                foreach (string line in lines)
                {
                    // Ignorar líneas vacías y comentarios
                    if (string.IsNullOrEmpty(line) || line.StartsWith("%"))
                    {
                        continue;
                    }

                    string processedLine = line;
                    // Eliminar el punto final si existe
                    if (processedLine.Length > 0 && processedLine[processedLine.Length - 1] == '.')
                    {
                        processedLine = processedLine.Substring(0, processedLine.Length - 1);
                    }

                    AddClause(processedLine);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ShowKnowledgeBase()
        {
            Console.WriteLine("\n=== Base de Conocimiento ===");
            Console.WriteLine("Hechos:");
            foreach (string fact in facts)
            {
                Console.WriteLine(fact + ".");
            }

            Console.WriteLine("\nReglas:");
            foreach (string rule in rules)
            {
                Console.WriteLine(rule + ".");
            }
            Console.WriteLine("=========================\n");
        }
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            PrologInterpreter prolog = new PrologInterpreter();
            string input;

            Console.WriteLine("Intérprete de Prolog Simple");
            Console.WriteLine("Comandos disponibles:");
            Console.WriteLine("1. Agregar cláusula: Escriba la cláusula directamente");
            Console.WriteLine("2. Consultar: ?- seguido de la consulta");
            Console.WriteLine("3. Cargar archivo: load nombre_archivo");
            Console.WriteLine("4. Guardar en archivo: save nombre_archivo");
            Console.WriteLine("5. Mostrar base de conocimiento: show");
            Console.WriteLine("6. Salir: exit\n");

            while (true)
            {
                Console.Write(">> ");
                input = Console.ReadLine();

                if (input == "exit")
                {
                    break;
                }
                else if (input == "show")
                {
                    prolog.ShowKnowledgeBase();
                }
                else if (input.StartsWith("load"))
                {
                    string filename = input.Substring(5);
                    if (prolog.LoadFromFile(filename))
                    {
                        Console.WriteLine("Archivo cargado exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine("Error al cargar el archivo.");
                    }
                }
                else if (input.StartsWith("save"))
                {
                    string filename = input.Substring(5);
                    if (prolog.SaveToFile(filename))
                    {
                        Console.WriteLine("Base de conocimiento guardada exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine("Error al guardar el archivo.");
                    }
                }
                else if (input.StartsWith("?- "))
                {
                    string query = input.Substring(3);
                    bool result = prolog.Evaluate(query);
                    Console.WriteLine(result ? "true" : "false");
                }
                else if (!string.IsNullOrEmpty(input))
                {
                    prolog.AddClause(input);
                    Console.WriteLine("Cláusula agregada.");
                }
            }
        }
    }
}

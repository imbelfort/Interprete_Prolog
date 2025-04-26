using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoProlog
{
    public partial class Form1 : Form
    {

        private PrologInterpreter prolog;

        public Form1()
        {
            InitializeComponent();
            prolog = new PrologInterpreter(AppendToTraceBox);
        }
        private void AppendToTraceBox(string message)
        {
            richTextBox1.AppendText(message + Environment.NewLine);
        }

        private void AppendColoredLine(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text + Environment.NewLine);

            // Restablecer al color por defecto para próximas líneas
            box.SelectionColor = box.ForeColor;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Check if there are clauses in the textBox3
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Por favor ingrese cláusulas en el área de texto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if there's a query in textBox1
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Por favor ingrese una consulta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            richTextBox1.Clear();
            prolog.TraceOutput = (type, message) =>
            {
                switch (type)
                {
                    case "eval":
                    case "info":
                        AppendColoredLine(richTextBox1, message, Color.Blue); break;
                    case "fail":
                    case "backtrack":
                        AppendColoredLine(richTextBox1, message, Color.Red); break;
                    case "success":
                        AppendColoredLine(richTextBox1, message, Color.Green); break;
                    default:
                        AppendColoredLine(richTextBox1, message, Color.Black); break;
                }
            };

            // Add the clauses from textBox3
            string[] lines = textBox3.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            prolog.ClearKnowledgeBase(); // Clear previous knowledge base

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    // Remove trailing period if present
                    if (trimmedLine.EndsWith("."))
                    {
                        trimmedLine = trimmedLine.Substring(0, trimmedLine.Length - 1);
                    }
                    prolog.AddClause(trimmedLine);
                }
            }

            // Process the query from textBox1
            string query = textBox1.Text.Trim();

            // Remove "?-" prefix if present
            if (query.StartsWith("?-"))
            {
                query = query.Substring(2).Trim();
            }

            // Remove trailing period if present
            if (query.EndsWith("."))
            {
                query = query.Substring(0, query.Length - 1);
            }



            // Evaluate the query
            bool result = prolog.Evaluate(query);

            // Display the result in label1
            label1.Text = result ? "true" : "false";
            label1.ForeColor = result ? Color.Green : Color.Red;
            foreach (string line in prolog.GetTraceLog())
            {
                richTextBox1.AppendText(line + Environment.NewLine);
            }

        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                openFileDialog.Title = "Abrir archivo de Prolog";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Read all text from the file
                        string fileContent = File.ReadAllText(openFileDialog.FileName);
                        textBox3.Text = fileContent;

                        // Clear previous results
                        label1.Text = "";
                        textBox1.Text = "";
                        richTextBox1.Clear();

                        MessageBox.Show("Archivo cargado exitosamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al abrir el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                saveFileDialog.Title = "Guardar archivo de Prolog";
                saveFileDialog.DefaultExt = "txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Write the content of textBox3 to the file
                        File.WriteAllText(saveFileDialog.FileName, textBox3.Text);
                        MessageBox.Show("Archivo guardado exitosamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al guardar el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

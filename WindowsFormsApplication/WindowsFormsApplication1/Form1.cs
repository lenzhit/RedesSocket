using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Cliente cliente;
        List<string> conectados;
        List<string> falsos;
        public List<Form2> formularios = new List<Form2>();
        int cuenta=0;
        int bandera=0;
        private int conteo;
        
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            conteo = 2;
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void comunicacion(object oo, string ee)
        {

            this.Invoke(new Actualizar(Actualiza), ee);
        }

        private delegate void Actualizar(string text);

        private void Actualiza(string text)
        {
            string mensaje = text;
            char caracter = text[1];
            int emisor = int.Parse(caracter.ToString());
            for (int i = 0; i < formularios.Count(); i++)
            {
                if (conectados[emisor] == formularios[i].ClienteSeleccionado)
                {
                    bool buscar = buscarFormulario(formularios[i].ClienteSeleccionado);
                    if (buscar == false)
                    {
                        Form2 formulario = new Form2(cliente);
                        formulario.ClienteSeleccionado = formularios[i].ClienteSeleccionado;
                        formulario.indice1 = formularios[i].indice1;
                        formulario.indice2 = formularios[i].indice2;
                        formulario.mensaje = mensaje;
                        formulario.Show();
                    }
                }
            }
        }

        private void server(object oo)
        {

            this.Invoke(new Server(Apagar));
        }

        private delegate void Server();

        private void Apagar()
        {
            falsos.Clear();
            conectados.Clear();
            listBox1.DataSource = null;
            listBox1.DataSource = falsos;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
            timer1.Enabled = false;
            timer2.Enabled = false;
        }

        private bool buscarFormulario(string text)
        {
            bool formularioEncontrado = false;
            foreach (Form formulario in Application.OpenForms)
            {
                Form2 formBuscado = formulario as Form2; // Asegúrate de hacer el cast correctamente
                if (formBuscado != null && formBuscado.ClienteSeleccionado == text)
                {
                    formularioEncontrado = true;
                    break;
                }
            }
            return formularioEncontrado;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text.Trim()) && !string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                cliente = new Cliente(textBox2.Text);
                cliente.usuario = textBox1.Text;
                cliente.conectar();
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = true;
                timer1.Enabled = true;
                timer2.Enabled = true;
                cliente.MensajeLlegando += new Cliente.DatosClienteEventHandler(comunicacion);
                cliente.Server += new Cliente.ServidorApagado(server);
            }
            else
            {
                MessageBox.Show("Error al Conectarse", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                for (int i=0;i<formularios.Count();i++)
                {
                    if (listBox1.SelectedItem.ToString() == formularios[i].ClienteSeleccionado)
                    {
                        Form2 formulario = new Form2(cliente);
                        formulario.ClienteSeleccionado = formularios[i].ClienteSeleccionado;
                        formulario.indice1= formularios[i].indice1;
                        formulario.indice2= formularios[i].indice2;
                        formulario.Show();
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                falsos = cliente.falsa;
                listBox1.DataSource = falsos;
                conectados = cliente.real;
                if (listBox1.Items.Count != cuenta)
                {
                    if (listBox1.Items.Count > cuenta)
                    {
                        if (bandera == 0)
                        {
                            for (int i = 0; i < listBox1.Items.Count; i++)
                            {
                                Form2 formCliente = new Form2(cliente);
                                formCliente.ClienteSeleccionado = listBox1.Items[i].ToString();
                                for (int j = 0; j < conectados.Count(); j++)
                                {
                                    if (conectados[j] == listBox1.Items[i].ToString())
                                    {
                                        formCliente.indice1 = j.ToString();
                                    }
                                }
                                for (int j = 0; j < conectados.Count(); j++)
                                {
                                    if (conectados[j] == textBox1.Text)
                                    {
                                        formCliente.indice2 = j.ToString();
                                    }
                                }
                                formularios.Add(formCliente);
                            }
                            bandera++;
                            cuenta = listBox1.Items.Count;
                        }
                        else
                        {
                            Form2 formCliente = new Form2(cliente);
                            formCliente.ClienteSeleccionado = listBox1.Items[listBox1.Items.Count - 1].ToString();
                            for (int j = 0; j < conectados.Count(); j++)
                            {
                                if (conectados[j] == listBox1.Items[listBox1.Items.Count - 1].ToString())
                                {
                                    formCliente.indice1 = j.ToString();
                                }
                            }
                            for (int j = 0; j < conectados.Count(); j++)
                            {
                                if (conectados[j] == textBox1.Text)
                                {
                                    formCliente.indice2 = j.ToString();
                                }
                            }
                            formularios.Add(formCliente);
                            cuenta = listBox1.Items.Count;
                        }
                    }
                    else
                    {
                        List<string> ayuda = new List<string>();
                        for (int a = 0; a < formularios.Count(); a++)
                        {
                            ayuda.Add(formularios[a].ClienteSeleccionado);
                        }
                        String[] elementosFaltantes = ayuda.Except(falsos).ToArray();
                        for (int a = 0; a < elementosFaltantes.Count(); a++)
                        {
                            Form[] formulariosAbiertos = Application.OpenForms.Cast<Form>().ToArray();

                            foreach (Form formulario in formulariosAbiertos)
                            {
                                Form2 form2 = formulario as Form2;
                                if (form2 != null && form2.ClienteSeleccionado == elementosFaltantes[a])
                                {
                                    formulario.Close();
                                }
                            }
                        }
                        formularios.Clear();
                        bandera = 0;
                        cuenta = 0;


                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            cliente.enviar("DISCONNECT");
            falsos.Clear();
            conectados.Clear();
            listBox1.DataSource = null;
            listBox1.DataSource = falsos;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
            timer1.Enabled = false;
            timer2.Enabled = false;
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                Form formulario = Application.OpenForms[i];

                if (formulario != Application.OpenForms[0])
                {
                    formulario.Close();
                }
            }
            MessageBox.Show("Desconectado", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

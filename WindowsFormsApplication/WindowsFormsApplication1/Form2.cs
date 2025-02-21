using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public string ClienteSeleccionado { get; set; }
        public string indice1;
        public string indice2;
        public string mensaje="";
        public List<Form2> formularios;
        Cliente cliente { get; set; }

        public int indicador;

        public Form2(Cliente cliente)
        {
            InitializeComponent();
            this.cliente = cliente;
            listView1.View = View.Details;
            listView1.Columns.Add("", listView1.Width / 2 - 2);
            listView1.Columns.Add("", listView1.Width / 2 - 2, HorizontalAlignment.Right);
            cliente.MensajeLlegando += new Cliente.DatosClienteEventHandler(comunicacion);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label2.Text = ClienteSeleccionado;
            if (mensaje != "")
            {
                ListViewItem item = new ListViewItem(mensaje.Substring(2));
                item.SubItems.Add("");
                listView1.Items.Add(item);
                mensaje = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cliente.enviar(indice1+indice2+textBox1.Text);
            ListViewItem item = new ListViewItem("");
            item.SubItems.Add(textBox1.Text);
            listView1.Items.Add(item);
            textBox1.Text = "";
        }

        private void comunicacion(object oo, string ee)
        {
            ActualizaListView(ee);
        }

        private delegate void ActualizarListView(string text);

        private void ActualizaListView(string text)
        {
            
            if (listView1.InvokeRequired)
            {
                listView1.Invoke(new ActualizarListView(ActualizaListView), text);
            }
            else
            {
                char emisor = text[1];
                if (indice1 == emisor.ToString())
                {
                    ListViewItem item = new ListViewItem(text.Substring(2));
                    item.SubItems.Add("");
                    listView1.Items.Add(item);
                }
            }
        }
    }
}

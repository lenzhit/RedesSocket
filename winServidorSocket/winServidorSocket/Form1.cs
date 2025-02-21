using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlClient;

namespace winServidorSocket
{
    public partial class Form1 : Form
    {
        Servidor servidor;
        List<Socket> listaConeccions;
        public Form1()
        {
            InitializeComponent();
            button3.Enabled= false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            servidor = new Servidor();
            servidor.Iniciar();
            servidor.ClienteConectado += new Servidor.DatosServidorEventHandler(comunicacion_Conectado);
            servidor.ClienteEliminado+= new Servidor.ClienteELiminado(eliminar);
            button1.Enabled = false;
            button3.Enabled = true;
        }

        private void comunicacion_Conectado(object oo, string ee, List<Socket> aa)
        {

            this.Invoke(new Actualizar(Actualiza), ee, aa);
        }

        private delegate void Actualizar(string text, List<Socket> listaConeccions);

        private void Actualiza(string text, List<Socket> listaConeccions)
        {
            listBox1.Items.Add(text);
            this.listaConeccions = listaConeccions;
        }

        private void eliminar(object oo, string ee, List<Socket> aa)
        {

            this.Invoke(new Elimina(Eliminar), ee, aa);
        }

        private delegate void Elimina(string text, List<Socket> listaConeccions);

        private void Eliminar(string text, List<Socket> listaConeccions)
        {
            listBox1.Items.Remove(text);
            this.listaConeccions = listaConeccions;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            servidor.DetenerServidor();
            button1.Enabled = true;
            button3.Enabled = false;
            listBox1.Items.Clear();
        }
    }
}

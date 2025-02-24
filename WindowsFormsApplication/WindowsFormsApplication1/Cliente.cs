using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApplication1
{
    public class Cliente
    {
        public Socket socket;
        IPEndPoint end;
        Thread recepcionando;
        public bool conectado = false;
        byte[] arreglo;
        public string usuario;
        string identificador;
        public List<string> real;
        public List<string> falsa;
        public string mensaje;

        public delegate void DatosClienteEventHandler(object oo, string ss);
        public event DatosClienteEventHandler MensajeLlegando;

        public delegate void ServidorApagado(object oo);
        public event ServidorApagado Server;

        public Cliente(string ip)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            end = new IPEndPoint(IPAddress.Parse(ip), 13000);
        }

        public void conectar()
        {
            try
            {
                socket.Connect(end);
                MessageBox.Show("Se Conectó","Éxito",MessageBoxButtons.OK,MessageBoxIcon.Information);
                identificador = usuario + " @";
                socket.Send(ASCIIEncoding.UTF8.GetBytes(identificador));
                recibe();
            }
            catch(Exception ex)
            {
                MessageBox.Show("No se Conectó", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private void recibe()
        {
            recepcionando = new Thread(recibeThread);
            recepcionando.Start();
        }

        private void recibeThread()
        {
            try
            {
                arreglo = new byte[1024];
                while (true)
                {
                    int bytesReceived = socket.Receive(arreglo);
                    string datosRecibidos = Encoding.UTF8.GetString(arreglo, 0, bytesReceived);
                    char caracter = datosRecibidos[0];
                    if (char.IsDigit(caracter))
                    {
                        mensaje = datosRecibidos;
                        pasandoMensaje();
                    }
                    else
                    {
                        string[] elementos = datosRecibidos.Split(',');
                        List<string> conectados = new List<string>(elementos);
                        string elementoExcluir = usuario;
                        List<string> nuevaLista = new List<string>();
                        foreach (string elemento in conectados)
                        {
                            if (elemento != elementoExcluir)
                            {
                                nuevaLista.Add(elemento);
                            }
                        }
                        falsa = nuevaLista;
                        real = conectados;
                    }
                }
            }
            catch(Exception ex)
            {
                server();
            }
        }

        protected virtual void pasandoMensaje()
        {
            MensajeLlegando?.Invoke(this, mensaje);
        }

        protected virtual void server()
        {
            Server?.Invoke(this);
        }


        public void enviar(string mensajito)
        {
            try
            {
                socket.Send(ASCIIEncoding.UTF8.GetBytes(mensajito));
            }
            catch
            {
                MessageBox.Show("Error al Enviar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

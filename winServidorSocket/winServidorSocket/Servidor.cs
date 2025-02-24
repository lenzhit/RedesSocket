using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;

namespace winServidorSocket
{
    internal class Servidor
    {

        public Socket socketPadre;
        public List<Socket> listaConeccions;
        public List<string> conectados=new List<string>();
        public IPEndPoint puntoLocal_Servidor;
        public Thread escuchando;
        bool continuarEscuando = true;
        byte[] arreglo; 
        byte[] lista;
        string[] partes;
        string arreglostring;
        string usuario;
        string contra;
        string eliminado;
        int numeroConect;

        public delegate void DatosServidorEventHandler(object oo,string ss, List<Socket> listaConeccions);
        public event DatosServidorEventHandler ClienteConectado;

        public delegate void ClienteELiminado(object oo, string ss, List<Socket> listaConeccions);
        public event ClienteELiminado ClienteEliminado;

        public void Iniciar()
        {
            listaConeccions = new List<Socket>();
            socketPadre = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            MessageBox.Show("Servidor Encendido","Éxito",MessageBoxButtons.OK,MessageBoxIcon.Information);

            //servidor en local
            puntoLocal_Servidor = new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[1], 13000);

            //servidor remoto
            //puntoLocal_Servidor = new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[4], 13000);
            
                
            MessageBox.Show(puntoLocal_Servidor.ToString());
            escuchando = new Thread(escuchaThread);
            escuchando.Start();
        }

        public void escuchaThread()
        {
            try
            {
                socketPadre.Bind(puntoLocal_Servidor);
                numeroConect = 0;
                while (continuarEscuando)
                {
                    arreglo = new byte[1024];
                    socketPadre.Listen(6);
                    Socket s_cliente = socketPadre.Accept();
                    s_cliente.Receive(arreglo);
                    arreglostring = ASCIIEncoding.UTF8.GetString(arreglo);
                    SepararCadenas(arreglostring);
                            usuario = partes[0];
                            listaConeccions.Add(s_cliente);
                            conectados.Add(usuario);
                            pasandoConectado();
                            numeroConect++;
                            byte[] enviar = datos();
                            for (int j = 0; j < numeroConect; j++)
                            {
                                listaConeccions[j].Send(enviar);
                           
                    }
                    Thread receiveThread = new Thread(RecibirMensajesCliente);
                    receiveThread.Start(s_cliente);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Servidor Apagado", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void SepararCadenas(string cadena)
        {
            string identificador = " @";
            partes= cadena.Split(new[] { identificador }, StringSplitOptions.None);
        }

        protected virtual void pasandoConectado()
        {
            ClienteConectado?.Invoke(this,usuario, listaConeccions);
        }

        protected virtual void pasandoEliminado()
        {
            ClienteEliminado?.Invoke(this,eliminado, listaConeccions);
        }

        public void DetenerServidor()
        {
            for (int j = 0; j < numeroConect; j++)
            {
                listaConeccions[j].Close();
            }
            listaConeccions.Clear();
            socketPadre.Close();
            continuarEscuando = false;
        }

        private void RecibirMensajesCliente(object socket)
        {
            Socket handler = (Socket)socket;
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {  
                    int bytesRead = handler.Receive(buffer);
                    if (bytesRead > 0)
                    {
                        string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        if (data == "DISCONNECT")
                        {
                            int posicion = listaConeccions.IndexOf(handler);
                            listaConeccions.Remove(handler);
                            eliminado = conectados[posicion];
                            conectados.Remove(conectados[posicion]);
                            numeroConect--;
                            byte[] enviar = datos();
                            for (int j = 0; j < numeroConect; j++)
                            {
                                listaConeccions[j].Send(enviar);
                            }
                            handler.Close();
                            pasandoEliminado();
                        }
                        else
                        {
                            char caracter = data[0];
                            int indice = int.Parse(caracter.ToString());
                            byte[] datosEnviar = Encoding.UTF8.GetBytes(data);
                            listaConeccions[indice].Send(datosEnviar);
                        }
                    }
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

       

        private byte[] datos()
        {
            List<string> nuevaListaconectados = new List<string>();

            foreach (string usuario in conectados)
            {
                nuevaListaconectados.Add(usuario);
            }
            string listaTexto = string.Join(",", nuevaListaconectados);
            byte[] datosEnviar = Encoding.UTF8.GetBytes(listaTexto);
            
            return datosEnviar; 
        }
    }
}

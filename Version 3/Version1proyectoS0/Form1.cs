using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Version1proyectoS0
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;

        bool atendercliente = true;
        string usuario;
        int conectado;

        delegate void DelegadoParaEscribir(string[] conectados);

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void AtenderServidor()
        {
            while (atendercliente)
            {
                try
                {
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    string cadena = Encoding.ASCII.GetString(msg2);
                    string[] parte = Encoding.ASCII.GetString(msg2).Split('/');

                    int indiceBarra = cadena.IndexOf('/');
                    string textoDespuesBarra = cadena.Substring(indiceBarra + 1);
                 
                    
               


                    int codigo = Convert.ToInt32(parte[0]);
                    string mensaje = parte[1].Split('\0')[0];
  

                    switch (codigo)
                    {
                        case 0:
                            this.BackColor = Color.Gray;
                            MessageBox.Show("Te has desconectado.");
                            break;
                        case 1: // Registrarse

                            if (mensaje == "0")
                                MessageBox.Show("Se ha registrado correctamente.");

                            //Excepciones
                            else if (mensaje == "1")
                                MessageBox.Show("Este nombre de usuario ya existe.");

                            else
                                MessageBox.Show("Error de consulta, pruebe otra vez.");

                            Nombre_Re.Clear();
                            Contra_Re.Clear();
                            ConfContra_Re.Clear();
                            break;
                        case 2: //Inicio Sesión
                            if (mensaje == "0")
                            {
                                listaconexion.Visible = true;
                                label11.Visible = true;


                                MessageBox.Show("Has iniciado sesión correctamente");
                                Nombre_In.Clear();
                                Contra_In.Clear();


                            }
                            //Excepciones
                            else if (mensaje == "1")
                            {
                                MessageBox.Show("El usuario introducido no existe");
                            }
                            else if (mensaje == "2")
                            {
                                MessageBox.Show("Contraseña incorrecta, vuelve a intentarlo");
                            }
                            else
                            {
                                MessageBox.Show("Error en la consulta. Pruebe otra vez.");
                            }
                            break;

                        case 3: //Peticion1
                            if (mensaje == "-1")
                                MessageBox.Show("Error de consulta. Prueba otra vez.");
                            else if (mensaje == "-2")
                                MessageBox.Show("Este jugador nunca ha jugado.");
                            else
                                MessageBox.Show("El usuario ha jugado los dias:" + textoDespuesBarra);
                            break;

                        case 4: //Peticion2

                            //Excepciones
                            if (mensaje == "-1")
                            {
                                MessageBox.Show("Error de consulta. Prueba otra vez.");
                            }
                            else if (mensaje == "-2")
                            {
                                MessageBox.Show("Ese dia no se jugo ninguna partida");
                            }

                            //Devuelve partida
                            else
                                MessageBox.Show("El ganador de aquella partida fue:" + mensaje);
                            break;

                        case 5://Peticion3

                            if (mensaje == "-1")
                            {
                                MessageBox.Show("Error de consulta. Prueba otra vez");
                            }
                            else
                            {
                                MessageBox.Show("Estos usuarios han jugado juntos alguna vez.");
                            }
                            break;

                        case 6: //Notificación de actualización de la lista de conectados
                            if (mensaje == "-1")
                                MessageBox.Show("No hay usuarios conectados.");

                            else
                            {
                                //MessageBox.Show("Se ha actualizado la lista de conectados.");
                                string[] conectados = mensaje.Split('*');

                                //Delegado modifica el DataGridView para poner o quitar un usuario
                                DelegadoParaEscribir delegado = new DelegadoParaEscribir(ListaConectados);
                                listaconexion.Invoke(delegado, new object[] { conectados });

                            }
                            break;
                    }
                }
                catch (SocketException)
                {
                    MessageBox.Show("Server desconectado");
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(conectado==0)
            {
                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse("10.4.119.5");
                IPEndPoint ipep = new IPEndPoint(direc, 50005);


                //Creamos el socket 
                this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    atendercliente = true;
                    server.Connect(ipep);//Intentamos conectar el socket
                    this.BackColor = Color.Green;
                    MessageBox.Show("Conectado");
                    conectado = 1;

                    ThreadStart ts = delegate { AtenderServidor(); };
                    atender = new Thread(ts);
                    atender.Start();
                }
                catch (SocketException ex)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                }
                catch (Exception)
                {
                    MessageBox.Show("Ha ocurrido un error");
                }
            }
        }

        private void Guardar_Click(object sender, EventArgs e)
        {
            try
            {


                if (Contra_Re.Text == ConfContra_Re.Text)
                {
                    string mensaje = "1/" + Nombre_Re.Text + "/" + Contra_Re.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                else
                    MessageBox.Show("Las contraseñas no coinciden");

            }
            catch (Exception)
            {
                MessageBox.Show("Compruebe su conexión con el servidor y vuelva a intentarlo");
            }
           

        }

        private void Iniciar_Click_1(object sender, EventArgs e)
        {
            try
            {


                string mensaje = "2/" + Nombre_In.Text + "/" + Contra_In.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch (Exception)
            {
                MessageBox.Show("Compruebe su conexión con el servidor y vuelva a intentarlo");
            }

        }

        private void RealizarPeticion_Click_1(object sender, EventArgs e)
        {
            try
            {


                if (Peticion1.Checked)
                {
                    string mensaje = "3/" + Nombres_Pet.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                else if (Peticion2.Checked)
                {
                    string mensaje = "4/" + Nombres_Pet.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                }
                else if (Peticion3.Checked)
                {
                    string mensaje = "5/" + Nombres_Pet.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Compruebe su conexión con el servidor y vuelva a intentarlo");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (conectado == 1)
                {
                    string mensaje = "0/";
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);


                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
            }
            catch (Exception) { }

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                atendercliente = false;
                string mensaje = "0/";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                atender.Abort();

                server.Shutdown(SocketShutdown.Both);
                server.Close();
                conectado = 0;
            }
            catch (Exception)
            {
                MessageBox.Show("Hasta pronto");
            }

        }
        public void ListaConectados(string[] conectados)
        {
            label11.Visible = true;
            listaconexion.Visible = true;
            listaconexion.ColumnCount = 1;
            listaconexion.RowCount = conectados.Length;
            listaconexion.ColumnHeadersVisible = false;
            listaconexion.RowHeadersVisible = false;
            listaconexion.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            listaconexion.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            listaconexion.SelectAll();

            for (int i = 0; i < conectados.Length; i++)
            {
                listaconexion.Rows[i].Cells[0].Value = conectados[i];
            }

            listaconexion.Show();

        }
    }
}
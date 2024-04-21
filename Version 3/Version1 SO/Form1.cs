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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9050);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado");

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

        }
        private void Registrarse_Click(object sender, EventArgs e)
        {
            if (Contra_Re.Text == ConfContra_Re.Text)
            {
                string mensaje = "1/" + Nombre_Re.Text + "/" + Contra_Re.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else if (Contra_Re.Text != ConfContra_Re.Text)
                MessageBox.Show("Hay datos que no coinciden");

            string respuesta;
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            respuesta = Encoding.ASCII.GetString(msg2);
            if (respuesta == "0")
                MessageBox.Show("El usuario se ha registrado correctamente");
            else if (respuesta == "1")
                MessageBox.Show("Error al registrarse, el nombre de usuario seleccionado ya existe");
            else
                MessageBox.Show("Se ha producido un error, porfavor intentelo de nuevo");
        }
        private void Iniciar_Click(object sender, EventArgs e)
        {
            string mensaje = "2/" + Nombre_In.Text + "/" + Contra_In.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            string respuesta;
            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            respuesta = Encoding.ASCII.GetString(msg2);

            if (respuesta == "0")
                MessageBox.Show("Se ha iniciado sesion correctamente");
            else if (respuesta == "1")
                MessageBox.Show("El usuario no esta registrado");
            else if (respuesta == "2")
                MessageBox.Show("Contraseña incorrecta");
            else
                MessageBox.Show("Se ha producido un error, porfavor intentelo de nuevo");


        }

        private void RealizarPeticion_Click(object sender, EventArgs e)
        {
            if (Peticion1.Checked)
            {
                string mensaje = "3/" + Nombres_Pet.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show("Las fechas de las partidas del jugador son:" + mensaje);
            }
            else if (Peticion2.Checked)
            {
                string mensaje = "4/" + Nombres_Pet.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];


                MessageBox.Show("El ganador de la partida de la fecha indicada es:" + mensaje);
            }
            else if (Peticion3.Checked)
            {
                string mensaje = "5/" + Nombres_Pet.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];


                MessageBox.Show(mensaje);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();


        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radio_Pet1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private GroupBox groupBox1;
        private Button Iniciar;
        private TextBox Contra_Re;
        private TextBox Nombre_Re;
        private TextBox ConfContra_Re;
        private Button Guardar;
        private Label label10;
        private Label label9;
        private Button RealizarPeticion;
        private RadioButton Peticion3;
        private RadioButton Peticion2;
        private RadioButton Peticion1;
        private TextBox Nombres_Pet;
        private TextBox Contra_In;
        private TextBox Nombre_In;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private LinkLabel linkLabel1;

        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.Nombre_In = new System.Windows.Forms.TextBox();
            this.Contra_In = new System.Windows.Forms.TextBox();
            this.Nombres_Pet = new System.Windows.Forms.TextBox();
            this.Peticion1 = new System.Windows.Forms.RadioButton();
            this.Peticion2 = new System.Windows.Forms.RadioButton();
            this.Peticion3 = new System.Windows.Forms.RadioButton();
            this.RealizarPeticion = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Guardar = new System.Windows.Forms.Button();
            this.ConfContra_Re = new System.Windows.Forms.TextBox();
            this.Nombre_Re = new System.Windows.Forms.TextBox();
            this.Contra_Re = new System.Windows.Forms.TextBox();
            this.Iniciar = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.groupBox1.Controls.Add(this.Iniciar);
            this.groupBox1.Controls.Add(this.Contra_Re);
            this.groupBox1.Controls.Add(this.Nombre_Re);
            this.groupBox1.Controls.Add(this.ConfContra_Re);
            this.groupBox1.Controls.Add(this.Guardar);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.RealizarPeticion);
            this.groupBox1.Controls.Add(this.Peticion3);
            this.groupBox1.Controls.Add(this.Peticion2);
            this.groupBox1.Controls.Add(this.Peticion1);
            this.groupBox1.Controls.Add(this.Nombres_Pet);
            this.groupBox1.Controls.Add(this.Contra_In);
            this.groupBox1.Controls.Add(this.Nombre_In);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.linkLabel1);
            this.groupBox1.Location = new System.Drawing.Point(29, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1039, 431);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Inicio";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(338, 75);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(0, 25);
            this.linkLabel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nombre de Usuario:";

            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Iniciar Sesión:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 25);
            this.label3.TabIndex = 3;
            this.label3.Text = "Contraseña:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(209, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 25);
            this.label4.TabIndex = 4;
            this.label4.Text = "Peticiones:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(841, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 25);
            this.label5.TabIndex = 5;
            this.label5.Text = "Registrarse";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(841, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(145, 25);
            this.label6.TabIndex = 6;
            this.label6.Text = "Nombre usuario:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(841, 202);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 25);
            this.label7.TabIndex = 7;
            this.label7.Text = "Contraseña";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(841, 273);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(186, 25);
            this.label8.TabIndex = 8;
            this.label8.Text = "Confirmar contraseña:";
            // 
            // Nombre_In
            // 
            this.Nombre_In.Location = new System.Drawing.Point(6, 125);
            this.Nombre_In.Name = "Nombre_In";
            this.Nombre_In.Size = new System.Drawing.Size(150, 31);
            this.Nombre_In.TabIndex = 10;
            // 
            // Contra_In
            // 
            this.Contra_In.Location = new System.Drawing.Point(6, 202);
            this.Contra_In.Name = "Contra_In";
            this.Contra_In.Size = new System.Drawing.Size(150, 31);
            this.Contra_In.TabIndex = 11;
            // 
            // Nombres_Pet
            // 
            this.Nombres_Pet.Location = new System.Drawing.Point(209, 60);
            this.Nombres_Pet.Name = "Nombres_Pet";
            this.Nombres_Pet.Size = new System.Drawing.Size(423, 31);
            this.Nombres_Pet.TabIndex = 12;
            // 
            // Peticion1
            // 
            this.Peticion1.AutoSize = true;
            this.Peticion1.Location = new System.Drawing.Point(209, 126);
            this.Peticion1.Name = "Peticion1";
            this.Peticion1.Size = new System.Drawing.Size(559, 29);
            this.Peticion1.TabIndex = 13;
            this.Peticion1.TabStop = true;
            this.Peticion1.Text = "Peticion1: Devuelve las fechas de las partidas del jugador indicado";
            this.Peticion1.UseVisualStyleBackColor = true;
            // 
            // Peticion2
            // 
            this.Peticion2.AutoSize = true;
            this.Peticion2.Location = new System.Drawing.Point(209, 185);
            this.Peticion2.Name = "Peticion2";
            this.Peticion2.Size = new System.Drawing.Size(503, 29);
            this.Peticion2.TabIndex = 14;
            this.Peticion2.TabStop = true;
            this.Peticion2.Text = "Peticion2: Que jugador gano la partida de la fecha indicada";
            this.Peticion2.UseVisualStyleBackColor = true;
            // 
            // Peticion3
            // 
            this.Peticion3.AutoSize = true;
            this.Peticion3.Location = new System.Drawing.Point(209, 259);
            this.Peticion3.Name = "Peticion3";
            this.Peticion3.Size = new System.Drawing.Size(496, 29);
            this.Peticion3.TabIndex = 15;
            this.Peticion3.TabStop = true;
            this.Peticion3.Text = "Peticion3: Cuantas veces han jugado juntos dos jugadores";
            this.Peticion3.UseVisualStyleBackColor = true;
            // 
            // RealizarPeticion
            // 
            this.RealizarPeticion.Location = new System.Drawing.Point(338, 339);
            this.RealizarPeticion.Name = "RealizarPeticion";
            this.RealizarPeticion.Size = new System.Drawing.Size(167, 34);
            this.RealizarPeticion.TabIndex = 16;
            this.RealizarPeticion.Text = "Realizar peticion";
            this.RealizarPeticion.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(209, 212);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(571, 25);
            this.label9.TabIndex = 17;
            this.label9.Text = "(Introducdir fecha mes y año separados por \"/\" y en el orden indicado)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(209, 291);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(381, 25);
            this.label10.TabIndex = 18;
            this.label10.Text = "(Introducir ambos nombres separados por \"/\")";
            // 
            // Guardar
            // 
            this.Guardar.Location = new System.Drawing.Point(844, 361);
            this.Guardar.Name = "Guardar";
            this.Guardar.Size = new System.Drawing.Size(112, 34);
            this.Guardar.TabIndex = 19;
            this.Guardar.Text = "Registrarse";
            this.Guardar.UseVisualStyleBackColor = true;
            // 
            // ConfContra_Re
            // 
            this.ConfContra_Re.Location = new System.Drawing.Point(841, 301);
            this.ConfContra_Re.Name = "ConfContra_Re";
            this.ConfContra_Re.Size = new System.Drawing.Size(150, 31);
            this.ConfContra_Re.TabIndex = 20;
            // 
            // Nombre_Re
            // 
            this.Nombre_Re.Location = new System.Drawing.Point(844, 161);
            this.Nombre_Re.Name = "Nombre_Re";
            this.Nombre_Re.Size = new System.Drawing.Size(150, 31);
            this.Nombre_Re.TabIndex = 21;
            // 
            // Contra_Re
            // 
            this.Contra_Re.Location = new System.Drawing.Point(844, 230);
            this.Contra_Re.Name = "Contra_Re";
            this.Contra_Re.Size = new System.Drawing.Size(150, 31);
            this.Contra_Re.TabIndex = 22;
            // 
            // Iniciar
            // 
            this.Iniciar.Location = new System.Drawing.Point(13, 268);
            this.Iniciar.Name = "Iniciar";
            this.Iniciar.Size = new System.Drawing.Size(112, 34);
            this.Iniciar.TabIndex = 23;
            this.Iniciar.Text = "Iniciar";
            this.Iniciar.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 500);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
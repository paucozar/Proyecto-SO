#include <stdio.h>
#include <mysql.h>
#include <string.h>
#include <stdlib.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <sys/types.h>
#include <unistd.h>
#include <ctype.h>
#include <pthread.h>

MYSQL *conn;
typedef struct{
	char nombre[25];
	int *socket;
}Jugador;

typedef struct{
	Jugador conectados[50];
	int num;
}ListaConectados;

ListaConectados listaC;
int i;
int sockets[100];

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

int DameGanador(char fecha[20], char ganador[250])
{

	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta[250];
	//"SELECT j.nombre AS ganador \
           FROM jugadores j \
           JOIN informacion i ON j.id_jugador = i.id_jugador \
           JOIN partidas p ON i.id_partida = p.id_partida \
           WHERE p.fecha = 'fecha_deseada' \
           ORDER BY i.posicion ASC \
           LIMIT 1;";
	sprintf (consulta, "SELECT jugador.username FROM jugador JOIN informacion ON jugador.id = informacion.id_j JOIN partida ON informacion.id_p = partida.id WHERE partida.fecha= '%s' ORDER BY informacion.posicion ASC LIMIT 1;",fecha);
	//strcpy (consulta, "SELECT jugador.username FROM jugador,partida,informacion WHERE partida.fecha = '");
	//strcat (consulta,fecha);
	//strcat (consulta, "'AND informacion.posicion=1 AND partida.id = informacion.id_p AND informacion.id_j = jugador.id");
	err = mysql_query (conn, consulta);
	if (err != 0) {
		return -1;

	}
	else{
		
		resultado = mysql_store_result (conn);
		row = mysql_fetch_row (resultado);
		if (row == NULL){
			return -2;
		}
		else{
			
			while (row != NULL){
				printf("%s", row[0]);
				sprintf(ganador,"%s -- %s",ganador,row[0]);
				row = mysql_fetch_row (resultado);
			}
			for (int i= 0; i < strlen(fecha)-2; i++){
				fecha[i] = fecha[i + 2];
			}
			int len = strlen(fecha);
			fecha[len - 1] = '\0';
			fecha[len - 2] = '\0';
			return 0;
			}
			
	}
}

int DameFecha(char nombre[20], char fecha[250])
{

	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta[250];
	strcpy (consulta, "SELECT partida.fecha FROM jugador,partida,informacion WHERE jugador.username = '");
	strcat (consulta,nombre);
	strcat (consulta, "'AND partida.id = informacion.id_p  AND informacion.id_j = jugador.id");
	err = mysql_query (conn, consulta);
	if (err != 0) {
		return -1;
	}
	else{
	
		resultado = mysql_store_result (conn);
		row = mysql_fetch_row (resultado);
		if (row == NULL){
			return -2;
		}
		else{
			while (row != NULL){
				sprintf(fecha,"%s -- %s",fecha,row[0]);
				row = mysql_fetch_row (resultado);
			}
			for (int i= 0; i < strlen(fecha)-2; i++){
				fecha[i] = fecha[i + 2];
			}
			int len = strlen(fecha);
			fecha[len - 1] = '\0';
			fecha[len - 2] = '\0';
			return 0;
			}
			
	}
	
}

int PartidasEntreEllos(char jugador1[20], char jugador2[20]) 
	
{ 
	
	int contador;
	int err; 
	// Estructura especial para almacenar resultados de consultas  
	
	MYSQL_RES *resultado; 
	MYSQL_ROW row; 
	char consulta1[250];
	char consulta2[250];
	
	sprintf (consulta1,"SELECT count(partida.id) FROM jugador,partida,informacion WHERE jugador.username = '%s' AND partida.id IN (SELECT partida.id FROM jugador,partida,informacion where jugador.username = '%s' AND jugador.id = informacion.id_j AND informacion.id_p = partida.id)", jugador1, jugador2);
	err=mysql_query (conn, consulta1);
	if (err !=0) { 
		return -1;
		
	}
	else{
		
		resultado = mysql_store_result (conn);
		row=mysql_fetch_row (resultado);


		while (row != NULL){
			contador = atoi(row[0]);
			row = mysql_fetch_row(resultado);
			
		}
			return contador;
	}
}

int Registrarse(char username[20], char password[20])
{
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[100];
	strcpy(consulta, "SELECT username FROM jugador WHERE username='");
	strcat(consulta,username);
	strcat(consulta, "'");
	
	err = mysql_query(conn, consulta);
	if(err!=0)
		return -1;
	else
	{
		resultado = mysql_store_result(conn);
		row = mysql_fetch_row(resultado);
		
		if(row==NULL)
		{
			err = mysql_query(conn, "SELECT * FROM jugador WHERE id IN (SELECT max(id) FROM jugador);");
			if(err!=0)
			{
				return -1;
			}
			resultado = mysql_store_result(conn);
			row = mysql_fetch_row(resultado);
			printf("%s\n", row[0]);
			int id = atoi(row[0])+ 1;
			sprintf(consulta,"INSERT INTO jugador VALUES (%d, '%s', '%s');", id,username,password);
			printf("%s\n",consulta);
			err = mysql_query(conn,consulta);
			if(err!=0)
				return -1;
			else
			   return 0;
		}
		else 
			return 1;
	}
}

int IniciarSesion (char username[20], char password[20])
{
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err;
	
	char consulta[100];
	strcpy(consulta, "SELECT contraseña FROM jugador WHERE username='");
	strcat(consulta,username);
	strcat(consulta, "'");
	err = mysql_query(conn,consulta);
	if(err!=0)
	{
		printf("Error al consultar la informacion en la base de datos %u %s", mysql_errno(conn),mysql_error(conn));
		return -1;
	}
	else
	{
		resultado = mysql_store_result(conn);
		row = mysql_fetch_row(resultado);
		if(row==NULL)
			return 1;
		else
		{
			printf("%s\n", row[0]);
			if(strcmp(password,row[0])==0)
				return 0;
			else
				return 2;
		}
	}
}
int DameListaConectados(char list[512])
{
	strcpy(list, "\0");
	if (listaC.num!=0)
	{
		int i;
		for(i=0; i< listaC.num; i++)
			sprintf(list, "%s%s*", list, listaC.conectados[i].nombre);
		list[strlen(list)-1]='\0';
		return 0;
	}
	else 
		return -1;
} 
void AnadirAListaConectados (char nombre[25],int *socket){
	
	if (nombre != NULL && socket != NULL){
		//Creamos un nuevo usuario que añadir a la lista
		Jugador nuevoJugador;
		strcpy(nuevoJugador.nombre,nombre);
		nuevoJugador.socket = socket;
		
		//Lo añadimos
		listaC.conectados[listaC.num]=nuevoJugador;
		listaC.num = listaC.num+1;
	}	
}
void RetirarDeListaConectados (char nombre[25]) {
	
	if (nombre != NULL){
		int n = 0;
		int encontrado = 0;
		
		while(n<listaC.num && encontrado==0){
			if (strcmp(listaC.conectados[n].nombre,nombre)==0){
				encontrado = 1;
			}
			else
				n++;
		}
		if (encontrado==1){
			while(n<listaC.num){
				listaC.conectados[n]=listaC.conectados[n+1];
				n++;
			}
			listaC.num = listaC.num-1;
		}
	}
}


void NotificarNuevaListaConectados(){
	
	char lista[512];
	char notificacion[512];
	
	//pthread_mutex_lock(&mutex);
	int res = DameListaConectados(lista);
	//pthread_mutex_unlock(&mutex);
	
	printf("Notificacion de actualizacion de ListaConectados\n");
	if (res == 0){
		printf("Lista de conectados con nuevos datos\n");
		sprintf(notificacion,"6/%s",lista);
	}
	else{
		printf("Lista de conectados vacia\n");
		sprintf(notificacion,"6/%s",lista);
	}
	
	//Envia actualización a todos los sockets
	int j;
	for (j=0;j<listaC.num;j++){
		write(listaC.conectados[j].socket,notificacion,strlen(notificacion));
	}
	
}
int *AtencionClientes(void *socket){
	
	char peticion[512];
	char respuesta[512];
	
	int ret;
	int sock_conn;
	int *s;
	s = (int *) socket;
	sock_conn = *s;
	
	int terminar = 0;
	while(terminar == 0)
	{
		ret = read(sock_conn,peticion,sizeof(peticion));
		printf("Recibida una peticion\n");
		
		
		printf("La peticion es: %s\n", peticion);
		peticion[ret]='\0';
		
	
		char nombre[20];
		char password[20];
		char fecha[250];
		char nombre2[20];
		
		char *p = strtok(peticion, "/");
		int codigo = atoi(p);
		
		
		if(codigo==0)
		{
			terminar = 1;
			
			pthread_mutex_lock(&mutex);
			RetirarDeListaConectados(nombre);
			NotificarNuevaListaConectados();
			
			pthread_mutex_unlock(&mutex);

		}
		else if(codigo==1) //REGISTRARSE
		{
			p = strtok(NULL, "/");
			strcpy(nombre, p);
			p = strtok (NULL, "/");
			strcpy (password, p);

				
			int res = Registrarse(nombre,password);
			sprintf(respuesta,"1/%d",res);
			
		}
		else if(codigo==2) //INICIO SESION
		{
			p = strtok(NULL,"/");
			strcpy(nombre,p);
				p = strtok(NULL,"/");
			strcpy(password,p);
				
			int res = IniciarSesion(nombre,password);
			sprintf(respuesta,"2/%d", res);
				
				//Añadimos a la lista de conectados si todo ha ido bien
			if (res == 0){
				pthread_mutex_lock(&mutex);  //Autoexclusion
				AnadirAListaConectados(nombre,sock_conn);
				NotificarNuevaListaConectados();
				pthread_mutex_unlock(&mutex);
					
					
			}
		}
		else if(codigo==3)
		{
			p = strtok(NULL,"/");
			strcpy(nombre,p);

				
			int res = DameFecha(nombre,fecha);
			if (res ==0)
				sprintf(respuesta,"3/%s",fecha);
			else
				sprintf(respuesta,"3/%d",res);
			
		}
		else if(codigo==4)
		{
			p = strtok(NULL, "/");
			strcpy(fecha,p);
			char ganador[250];
			
			int res = DameGanador(fecha, ganador);
			if (res == 0){
				sprintf(respuesta,"4/%s",ganador);
			}
			else{
				sprintf(respuesta,"4/%d",res);
			}
		}
		else if(codigo==5)
		{
			p = strtok(NULL,"/");
			strcpy(nombre,p);
			p = strtok(NULL,"/");
			strcpy(nombre2,p);
			
			int res = PartidasEntreEllos(nombre, nombre2);
			
			sprintf(respuesta,"5/%d",res);
		
		}

		if (codigo!=0){
			write (sock_conn,respuesta, strlen(respuesta));
			printf("Codigo: %d , Resultado: %s\n",codigo,respuesta);
			
		}	// Y lo enviamos
		
	}
	close(sock_conn);
	pthread_exit(0);
}





int main(int argc, char *argv[]) {
	
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	
	listaC.num=0;
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) <0)
		printf("Error creando socket");
	
	memset(&serv_adr,0,sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	serv_adr.sin_port = htons(9100);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) <0)
		printf("Error en el bind");
	
	if (listen(sock_listen, 2) <0)
		printf("Error en el listen");
	
	conn = mysql_init(NULL);
	if(conn==NULL)
	{
		printf("Error al crear la conexion: %u %s\n", mysql_errno(conn),mysql_error(conn));
		exit(1);
	}
	
	conn = mysql_real_connect(conn,"localhost","root","mysql","juego",0,NULL,0);
	if (conn==NULL){
		printf("Error al crear la connexiÃ³n: %u %s\n",mysql_errno(conn),mysql_error(conn));
		exit(1);
	}
	pthread_t thread[100];
	pthread_mutex_init(&mutex,NULL);
	
	for(;;)
	{
		printf("Escuchando\n");
		
		sock_conn = accept(sock_listen,NULL,NULL);
		printf("He recibido conexion\n");
		sockets[i] = sock_conn;
		pthread_create (&thread[i], NULL, AtencionClientes, &sockets[i]);
	}
			
	pthread_mutex_destroy(&mutex);
	exit(0);
}


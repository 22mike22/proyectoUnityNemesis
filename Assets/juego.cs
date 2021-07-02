using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class juego : MonoBehaviour
{
    public GameObject cubo;
    public GameObject lamparaOn;
    public GameObject lamparaOff;
    public GameObject relojNo;
    private ArrayList EDJ;
    public GameObject transparente;
    public GameObject cuboPapelera;
    private GameObject aux;
    private GameObject SelecionDeCubo;
    private int objetoSeleccionado;
    private ArrayList objetos = new ArrayList();
    private const float menor = 0.499994f;
    private const float mayor = 0.499996f;
    public int[,,,] cordenadas;
    public GameObject esfera;
    public GameObject generador;
    public GameObject and;
    public GameObject or;
    public GameObject xor;
    public GameObject not;
    public GameObject tipoD;
    public GameObject relojO;
    public float tiempoReloj;
    public float tiempo;
    private bool noFuente;
    private bool acabo = true;
    public AudioClip musica1;
    public bool menu = false;
    public GameObject elMenu;
    private bool actualiza = true;

    void Start()
    {
        //se ponen todos los objetos en un array para que sea mas facil su selecciona
        EDJ = new ArrayList();
        EDJ.Add(cubo);//1
        EDJ.Add(lamparaOff);//2
        EDJ.Add(generador);//3
        EDJ.Add(and);//4
        EDJ.Add(or);//5
        EDJ.Add(xor);//6
        EDJ.Add(not);//7
        EDJ.Add(tipoD);//8
        EDJ.Add(relojO);//9
        EDJ.Add(relojNo);//10
        //
        objetoSeleccionado = 1;
        cordenadas = new int[0, 0, 0, 4];
        tiempo = 0;
        tiempoReloj = 0.1f;
        noFuente = true;
        menu = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))//cambia de estado la variable que bloquea el juego y muestra el menu
        {
            menu = !menu;
            actualiza = true;
        }
        if (menu)
        {
            if (actualiza) elMenu.SetActive(false);//se desactiva el menu
            actualiza = false;//esta variable es para que no se tenga que estar desactivando una y otra vez el menu
            seleccionar();//
            //tiempo += Time.deltaTime;//se incrementa las variables de tiempo
            tiempoReloj += Time.deltaTime;
            Destroy(SelecionDeCubo);//se destruye el objeto de selecion para actualizarlo
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//se manda un rayo desde la posicion del mouse

            if (Physics.Raycast(ray, out hit, 100))//se lanza el rayo
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))//si se da el click principal
                {
                    float x = Mathf.Round(hit.point.x);
                    float y = Mathf.Round(hit.point.y);
                    float z = Mathf.Round(hit.point.z);//se obtiene las cordenadas donde choca y se redondea para tener un sistema de cordenadas de solo enteros

                    aux = Instantiate(getElemento(), new Vector3(x, y, z), Quaternion.identity);//se crea el objeto dentro del juego con las cordenadas especificadas
                    if (objetoSeleccionado > 2) aux.transform.Rotate(0, mirarAng(), 0);//se ve si se esta seleccionando un objeto al que si le afecte la rotacion y sí si se le rota
                    ArrayList objeto = new ArrayList();
                    objeto.Add(x);
                    objeto.Add(y);
                    objeto.Add(z);
                    objeto.Add(aux);//se crea un array con las cordenadas y el GameObject

                    objetos.Add(objeto);//se añade al array de todos los objetos
                    int[] cordenada = { traductorDeCordenadas((int)x), traductorDeCordenadas((int)y), traductorDeCordenadas((int)z) };//se añade al array de int que contiene la informacion de todos los objetos y estan separados para facilitar la logica del juego
                    switch (objetoSeleccionado)//se crea el estado inicial de ese objeto
                    {
                        case 1:
                        case 2:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 10:
                            insertar(cordenada[0], cordenada[1], cordenada[2], new int[] { objetoSeleccionado, 0, 0, direcion(aux) });
                            break;
                        case 3:
                        case 9:
                            insertar(cordenada[0], cordenada[1], cordenada[2], new int[] { objetoSeleccionado, 1, 0, direcion(aux) });
                            break;
                        default:
                            break;
                    }
                }
                else
                    if (Input.GetKeyUp(KeyCode.Mouse1))//si se levanta el click secundario
                {
                    ArrayList puntos = cordenadasDeClickDerecho(hit);
                    float x = (float)puntos[0];
                    float y = (float)puntos[1];
                    float z = (float)puntos[2];//se obtiene las cordenas del bloque

                    for (int i = 0; objetos.Count > i; i++)
                    {
                        ArrayList objeto = (ArrayList)objetos[i];
                        if (x == (float)objeto[0] & y == (float)objeto[1] & z == (float)objeto[2])//se itera para buscar el objeto al que seleccionas
                        {
                            Destroy((GameObject)objeto[3]);//lo destruye
                            objetos.RemoveAt(i);//lo remueve del array
                            cordenadas[traductorDeCordenadas((int)x), traductorDeCordenadas((int)y), traductorDeCordenadas((int)z), 0] = 0;//y en la logica se le da el codigo de objeto borrado
                            break;
                        }
                    }
                }
                else if (Input.GetKey(KeyCode.Mouse1))//se presiono el clic secundario pero no se ha levantado
                {
                    ArrayList puntos = cordenadasDeClickDerecho(hit);
                    SelecionDeCubo = pintarPapelera((float)puntos[0], (float)puntos[1], (float)puntos[2]);//se pinta el objeto papelera
                }
                else
                {
                    SelecionDeCubo = pintarTransparente(hit);//se pinta el objeto que indica el cubo al que apunta
                }
            }
            if (tiempoReloj >= 0.1f)//es el delay de cada actualizacion de logica
            {
                Thread hilo = new Thread(actualizar);//en otro hilo se crea la actualizacion para evitar el lag
                hilo.Start();//se incia el hilo
                if (acabo) lamparas();//si el hilo termina lamparas se actualiza
                tiempoReloj = 0f;//reinicia el tiempo
            }
        }
        else
        {
            elMenu.SetActive(true);//se activa el menu
            actualiza = false;
        }
    }
    public void eliminarTodo()
    {
        for (int i = 0; i < objetos.Count; i++)
        {
            Destroy((GameObject)((ArrayList)objetos[i])[3]);//destruye todos los objetos
        }
        objetos = new ArrayList();//crea otro array limpio
        cordenadas = new int[0, 0, 0, 4];//crea otro array de logica nuevo
    }
    private void actualizar()
    {
        if (acabo)//evita que se ejecute varias veces el hilo
        {
            acabo = false;
            tiempo++;
            relojes();//actualiza el reloj
            cobre();//actualiza el cobre
            compuertas();//actualiza compuertas

            acabo = true;
        }
    }
    private void compuertas()
    {
        for (int x = 0; x < cordenadas.GetLength(0); x++)
            for (int y = 0; y < cordenadas.GetLength(1); y++)
                for (int z = 0; z < cordenadas.GetLength(2); z++)
                    if (!vacio(x, y, z))//Se itera y se verifica que el objeto no sea vacio para evitar error
                        if (cordenadas[x, y, z, 0] < 9 && cordenadas[x, y, z, 0] > 3)//se verifica si es una compuerta
                        {
                            int ang = cordenadas[x, y, z, 3];//se le el angulo de la compuerta
                            switch (cordenadas[x, y, z, 0])//se selecciona el tipo de compuerta
                            {
                                case 4:
                                    andCompuerta(x, y, z, ang);//se llama a la funcion de cada objeto
                                    break;
                                case 5:
                                    orCompuerta(x, y, z, ang);
                                    break;
                                case 6:
                                    xorCompuerta(x, y, z, ang);
                                    break;
                                case 7:
                                    notCompuerta(x, y, z, ang);
                                    break;
                                case 8:
                                    memoriaTipoD(x, y, z, ang);
                                    break;
                                default:
                                    break;
                            }
                        }
    }
    private void andCompuerta(int x, int y, int z, int ang)
    {
        bool[] inputs = entradas(x, y, z, ang);
        cordenadas[x, y, z, 1] = inputs[0] && inputs[1] ? 1 : 0;//se ejecuta su comparacion correspondiente
    }
    private void orCompuerta(int x, int y, int z, int ang)
    {
        bool[] inputs = entradas(x, y, z, ang);
        cordenadas[x, y, z, 1] = inputs[0] || inputs[1] ? 1 : 0;
    }
    private void xorCompuerta(int x, int y, int z, int ang)
    {
        bool[] inputs = entradas(x, y, z, ang);
        cordenadas[x, y, z, 1] = inputs[0] ^ inputs[1] ? 1 : 0;
    }
    private void notCompuerta(int x, int y, int z, int ang)
    {
        bool input = entrada(x, y, z, ang);//el not tiene la entrada en otra parte por eso es el unico que funciona diferente aqui
        cordenadas[x, y, z, 1] = input ? 0 : 1;
    }
    private void memoriaTipoD(int x, int y, int z, int ang)
    {
        bool[] inputs = entradas(x, y, z, ang);
        if (!inputs[0])//cuando la señal es un no se actualiza su estado
            cordenadas[x, y, z, 1] = inputs[1] ? 1 : 0;//lee la entrada
    }
    private bool[] entradas(int x, int y, int z, int ang)
    {
        bool[] inputs = new bool[2];
        int[] examinar = new int[] { x, y, z, ang };
        examinar = direcion(examinar, new int[] { sumarCordenada(examinar[0], 1), examinar[1], examinar[2] });//se obtiene la ubicacion de las cordenadas de la entrada 0
        inputs[0] = energia(examinar[0], examinar[1], examinar[2]);//se examina si tiene energia
        examinar = new int[] { x, y, z, ang };//se obtiene regresa ala compuerta
        examinar = direcion(examinar, new int[] { sumarCordenada(examinar[0], -1), examinar[1], examinar[2] });
        inputs[1] = energia(examinar[0], examinar[1], examinar[2]);//se lee la entrada
        return inputs;
    }
    private bool entrada(int x, int y, int z, int ang)
    {
        int[] examinar = new int[] { x, y, z, ang };
        examinar = direcion(examinar, new int[] { examinar[0], examinar[1], sumarCordenada(examinar[2], -1) });
        return energia(examinar[0], examinar[1], examinar[2]);//funciona igual solo que la entrada esta al lado contrario de la salida
    }

    private bool energia(int x, int y, int z)
    {
        if (vacio(x, y, z)) return false;//se no hay nada no hay energia
        if (cordenadas[x, y, z, 0] == 1 || cordenadas[x, y, z, 0] == 3 || cordenadas[x, y, z, 0] == 9 || cordenadas[x, y, z, 0] == 10)//se lee si es un objeto con energia exeptuando las otras compuertas
            return cordenadas[x, y, z, 1] == 1;
        else return false;
    }
    private void relojes()
    {
        for (int x = 0; x < cordenadas.GetLength(0); x++)
            for (int y = 0; y < cordenadas.GetLength(1); y++)
                for (int z = 0; z < cordenadas.GetLength(2); z++)
                    if (!vacio(x, y, z))//se evita que intentes leer fuera del array
                        if (cordenadas[x, y, z, 0] == 9 || cordenadas[x, y, z, 0] == 10)//se ve si es un reloj
                            if (tiempo >= 20)//se ve si se ha ejecutado 20 o mas actualizaciones
                                cordenadas[x, y, z, 1] = cordenadas[x, y, z, 0] == 9 ? 0 : 1;//si hay mas de 20 actualizaciones se apaga el reloj
                            else
                                cordenadas[x, y, z, 1] = cordenadas[x, y, z, 0] == 9 ? 1 : 0;
        tiempo = tiempo >= 20 ? 0 : tiempo;
    }
    private void cobre()
    {
        for (int x = 0; x < cordenadas.GetLength(0); x++)
            for (int y = 0; y < cordenadas.GetLength(1); y++)
                for (int z = 0; z < cordenadas.GetLength(2); z++)
                    if (!vacio(x, y, z))
                        if (cordenadas[x, y, z, 0] == 1)
                            cordenadas[x, y, z, 2] = 0;//se hace que todos los cobres no se han modificado
        for (int x = 0; x < cordenadas.GetLength(0); x++)
            for (int y = 0; y < cordenadas.GetLength(1); y++)
                for (int z = 0; z < cordenadas.GetLength(2); z++)
                {
                    if (!vacio(x, y, z))
                    {
                        if (cordenadas[x, y, z, 0] == 1 && cordenadas[x, y, z, 2] == 0)
                        {
                            enFrente(x, y, z);//se ejecuta una funcion para expandir la señal
                            noFuente = true;
                        }
                    }
                }
    }
    private void lamparas()
    {
        for (int x = 0; x < cordenadas.GetLength(0); x++)
            for (int y = 0; y < cordenadas.GetLength(1); y++)
                for (int z = 0; z < cordenadas.GetLength(2); z++)
                {
                    if (!vacio(x, y, z))
                        if (cordenadas[x, y, z, 0] == 2)//se itera todas las lamparas
                        {
                            lampara(x, y, z);
                        }
                }
    }
    private void lampara(int x, int y, int z)
    {
        if (energiaAlrededor(x, y, z))//revisa si hay energia y se enciende o apaga segun la energia
        {
            encender(x, y, z);
        }
        else
        {
            apagar(x, y, z);
        }
    }
    private void encender(int x, int y, int z)
    {
        if (cordenadas[x, y, z, 1] == 1) return;//se revisa si ya se actualizo esto para evitar que se este destruyendo y creando una y otra vez aunque su estado no cambie
        x = traductorDeCordenadasInverso(x);
        y = traductorDeCordenadasInverso(y);
        z = traductorDeCordenadasInverso(z);//se obtienen las cordenadas del objeto
        ArrayList luz = getPorCordenada(x, y, z);//se obtiene el array que contiene el gameobject de la lampara
        Destroy((GameObject)luz[3]);//se destruye
        luz[3] = Instantiate(lamparaOn, new Vector3(x, y, z), Quaternion.identity);//se crea una nueva lampara encendida
        x = traductorDeCordenadas(x);
        y = traductorDeCordenadas(y);
        z = traductorDeCordenadas(z);//se obtiene las cordenadas para el array int
        cordenadas[x, y, z, 1] = 1;//se pone en uno para evtiar que vuelva actualizarse si no cambia de estado
    }
    private void apagar(int x, int y, int z)//lo mismo que encender pero contrario
    {
        if (cordenadas[x, y, z, 1] == 0) return;
        x = traductorDeCordenadasInverso(x);
        y = traductorDeCordenadasInverso(y);
        z = traductorDeCordenadasInverso(z);
        ArrayList luz = getPorCordenada(x, y, z);
        Destroy((GameObject)luz[3]);
        luz[3] = Instantiate(lamparaOff, new Vector3(x, y, z), Quaternion.identity);
        x = traductorDeCordenadas(x);
        y = traductorDeCordenadas(y);
        z = traductorDeCordenadas(z);
        cordenadas[x, y, z, 1] = 0;
    }
    private bool energiaAlrededor(int x, int y, int z)//se explica en otro lado
    {
        for (int i = 1; i >= -1; i -= 2)
        {
            int aux = sumarCordenada(x, i);
            if (!vacio(aux, y, z))
            {
                if ((cordenadas[aux, y, z, 0] == 1 || cordenadas[aux, y, z, 0] > 2) && cordenadas[aux, y, z, 1] == 1)
                {
                    return true;
                }
            }
            aux = sumarCordenada(y, i);
            if (!vacio(x, aux, z))
            {
                if ((cordenadas[x, aux, z, 0] == 1 || cordenadas[x, aux, z, 0] > 2) && cordenadas[x, aux, z, 1] == 1)
                {
                    return true;
                }
            }
            aux = sumarCordenada(z, i);
            if (!vacio(x, y, aux))
            {
                if ((cordenadas[x, y, aux, 0] == 1 || cordenadas[x, y, aux, 0] > 2) && cordenadas[x, y, aux, 1] == 1)
                {
                    return true;
                }
            }

        }
        return false;
    }
    private void enFrente(int x, int y, int z)//so lo son para no pasar tantos parametros cada vez que se usa
    {
        enFrente(x, y, z, new bool[] { true, true, true }, new bool[] { false, false, false }, true, 2);
    }
    private void enFrente(int x, int y, int z, bool apagar)
    {
        enFrente(x, y, z, new bool[] { true, true, true }, new bool[] { false, false, false }, apagar, 2);
    }
    private void enFrente(int x, int y, int z, bool[] buscarEnEje, bool[] expandirEnEje, bool apagar, int adelante)
    {
        int ciclos = 0;//evita que se cicle
        bool inicia = expandirEnEje[0] || expandirEnEje[1] || expandirEnEje[2];//se revisa si es el inicial
        do
        {
            if (apagar == noFuente)//esto es para terminar la funcion en caso que se descubra la funte, cuando se encuentra se ejecuta otra vez enfrente pero esta vez para expandir la energia
            {
                cordenadas[x, y, z, 2] = apagar ? 1 : 2;
                cordenadas[x, y, z, 1] = noFuente ? 0 : 1;
                int modificador = apagar ? 0 : 1;
                if (existeCobreAlrededor(x, y, z, buscarEnEje, modificador))//busca cobre al rededor
                {
                    int[,] cobres = alrededorCobre(x, y, z, buscarEnEje, modificador);//obtenemos las cordenadas de los bloques
                    for (int i = 0; cobres.GetLength(0) > i; i++)
                    {
                        bool[] aux = new bool[3];
                        aux[0] = (x - cobres[i, 0]) > 0;
                        aux[1] = (y - cobres[i, 1]) > 0;
                        aux[2] = (z - cobres[i, 2]) > 0;
                        int aumento = 1000;
                        aumento = (x != cobres[i, 0]) ? diferenciaCordenada(x, cobres[i, 0]) : aumento;
                        aumento = (y != cobres[i, 1]) ? diferenciaCordenada(y, cobres[i, 1]) : aumento;
                        aumento = (z != cobres[i, 2]) ? diferenciaCordenada(z, cobres[i, 2]) : aumento;
                        bool[] auxBuscarEnEje = new bool[3];
                        auxBuscarEnEje[0] = !aux[0];
                        auxBuscarEnEje[1] = !aux[1];
                        auxBuscarEnEje[2] = !aux[2];//estos evitan busquen la señal en un eje, pues mas abajo ya existe codigo para expandirla señal en el eje esto para evitar consumo de ram exesiva
                        //se calculan todos los parametros a pasar
                        enFrente(cobres[i, 0], cobres[i, 1], cobres[i, 2], auxBuscarEnEje, aux, apagar, aumento);
                    }
                }
                if (apagar != noFuente) break;//no se ejecuta pero esta por evitar bugs
                if (existeFuenteAlrededor(x, y, z) && apagar)
                {
                    noFuente = false;//se cambia el estado de la no fuente
                    enFrente(x, y, z, false);//se re ejecuta el comando pero esta vez con se expandira energia
                    return;
                }
                else if (inicia)
                {
                    x = expandirEnEje[0] ? sumarCordenada(x, adelante) : x;
                    y = expandirEnEje[1] ? sumarCordenada(y, adelante) : y;
                    z = expandirEnEje[2] ? sumarCordenada(z, adelante) : z;
                    if (!vacio(x, y, z))
                        if (cordenadas[x, y, z, 0] != 1) return;//revisa si no esta vacio y si no es uno, si no se sigue el bucle
                }
            }
            else
            {
                break;
            }
            ciclos++;
        }
        while (!vacio(x, y, z) && inicia && ciclos < 1000000);
    }
    private int diferenciaCordenada(int c, int a)
    {
        c = traductorDeCordenadasInverso(c);
        a = traductorDeCordenadasInverso(a);
        int diferencia;
        diferencia = a - c;
        return diferencia;//resta cordenadas
    }
    private int[,] alrededorCobre(int x, int y, int z, bool[] buscarEnEje, int modificado)//esta funcion revisa si hay cobre o otro bloque al rededor en caso de las otras funciones parecidas
    {
        int[,] retornar = new int[0, 0];
        for (int i = 1; i >= -1; i -= 2)
        {
            int aux = sumarCordenada(x, i);
            if (!vacio(aux, y, z) && buscarEnEje[0])
            {
                if (cordenadas[aux, y, z, 0] == 1 && cordenadas[aux, y, z, 2] <= modificado)
                {
                    retornar = insertar(retornar, new int[] { aux, y, z });
                }
            }
            aux = sumarCordenada(y, i);
            if (!vacio(x, aux, z) && buscarEnEje[1])
            {
                if (cordenadas[x, aux, z, 0] == 1 && cordenadas[x, aux, z, 2] <= modificado)
                {
                    retornar = insertar(retornar, new int[] { x, aux, z });
                }
            }
            aux = sumarCordenada(z, i);
            if (!vacio(x, y, aux) && buscarEnEje[2])
            {
                if (cordenadas[x, y, aux, 0] == 1 && cordenadas[x, y, aux, 2] <= modificado)
                {
                    retornar = insertar(retornar, new int[] { x, y, aux });
                }
            }
        }
        if (retornar.Length > 0) return retornar;
        int[,] aux2 = new int[1, 1];
        aux2[0, 0] = 0;
        return aux2;
    }
    private int[,] insertar(int[,] retornar, int[] aux)//se usa para insertar en array y evitar bugs
    {
        retornar = (int[,])retornar.Clone();
        if (retornar.Length == 0)
        {
            retornar = new int[1, 3];
            for (int i = 0; i < 3; i++)
                retornar[0, i] = aux[i];
        }
        else
        {
            int[,] aux2 = (int[,])retornar.Clone();
            retornar = new int[aux2.GetLength(0) + 1, 3];
            for (int i = 0; i < aux2.GetLength(0); i++)
                for (int c = 0; c < aux2.GetLength(1); c++)
                    retornar[i, c] = aux2[i, c];
            for (int i = 0; i < aux.Length; i++)
            {
                retornar[retornar.GetLength(0) - 1, i] = aux[i];
            }
        }
        return retornar;
    }
    private bool existeCobreAlrededor(int x, int y, int z, bool[] buscarEnEje, int modificado)//busca cobre alrededor pero no pasa sus posiciones solo si hay
    {
        for (int i = 1; i >= -1; i -= 2)
        {
            int aux = sumarCordenada(x, i);
            if (!vacio(aux, y, z) && buscarEnEje[0])
            {
                if (cordenadas[aux, y, z, 0] == 1 && cordenadas[aux, y, z, 2] <= modificado)
                {
                    return true;
                }
            }
            aux = sumarCordenada(y, i);
            if (!vacio(x, aux, z) && buscarEnEje[1])
            {
                if (cordenadas[x, aux, z, 0] == 1 && cordenadas[x, aux, z, 2] <= modificado)
                {
                    return true;
                }
            }
            aux = sumarCordenada(z, i);
            if (!vacio(x, y, aux) && buscarEnEje[2])
            {
                if (cordenadas[x, y, aux, 0] == 1 && cordenadas[x, y, aux, 2] <= modificado)
                {
                    return true;
                }
            }
        }

        return false;
    }
    private bool existeFuenteAlrededor(int x, int y, int z)//igual que arriba
    {
        for (int i = 1; i >= -1; i -= 2)
        {
            int aux = sumarCordenada(x, i);
            if (!vacio(aux, y, z))
            {
                if ((cordenadas[aux, y, z, 0] == 3 || cordenadas[aux, y, z, 0] == 9) && cordenadas[aux, y, z, 1] == 1)
                    return true;
                else if (cordenadas[aux, y, z, 0] > 3)
                    if (leerSalida(aux, y, z, new int[] { x, y, z }))
                        return true;
            }
            aux = sumarCordenada(y, i);
            if (!vacio(x, aux, z))
            {
                if ((cordenadas[x, aux, z, 0] == 3 || cordenadas[x, aux, z, 0] == 9) && cordenadas[x, aux, z, 2] == 1)
                    return true;
                else if (cordenadas[x, aux, z, 0] > 3)
                    if (leerSalida(x, aux, z, new int[] { x, y, z }))
                        return true;
            }
            aux = sumarCordenada(z, i);
            if (!vacio(x, y, aux))
            {
                if (cordenadas[x, y, aux, 0] == 3 || cordenadas[x, y, aux, 0] == 9 && cordenadas[x, y, aux, 2] == 1)
                    return true;
                else if (cordenadas[x, y, aux, 0] > 3)
                    if (leerSalida(x, y, aux, new int[] { x, y, z }))
                        return true;
            }

        }
        return false;
    }
    private bool leerSalida(int x, int y, int z, int[] origen)//le las salidas para simplificar las compuertas
    {
        if (esIgual(direcion(new int[] { x, y, z, cordenadas[x, y, z, 3] }, new int[] { x, y, sumarCordenada(z, 1) }), origen))
        {
            return cordenadas[x, y, z, 1] == 1;
        }
        return false;
    }
    private bool esIgual(int[] primero, int[] segundo)//revisa que dos array son iguales
    {
        if (primero.Length != segundo.Length) return false;
        bool igual = true;
        for (int i = 0; i < primero.Length; i++)
        {
            if (primero[i] != segundo[i])
                igual = false;
        }
        return igual;
    }
    public ArrayList getPorCordenada(int x, int y, int z)//regruesa el array que contiene el gameobject y las cordenadas
    {
        for (int i = 0; i < objetos.Count; i++)
        {
            ArrayList objeto = (ArrayList)objetos[i];
            if (x == (float)objeto[0] & y == (float)objeto[1] & z == (float)objeto[2])
            {
                return objeto;
            }
        }
        return new ArrayList();
    }

    GameObject pintarTransparente(RaycastHit hit)//solo es para evitar escrivir lo mismo una y otra vez
    {
        float x = Mathf.Round(hit.point.x);
        float y = Mathf.Round(hit.point.y);
        float z = Mathf.Round(hit.point.z);

        return Instantiate(transparente, new Vector3(x, y, z), Quaternion.identity);
    }
    GameObject pintarPapelera(float x, float y, float z)
    {
        return Instantiate(cuboPapelera, new Vector3(x, y, z), Quaternion.identity);
    }
    private ArrayList cordenadasDeClickDerecho(RaycastHit hit)//identifica el bloque al que se dio clic
    {
        float x = Mathf.Round(hit.point.x);
        float y = Mathf.Round(hit.point.y);
        float z = Mathf.Round(hit.point.z);

        bool auxX = x - hit.point.x > menor & x - hit.point.x < mayor;
        bool auxY = y - hit.point.y > menor & y - hit.point.y < mayor;
        bool auxZ = z - hit.point.z > menor & z - hit.point.z < mayor;

        if (auxX) x -= 1;
        else if (auxY) y -= 1;
        else if (auxZ) z -= 1;

        auxX = x - hit.point.x < -menor & x - hit.point.x > -mayor;
        auxY = y - hit.point.y < -menor & y - hit.point.y > -mayor;
        auxZ = z - hit.point.z < -menor & z - hit.point.z > -mayor;

        if (auxX) x += 1;
        else if (auxY) y += 1;
        else if (auxZ) z += 1;

        ArrayList puntos = new ArrayList(3);
        puntos.Add(x);
        puntos.Add(y);
        puntos.Add(z);

        return puntos;
    }
    private void seleccionar()//seleciona la entrada de teclado
    {
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha1) ? 1 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha2) ? 2 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha3) ? 3 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha4) ? 4 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha5) ? 5 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha6) ? 6 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha7) ? 7 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha8) ? 8 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha9) ? 9 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha0) ? 10 : objetoSeleccionado;
    }
    private GameObject getElemento()
    {
        return (GameObject)EDJ[objetoSeleccionado - 1];
    }
    private float mirarAng()//ve cual es el angulo mas cercano a los multiplos de 90
    {
        float y = esfera.transform.rotation.eulerAngles.y;
        if (y <= 45) y = 0;
        else if (y <= 135) y = 90;
        else if (y <= 225) y = 180;
        else if (y <= 315) y = 270;
        else y = 0;
        return y;
    }
    private int[] direcion(int[] origen, int[] punto)//es el algoritmo para rotar un punto respecto a otro
    {
        int x = origen[0]; int y = origen[1]; int z = origen[2];
        punto[0] -= origen[0];
        punto[2] -= origen[2];
        int ya = origen[3];
        print(ya);
        int aux;
        switch (ya)
        {
            case 3:
                aux = punto[0];
                punto[0] = -punto[2];
                punto[2] = aux;
                break;
            case 1:
                aux = punto[0];
                punto[0] = punto[2];
                punto[2] = -aux;
                break;
            case 2:
                aux = punto[0];
                punto[0] *= -1;
                punto[2] = -aux;
                break;
            default:
                break;
        }
        punto[0] += origen[0];
        punto[2] += origen[2];
        return punto;
    }

    private bool vacio(int x, int y, int z)//es quien revisa si se esta leyendo fuera del array
    {
        if (cordenadas.GetLongLength(0) <= x | x < 0) return true;
        if (cordenadas.GetLongLength(1) <= y | y < 0) return true;
        if (cordenadas.GetLongLength(2) <= z | z < 0) return true;
        return cordenadas[x, y, z, 0] == -1;

    }
    private int sumarCordenada(int c, int a)//suma cordenadas en el sistema de solo positivos
    {
        return traductorDeCordenadas(traductorDeCordenadasInverso(c) + a);
    }
    private int restarCordenada(int c, int a)
    {
        return traductorDeCordenadas(traductorDeCordenadasInverso(c) - a);
    }
    private void insertar(int x, int y, int z, int[] numObjeto)//inserta elementos en el array de cordenadas
    {
        int xTamano = cordenadas.GetLength(0);
        int yTamano = cordenadas.GetLength(1);
        int zTamano = cordenadas.GetLength(2);
        if (x >= xTamano || y >= yTamano || z >= zTamano)
        {
            int x2 = x >= xTamano ? x + 1 : xTamano;
            int y2 = y >= yTamano ? y + 1 : yTamano;
            int z2 = z >= zTamano ? z + 1 : zTamano;
            int[,,,] aux = new int[x2, y2, z2, numObjeto.Length];
            for (int i = 0; i < xTamano; i++)
                for (int k = 0; k < yTamano; k++)
                    for (int c = 0; c < zTamano; c++)
                        for (int p = 0; p < numObjeto.Length; p++)
                            aux[i, k, c, p] = cordenadas[i, k, c, p];
            for (int i = 0; i < numObjeto.Length; i++)
                aux[x, y, z, i] = numObjeto[i];
            cordenadas = aux;
        }
        else
        {
            cordenadas[x, y, z, 0] = numObjeto[0];
            cordenadas[x, y, z, 1] = numObjeto[1];
            cordenadas[x, y, z, 2] = numObjeto[2];
        }
    }
    public int traductorDeCordenadas(int cordenada)//traduce cordenadas de un sistema cartesiano a un sistema de solo positivos
    {
        cordenada *= 2;
        if (cordenada < 0)
        {
            cordenada *= -1;
            cordenada++;
        }
        return cordenada;
    }
    public int traductorDeCordenadasInverso(int cordenada)
    {
        if (cordenada % 2 == 1)
        {
            cordenada--;
            return -cordenada / 2;
        }
        else
        {
            return cordenada / 2;
        }
    }
    private void imprimirArray(int[] cordenada)//imprime un array 
    {
        string s = "";
        for (int i = 0; cordenada.Length > i; i++)
        {
            s += cordenada[i];
            if (i != cordenada.Length - 1)
            {
                s += ", ";
            }
        }
        print(s);
    }
    private void clonar(ArrayList[,,] de, ArrayList[,,] a)//clona arrays
    {
        for (int x = 0; x < de.GetLongLength(0); x++)
        {
            for (int y = 0; y < de.GetLongLength(1); y++)
            {
                for (int z = 0; z < de.GetLongLength(2); z++)
                {
                    a[x, y, z] = de[x, y, z];
                }
            }
        }
    }
    private bool esIgualCordenada(int[] lista, int x, int y, int z)
    {
        return lista[0] == x && lista[1] == y && lista[2] == z;
    }
    public float gradosARadianes(float ang)
    {
        return ang * Mathf.PI / 180;
    }
    private int direcion(GameObject g)//obtiene las cordenadas y redondea
    {
        Vector3 angulo = g.transform.rotation.eulerAngles;
        float yAng = angulo.y;
        yAng = yAng < 0 ? (yAng - 360) : yAng;
        yAng = yAng / 90;
        return (int)Mathf.Round(yAng);
    }
}

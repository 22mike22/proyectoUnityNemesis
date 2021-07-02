using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crear : MonoBehaviour
{
    public GameObject cubo;
    public GameObject lamparaOn;
    public GameObject lamparaOff;
    public GameObject plastico;
    private ArrayList EDJ;
    public GameObject transparente;
    public GameObject cuboPapelera;
    private GameObject aux;
    private GameObject SelecionDeCubo;
    private int objetoSeleccionado = 0;
    private ArrayList objetos = new ArrayList();
    private const float menor = 0.499994f;
    private const float mayor = 0.499996f;
    public ArrayList[,,] cordenadas;
    public GameObject esfera;//1
    public GameObject generador;//2
    public GameObject and;//3
    public GameObject or;//4
    public GameObject xor;
    public GameObject not;
    public GameObject tipoD;
    public GameObject relojO;
    public float tiempo;

    void Start()
    {
        EDJ = new ArrayList();
        EDJ.Add(cubo);//1
        EDJ.Add(generador);//2
        EDJ.Add(lamparaOff);//3
        EDJ.Add(and);//4
        EDJ.Add(or);//5
        EDJ.Add(xor);//6
        EDJ.Add(not);//7
        EDJ.Add(tipoD);//8
        EDJ.Add(relojO);//9
        EDJ.Add(plastico);//10
        objetoSeleccionado = 0;
        cordenadas = new ArrayList[1, 2, 2];
        tiempo = 0;
    }
    void Update()
    {
        seleccionar();
        //print(Time.deltaTime);
        tiempo += Time.deltaTime;
        Destroy(SelecionDeCubo);
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                float x = Mathf.Round(hit.point.x);
                float y = Mathf.Round(hit.point.y);
                float z = Mathf.Round(hit.point.z);

                aux = Instantiate(getElemento(), new Vector3(x, y, z), Quaternion.identity);
                if (objetoSeleccionado > 2) aux.transform.Rotate(0, mirarAng(), 0);
                ArrayList objeto = new ArrayList();
                objeto.Add(x);
                objeto.Add(y);
                objeto.Add(z);
                objeto.Add(aux);

                objetos.Add(objeto);
                int[] cordenada = { traductorDeCordenadas((int)x), traductorDeCordenadas((int)y), traductorDeCordenadas((int)z) };
                ArrayList aux2 = new ArrayList();
                aux2.Add(objetoSeleccionado);
                        aux2.Add(true);
                aux2.Add(false);
                aux2.Add(0.0f);
                        insertar(aux2, cordenada[0], cordenada[1], cordenada[2]);

                            }
            else
                if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                ArrayList puntos = cordenadasDeClickDerecho(hit);
                float x = (float)puntos[0];
                float y = (float)puntos[1];
                float z = (float)puntos[2];

                for (int i = 0; objetos.Count > i; i++)
                {
                    ArrayList objeto = (ArrayList)objetos[i];
                    if (x == (float)objeto[0] & y == (float)objeto[1] & z == (float)objeto[2])
                    {
                        Destroy((GameObject)objeto[3]);
                        objetos.RemoveAt(i);
                        cordenadas[traductorDeCordenadas((int)x), traductorDeCordenadas((int)y), traductorDeCordenadas((int)z)] = null;
                        break;
                    }
                }
            }
            else if (Input.GetKey(KeyCode.Mouse1))
            {
                ArrayList puntos = cordenadasDeClickDerecho(hit);
                SelecionDeCubo = pintarPapelera((float)puntos[0], (float)puntos[1], (float)puntos[2]);
            }
            else
            {
                SelecionDeCubo = pintarTransparente(hit);
            }
        }

        actualisar();
        if (tiempo > 1) tiempo = 0;
    }
    public ArrayList getPorCordenada(int x, int y, int z)
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
    GameObject pintarTransparente(RaycastHit hit)
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
    private ArrayList cordenadasDeClickDerecho(RaycastHit hit)
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
    private void seleccionar()
    {
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha1) ? 0 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha2) ? 1 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha3) ? 2 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha4) ? 3 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha5) ? 4 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha6) ? 5 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha7) ? 6 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha8) ? 7 : objetoSeleccionado;
        objetoSeleccionado = Input.GetKeyUp(KeyCode.Alpha9) ? 8 : objetoSeleccionado;
    }
    private GameObject getElemento()
    {
        return (GameObject)EDJ[objetoSeleccionado];
    }
    private float mirarAng()
    {
        float y = esfera.transform.rotation.eulerAngles.y;
        if (y < 45) y = 0;
        else if (y < 135) y = 90;
        else if (y < 225) y = 180;
        else if (y < 315) y = 270;
        else y = 0;
        return y;
    }
    private void actualisar()
    {
        ponerCablesACero();
        fuentesDeEnergia();
        MemoriaTipoD();
        compuertas();
        compuertaNot();
        comprobarLamparas();
    }
    private void ponerCablesACero()
    {
        //print(cordenadas.GetLongLength(0));
        for (int x = 0; x < cordenadas.GetLongLength(0); x++)
        {
            for (int y = 0; y < cordenadas.GetLongLength(1); y++)
            {
                for (int z = 0; z < cordenadas.GetLongLength(2); z++)
                {
                    if (cordenadas[x, y, z] == null) continue;
                    if ((int)cordenadas[x, y, z][0] == 0)
                    {
                        cordenadas[x, y, z][1] = false;
                    }
                }
            }
        }
    }

    private void fuentesDeEnergia()
    {
        for (int x = 0; x < cordenadas.GetLongLength(0); x++)
        {
            for (int y = 0; y < cordenadas.GetLongLength(1); y++)
            {
                for (int z = 0; z < cordenadas.GetLongLength(2); z++)
                {
                    if (vacio(x, y, z)) continue;
                    if ((int)cordenadas[x, y, z][0] == 1)
                    {
                        buscar(x, y, z);
                    }
                    if ((int)cordenadas[x, y, z][0] == 8)
                        reloj(x, y, z);
                }
            }
        }
    }
    private void buscar(int x, int y, int z)
    {
        for (int i = 1; i >= -1; i -= 2)
        {
            int xa, ya, za;
            xa = traductorDeCordenadas(traductorDeCordenadasInverso(x) + i);
            ya = traductorDeCordenadas(traductorDeCordenadasInverso(y) + i);
            za = traductorDeCordenadas(traductorDeCordenadasInverso(z) + i);
            if (!vacio(xa, y, z))
            {
                if ((int)cordenadas[xa, y, z][0] == 0)
                {
                    expandir(xa, y, z, i, 0, 0);
                }

            }
            if (!vacio(x, ya, z))
            {
                if ((int)cordenadas[x, ya, z][0] == 0)
                {
                    expandir(x, ya, z, 0, i, 0);
                }

            }
            if (!vacio(x, y, za))
            {
                if ((int)cordenadas[x, y, za][0] == 0)
                {
                    expandir(x, y, za, 0, 0, i);
                }

            }

        }
    }
    private void expandir(int x, int y, int z, int xa, int ya, int za)
    {
        while ((int)cordenadas[x, y, z][0] == 0 && !(bool)cordenadas[x, y, z][1])
        {
            cordenadas[x, y, z][1] = true;
            buscar(x, y, z);
            x = sumarCordenada(x, xa);
            y = sumarCordenada(y, ya);
            z = sumarCordenada(z, za);

            if (vacio(x, y, z)) break;
        }
    }
    private void comprobarLamparas()
    {
        bool activada = false;
        for (int x = 0; x < cordenadas.GetLongLength(0); x++)
        {
            for (int y = 0; y < cordenadas.GetLongLength(1); y++)
            {
                for (int z = 0; z < cordenadas.GetLongLength(2); z++)
                {
                    if (vacio(x, y, z)) continue;
                    if ((int)cordenadas[x, y, z][0] == 2)
                    {
                        for (int i = 1; i >= -1; i -= 2)
                        {
                            if (encendido(x, y, z, i, 0, 0))
                            {
                                if((bool)cordenadas[x,y,z][1])encender(x, y, z);
                                cordenadas[x, y, z][1] = false;
                                activada = true;
                                continue;
                            }
                            if (encendido(x, y, z, 0, i, 0))
                            {
                                if((bool)cordenadas[x,y,z][1])encender(x, y, z);
                                cordenadas[x, y, z][1] = false;

                                activada = true;
                                continue;
                            }
                            if (encendido(x, y, z, 0, 0, i))
                            {
                                if((bool)cordenadas[x,y,z][1])encender(x, y, z);
                                cordenadas[x, y, z][1] = false;

                                activada = true;
                                continue;
                            }
                        }
                        if (activada)
                        {
                            activada = false;
                            continue;
                        }
                        if(!(bool)cordenadas[x,y,z][1]) apagar(x, y, z);
                        cordenadas[x, y, z][1] = true;
                    }
                }
            }
        }
    }
    private void compuertas()
    {
        for (int x = 0; x < cordenadas.GetLongLength(0); x++)
            for (int y = 0; y < cordenadas.GetLongLength(1); y++)
                for (int z = 0; z < cordenadas.GetLongLength(2); z++)
                {

                    if (vacio(x, y, z)) continue;
                    if ((int)cordenadas[x, y, z][0] == 3)
                        compuertaOr(x, y, z);
                    else if ((int)cordenadas[x, y, z][0] == 4)
                        compuertaAnd(x, y, z);
                    else if ((int)cordenadas[x, y, z][0] == 5)
                        compuertaXor(x, y, z);
                    //else if ((int)cordenadas[x, y, z][0] == 6)
                        //compuertaNot(x, y, z);
                    //else if ((int)cordenadas[x, y, z][0] == 7)
                        //MemoriaTipoD(x, y, z);
                                  }
    }
    private void compuertaOr(int x, int y, int z)
    {
        bool[] entradas = leerEntradas(x, y, z);
        int[] direcionSalida = direcionS(x, y, z);
        int[] salida = { sumarCordenada(x, direcionSalida[0]), sumarCordenada(y, direcionSalida[1]), sumarCordenada(z, direcionSalida[2]) };
        if (entradas[0] || entradas[1])
        {
            expandirCompuertas(salida);
        }
    }
    private void compuertaAnd(int x, int y, int z)
    {
        bool[] entradas = leerEntradas(x, y, z);
        int[] direcionSalida = direcionS(x, y, z);
        int[] salida = { sumarCordenada(x, direcionSalida[0]), sumarCordenada(y, direcionSalida[1]), sumarCordenada(z, direcionSalida[2]) };
        if (entradas[0] && entradas[1])
        {
            expandirCompuertas(salida);
        }
    }
    private void compuertaXor(int x, int y, int z)
    {
        bool[] entradas = leerEntradas(x, y, z);
        int[] direcionSalida = direcionS(x, y, z);
        int[] salida = { sumarCordenada(x, direcionSalida[0]), sumarCordenada(y, direcionSalida[1]), sumarCordenada(z, direcionSalida[2]) };
        if (entradas[0] ^ entradas[1])
        {
            expandirCompuertas(salida);
        }
    }
    private void MemoriaTipoD()
    {
        for (int x = 0; x < cordenadas.GetLongLength(0); x++)
            for (int y = 0; y < cordenadas.GetLongLength(1); y++)
                for (int z = 0; z < cordenadas.GetLongLength(2); z++)
                {
                    if (vacio(x, y, z)) continue;
                    if ((int)cordenadas[x, y, z][0] != 7) continue;
                    if ((bool)cordenadas[x, y, z][2])
                    {
                        cordenadas[x, y, z][2] = false;
                        continue;
                    }
                    bool[] entradas = leerEntradas(x, y, z);
                    int[] direcionSalida = direcionS(x, y, z);
                    int[] salida = { sumarCordenada(x, direcionSalida[0]), sumarCordenada(y, direcionSalida[1]), sumarCordenada(z, direcionSalida[2]) };
                    if (!entradas[0])
                    {
                        cordenadas[x, y, z][1] = entradas[1];
                        if ((bool)cordenadas[x, y, z][1]) expandirCompuertas(salida);
                        cordenadas[x, y, z][2] = false ;
                        compuertaNot();
                    }
                    else if((bool)cordenadas[x,y,z][1])
                        expandirCompuertas(salida);                }
        
    }
    private void reloj(int x, int y, int z)
    {
        if (tiempo <= 1)
        {
            buscar(x, y, z);
        } 
    }

    
    private void compuertaNot()
    {
        for (int x = 0; x < cordenadas.GetLongLength(0); x++)
            for (int y = 0; y < cordenadas.GetLongLength(1); y++)
                for (int z = 0; z < cordenadas.GetLongLength(2); z++)
                {
                    if (vacio(x, y, z)) continue;
                    if ((int)cordenadas[x, y, z][0] != 6) continue;


                    int[] direcionSalida = direcionS(x, y, z);
                    int[] salida = { sumarCordenada(x, direcionSalida[0]), sumarCordenada(y, direcionSalida[1]), sumarCordenada(z, direcionSalida[2]) };
                    bool entrada = entradaUnica(x, y, z);
                    print(entrada + " cambia");
                    if (!entrada)
                    {
                        expandirCompuertas(salida);
                    }
                }
    }
    private bool entradaUnica(int x,int y,int z)
    {
        int[] direcionSalida = direcionS(x, y, z);
        int[] entrada = { sumarCordenada(x, -direcionSalida[0]), sumarCordenada(y, -direcionSalida[1]), sumarCordenada(z, -direcionSalida[2]) };
        if (vacio(entrada[0], entrada[1], entrada[2]))
            return false;
        else
            return (bool)cordenadas[entrada[0], entrada[1], entrada[2]][1];
    }

    private void expandirCompuertas(int[] salida)
    {
        if (!vacio(salida[0], salida[1], salida[2]))
        {
            cordenadas[salida[0], salida[1], salida[2]][1] = true;
            buscar(salida[0], salida[1], salida[2]);
        }
    }
    private bool[] leerEntradas(int x, int y, int z)
    {
        bool[] entrada = { false, false };
        int[] entPos1, entPos2;
        int[] posicion = { x, y, z };
        entPos1 = new int[3];
        entPos2 = new int[3];
        int[] aux = direcion(x, y, z);
        for (int i = 0; i < 3; i++)
        {
            entPos1[i] = sumarCordenada(posicion[i], aux[i]);
            entPos2[i] = sumarCordenada(posicion[i], -aux[i]);
        }
        if (vacio(entPos1[0], entPos1[1], entPos1[2]))
        {
            entrada[0] = false;
        }
        else
        {
            ArrayList aux2 = cordenadas[entPos1[0], entPos1[1], entPos1[2]];
            entrada[0] = (int)aux2[0] == 1 || ((int)aux2[0] == 0 && (bool)aux2[1]);
        }
        if (vacio(entPos2[0], entPos2[1], entPos2[2]))
        {
            entrada[1] = false;
        }
        else
        {
            ArrayList aux2 = cordenadas[entPos2[0], entPos2[1], entPos2[2]];
            entrada[1] = (int)aux2[0] == 1 || ((int)aux2[0] == 0 && (bool)aux2[1]);
        }
        return entrada;
    }
    private int[] direcionS(int x, int y, int z)
    {
        int[] direcionEntradas = direcion(x, y, z);
        int[] dire = new int[3];
        if (esIgualCordenada(direcionEntradas, 1, 0, 0)) { int[] aux = { 0, 0, 1 }; dire = aux; }
        else if (esIgualCordenada(direcionEntradas, 0, 0, 1)) { int[] aux = { -1, 0, 0 }; dire = aux; }
        else if (esIgualCordenada(direcionEntradas, -1, 0, 0)) { int[] aux = { 0, 0, -1 }; dire = aux; }
        else { int[] aux = { 1, 0, 0 }; dire = aux; }
        return dire;
    }
    private int[] direcion(int x, int y, int z)
    {
        Vector3 angulo = ((GameObject)getPorCordenada(traductorDeCordenadasInverso(x), traductorDeCordenadasInverso(y), traductorDeCordenadasInverso(z))[3]).transform.rotation.eulerAngles;
        int ya = (int)angulo.y;
        int[] retornar = { 0, 0, 0 };
        if (ya == 0)
        {
            int[] aux = { 1, 0, 0 };
            retornar = aux;
        }
        else if (ya == 90)
        {
            int[] aux = { 0, 0, -1 };
            retornar = aux;
        }
        else if (ya == -180)
        {
            int[] aux = { -1, 0, 0 };
            retornar = aux;
        }
        else
        {
            int[] aux = { 0, 0, 1 };
            retornar = aux;
        }
        return retornar;
    }
    private void encender(int x, int y, int z)
    {
        
        int x2 = traductorDeCordenadasInverso(x);
        int y2 = traductorDeCordenadasInverso(y);
        int z2 = traductorDeCordenadasInverso(z);

        Destroy((GameObject)getPorCordenada(x2, y2, z2)[3]);
        getPorCordenada(x2, y2, z2)[3] = Instantiate(lamparaOn, new Vector3(x2, y2, z2), Quaternion.identity);
    }
    private void apagar(int x, int y, int z)
    {
        int x2 = traductorDeCordenadasInverso(x);
        int y2 = traductorDeCordenadasInverso(y);
        int z2 = traductorDeCordenadasInverso(z);
        //int[] cosas2 = { getPorCordenada(x2, y2, z2)[0] };
        //imprimirArray(cosas2);
        Destroy((GameObject)(getPorCordenada(x2, y2, z2)[3]));
        getPorCordenada(x2, y2, z2)[3] = Instantiate(lamparaOff, new Vector3(x2, y2, z2), Quaternion.identity);
    }
    private bool encendido(int x, int y, int z, int xi, int yi, int zi)
    {
        xi = sumarCordenada(x, xi);
        yi = sumarCordenada(y, yi);
        zi = sumarCordenada(z, zi);
        if (cordenadas.GetLongLength(0) <= xi && cordenadas.GetLongLength(1) <= yi && cordenadas.GetLongLength(2) <= zi) return false;
        if (vacio(xi, yi, zi)) return false;

        else
        {
            print(cordenadas[xi, yi, zi][1]);
            return (int)cordenadas[xi, yi, zi][0] == 0 && (bool)cordenadas[xi, yi, zi][1];
        }
    }
    private bool vacio(int x, int y, int z)
    {
        if (cordenadas.GetLongLength(0) <= x | x < 0) return true;
        if (cordenadas.GetLongLength(1) <= y | y < 0) return true;
        if (cordenadas.GetLongLength(2) <= z | z < 0) return true;
        return cordenadas[x, y, z] == null;
    }
    private int sumarCordenada(int c, int a)
    {
        return traductorDeCordenadas(traductorDeCordenadasInverso(c) + a);
    }
    private void insertar(ArrayList elemento, int x, int y, int z)
    {
        int Sx, Sy, Sz, lx, ly, lz;
        lx = (int)cordenadas.GetLongLength(0);
        ly = (int)cordenadas.GetLongLength(1);
        lz = (int)cordenadas.GetLongLength(2);
        Sx = x >= lx ? x + 1 : lx;
        Sy = y >= ly ? y + 1 : ly;
        Sz = z >= lz ? z + 1 : lz;
        if (x >= lx | y >= ly | z >= lz)
        {
            ArrayList[,,] aux = new ArrayList[Sx, Sy, Sz];
            clonar(cordenadas, aux);
            cordenadas = aux;
        }
        cordenadas[x, y, z] = elemento;
    }
    public int traductorDeCordenadas(int cordenada)
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
    private void imprimirArray(int[] cordenada)
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
    private void clonar(ArrayList[,,] de, ArrayList[,,] a)
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
}


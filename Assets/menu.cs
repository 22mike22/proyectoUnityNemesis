using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class menu : MonoBehaviour
{
    private float tiempo=0;
    public AudioClip track1;
    public AudioClip track2;
    public AudioClip track3;
    private AudioClip[] tracks=new AudioClip[10];
    private float duracion=0;
    private AudioSource audio;
    private byte selecion=0;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        tracks[0] = track1;
        tracks[1] = track2;
        tracks[2] = track3;
        selecionar(1);
    }

    // Update is called once per frame
    void Update()
    {
        tiempo += Time.deltaTime;
        if (tiempo > duracion)
        {
            audio.Stop();
            audio.clip = tracks[selecion];
            audio.Play();
            tiempo = 0;
        }
    }
    public void Menu (string level1)
    {
        print("menu" + level1);
        SceneManager.LoadScene(level1);
    }
    public void boton1()
    {
        selecionar(0);
    }
    public void boton2()
    {
        selecionar(1);
    }
    public void boton3()
    {
        selecionar(2);
    }
    public void boton4()
    {
        audio.Stop();
        tiempo = -1;
    }
    public void selecionar(byte selecion)
    {
        this.selecion = selecion;
        duracion = tracks[selecion].length;
        tiempo = 10000;
    }
}

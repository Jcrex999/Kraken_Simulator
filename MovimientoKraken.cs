using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoKraken : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento = 0.05f;  // Velocidad de movimiento por defecto
    [SerializeField] private float velocidadRotacion = 100f;  // Velocidad de rotaci�n
    [SerializeField] private float velocidadMaxima = 20f;     // Velocidad m�xima
    [SerializeField] private float velocidadMinima = 0.05f;      // Velocidad m�nima
    [SerializeField] private float velocidadIncremento = 0.05f; // Incremento de velocidad por frame

    private float movimientoFrontal = 0f;
    private float rotacionHorizontal = 0f;

    [SerializeField] private Rigidbody rb3D;

    private EntradaMovimiento entradaMovimiento;

    private void Awake()
    {
        // Inicializamos el sistema de entrada
        entradaMovimiento = new EntradaMovimiento();
    }

    private void OnEnable()
    {
        // Habilitamos el sistema de entrada cuando el objeto est� activo
        entradaMovimiento.Enable();
    }

    private void OnDisable()
    {
        // Deshabilitamos el sistema de entrada cuando el objeto no est� activo
        entradaMovimiento.Disable();
    }

    private void Update()
    {
        // Leer entrada del movimiento hacia adelante/atr�s con el joystick izquierdo (Vertical)
        movimientoFrontal = entradaMovimiento.Movimiento.Vertical.ReadValue<float>();

        // Leer entrada de la rotaci�n con el joystick izquierdo (Horizontal)
        rotacionHorizontal = entradaMovimiento.Movimiento.Horizontal.ReadValue<float>();

        // Aumentar o disminuir velocidad continuamente mientras se presionan los botones
        if (entradaMovimiento.Movimiento.Acelerar.ReadValue<float>() > 0)
        {
            Acelerar();
        }

        if (entradaMovimiento.Movimiento.Desacelerar.ReadValue<float>() > 0)
        {
            Desacelerar();
        }

        // Verificar si el bot�n Stop (X del mando) fue presionado
        if (entradaMovimiento.Movimiento.Stop.triggered)
        {
            Detener();
        }
    }

    private void FixedUpdate()
    {
        // Aplicar movimiento basado en la velocidad ajustada
        Mover(movimientoFrontal * Time.fixedDeltaTime);

        // Aplicar rotaci�n basada en la rotaci�n ajustada
        Rotar(rotacionHorizontal * velocidadRotacion * Time.fixedDeltaTime);
    }

    private void Mover(float mover)
    {
        // Calcular la velocidad objetivo hacia adelante o atr�s
        Vector3 velocidadObjetivo = transform.forward * mover * velocidadMovimiento;

        // Aplicar suavizado al movimiento
        rb3D.velocity = Vector3.SmoothDamp(rb3D.velocity, velocidadObjetivo, ref velocidadObjetivo, 200f);
        Debug.Log($"Mover: Velocidad actual = {velocidadObjetivo}");
    }

    private void Rotar(float rotar)
    {
        // Aplicar rotaci�n alrededor del eje Y
        Quaternion rotacion = Quaternion.Euler(0f, rotar, 0f);
        rb3D.MoveRotation(rb3D.rotation * rotacion);
    }

    private void Acelerar()
    {
        // Incrementar la velocidad sin superar el l�mite m�ximo
        velocidadMovimiento = Mathf.Min(velocidadMovimiento + velocidadIncremento, velocidadMaxima);
        Debug.Log($"Acelerando: Velocidad actual = {velocidadMovimiento}");
    }

    private void Desacelerar()
    {
        // Reducir la velocidad sin bajar del l�mite m�nimo
        velocidadMovimiento = Mathf.Max(velocidadMovimiento - velocidadIncremento, velocidadMinima);
        Debug.Log($"Desacelerando: Velocidad actual = {velocidadMovimiento}");
    }

    private void Detener()
    {
        // Detener el movimiento estableciendo la velocidad del Rigidbody a cero
        rb3D.velocity = Vector3.zero;
        Debug.Log("Movimiento detenido.");
    }
}

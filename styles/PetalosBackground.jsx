import React, { useEffect, useState, useRef } from "react";
import "./PetalosBackground.css";

const PetalosRosaRealistas = () => {
  const [petalos, setPetalos] = useState([]);
  const animationFrameRef = useRef();
  const lastTimeRef = useRef(0);
  const totalPetalosRef = useRef(0);

  useEffect(() => {
    // Constante con la cantidad de pétalos a mantener en pantalla
    const CANTIDAD_PETALOS = 50;

    // Función para crear un pétalo con propiedades realistas
    const crearPetaloRealista = () => {
      // Generar un ID único para cada pétalo
      const id = totalPetalosRef.current++;

      // Seleccionar un tipo aleatorio de pétalo de rosa (formas variadas)
      const tipoForma = Math.floor(Math.random() * 4);

      // Colores realistas de pétalos de rosa
      const esBlanco = Math.random() > 0.8;
      let colorBase;

      if (esBlanco) {
        // Pétalos blancos o pálidos
        const saturation = Math.floor(Math.random() * 10 + 90); // 90-100%
        colorBase = `rgb(255, ${
          saturation + 150 > 255 ? 255 : saturation + 150
        }, ${saturation + 150 > 255 ? 255 : saturation + 150})`;
      } else {
        // Variaciones de pétalos rosa (desde rosa pálido hasta rosa intenso)
        const r = 255;
        const g = Math.floor(Math.random() * 80) + 130; // 130-210
        const b = Math.floor(Math.random() * 70) + 160; // 160-230
        colorBase = `rgb(${r}, ${g}, ${b})`;
      }

      // Propiedades físicas y de movimiento - ajustadas para caída más moderada
      const size = Math.random() * 25 + 15; // Tamaño entre 15px y 40px
      const aspectRatio = 1 + Math.random() * 0.5; // Proporción largo/ancho entre 1 y 1.5
      const opacity = Math.random() * 0.4 + 0.6; // Opacidad entre 0.6 y 1

      // Velocidad de caída garantizada pero lenta
      const velocidadBase = Math.random() * 0.15 + 0.1; // Valor reducido para caída más lenta

      // Distribuir los pétalos por toda la pantalla para el inicio
      const startingY = Math.random() * 100; // Posición inicial aleatoria en toda la pantalla

      return {
        id,
        tipoForma,
        posX: Math.random() * 100, // Posición horizontal aleatoria (%)
        posY: startingY, // Iniciar en posición aleatoria en pantalla
        size,
        height: size * aspectRatio,
        color: colorBase,
        opacity,
        rotacion: Math.random() * 360, // Rotación inicial aleatoria
        rotacionVelocidad: (Math.random() * 1 - 0.5) * 0.3, // Velocidad de rotación muy reducida
        velocidadY: velocidadBase, // Velocidad vertical garantizada positiva (hacia abajo)
        oscilacionX: Math.random() * 20 + 5, // Amplitud de oscilación horizontal reducida
        oscilacionVelocidad: Math.random() * 0.005 + 0.002, // Velocidad de oscilación reducida
        oscilacionOffset: Math.random() * Math.PI * 2, // Offset de oscilación
        active: true,
      };
    };

    // Inicializar pétalos
    const inicializarPetalos = () => {
      const petalosIniciales = [];
      for (let i = 0; i < CANTIDAD_PETALOS; i++) {
        petalosIniciales.push(crearPetaloRealista());
      }
      setPetalos(petalosIniciales);
    };

    // Función de actualización basada en tiempo
    const actualizarPetalos = (timestamp) => {
      if (!lastTimeRef.current) lastTimeRef.current = timestamp;
      const deltaTime = Math.min(timestamp - lastTimeRef.current, 50); // Limitar deltaTime para evitar saltos
      lastTimeRef.current = timestamp;

      // Factor para hacer consistente la animación entre diferentes FPS
      const timeScale = deltaTime / 16; // Normalizar a ~60fps

      setPetalos((prevPetalos) => {
        return prevPetalos.map((petalo) => {
          // Garantizar que la velocidad Y siempre sea positiva (hacia abajo)
          const velocidadY = Math.max(0.05, petalo.velocidadY);

          // Actualizar posición Y (caída) - multiplicador reducido para movimiento más lento
          const newPosY = petalo.posY + velocidadY * timeScale * 0.5;

          // Actualizar posición X (oscilación sinusoidal) - amplitud reducida
          const time =
            timestamp * petalo.oscilacionVelocidad + petalo.oscilacionOffset;
          const offsetX =
            Math.sin(time) * petalo.oscilacionX * 0.03 * timeScale;

          // Actualizar rotación - movimiento más lento
          const newRotacion =
            petalo.rotacion + petalo.rotacionVelocidad * timeScale * 0.5;

          // Si el pétalo salió de la pantalla, reposicionarlo arriba
          if (newPosY > 105) {
            return {
              ...crearPetaloRealista(),
              posY: -5 - Math.random() * 10, // Iniciar justo encima de la pantalla
            };
          }

          // Devolver pétalo actualizado
          return {
            ...petalo,
            posY: newPosY,
            posX: petalo.posX + offsetX,
            rotacion: newRotacion,
          };
        });
      });

      animationFrameRef.current = requestAnimationFrame(actualizarPetalos);
    };

    // Iniciar el sistema
    inicializarPetalos();
    animationFrameRef.current = requestAnimationFrame(actualizarPetalos);

    // Limpieza al desmontar
    return () => {
      if (animationFrameRef.current) {
        cancelAnimationFrame(animationFrameRef.current);
      }
    };
  }, []);

  return (
    <div className="petalos-container">
      {petalos.map((petalo) => (
        <div
          key={petalo.id}
          className={`petalo-rosa petalo-tipo-${petalo.tipoForma}`}
          style={{
            left: `${petalo.posX}%`,
            top: `${petalo.posY}%`,
            width: `${petalo.size}px`,
            height: `${petalo.height}px`,
            backgroundColor: petalo.color,
            opacity: petalo.opacity,
            transform: `rotate(${petalo.rotacion}deg)`,
          }}
        >
          <div className="petalo-brillo"></div>
        </div>
      ))}
    </div>
  );
};

export default PetalosRosaRealistas;

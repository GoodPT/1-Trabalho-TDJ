namespace Pong;

using System;
using Microsoft.Xna.Framework;

// Classe auxiliar para gerar direções aleatórias para a bola
public class DirectionsHelper
{
    // Método que devolve uma direção aleatória
    public static Vector2 GetRandomDirection(bool right)
    {
        double x, y;
        var random = new Random();

        // Define a direção horizontal (x): se "right" for true, vai para a direita (+1), senão para a esquerda (-1)
        x = right ? 1 : -1;

        // Gera uma direção vertical (y) aleatória, mas limitada para não ficar demasiado vertical
        y = Math.Clamp(random.NextDouble() * 2 - 1, -0.7, 0.7);

        // Devolve o vetor de direção
        return new Vector2((float)x, (float)y);
    }
}
